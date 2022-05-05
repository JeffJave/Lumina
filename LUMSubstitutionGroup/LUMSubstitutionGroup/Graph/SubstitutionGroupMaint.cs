using System;
using LUMSubstitutionGroup.DAC;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.IN;

namespace LUMSubstitutionGroup.Graph
{
    public class SubstitutionGroupMaint : PXGraph<SubstitutionGroupMaint, SubstitutionGroup>
    {
        public SelectFrom<SubstitutionGroup>.View SubstitutionGroupView;
        public SelectFrom<SubstitutionGroupLine>.Where<SubstitutionGroupLine.substitutionGroupID.IsEqual<SubstitutionGroup.substitutionGroupID.FromCurrent>>.View SubstitutionGroupLineView;

        #region Event Handler
        protected virtual void SubstitutionGroupLine_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            var substitutionGroupLineCache = this.SubstitutionGroupLineView.Cache.Cached.RowCast<SubstitutionGroupLine>();
            foreach (SubstitutionGroupLine line in substitutionGroupLineCache)
            {
                var sameInvnetoryInDB = SelectFrom<SubstitutionGroupLine>.Where<SubstitutionGroupLine.inventoryID.IsEqual<@P.AsInt>>.View.Select(this, line.InventoryID)?.TopFirst;
                if (sameInvnetoryInDB != null)
                {
                    var iNName = SelectFrom<InventoryItem>.Where<InventoryItem.inventoryID.IsEqual<@P.AsInt>>.View.Select(this, line.InventoryID)?.TopFirst.InventoryCD;
                    if (iNName != null) throw new Exception($"Inventory ID: {iNName} exists in Substitution Group : {sameInvnetoryInDB.SubstitutionGroupID}.");
                }
            }
        }
        #endregion

    }
}