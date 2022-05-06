using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.PM;
using PX.Data.Webhooks;
using System.Net;
using Newtonsoft.Json;
using TestWebhook.DAC;
using TestWebhook.Graph;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using PX.Objects.CS;
using PX.Objects.AR;

namespace TestWebhook
{
    public class ToggleWebhookHandler : IWebhookHandler
    {
        private string _ChannelAccessToken = "A+Xu3cOggd518Phf/PXDnf3jXZpIZ/j8RyDAimmirH3bXHEZdH5lCQ4b45XGTxpe5oJazpBVn/kEcQgTCmL+IbM+1HTA756Ss4Ugl4vSkjzwaeKuXqQP7i9ORwJ2SM6+DDBFL7220Bc/ZDk3lEXuCgdB04t89/1O/w1cDnyilFU=";
        private string _LineAPI_ReplyMsg = "https://api.line.me/v2/bot/message/reply";
        private string _numberingID = "LINEUSER";

        //Replay msg
        string _ReplyMsg = "很抱歉，我們目前無法處理此訊息。";

        private Dictionary<string, string> _DicUserStatus = new Dictionary<string, string>
        {
            {"NewCustomer", "0" },
            {"NameCompleted", "1" },
            {"EmailCompleted", "2" },
            {"InfoCompleted", "3" },
            {"Created", "4" },
            {"EditInfo", "5" }
        };

        private Dictionary<string, string> _DicUserStatusRReply = new Dictionary<string, string>
        {
            {"0", "請輸入您的姓名：" },
            {"1", "請輸入您的Email："},
            {"2", "Email輸入完成" },
            {"3", "[資料完成尚未創建會員]" },
            {"4", "會員檔案建立完成" },
            {"5", "[資料編輯中]" }
        };

        public async Task<IHttpActionResult> ProcessRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string postData = request.Content.ReadAsStringAsync().Result;
            var receivedMessage = JsonConvert.DeserializeObject<LineReceivedMessage>(postData);

            //Infomation from received Msg
            string userid = receivedMessage.events[0].source.userId; //使用者 ID
            string message = receivedMessage.events[0].message.text; //文字訊息
            var replyToken = receivedMessage.events[0].replyToken; //回傳的 token

            //Try to get customer info
            var curCustomer = getCustomerInfo(userid);

            //Check content is Email or not
            bool isEmail = new InfoValidateHelper().validateEmail(message);
            if (isEmail)
            {
                UpdateLineUserInfo(userid, null, message);
                _ReplyMsg = "Email 設定完成";
            }

            if (message != null)
            {
                if (message == "我想成為會員" && curCustomer == null)
                {
                    if (CreateNewUser(userid)) _ReplyMsg = _DicUserStatusRReply["0"];
                    else _ReplyMsg = "建立會員失敗，請稍後再嘗試";
                }

                if (curCustomer?.ApplyStatus == "0" && curCustomer?.Name == null) //NewCustomer
                {
                    if (UpdateLineUserInfo(userid, message, null)) _ReplyMsg = _DicUserStatusRReply["1"];
                    else _ReplyMsg = "會員姓名更新失敗，請稍後再嘗試";
                }

                if (curCustomer?.ApplyStatus == "1" && curCustomer?.Email == null) //NameCompleted
                {
                    if (UpdateLineUserInfo(userid, null, message)) 
                    {
                        _ReplyMsg = _DicUserStatusRReply["2"];
                        var curCustomerInfo = getCustomerInfo(userid);
                        if (curCustomerInfo?.Email != null && curCustomerInfo?.Name != null) _ReplyMsg = $"您的姓名為：{curCustomerInfo?.Name}, \nEmail為：{curCustomerInfo?.Email}, \n請問是否正確？";
                    }
                    else _ReplyMsg = "會員Email更新失敗，請稍後再嘗試";
                }

                if (message == "正確" && curCustomer?.ApplyStatus == "3" && curCustomer?.Email != null && curCustomer?.Name != null) //InfoCompleted
                {
                    if (CreateCustomer(userid))
                    {
                        if (UpdateIsCustomer(userid)) _ReplyMsg = _DicUserStatusRReply["4"];
                    }
                    else _ReplyMsg = _DicUserStatusRReply["3"];
                }
            }

            var replyMessage = new
            {
                replyToken = replyToken,
                messages = new object[] { new {
                type = "text",
                text = _ReplyMsg
                }
                }
            };

            string replyContent = JsonConvert.SerializeObject(replyMessage);
            byte[] replyContentBytes = Encoding.UTF8.GetBytes(replyContent);

            WebClient webClient = new WebClient();
            webClient.Headers.Clear();
            webClient.Headers.Add("Content-Type", "application/json");
            webClient.Headers.Add("Authorization", "Bearer " + _ChannelAccessToken);

            webClient.UploadData(_LineAPI_ReplyMsg, replyContentBytes);

            return new OkResult(request);
        }
        private IDisposable GetAdminScope()
        {
            var userName = "admin";
            if (PXDatabase.Companies.Length > 0)
            {
                var company = PXAccess.GetCompanyName();
                if (string.IsNullOrEmpty(company))
                {
                    company = PXDatabase.Companies[0];
                }
                userName = userName + "@" + company;
            }
            return new PXLoginScope(userName);
        }

