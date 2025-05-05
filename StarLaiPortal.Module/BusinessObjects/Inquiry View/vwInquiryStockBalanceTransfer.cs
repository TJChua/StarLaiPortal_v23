﻿using DevExpress.Data.Filtering;
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

// 2023-10-16 - add available - ver 1.0.11
// 2023-10-16 - add warehouse 52 - ver 1.0.11
// 2024-10-09 - add new process field - ver 1.0.21

namespace StarLaiPortal.Module.BusinessObjects.Inquiry_View
{
    [DefaultClassOptions]
    [NavigationItem("Reports")]
    [XafDisplayName("Stock Balance Transfer")]
    [DefaultProperty("OldCode")]
    [Appearance("HideNew", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideLink", AppearanceItemType.Action, "True", TargetItems = "Link", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideUnlink", AppearanceItemType.Action, "True", TargetItems = "Unlink", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("hideSave", AppearanceItemType = "Action", TargetItems = "Save", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    //[Appearance("HideResetViewSetting", AppearanceItemType.Action, "True", TargetItems = "ResetViewSettings", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideExport", AppearanceItemType.Action, "True", TargetItems = "Export", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideRefresh", AppearanceItemType.Action, "True", TargetItems = "Refresh", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]

    public class vwInquiryStockBalanceTransfer : XPLiteObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public vwInquiryStockBalanceTransfer(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        [Key]
        [Browsable(true)]
        [XafDisplayName("Item Code")]
        [Appearance("ItemCode", Enabled = false)]
        [Index(0), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string ItemCode
        {
            get; set;
        }

        [XafDisplayName("Old Code")]
        [Appearance("LegacyItemCode", Enabled = false)]
        [Index(3)]
        public string LegacyItemCode
        {
            get; set;
        }

        [XafDisplayName("Description")]
        [Appearance("ItemName", Enabled = false)]
        [Index(5)]
        public string ItemName
        {
            get; set;
        }

        [XafDisplayName("Min")]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Appearance("MinLevel", Enabled = false)]
        [Index(8)]
        public decimal MinLevel
        {
            get; set;
        }


        [XafDisplayName("Max")]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Appearance("MaxLevel", Enabled = false)]
        [Index(10)]
        public decimal MaxLevel
        {
            get; set;
        }

        [XafDisplayName("Item Type")]
        [Appearance("ItemType", Enabled = false)]
        [Index(13)]
        public string ItemType
        {
            get; set;
        }

        [XafDisplayName("45(S1)")]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Appearance("S145", Enabled = false)]
        [Index(15)]
        public decimal S145
        {
            get; set;
        }

        [XafDisplayName("18-20(S2)")]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Appearance("S1820", Enabled = false)]
        [Index(20)]
        public decimal S1820
        {
            get; set;
        }

        [XafDisplayName("28(S4)")]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Appearance("S428", Enabled = false)]
        [Index(23)]
        public decimal S428
        {
            get; set;
        }

        [XafDisplayName("On9")]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Appearance("On9", Enabled = false)]
        [Index(24)]
        public decimal On9
        {
            get; set;
        }

        // Start ver 1.0.11
        [XafDisplayName("52")]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Appearance("Whs52", Enabled = false)]
        [Index(25)]
        public decimal Whs52
        {
            get; set;
        }
        // End ver 1.0.11

        [XafDisplayName("Transfer Qty")]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Appearance("TransferQty", Enabled = false)]
        [Index(26)]
        public decimal TransferQty
        {
            get; set;
        }

        // Start ver 1.0.11
        [XafDisplayName("Available")]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Appearance("Available", Enabled = false)]
        [Index(27)]
        public decimal Available
        {
            get; set;
        }
        // End ver 1.0.11

        // Start ver 1.0.21
        [XafDisplayName("45(S1) Process")]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Appearance("S145_Process", Enabled = false)]
        [Index(40)]
        public decimal S145_Process
        {
            get; set;
        }

        [XafDisplayName("18-20(S2) Process")]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Appearance("S1820_Process", Enabled = false)]
        [Index(41)]
        public decimal S1820_Process
        {
            get; set;
        }

        [XafDisplayName("28(S4) Process")]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Appearance("S428_Process", Enabled = false)]
        [Index(42)]
        public decimal S428_Process
        {
            get; set;
        }

        [XafDisplayName("On9 Process")]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Appearance("On9_Process", Enabled = false)]
        [Index(43)]
        public decimal On9_Process
        {
            get; set;
        }

        [XafDisplayName("52 Process")]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Appearance("Whs52_Process", Enabled = false)]
        [Index(44)]
        public decimal Whs52_Process
        {
            get; set;
        }

        [XafDisplayName("Others")]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Appearance("Others", Enabled = false)]
        [Index(45)]
        public decimal Others
        {
            get; set;
        }

        [XafDisplayName("Others Process")]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Appearance("Others_Process", Enabled = false)]
        [Index(46)]
        public decimal Others_Process
        {
            get; set;
        }
        // End ver 1.0.21
    }
}