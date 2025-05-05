﻿using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using StarLaiPortal.Module.BusinessObjects.Pick_List;
using StarLaiPortal.Module.BusinessObjects.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

// 2023-08-16 Add SAPPONo, PortalPoNo, AsnNo ver 1.0.8
// 2023-04-09 fix speed issue ver 1.0.8.1

namespace StarLaiPortal.Module.BusinessObjects.GRN
{
    [DefaultClassOptions]
    [XafDisplayName("GRPO")]
    [NavigationItem("GRPO")]
    [DefaultProperty("DocNum")]
    [Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Criteria = "not (Status in (0))", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideSubmit", AppearanceItemType.Action, "True", TargetItems = "SubmitGRN", Criteria = "not (Status in (0))", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideCancel", AppearanceItemType.Action, "True", TargetItems = "CancelGRN", Criteria = "not (Status in (0))", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]

    [Appearance("HideExportPO", AppearanceItemType.Action, "True", TargetItems = "ExportGRN", Criteria = "DocNum = null", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideImportPO", AppearanceItemType.Action, "True", TargetItems = "ImportGRN", Criteria = "DocNum = null", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]

    public class GRN : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public GRN(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
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
            InvoiceDate = DateTime.Now;
            ETA = DateTime.Now;
            ESR = DateTime.Now;

            Status = DocStatus.Draft;
            DocType = DocTypeList.GRN;
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
        [XafDisplayName("No.")]
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

        private vwBusniessPartner _Supplier;
        [XafDisplayName("Supplier")]
        [NoForeignKey]
        [ImmediatePostData]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [DataSourceCriteria("ValidFor = 'Y' and CardType = 'S'")]
        [Appearance("Supplier", Enabled = false, Criteria = "not IsNew")]
        [Index(5), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public vwBusniessPartner Supplier
        {
            get { return _Supplier; }
            set
            {
                SetPropertyValue("Supplier", ref _Supplier, value);
                if (!IsLoading && value != null)
                {
                    SupplierName = Supplier.BPName;
                }
                else if (!IsLoading && value == null)
                {
                    SupplierName = null;
                }
            }
        }

        private string _SupplierName;
        [XafDisplayName("Supplier Name")]
        [Appearance("SupplierName", Enabled = false)]
        [Index(8), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public string SupplierName
        {
            get { return _SupplierName; }
            set
            {
                SetPropertyValue("SupplierName", ref _SupplierName, value);
            }
        }

        private string _InvoiceNo;
        [XafDisplayName("Invoice No")]
        [Index(10), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public string InvoiceNo
        {
            get { return _InvoiceNo; }
            set
            {
                SetPropertyValue("InvoiceNo", ref _InvoiceNo, value);
            }
        }

        private vwVehicle _Vehicle;
        [NoForeignKey]
        [XafDisplayName("Vehicle")]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [Index(13), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public vwVehicle Vehicle
        {
            get { return _Vehicle; }
            set
            {
                SetPropertyValue("Vehicle", ref _Vehicle, value);
            }
        }

        private string _Container;
        [XafDisplayName("Container")]
        [Index(15), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public string Container
        {
            get { return _Container; }
            set
            {
                SetPropertyValue("Container", ref _Container, value);
            }
        }

        private vwWarehouse _Warehouse;
        [NoForeignKey]
        [XafDisplayName("Warehouse")]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [Index(18), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public vwWarehouse Warehouse
        {
            get { return _Warehouse; }
            set
            {
                SetPropertyValue("Warehouse", ref _Warehouse, value);
            }
        }

        private DateTime _DocDate;
        [XafDisplayName("Date")]
        [Index(20), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public DateTime DocDate
        {
            get { return _DocDate; }
            set
            {
                SetPropertyValue("_DocDate", ref _DocDate, value);
            }
        }

        private DateTime _InvoiceDate;
        [XafDisplayName("Invoice Date")]
        [Index(23), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public DateTime InvoiceDate
        {
            get { return _InvoiceDate; }
            set
            {
                SetPropertyValue("InvoiceDate", ref _InvoiceDate, value);
            }
        }

        private DateTime _ETA;
        [XafDisplayName("ETA")]
        [Index(25), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public DateTime ETA
        {
            get { return _ETA; }
            set
            {
                SetPropertyValue("ETA", ref _ETA, value);
            }
        }

        private DateTime _ESR;
        [XafDisplayName("ESR")]
        [Index(28), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public DateTime ESR
        {
            get { return _ESR; }
            set
            {
                SetPropertyValue("ESR", ref _ESR, value);
            }
        }

        private DocStatus _Status;
        [XafDisplayName("Status")]
        [Appearance("Status", Enabled = false)]
        [Index(30), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public DocStatus Status
        {
            get { return _Status; }
            set
            {
                SetPropertyValue("Status", ref _Status, value);
            }
        }

        private PrintStatus _PrintStatus;
        [XafDisplayName("Print Status")]
        [Appearance("PrintStatus", Enabled = false)]
        [Index(31), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public PrintStatus PrintStatus
        {
            get { return _PrintStatus; }
            set
            {
                SetPropertyValue("PrintStatus", ref _PrintStatus, value);
            }
        }

        // Start ver 1.0.8
        // Start ver 1.0.8.1
        //[NonPersistent]
        private string _SAPPONo;
        // End ver 1.0.8.1
        [XafDisplayName("SAP PO No.")]
        [Index(33), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("SAPPONo", Enabled = false)]
        public string SAPPONo
        {
            // Start ver 1.0.8.1
            //get
            //{
            //    string rtn = null;
            //    string dupso = null;
            //    foreach (GRNDetails dtl in this.GRNDetails)
            //    {
            //        if (dtl.PONo != null)
            //        {
            //            if (dupso != dtl.PONo)
            //            {
            //                if (rtn == null)
            //                {
            //                    rtn = dtl.PONo;
            //                }
            //                else
            //                {
            //                    rtn = rtn + ", " + dtl.PONo;
            //                }

            //                dupso = dtl.PONo;
            //            }
            //        }
            //    }

            //    return rtn;
            //}
            get { return _SAPPONo; }
            set
            {
                SetPropertyValue("SAPPONo", ref _SAPPONo, value);
            }
            // End ver 1.0.8.1
        }

        // Start ver 1.0.8.1
        //[NonPersistent]
        private string _PortalPONo;
        // End ver 1.0.8.1
        [XafDisplayName("Portal PO No.")]
        [Index(35), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("PortalPONo", Enabled = false)]
        public string PortalPONo
        {
            // Start ver 1.0.8.1
            //get
            //{
            //    string rtn = null;
            //    string dupso = null;
            //    foreach (GRNDetails dtl in this.GRNDetails)
            //    {
            //        if (dtl.PORefNo != null)
            //        {
            //            if (dupso != dtl.PORefNo)
            //            {
            //                if (rtn == null)
            //                {
            //                    rtn = dtl.PORefNo;
            //                }
            //                else
            //                {
            //                    rtn = rtn + ", " + dtl.PORefNo;
            //                }

            //                dupso = dtl.PORefNo;
            //            }
            //        }
            //    }

            //    return rtn;
            //}
            get { return _PortalPONo; }
            set
            {
                SetPropertyValue("PortalPONo", ref _PortalPONo, value);
            }
            // End ver 1.0.8.1
        }

        // Start ver 1.0.8.1
        //[NonPersistent]
        private string _ASNNo;
        // End ver 1.0.8.1
        [XafDisplayName("ASN No.")]
        [Index(38), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("ASNNo", Enabled = false)]
        public string ASNNo
        {
            // Start ver 1.0.8.1
            //get
            //{
            //    string rtn = null;
            //    string dupso = null;
            //    foreach (GRNDetails dtl in this.GRNDetails)
            //    {
            //        if (dtl.ASNBaseDoc != null)
            //        {
            //            if (dupso != dtl.ASNBaseDoc)
            //            {
            //                if (rtn == null)
            //                {
            //                    rtn = dtl.ASNBaseDoc;
            //                }
            //                else
            //                {
            //                    rtn = rtn + ", " + dtl.ASNBaseDoc;
            //                }

            //                dupso = dtl.ASNBaseDoc;
            //            }
            //        }
            //    }

            //    return rtn;
            //}
            get { return _ASNNo; }
            set
            {
                SetPropertyValue("ASNNo", ref _ASNNo, value);
            }
            // End ver 1.0.8.1
        }
        // End ver 1.0.8

        private string _Remarks;
        [XafDisplayName("Remarks")]
        [Index(50), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public string Remarks
        {
            get { return _Remarks; }
            set
            {
                SetPropertyValue("Remarks", ref _Remarks, value);
            }
        }

        private string _SAPDocNum;
        [XafDisplayName("SAP GRN No.")]
        [Appearance("SAPDocNum", Enabled = false)]
        [Index(53), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public string SAPDocNum
        {
            get { return _SAPDocNum; }
            set
            {
                SetPropertyValue("SAPDocNum", ref _SAPDocNum, value);
            }
        }

        private bool _Sap;
        [XafDisplayName("Sap")]
        [Index(80), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public bool Sap
        {
            get { return _Sap; }
            set
            {
                SetPropertyValue("Sap", ref _Sap, value);
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
                foreach (GRNDetails dtl in this.GRNDetails)
                {
                    return true;
                }

                return false;
            }
        }

        [Browsable(false)]
        public bool IsValid1
        {
            get
            {
                foreach (GRNDetails dtl in this.GRNDetails)
                {
                    if (dtl.Received <= 0)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        [Association("GRN-GRNDetails")]
        [XafDisplayName("Content")]
        public XPCollection<GRNDetails> GRNDetails
        {
            get { return GetCollection<GRNDetails>("GRNDetails"); }
        }

        [Association("GRN-GRNDocTrail")]
        [XafDisplayName("Status History")]
        public XPCollection<GRNDocTrail> GRNDocTrail
        {
            get { return GetCollection<GRNDocTrail>("GRNDocTrail"); }
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
                    GRNDocTrail ds = new GRNDocTrail(Session);
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
                    this.GRNDocTrail.Add(ds);
                }
            }
        }
    }
}