        private bool isEmailBeenUsed(string email)
        {
            LumCustomerFromLineBotMaint lumCustomerFromLineBotMaint = PXGraph.CreateInstance<LumCustomerFromLineBotMaint>();
            var curLumCustomerFromLineBot = SelectFrom<LumCustomerFromLineBot>.Where<LumCustomerFromLineBot.email.IsEqual<@P.AsString>>.View.Select(lumCustomerFromLineBotMaint, email);
            bool result = curLumCustomerFromLineBot.TopFirst == null ? false : true;
            return result;
        }

        private LumCustomerFromLineBot getCustomerInfo(string userID)
        {
            return SelectFrom<LumCustomerFromLineBot>.Where<LumCustomerFromLineBot.channelAccessToken.IsEqual<@P.AsString>.And<LumCustomerFromLineBot.userid.IsEqual<@P.AsString>>>.View.Select(PXGraph.CreateInstance<LumCustomerFromLineBotMaint>(), _ChannelAccessToken, userID).TopFirst;
        }

        private bool CreateNewUser(string userID)
        {
            try
            {
                LumCustomerFromLineBotMaint lumCustomerFromLineBotMaint = PXGraph.CreateInstance<LumCustomerFromLineBotMaint>();
                LumCustomerFromLineBot lumCustomerFromLineBot = lumCustomerFromLineBotMaint.LumCustomerFromLineBotView.Insert();
                lumCustomerFromLineBot.ChannelAccessToken = _ChannelAccessToken;
                lumCustomerFromLineBot.Userid = userID;
                lumCustomerFromLineBot.ApplyStatus = _DicUserStatus["NewCustomer"];
                lumCustomerFromLineBotMaint.Actions.PressSave();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool UpdateLineUserInfo(string userID, string name, string email)
        {
            try
            {
                LumCustomerFromLineBotMaint lumCustomerFromLineBotMaint = PXGraph.CreateInstance<LumCustomerFromLineBotMaint>();
                LumCustomerFromLineBot lumCustomerFromLineBot = SelectFrom<LumCustomerFromLineBot>.
                                                                Where<LumCustomerFromLineBot.channelAccessToken.IsEqual<@P.AsString>.
                                                                And<LumCustomerFromLineBot.userid.IsEqual<@P.AsString>>>.View.
                                                                Select(lumCustomerFromLineBotMaint, _ChannelAccessToken, userID).TopFirst;

                if (email != null && new InfoValidateHelper().validateEmail(email))
                {
                    lumCustomerFromLineBot.Email = email;
                    lumCustomerFromLineBot.ApplyStatus = _DicUserStatus["EmailCompleted"];
                }

                if (name != null && name.Length > 0)
                {
                    lumCustomerFromLineBot.Name = name;
                    lumCustomerFromLineBot.ApplyStatus = _DicUserStatus["NameCompleted"];
                }

                lumCustomerFromLineBotMaint.LumCustomerFromLineBotView.Update(lumCustomerFromLineBot);
                lumCustomerFromLineBotMaint.Actions.PressSave();

                if (lumCustomerFromLineBot?.Name != null && lumCustomerFromLineBot?.Email != null)
                {
                    lumCustomerFromLineBot.ApplyStatus = _DicUserStatus["InfoCompleted"];
                    lumCustomerFromLineBotMaint.LumCustomerFromLineBotView.Update(lumCustomerFromLineBot);
                    lumCustomerFromLineBotMaint.Actions.PressSave();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool CreateCustomer(string userid)
        {
            bool result = false;
            try
            {
                var curCustomer = getCustomerInfo(userid);
                
                CustomerMaint customerMaint = PXGraph.CreateInstance<CustomerMaint>();
                Customer customer = new Customer();
                //auto seq ID
                customer.AcctName = curCustomer.Name;
                customerMaint.BAccount.Insert(customer);
                //Update Email
                var defContact = customerMaint.GetExtension<CustomerMaint.DefContactAddressExt>();
                defContact.DefContact.Current.EMail = curCustomer.Email;

                customerMaint.BAccount.Update(customer);
                customerMaint.Actions.PressSave();
                _ReplyMsg = "會員新增完成。";
                result = true;
            }
            catch (Exception ex)
            {
                _ReplyMsg = "新增會員出現錯誤。";
            }
            return result;
        }

        private bool UpdateIsCustomer(string userID)
        {
            bool result = false;
            try 
            {
                LumCustomerFromLineBotMaint lumCustomerFromLineBotMaint = PXGraph.CreateInstance<LumCustomerFromLineBotMaint>();
                LumCustomerFromLineBot lumCustomerFromLineBot = SelectFrom<LumCustomerFromLineBot>.
                                                                Where<LumCustomerFromLineBot.channelAccessToken.IsEqual<@P.AsString>.
                                                                And<LumCustomerFromLineBot.userid.IsEqual<@P.AsString>>>.View.
                                                                Select(lumCustomerFromLineBotMaint, _ChannelAccessToken, userID).TopFirst;
                if (lumCustomerFromLineBot != null && lumCustomerFromLineBot.Email != null && lumCustomerFromLineBot.Name != null)
                {
                    lumCustomerFromLineBot.IsCustomer = true;
                    lumCustomerFromLineBot.CreatedCustomerDateTime = DateTime.Now;
                    lumCustomerFromLineBotMaint.LumCustomerFromLineBotView.Update(lumCustomerFromLineBot);
                    lumCustomerFromLineBotMaint.Actions.PressSave();

                    result = true;
                }
            }
            catch (Exception ex)
            {
                _ReplyMsg = "更新會員完成狀態出現錯誤。";
            }
            return result;
        }

    }
}
