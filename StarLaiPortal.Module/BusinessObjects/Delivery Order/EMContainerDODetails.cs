using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using StarLaiPortal.Module.BusinessObjects.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace StarLaiPortal.Module.BusinessObjects.Delivery_Order
{
    [Appearance("HideNew", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("LinkDoc", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("UnlinkDoc", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [XafDisplayName("EM Container DO Details")]
    public class EMContainerDODetails : XPObject
    { 
        public EMContainerDODetails(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();

            ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;
            if (user != null)
            {
                CreateUser = Session.GetObjectByKey<ApplicationUser>(user.Oid);
            }
            else
            {
                CreateUser = Session.GetObjectByKey<ApplicationUser>(Guid.Parse("100348B5-290E-47DF-9355-557C7E2C56D3"));
            }
            CreateDate = DateTime.Now;
        }

        private ApplicationUser _CreateUser;
        [XafDisplayName("Create User")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(300), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public ApplicationUser CreateUser
        {
            get { return _CreateUser; }
            set
            {
                SetPropertyValue("CreateUser", ref _CreateUser, value);
            }
        }

        private DateTime? _CreateDate;
        [Index(301), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public DateTime? CreateDate
        {
            get { return _CreateDate; }
            set
            {
                SetPropertyValue("CreateDate", ref _CreateDate, value);
            }
        }

        private ApplicationUser _UpdateUser;
        [XafDisplayName("Update User"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(302), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public ApplicationUser UpdateUser
        {
            get { return _UpdateUser; }
            set
            {
                SetPropertyValue("UpdateUser", ref _UpdateUser, value);
            }
        }

        private DateTime? _UpdateDate;
        [Index(303), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public DateTime? UpdateDate
        {
            get { return _UpdateDate; }
            set
            {
                SetPropertyValue("UpdateDate", ref _UpdateDate, value);
            }
        }

        private vwItemMasters _ItemCode;
        [ImmediatePostData]
        [NoForeignKey]
        [XafDisplayName("Item Code")]
        // Start ver 1.0.15
        //[LookupEditorMode(LookupEditorMode.AllItems)]
        // End ver 1.0.15
        [Index(1), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("ItemCode", Enabled = false)]
        [RuleRequiredField(DefaultContexts.Save)]
        public vwItemMasters ItemCode
        {
            get { return _ItemCode; }
            set
            {
                SetPropertyValue("ItemCode", ref _ItemCode, value);
                if (!IsLoading && value != null)
                {
                    ItemDesc = ItemCode.ItemName;
                    LegacyItemCode = ItemCode.LegacyItemCode;
                    CatalogNo = ItemCode.CatalogNo;
                }
                else if (!IsLoading && value != null)
                {
                    ItemDesc = null;
                    LegacyItemCode = null;
                    CatalogNo = null;
                }
            }
        }

        private string _LegacyItemCode;
        [XafDisplayName("Legacy Item Code")]
        [Index(3), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("LegacyItemCode", Enabled = false)]
        public string LegacyItemCode

        {
            get { return _LegacyItemCode; }
            set
            {
                SetPropertyValue("LegacyItemCode", ref _LegacyItemCode, value);
            }
        }

        private string _ItemDesc;
        [RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Item Description")]
        [Appearance("ItemDesc", Enabled = false)]
        [Index(5), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public string ItemDesc
        {
            get { return _ItemDesc; }
            set
            {
                SetPropertyValue("ItemDesc", ref _ItemDesc, value);
            }
        }

        private string _CatalogNo;
        [XafDisplayName("Catalog No")]
        [Appearance("CatalogNo", Enabled = false)]
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public string CatalogNo
        {
            get { return _CatalogNo; }
            set
            {
                SetPropertyValue("CatalogNo", ref _CatalogNo, value);
            }
        }

        private vwWarehouse _Warehouse;
        [NoForeignKey]
        [RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Warehouse")]
        [Appearance("Warehouse", Enabled = false)]
        [Index(13), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public vwWarehouse Warehouse
        {
            get { return _Warehouse; }
            set
            {
                SetPropertyValue("Warehouse", ref _Warehouse, value);
                if (!IsLoading && value != null)
                {
                    Bin = Session.FindObject<vwBin>(CriteriaOperator.Parse("AbsEntry = ?", Warehouse.DftBinAbs));
                }
            }
        }

        private vwBin _Bin;
        [NoForeignKey]
        [XafDisplayName("Bin")]
        [Appearance("Bin", Enabled = false)]
        [Index(15), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public vwBin Bin
        {
            get { return _Bin; }
            set
            {
                SetPropertyValue("Bin", ref _Bin, value);
            }
        }

        private decimal _Quantity;
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:N0}")]
        [ModelDefault("EditMask", "d")]
        [XafDisplayName("Quantity")]
        [Appearance("Quantity", Enabled = false)]
        [Index(18), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public decimal Quantity
        {
            get { return _Quantity; }
            set
            {
                SetPropertyValue("Quantity", ref _Quantity, value);
            }
        }

        private decimal _Loaded;
        [ImmediatePostData]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:N0}")]
        [ModelDefault("EditMask", "d")]
        [XafDisplayName("Loaded")]
        [Index(20), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public decimal Loaded
        {
            get { return _Loaded; }
            set
            {
                SetPropertyValue("Loaded", ref _Loaded, value);
                if (!IsLoading && value != 0)
                {
                    if (Loaded > Quantity)
                    {
                        Loaded = Quantity;
                    }

                    if (Loaded <= 0)
                    {
                        Loaded = 1;
                    }

                    Total = Loaded * Price;
                    Variance = Loaded - Quantity;
                }
            }
        }

        private decimal _Variance;
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:N0}")]
        [ModelDefault("EditMask", "d")]
        [XafDisplayName("Variance")]
        [Appearance("Variance", Enabled = false)]
        [Index(23), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public decimal Variance
        {
            get { return _Variance; }
            set
            {
                SetPropertyValue("Variance", ref _Variance, value);
            }
        }

        private decimal _Price;
        [ImmediatePostData]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [XafDisplayName("Price")]
        [Index(25), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public decimal Price
        {
            get { return _Price; }
            set
            {
                SetPropertyValue("Price", ref _Price, value);
                if (!IsLoading && value != 0)
                {
                    Total = Quantity * Price;
                }
            }
        }

        private decimal _Total;
        [ImmediatePostData]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [XafDisplayName("Total")]
        [Appearance("Total", Enabled = false)]
        [Index(28), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public decimal Total
        {
            get { return _Total; }
            set
            {
                SetPropertyValue("Total", ref _Total, value);
            }
        }

        private string _BaseDoc;
        [XafDisplayName("DO Number")]
        [Appearance("BaseDoc", Enabled = false)]
        [Index(30), VisibleInListView(true), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public string BaseDoc
        {
            get { return _BaseDoc; }
            set
            {
                SetPropertyValue("BaseDoc", ref _BaseDoc, value);
            }
        }

        private string _BaseId;
        [XafDisplayName("BaseId")]
        [Appearance("BaseId", Enabled = false)]
        [Index(33), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public string BaseId
        {
            get { return _BaseId; }
            set
            {
                SetPropertyValue("BaseId", ref _BaseId, value);
            }
        }

        private DateTime _BaseDate;
        [XafDisplayName("DO Date")]
        [Appearance("BaseDate", Enabled = false)]
        [Index(35), VisibleInListView(true), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public DateTime BaseDate
        {
            get { return _BaseDate; }
            set
            {
                SetPropertyValue("BaseDate", ref _BaseDate, value);
            }
        }

        private int _DOLine;
        [XafDisplayName("DO Line")]
        [Appearance("DOLine", Enabled = false)]
        [Index(38), VisibleInListView(true), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public int DOLine
        {
            get { return _DOLine; }
            set
            {
                SetPropertyValue("DOLine", ref _DOLine, value);
            }
        }

        private string _SODocNum;
        [XafDisplayName("SO Doc Num")]
        [Appearance("SODocNum", Enabled = false)]
        [Index(40), VisibleInListView(true), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public string SODocNum
        {
            get { return _SODocNum; }
            set
            {
                SetPropertyValue("SODocNum", ref _SODocNum, value);
            }
        }

        private string _SOBaseID;
        [XafDisplayName("SO Base ID")]
        [Appearance("SOBaseID", Enabled = false)]
        [Index(43), VisibleInListView(true), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public string SOBaseID
        {
            get { return _SOBaseID; }
            set
            {
                SetPropertyValue("SOBaseID", ref _SOBaseID, value);
            }
        }

        private vwSalesPerson _ContactPerson;
        [NoForeignKey]
        [XafDisplayName("Contact Person")]
        [Appearance("ContactPerson", Enabled = false)]
        [Index(45), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public vwSalesPerson ContactPerson
        {
            get { return _ContactPerson; }
            set
            {
                SetPropertyValue("ContactPerson", ref _ContactPerson, value);
            }
        }

        private string _PickListDocNum;
        [XafDisplayName("PickListDocNum")]
        [Appearance("PickListDocNum", Enabled = false)]
        [Index(48), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public string PickListDocNum
        {
            get { return _PickListDocNum; }
            set
            {
                SetPropertyValue("PickListDocNum", ref _PickListDocNum, value);
            }
        }

        private string _PackListLine;
        [XafDisplayName("PackListLine")]
        [Appearance("PackListLine", Enabled = false)]
        [Index(50), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public string PackListLine
        {
            get { return _PackListLine; }
            set
            {
                SetPropertyValue("PackListLine", ref _PackListLine, value);
            }
        }

        private vwEIVClass _EIVClassification;
        [NoForeignKey]
        [XafDisplayName("Classification")]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("EIVClassification", Enabled = false)]
        [Index(53), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public vwEIVClass EIVClassification
        {
            get { return _EIVClassification; }
            set
            {
                SetPropertyValue("EIVClassification", ref _EIVClassification, value);
            }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        private EMContainer _EMContainer;
        [Association("EMContainer-EMContainerDODetails")]
        [Index(99), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("EMContainer", Enabled = false)]
        public EMContainer EMContainer
        {
            get { return _EMContainer; }
            set { SetPropertyValue("EMContainer", ref _EMContainer, value); }
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if (!(Session is NestedUnitOfWork)
                && (Session.DataLayer != null)
                    && (Session.ObjectLayer is SimpleObjectLayer)
                        )
            {
                ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;
                if (user != null)
                {
                    UpdateUser = Session.GetObjectByKey<ApplicationUser>(user.Oid);
                }
                else
                {
                    UpdateUser = Session.GetObjectByKey<ApplicationUser>(Guid.Parse("100348B5-290E-47DF-9355-557C7E2C56D3"));
                }
                UpdateDate = DateTime.Now;

                if (Session.IsNewObject(this))
                {

                }
            }
        }
    }
}