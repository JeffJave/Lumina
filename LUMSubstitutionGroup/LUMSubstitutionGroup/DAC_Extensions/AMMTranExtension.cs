using JAMS.AM;
using System;
using System.Collections;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.AM;

namespace JAMS.AM
{
    public class AMMTranExt : PXCacheExtension<AMMTran>
    {
        #region UsrRefLineNbr
        [PXDBInt()]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Subst. Line")]
        public virtual int? UsrRefLineNbr { get; set; }
        public abstract class usrRefLineNbr : PX.Data.BQL.BqlInt.Field<usrRefLineNbr> { }
        #endregion
    }
}