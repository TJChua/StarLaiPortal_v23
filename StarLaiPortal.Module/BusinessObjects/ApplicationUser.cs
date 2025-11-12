using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using StarLaiPortal.Module.BusinessObjects.Setup;

// 2025-11-10 - Add Security control - ver 1.0.26

namespace StarLaiPortal.Module.BusinessObjects {
    [MapInheritance(MapInheritanceType.ParentTable)]
    [DefaultProperty(nameof(UserName))]
    public class ApplicationUser : PermissionPolicyUser, ISecurityUserWithLoginInfo {
        public ApplicationUser(Session session) : base(session) { }

        [Browsable(false)]
        [DevExpress.Xpo.Aggregated, Association("User-LoginInfo")]
        public XPCollection<ApplicationUserLoginInfo> LoginInfo {
            get { return GetCollection<ApplicationUserLoginInfo>(nameof(LoginInfo)); }
        }

        IEnumerable<ISecurityUserLoginInfo> IOAuthSecurityUser.UserLogins => LoginInfo.OfType<ISecurityUserLoginInfo>();

        ISecurityUserLoginInfo ISecurityUserWithLoginInfo.CreateUserLoginInfo(string loginProviderName, string providerUserKey) {
            ApplicationUserLoginInfo result = new ApplicationUserLoginInfo(Session);
            result.LoginProviderName = loginProviderName;
            result.ProviderUserKey = providerUserKey;
            result.User = this;
            return result;
        }

        private StaffInfo _Staff;
        [RuleRequiredField(DefaultContexts.Save)]
        [NoForeignKey]
        [DataSourceCriteria("IsActive = 'True'")]
        [VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public StaffInfo Staff
        {
            get { return _Staff; }
            set
            {
                SetPropertyValue("Staff", ref _Staff, value);
            }

        }

        // Start ver 1.0.2
        private bool _AccLocked;
        [ImmediatePostData]
        [XafDisplayName("Account Locked")]
        [VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public bool AccLocked
        {
            get { return _AccLocked; }
            set
            {
                SetPropertyValue("AccLocked", ref _AccLocked, value);
                if (!IsLoading)
                {
                    LoginFailedCount = 0;
                }
            }
        }

        private int _LoginFailedCount;
        [XafDisplayName("Login Failed Count")]
        [VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public int LoginFailedCount
        {
            get { return _LoginFailedCount; }
            set
            {
                SetPropertyValue("LoginFailedCount", ref _LoginFailedCount, value);
            }
        }

        private DateTime _LastPasswordChanged;
        [XafDisplayName("Last Password Changed")]
        [VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public DateTime LastPasswordChanged
        {
            get { return _LastPasswordChanged; }
            set
            {
                SetPropertyValue("LastPasswordChanged", ref _LastPasswordChanged, value);
            }
        }
        // End ver 1.0.2
    }
}
