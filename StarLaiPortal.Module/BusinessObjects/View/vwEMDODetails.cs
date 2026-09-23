using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace StarLaiPortal.Module.BusinessObjects.View
{
    [Appearance("HideNew", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideLink", AppearanceItemType.Action, "True", TargetItems = "Link", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideUnlink", AppearanceItemType.Action, "True", TargetItems = "Unlink", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideResetViewSetting", AppearanceItemType.Action, "True", TargetItems = "ResetViewSettings", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideExport", AppearanceItemType.Action, "True", TargetItems = "Export", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideRefresh", AppearanceItemType.Action, "True", TargetItems = "Refresh", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [XafDisplayName("EM DO Details")]
    public class vwEMDODetails : XPLiteObject
    { 
        public vwEMDODetails(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        [Key]
        [Browsable(true)]
        [XafDisplayName("PriKey")]
        [Appearance("PriKey", Enabled = false)]
        [Index(0), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public string PriKey
        {
            get; set;
        }

        [XafDisplayName("DO Doc Num")]
        [Appearance("PortalNum", Enabled = false)]
        [Index(5), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public string PortalNum
        {
            get; set;
        }

        [XafDisplayName("DO Line Num")]
        [Appearance("DOLineNo", Enabled = false)]
        [Index(8), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public int DOLineNo
        {
            get; set;
        }

        [XafDisplayName("Item Code")]
        [Appearance("ItemCode", Enabled = false)]
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public string ItemCode
        {
            get; set;
        }

        [XafDisplayName("Warehouse")]
        [NoForeignKey]
        [Appearance("Warehouse", Enabled = false)]
        [Index(13), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public vwWarehouse Warehouse
        {
            get; set;
        }

        [XafDisplayName("Bin")]
        [NoForeignKey]
        [Appearance("Bin", Enabled = false)]
        [Index(15), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public vwBin Bin
        {
            get; set;
        }

        [XafDisplayName("Quantity")]
        [Appearance("Quantity", Enabled = false)]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Index(18), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public decimal Quantity
        {
            get; set;
        }

        [XafDisplayName("Price")]
        [Appearance("Price", Enabled = false)]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Index(20), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public decimal Price
        {
            get; set;
        }

        [XafDisplayName("LineOid")]
        [Appearance("LineOid", Enabled = false)]
        [Index(23), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public int LineOid
        {
            get; set;
        }

        [XafDisplayName("Posting Date")]
        [Appearance("PostingDate", Enabled = false)]
        [Index(25), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public DateTime PostingDate
        {
            get; set;
        }

        [XafDisplayName("SODocNum")]
        [Appearance("SODocNum", Enabled = false)]
        [Index(28), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public string SODocNum
        {
            get; set;
        }

        [XafDisplayName("SOBaseID")]
        [Appearance("SOBaseID", Enabled = false)]
        [Index(30), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public string SOBaseID
        {
            get; set;
        }

        [XafDisplayName("ContactPerson")]
        [NoForeignKey]
        [Appearance("ContactPerson", Enabled = false)]
        [Index(33), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public vwSalesPerson ContactPerson
        {
            get; set;
        }

        [XafDisplayName("PickListDocNum")]
        [Appearance("PickListDocNum", Enabled = false)]
        [Index(35), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public string PickListDocNum
        {
            get; set;
        }

        [XafDisplayName("PackListLine")]
        [Appearance("PackListLine", Enabled = false)]
        [Index(38), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public string PackListLine
        {
            get; set;
        }

        [XafDisplayName("EIVClassification")]
        [NoForeignKey]
        [Appearance("EIVClassification", Enabled = false)]
        [Index(40), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public vwEIVClass EIVClassification
        {
            get; set;
        }

        [XafDisplayName("Customer")]
        [NoForeignKey]
        [Appearance("Customer", Enabled = false)]
        [Index(43), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public vwBusniessPartner Customer
        {
            get; set;
        }
    }
}