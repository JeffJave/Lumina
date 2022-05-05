using System;
using JAMS.AM;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.IN;

namespace LUMSubstitutionGroup.DAC
{   
    [Serializable]
    [PXCacheName("SubstitutionGroupLine")]
    public class SubstitutionGroupLine : IBqlTable
    {
        #region SubstitutionGroupID
        [PXDBString(20, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Subst. Group ID", Enabled = false, Visible = false)]
        [PXDBDefault(typeof(SubstitutionGroup.substitutionGroupID))]
        [PXParent(typeof(SelectFrom<SubstitutionGroup>.Where<SubstitutionGroup.substitutionGroupID.IsEqual<SubstitutionGroupLine.substitutionGroupID.FromCurrent>>))]
        public virtual string SubstitutionGroupID { get; set; }
        public abstract class substitutionGroupID : PX.Data.BQL.BqlString.Field<substitutionGroupID> { }
        #endregion

        #region LineNbr
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Line Nbr", Enabled = false, Visible = false)]
        [PXLineNbr(typeof(SubstitutionGroup.lineCntr))]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion

        #region InventoryID
        [StockItem(IsKey = true, DirtyRead = true, DisplayName = "Inventory ID", TabOrder = 1)]
        [PXDefault()]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region IsActive
        [PXDBBool()]
        [PXUIField(DisplayName = "Is Active")]
        [PXDefault(true)]
        public virtual bool? IsActive { get; set; }
        public abstract class isActive : PX.Data.BQL.BqlBool.Field<isActive> { }
        #endregion

        #region SubstitutionGroupType
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Substitution Type", Enabled = false)]
        [PXDefault("Alternative")]
        public virtual string SubstitutionGroupType { get; set; }
        public abstract class substitutionGroupType : PX.Data.BQL.BqlString.Field<substitutionGroupType> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region Virtual Column

        [PXDecimal(4)]
        [PXUIField(DisplayName = "Qty On Hand", Visibility = PXUIVisibility.SelectorVisible)]
        [PXDBScalar(typeof(Search4<INSiteStatus.qtyOnHand, Where<INSiteStatus.inventoryID, Equal<SubstitutionGroupLine.inventoryID>>, Aggregate<GroupBy<INSiteStatus.inventoryID , Sum<INSiteStatus.qtyOnHand>>>>))]
        public virtual Decimal? VirQtyOnHand { get; set; }
        public abstract class virQtyOnHand : PX.Data.BQL.BqlDecimal.Field<virQtyOnHand> { }

        [PXDecimal(4)]
        [PXUIField(DisplayName = "Qty Available", Visibility = PXUIVisibility.SelectorVisible)]
        [PXDBScalar(typeof(Search4<INSiteStatus.qtyAvail, Where<INSiteStatus.inventoryID, Equal<SubstitutionGroupLine.inventoryID>>, Aggregate<GroupBy<INSiteStatus.inventoryID, Sum<INSiteStatus.qtyAvail>>>>))]
        public virtual Decimal? VirQtyAvail { get; set; }
        public abstract class virQtyAvail : PX.Data.BQL.BqlDecimal.Field<virQtyAvail> { }

        #endregion
    }
}