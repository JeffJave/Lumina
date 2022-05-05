using System;
using System.Collections;
using PX.Data;
using PX.Data.BQL;

namespace LUMSubstitutionGroup.DAC
{
    public class LUMSubstitutionGroupCustomSelector : PXCustomSelectorAttribute
    {
        public LUMSubstitutionGroupCustomSelector() : base(typeof(SubstitutionGroup.substitutionGroupID),
                                                           typeof(SubstitutionGroup.substitutionGroupStatus),
                                                           typeof(SubstitutionGroup.substitutionGroupDate),
                                                           typeof(SubstitutionGroup.descripton))
        { }

        protected virtual IEnumerable GetRecords()
        {
            foreach (SubstitutionGroup substitutionGroup in PXSelect<SubstitutionGroup, Where<SubstitutionGroup.substitutionGroupID, NotEqual<Required<SubstitutionGroup.substitutionGroupID>>>, 
                                                                                        OrderBy<SubstitutionGroup.substitutionGroupID.Asc>>.Select(this._Graph, "<NEW>"))
            {
                yield return substitutionGroup;
            }
        }
    }

    [Serializable]
    [PXCacheName("SubstitutionGroup")]
    public class SubstitutionGroup : IBqlTable
    {
        #region SubstitutionGroupID
        [PXDBString(20, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Subst. Group ID")]
        [LUMSubstitutionGroupCustomSelector(DirtyRead = true)]
        [PXDefault("<NEW>")]
        public virtual string SubstitutionGroupID { get; set; }
        public abstract class substitutionGroupID : PX.Data.BQL.BqlString.Field<substitutionGroupID> { }
        #endregion

        #region SubstitutionGroupStatus
        [PXDBString(2, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Status", Enabled = false)]
        [PXStringList(
                new string[] { "0", "1", "2", "3" },
                new string[] { "On Hold", "Submitted", "Approved", "Cancelled" })]
        [PXDefault("0")]
        public virtual string SubstitutionGroupStatus { get; set; }
        public abstract class substitutionGroupStatus : PX.Data.BQL.BqlString.Field<substitutionGroupStatus> { }
        #endregion

        #region SubstitutionGroupDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Date")]
        [PXDefault(typeof(AccessInfo.businessDate))]
        public virtual DateTime? SubstitutionGroupDate { get; set; }
        public abstract class substitutionGroupDate : PX.Data.BQL.BqlDateTime.Field<substitutionGroupDate> { }
        #endregion

        #region Descripton
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Descripton")]
        public virtual string Descripton { get; set; }
        public abstract class descripton : PX.Data.BQL.BqlString.Field<descripton> { }
        #endregion

        #region LineCntr
        [PXDBInt()]
        [PXUIField(DisplayName = "Line Cntr")]
        [PXDefault(0)]
        public virtual int? LineCntr { get; set; }
        public abstract class lineCntr : PX.Data.BQL.BqlInt.Field<lineCntr> { }
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
    }
}