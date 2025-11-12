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
using System.Text;

namespace StarLaiPortal.Module.BusinessObjects.Setup
{
    [XafDisplayName("Security Control")]
    [NavigationItem("Setup")]
    [Appearance("HideNew", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideLink", AppearanceItemType.Action, "True", TargetItems = "Link", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideUnlink", AppearanceItemType.Action, "True", TargetItems = "Unlink", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideResetViewSetting", AppearanceItemType.Action, "True", TargetItems = "ResetViewSettings", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideExport", AppearanceItemType.Action, "True", TargetItems = "Export", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideRefresh", AppearanceItemType.Action, "True", TargetItems = "Refresh", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("SaveDocRecord", AppearanceItemType = "Action", TargetItems = "Save", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("SaveAndCloseDocRecord", AppearanceItemType = "Action", TargetItems = "SaveAndClose", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideCancel", AppearanceItemType.Action, "True", TargetItems = "Cancel", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    public class SecurityControl : XPObject
    { 
        public SecurityControl(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        private string _AccessRestrictedFrom;
        [XafDisplayName("From")]
        [ModelDefault("DisplayFormat", "{0: HH:mm}")]
        [ModelDefault("EditMask", "HH:mm")]
        [Index(0), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        public string AccessRestrictedFrom
        {
            get { return _AccessRestrictedFrom; }
            set
            {
                SetPropertyValue("AccessRestrictedFrom", ref _AccessRestrictedFrom, value);
            }
        }

        private string _AccessRestrictedTo;
        [XafDisplayName("To")]
        [ModelDefault("DisplayFormat", "{0: HH:mm}")]
        [ModelDefault("EditMask", "HH:mm")]
        [Index(3), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        public string AccessRestrictedTo
        {
            get { return _AccessRestrictedTo; }
            set
            {
                SetPropertyValue("AccessRestrictedTo", ref _AccessRestrictedTo, value);
            }
        }

        private int _PasswordExpiredPeriod;
        [XafDisplayName("Password Expired Period (Days)")]
        [Index(5), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        public int PasswordExpiredPeriod
        {
            get { return _PasswordExpiredPeriod; }
            set
            {
                SetPropertyValue("PasswordExpiredPeriod", ref _PasswordExpiredPeriod, value);
            }
        }

        private int _FailedLoginCount;
        [XafDisplayName("Max Failed Login Count")]
        [Index(8), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        public int FailedLoginCount
        {
            get { return _FailedLoginCount; }
            set
            {
                SetPropertyValue("FailedLoginCount", ref _FailedLoginCount, value);
            }
        }
    }
}