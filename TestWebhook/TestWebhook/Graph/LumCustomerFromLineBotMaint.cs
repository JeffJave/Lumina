using System;
using PX.Data;
using PX.Data.BQL.Fluent;
using TestWebhook.DAC;

namespace TestWebhook.Graph
{
    public class LumCustomerFromLineBotMaint : PXGraph<LumCustomerFromLineBotMaint>
    {

        //public PXSave<LumCustomerFromLineBot> Save;
        //public PXCancel<LumCustomerFromLineBot> Cancel;

        public SelectFrom<LumCustomerFromLineBot>.View LumCustomerFromLineBotView;

        public LumCustomerFromLineBotMaint()
        {
            LumCustomerFromLineBotView.AllowDelete = LumCustomerFromLineBotView.AllowInsert = LumCustomerFromLineBotView.AllowUpdate = false;
        }
    }
}