using System;
using PX.Objects;
using PX.Data;
using System.Collections;
using LUMSubstitutionGroup.DAC;
using PX.Objects.IN;
using PX.Data.BQL;
using System.Linq;
using PX.Objects.TX;
using PX.Objects.AM;
using PX.Data.BQL.Fluent;

namespace JAMS.AM
{
    public class MaterialEntry_Extension : PXGraphExtension<MaterialEntry>
    {
        public SelectFrom<SubstitutionGroupLine>.
                LeftJoin<SubstitutionGroupLine2>.On<SubstitutionGroupLine2.substitutionGroupID.IsEqual<SubstitutionGroupLine.substitutionGroupID>>.
                Where<SubstitutionGroupLine.isActive.IsEqual<True>.And<SubstitutionGroupLine2.inventoryID.IsEqual<AMMTran.inventoryID.FromCurrent>>.And<SubstitutionGroupLine.virQtyOnHand.IsGreater<Zero>>>.
                OrderBy<Asc<SubstitutionGroupLine.lineNbr>>.View SubstitutionGroupLineView;

        public override void Initialize()
        {
            base.Initialize();
            SubstitutionGroupLineView.AllowDelete = SubstitutionGroupLineView.AllowInsert = SubstitutionGroupLineView.AllowUpdate = false;
        }

        #region ADD button
        public PXAction<AMMTran> Add;
        [PXButton(CommitChanges = true)]
        protected void add()
        {
            SubstitutionGroupLine curSubstitutionGroupLine = Base.Caches[typeof(SubstitutionGroupLine)].Current as SubstitutionGroupLine;
            AMMTran curAMMTran = Base.Caches[typeof(AMMTran)].Current as AMMTran;

            //check inventory between SubstitutionGroupLine and AMMTran
            if (curAMMTran.InventoryID != curSubstitutionGroupLine.InventoryID)
            {
                //Copy Current AMMTran Line and Alternative Parts Number(inventoryID) from PopUp Panel
                AMMTran substitutionAMMTran = Base.transactions.Insert();
                substitutionAMMTran.ProdOrdID = curAMMTran.ProdOrdID;
                substitutionAMMTran.OperationID = curAMMTran.OperationID;
                substitutionAMMTran.InventoryID = curSubstitutionGroupLine.InventoryID;
                substitutionAMMTran.Qty = curAMMTran.Qty;

                //Set Reference Line Nbr = Original Line Nbr
                AMMTranExt aMMTranExt = substitutionAMMTran.GetExtension<AMMTranExt>();
                aMMTranExt.UsrRefLineNbr = curAMMTran.LineNbr;
                Base.transactions.Update(substitutionAMMTran);
                //Base.Save.Press();

                //Set Current AMMTran Line: Qty = 0
                curAMMTran.Qty = 0;
                Base.transactions.Cache.RaiseFieldUpdated<AMMTran.qty>(curAMMTran, null);
                //Base.transactions.Update(curAMMTran);
                //Base.Save.Press();
            }
        }
        #endregion

        #region Event Handlers

        #endregion
    }

    #region Unbound DAC
    [Serializable]
    [PXCacheName("SubstitutionGroupLine")]
    [PXProjection(typeof(Select<SubstitutionGroupLine>))]
    public class SubstitutionGroupLine2 : IBqlTable
    {
        #region SubstitutionGroupID
        [PXDBString(20, IsKey = true, IsUnicode = true, InputMask = "", BqlField = typeof(SubstitutionGroupLine.substitutionGroupID))]
        [PXUIField(DisplayName = "Subst. Group ID", Enabled = false, Visible = false)]
        public virtual string SubstitutionGroupID { get; set; }
        public abstract class substitutionGroupID : PX.Data.BQL.BqlString.Field<substitutionGroupID> { }
        #endregion

        #region LineNbr
        [PXDBInt(IsKey = true, BqlField = typeof(SubstitutionGroupLine.lineNbr))]
        [PXUIField(DisplayName = "Line Nbr", Enabled = false, Visible = false)]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion

        #region InventoryID
        [StockItem(IsKey = true, DirtyRead = true, DisplayName = "Inventory ID", TabOrder = 1, BqlField = typeof(SubstitutionGroupLine.inventoryID))]
        [PXDefault()]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region IsActive
        [PXDBBool(BqlField = typeof(SubstitutionGroupLine.isActive))]
        [PXUIField(DisplayName = "Is Active")]
        [PXDefault(true)]
        public virtual bool? IsActive { get; set; }
        public abstract class isActive : PX.Data.BQL.BqlBool.Field<isActive> { }
        #endregion

        #region SubstitutionGroupType
        [PXDBString(20, IsUnicode = true, InputMask = "", BqlField = typeof(SubstitutionGroupLine.substitutionGroupType))]
        [PXUIField(DisplayName = "Substitution Type", Enabled = false)]
        [PXDefault("Alternative")]
        public virtual string SubstitutionGroupType { get; set; }
        public abstract class substitutionGroupType : PX.Data.BQL.BqlString.Field<substitutionGroupType> { }
        #endregion
    }
    #endregion
}