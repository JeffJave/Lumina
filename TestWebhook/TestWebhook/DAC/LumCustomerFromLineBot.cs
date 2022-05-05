using System;
using PX.Data;

namespace TestWebhook.DAC
{
    [Serializable]
    [PXCacheName("LumCustomerFromLineBot")]
    public class LumCustomerFromLineBot : IBqlTable
    {
        #region ChannelAccessToken
        [PXDBString(256, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Channel Access Token")]
        public virtual string ChannelAccessToken { get; set; }
        public abstract class channelAccessToken : PX.Data.BQL.BqlString.Field<channelAccessToken> { }
        #endregion

        #region Userid
        [PXDBString(64, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Userid")]
        public virtual string Userid { get; set; }
        public abstract class userid : PX.Data.BQL.BqlString.Field<userid> { }
        #endregion

        #region ApplyStatus
        [PXDBString(1, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Apply Status")]
        public virtual string ApplyStatus { get; set; }
        public abstract class applyStatus : PX.Data.BQL.BqlString.Field<applyStatus> { }
        #endregion

        #region Email
        [PXDBString(64, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Email")]
        public virtual string Email { get; set; }
        public abstract class email : PX.Data.BQL.BqlString.Field<email> { }
        #endregion

        #region Name
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Name")]
        public virtual string Name { get; set; }
        public abstract class name : PX.Data.BQL.BqlString.Field<name> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region IsCustomer
        [PXDBBool()]
        [PXUIField(DisplayName = "isCustomer")]
        public virtual bool? IsCustomer { get; set; }
        public abstract class isCustomer : PX.Data.BQL.BqlBool.Field<isCustomer> { }
        #endregion

        #region CreatedCustomerDateTime
        [PXDBDate()]
        [PXUIField(DisplayName = "Created Customer DateTime")]
        public virtual DateTime? CreatedCustomerDateTime { get; set; }
        public abstract class createdCustomerDateTime : PX.Data.BQL.BqlDateTime.Field<createdCustomerDateTime> { }
        #endregion
    }
}