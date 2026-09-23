using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.XtraPrinting.Native;
using StarLaiPortal.Module.BusinessObjects.Purchase_Order;
using StarLaiPortal.Module.BusinessObjects.Setup;
using StarLaiPortal.Module.BusinessObjects.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace StarLaiPortal.Module.BusinessObjects.Delivery_Order
{
    [XafDisplayName("EM Container")]
    [NavigationItem("Delivery Order")]
    [DefaultProperty("DocNum")]

    [Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Criteria = "not (Status in (0, 3, 6))", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideSubmit", AppearanceItemType.Action, "True", TargetItems = "SubmitEMC", Criteria = "not (Status in (0))", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideCancel", AppearanceItemType.Action, "True", TargetItems = "CancelEMC", Criteria = "not (Status in (0))", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideFullTextSearch", AppearanceItemType.Action, "True", TargetItems = "FullTextSearch", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]

    [Appearance("HideImportDO", AppearanceItemType.Action, "True", TargetItems = "ImportEMDO", Criteria = "not (Status in (0)) or Customer = null", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideImportEXTDO", AppearanceItemType.Action, "True", TargetItems = "ImportExternalDO", Criteria = "not (Status in (0)) or Customer = null", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideCopyFromDO", AppearanceItemType.Action, "True", TargetItems = "CopyFromDO", Criteria = "not (Status in (0))", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    public class EMContainer : XPObject
    { 
        public EMContainer(Session session)
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
            DocDate = DateTime.Now;

            DocType = DocTypeList.EMC;
            Status = DocStatus.Draft;
        }

        private ApplicationUser _CreateUser;
        [XafDisplayName("Create User")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Appearance("CreateUser", Enabled = false)]
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
        [Appearance("CreateDate", Enabled = false)]
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
        [Appearance("UpdateUser", Enabled = false)]
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
        [Appearance("UpdateDate", Enabled = false)]
        public DateTime? UpdateDate
        {
            get { return _UpdateDate; }
            set
            {
                SetPropertyValue("UpdateDate", ref _UpdateDate, value);
            }
        }

        private DocTypeList _DocType;
        [Appearance("DocType", Enabled = false, Criteria = "not IsNew")]
        [Index(304), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public virtual DocTypeList DocType
        {
            get { return _DocType; }
            set
            {
                SetPropertyValue("DocType", ref _DocType, value);
            }
        }

        private string _DocNum;
        [XafDisplayName("Doc. No.")]
        [Appearance("DocNum", Enabled = false)]
        [Index(3), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public string DocNum
        {
            get { return _DocNum; }
            set
            {
                SetPropertyValue("DocNum", ref _DocNum, value);
            }
        }

        private vwBusniessPartner _Customer;
        [XafDisplayName("Customer")]
        [NoForeignKey]
        [ImmediatePostData]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [DataSourceCriteria("ValidFor = 'Y' and CardType = 'C'")]
        [Appearance("Customer", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(5), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public vwBusniessPartner Customer
        {
            get { return _Customer; }
            set
            {
                SetPropertyValue("Customer", ref _Customer, value);
                if (!IsLoading && value != null)
                {
                    CustomerName = Customer.BPName;
                }
                else if (!IsLoading && value == null)
                {
                    CustomerName = null;
                }
            }
        }

        private string _CustomerName;
        [XafDisplayName("Customer Name")]
        [Appearance("CustomerName", Enabled = false)]
        [Index(8), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public string CustomerName
        {
            get { return _CustomerName; }
            set
            {
                SetPropertyValue("CustomerName", ref _CustomerName, value);
            }
        }

        private DateTime _DocDate;
        [XafDisplayName("Date")]
        [Index(10), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public DateTime DocDate
        {
            get { return _DocDate; }
            set
            {
                SetPropertyValue("_DocDate", ref _DocDate, value);
            }
        }

        private DateTime _InvoiceDate;
        [XafDisplayName("Invoice  Date")]
        [Index(13), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public DateTime InvoiceDate
        {
            get { return _InvoiceDate; }
            set
            {
                SetPropertyValue("InvoiceDate", ref _InvoiceDate, value);
            }
        }

        private DocStatus _Status;
        [XafDisplayName("Status")]
        [Appearance("Status", Enabled = false)]
        [Index(15), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public DocStatus Status
        {
            get { return _Status; }
            set
            {
                SetPropertyValue("Status", ref _Status, value);
            }
        }

        private string _ContainerNo;
        [XafDisplayName("Container No")]
        [Index(18), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        [Size(30)]
        public string ContainerNo
        {
            get { return _ContainerNo; }
            set
            {
                SetPropertyValue("ContainerNo", ref _ContainerNo, value);
            }
        }

        private string _DestinationPort;
        [XafDisplayName("Destination Port")]
        [Index(20), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        [Size(30)]
        public string DestinationPort
        {
            get { return _DestinationPort; }
            set
            {
                SetPropertyValue("DestinationPort", ref _DestinationPort, value);
            }
        }

        private string _JobNo;
        [XafDisplayName("Job No")]
        [Index(23), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        [Size(30)]
        public string JobNo
        {
            get { return _JobNo; }
            set
            {
                SetPropertyValue("JobNo", ref _JobNo, value);
            }
        }

        private string _VesselNumber;
        [XafDisplayName("Vessel Number")]
        [Index(25), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        [Size(30)]
        public string VesselNumber
        {
            get { return _VesselNumber; }
            set
            {
                SetPropertyValue("VesselNumber", ref _VesselNumber, value);
            }
        }

        private DateTime _EstimateArrivalDate;
        [XafDisplayName("Estimate Arrival Date")]
        [Index(28), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public DateTime EstimateArrivalDate
        {
            get { return _EstimateArrivalDate; }
            set
            {
                SetPropertyValue("EstimateArrivalDate", ref _EstimateArrivalDate, value);
            }
        }

        private DateTime _ActualArrivalDate;
        [XafDisplayName("Actual Arrival Date")]
        [Index(30), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public DateTime ActualArrivalDate
        {
            get { return _ActualArrivalDate; }
            set
            {
                SetPropertyValue("ActualArrivalDate", ref _ActualArrivalDate, value);
            }
        }

        private DateTime _SealingDate;
        [XafDisplayName("Sealing Date")]
        [Index(33), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public DateTime SealingDate
        {
            get { return _SealingDate; }
            set
            {
                SetPropertyValue("SealingDate", ref _SealingDate, value);
            }
        }

        private DateTime _CIPLDrafted;
        [XafDisplayName("CIPL Drafted")]
        [Index(35), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public DateTime CIPLDrafted
        {
            get { return _CIPLDrafted; }
            set
            {
                SetPropertyValue("CIPLDrafted", ref _CIPLDrafted, value);
            }
        }

        private DateTime _CustomDocSubmitted;
        [XafDisplayName("Custom Doc Submitted")]
        [Index(38), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public DateTime CustomDocSubmitted
        {
            get { return _CustomDocSubmitted; }
            set
            {
                SetPropertyValue("CustomDocSubmitted", ref _CustomDocSubmitted, value);
            }
        }

        private DateTime _PullOutDate;
        [XafDisplayName("Pull Out Date")]
        [Index(40), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public DateTime PullOutDate
        {
            get { return _PullOutDate; }
            set
            {
                SetPropertyValue("PullOutDate", ref _PullOutDate, value);
            }
        }

        private DateTime _PortOfLoadingEta;
        [XafDisplayName("Port Of Loading Eta")]
        [ModelDefault("DisplayFormat", "{0: dd/MM/yyyy hh:mm tt}")]
        [Index(43), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public DateTime PortOfLoadingEta
        {
            get { return _PortOfLoadingEta; }
            set
            {
                SetPropertyValue("PortOfLoadingEta", ref _PortOfLoadingEta, value);
            }
        }

        private DateTime _ActualTimeDeparture;
        [XafDisplayName("Actual Time Departure")]
        [ModelDefault("DisplayFormat", "{0: dd/MM/yyyy hh:mm tt}")]
        [Index(45), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public DateTime ActualTimeDeparture
        {
            get { return _ActualTimeDeparture; }
            set
            {
                SetPropertyValue("ActualTimeDeparture", ref _ActualTimeDeparture, value);
            }
        }

        private DateTime _FinalDestinationETA;
        [XafDisplayName("Final Destination ETA")]
        [ModelDefault("DisplayFormat", "{0: dd/MM/yyyy hh:mm tt}")]
        [Index(48), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public DateTime FinalDestinationETA
        {
            get { return _FinalDestinationETA; }
            set
            {
                SetPropertyValue("FinalDestinationETA", ref _FinalDestinationETA, value);
            }
        }

        private DateTime _ActualTimeArrival;
        [XafDisplayName("Actual Time Arrival")]
        [ModelDefault("DisplayFormat", "{0: dd/MM/yyyy hh:mm tt}")]
        [Index(50), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public DateTime ActualTimeArrival
        {
            get { return _ActualTimeArrival; }
            set
            {
                SetPropertyValue("ActualTimeArrival", ref _ActualTimeArrival, value);
            }
        }

        private string _Remarks;
        [XafDisplayName("Remarks")]
        [Size(100)]
        [ModelDefault("RowCount", "2")]
        [Index(80), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public string Remarks
        {
            get { return _Remarks; }
            set
            {
                SetPropertyValue("Remarks", ref _Remarks, value);
            }
        }

        private bool _SapINV;
        [XafDisplayName("SapINV")]
        [Index(98), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public bool SapINV
        {
            get { return _SapINV; }
            set
            {
                SetPropertyValue("SapINV", ref _SapINV, value);
            }
        }

        private string _SAPINVDocNum;
        [XafDisplayName("SAP AR Inv Num")]
        [Appearance("SAPINVDocNum", Enabled = false)]
        [Index(99), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public string SAPINVDocNum
        {
            get { return _SAPINVDocNum; }
            set
            {
                SetPropertyValue("SAPINVDocNum", ref _SAPINVDocNum, value);
            }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        [Browsable(false)]
        public bool IsValid
        {
            get
            {
                foreach (EMContainerDODetails dtl in this.EMContainerDODetails)
                {
                    return true;
                }

                return false;
            }
        }

        [Association("EMContainer-EMContainerDO")]
        [XafDisplayName("DO")]
        [Appearance("EMContainerDO", Enabled = false, Criteria = "not (Status in (0))")]
        public XPCollection<EMContainerDO> EMContainerDO
        {
            get { return GetCollection<EMContainerDO>("EMContainerDO"); }
        }

        [Association("EMContainer-EMContainerDODetails")]
        [XafDisplayName("DO Details")]
        [Appearance("EMContainerDODetails", Enabled = false, Criteria = "not (Status in (0))")]
        public XPCollection<EMContainerDODetails> EMContainerDODetails
        {
            get { return GetCollection<EMContainerDODetails>("EMContainerDODetails"); }
        }

        [Association("EMContainer-EMContainerExtDO")]
        [XafDisplayName("External DO")]
        [Appearance("EMContainerExtDO", Enabled = false, Criteria = "not (Status in (0))")]
        public XPCollection<EMContainerExtDO> EMContainerExtDO
        {
            get { return GetCollection<EMContainerExtDO>("EMContainerExtDO"); }
        }

        [Association("EMContainer-EMContainerDocTrail")]
        [XafDisplayName("Status History")]
        public XPCollection<EMContainerDocTrail> EMContainerDocTrail
        {
            get { return GetCollection<EMContainerDocTrail>("EMContainerDocTrail"); }
        }

        private XPCollection<AuditDataItemPersistent> auditTrail;
        public XPCollection<AuditDataItemPersistent> AuditTrail
        {
            get
            {
                if (auditTrail == null)
                {
                    auditTrail = AuditedObjectWeakReference.GetAuditTrail(Session, this);
                }
                return auditTrail;
            }
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
                    EMContainerDocTrail ds = new EMContainerDocTrail(Session);
                    ds.DocStatus = DocStatus.Draft;
                    ds.DocRemarks = "";
                    if (user != null)
                    {
                        ds.CreateUser = Session.GetObjectByKey<ApplicationUser>(user.Oid);
                        ds.UpdateUser = Session.GetObjectByKey<ApplicationUser>(user.Oid);
                    }
                    else
                    {
                        ds.CreateUser = Session.GetObjectByKey<ApplicationUser>(Guid.Parse("100348B5-290E-47DF-9355-557C7E2C56D3"));
                        ds.UpdateUser = Session.GetObjectByKey<ApplicationUser>(Guid.Parse("100348B5-290E-47DF-9355-557C7E2C56D3"));
                    }
                    ds.CreateDate = DateTime.Now;
                    ds.UpdateDate = DateTime.Now;
                    this.EMContainerDocTrail.Add(ds);
                }
            }
        }
    }
}