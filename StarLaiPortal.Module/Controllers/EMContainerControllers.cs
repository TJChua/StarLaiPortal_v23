using Admiral.ImportData;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.XtraReports.Serialization;
using StarLaiPortal.Module.BusinessObjects;
using StarLaiPortal.Module.BusinessObjects.Advanced_Shipment_Notice;
using StarLaiPortal.Module.BusinessObjects.Delivery_Order;
using StarLaiPortal.Module.BusinessObjects.Purchase_Order;
using StarLaiPortal.Module.BusinessObjects.Sales_Quotation;
using StarLaiPortal.Module.BusinessObjects.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace StarLaiPortal.Module.Controllers
{
    public partial class EMContainerControllers : ViewController
    {
        GeneralControllers genCon;
        public EMContainerControllers()
        {
            InitializeComponent();
        }
        protected override void OnActivated()
        {
            base.OnActivated();

            this.SubmitEMC.Active.SetItemValue("Enabled", false);
            this.CancelEMC.Active.SetItemValue("Enabled", false);
            this.ImportEMDO.Active.SetItemValue("Enabled", false);
            this.ImportExternalDO.Active.SetItemValue("Enabled", false);
            this.CopyFromDO.Active.SetItemValue("Enabled", false);
            this.DeleteDO.Active.SetItemValue("Enabled", false);
            this.PrintInvoiceEM.Active.SetItemValue("Enabled", false);
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            genCon = Frame.GetController<GeneralControllers>();

            if (View.Id == "EMContainer_DetailView")
            {
                if (((DetailView)View).ViewEditMode == ViewEditMode.View)
                {
                    this.SubmitEMC.Active.SetItemValue("Enabled", true);
                    this.CancelEMC.Active.SetItemValue("Enabled", true);
                    this.PrintInvoiceEM.Active.SetItemValue("Enabled", true);
                }
                else
                {
                    this.SubmitEMC.Active.SetItemValue("Enabled", false);
                    this.CancelEMC.Active.SetItemValue("Enabled", false);
                    this.PrintInvoiceEM.Active.SetItemValue("Enabled", false);
                }

                if (((DetailView)View).ViewEditMode == ViewEditMode.Edit)
                {
                    this.ImportEMDO.Active.SetItemValue("Enabled", true);
                    this.ImportExternalDO.Active.SetItemValue("Enabled", true);
                    this.CopyFromDO.Active.SetItemValue("Enabled", true);
                }
                else
                {
                    this.ImportEMDO.Active.SetItemValue("Enabled", false);
                    this.ImportExternalDO.Active.SetItemValue("Enabled", false);
                    this.CopyFromDO.Active.SetItemValue("Enabled", false);
                }
            }
            else if (View.Id == "EMContainer_EMContainerDO_ListView")
            {
                if (View is ListView && !View.IsRoot)
                {
                    if (View.ObjectSpace.Owner is DetailView)
                    {
                        DetailView masterview = Application.MainWindow.View as DetailView;
                        EMContainer master = (EMContainer)masterview.CurrentObject;
                        if (masterview.Id == "EMContainer_DetailView")
                        {
                            if (master.Status == DocStatus.Draft)
                            {
                                this.DeleteDO.Active.SetItemValue("Enabled", View.ObjectSpace.Owner is DetailView && ((DetailView)View.ObjectSpace.Owner).ViewEditMode == ViewEditMode.Edit);
                            }
                            else
                            {
                                this.DeleteDO.Active.SetItemValue("Enabled", false);
                            }
                        }
                    }
                }
            }
            else
            {
                this.SubmitEMC.Active.SetItemValue("Enabled", false);
                this.CancelEMC.Active.SetItemValue("Enabled", false);
                this.ImportEMDO.Active.SetItemValue("Enabled", false);
                this.ImportExternalDO.Active.SetItemValue("Enabled", false);
                this.CopyFromDO.Active.SetItemValue("Enabled", false);
                this.DeleteDO.Active.SetItemValue("Enabled", false);
                this.PrintInvoiceEM.Active.SetItemValue("Enabled", false);
            }
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }

        public void openNewView(IObjectSpace os, object target, ViewEditMode viewmode)
        {
            ShowViewParameters svp = new ShowViewParameters();
            DetailView dv = Application.CreateDetailView(os, target);
            dv.ViewEditMode = viewmode;
            dv.IsRoot = true;
            svp.CreatedView = dv;

            Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));

        }
        public void showMsg(string caption, string msg, InformationType msgtype)
        {
            MessageOptions options = new MessageOptions();
            options.Duration = 3000;
            //options.Message = string.Format("{0} task(s) have been successfully updated!", e.SelectedObjects.Count);
            options.Message = string.Format("{0}", msg);
            options.Type = msgtype;
            options.Web.Position = InformationPosition.Right;
            options.Win.Caption = caption;
            options.Win.Type = WinMessageType.Flyout;
            Application.ShowViewStrategy.ShowMessage(options);
        }

        private void SubmitEMC_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            EMContainer selectedObject = (EMContainer)e.CurrentObject;
            SqlConnection conn = new SqlConnection(genCon.getConnectionString());

            if (selectedObject.IsValid == true)
            {
                selectedObject.Status = DocStatus.Open;
                selectedObject.InvoiceDate = DateTime.Now;

                EMContainerDocTrail ds = ObjectSpace.CreateObject<EMContainerDocTrail>();
                ds.DocStatus = DocStatus.Open;
                ds.DocRemarks = "";
                selectedObject.EMContainerDocTrail.Add(ds);

                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();

                IObjectSpace os = Application.CreateObjectSpace();
                EMContainer trx = os.FindObject<EMContainer>(new BinaryOperator("Oid", selectedObject.Oid));

                openNewView(os, trx, ViewEditMode.View);
                showMsg("Successful", "Submit Done.", InformationType.Success);
            }
            else
            {
                showMsg("Error", "No Content.", InformationType.Error);
            }
        }

        private void CancelEMC_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            EMContainer selectedObject = (EMContainer)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            selectedObject.Status = DocStatus.Cancelled;
            EMContainerDocTrail ds = ObjectSpace.CreateObject<EMContainerDocTrail>();
            ds.DocStatus = DocStatus.Cancelled;
            ds.DocRemarks = p.ParamString;
            selectedObject.EMContainerDocTrail.Add(ds);

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            IObjectSpace os = Application.CreateObjectSpace();
            EMContainer trx = os.FindObject<EMContainer>(new BinaryOperator("Oid", selectedObject.Oid));
            openNewView(os, trx, ViewEditMode.View);
            showMsg("Successful", "Cancel Done.", InformationType.Success);
        }

        private void CancelEMC_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(StringParameters));
            StringParameters message = os.CreateObject<StringParameters>();

            DetailView dv = Application.CreateDetailView(os, message);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void ImportEMDO_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            EMContainer selectedObject = (EMContainer)e.CurrentObject;

            IObjectSpace os = Application.CreateObjectSpace();
            EMContainer trx = os.FindObject<EMContainer>(new BinaryOperator("Oid", selectedObject.Oid));

            foreach(EMContainerDO dtl in trx.EMContainerDO)
            {
                EMContainerDODetails details = os.FindObject<EMContainerDODetails>(CriteriaOperator.Parse("BaseDoc = ? and EMContainer.Oid = ?", 
                    dtl.DONumber, trx.Oid));

                if (details == null)
                {
                    IList<vwEMDODetails> itemlist = os.GetObjects<vwEMDODetails>
                        (CriteriaOperator.Parse("PortalNum = ?", dtl.DONumber));

                    int count = 1;

                    if (selectedObject.EMContainerDODetails.Where(x => x.BaseDoc == dtl.DONumber).Count() > 0)
                    {
                        count = selectedObject.EMContainerDODetails.Where(x => x.BaseDoc == dtl.DONumber).Max(pp => pp.DOLine);
                    }

                    foreach (vwEMDODetails item in itemlist)
                    {
                        EMContainerDODetails newdetails = os.CreateObject<EMContainerDODetails>();

                        newdetails.ItemCode = newdetails.Session.GetObjectByKey<vwItemMasters>(item.ItemCode);

                        if (item.Warehouse != null)
                        {
                            newdetails.Warehouse = newdetails.Session.GetObjectByKey<vwWarehouse>(item.Warehouse.WarehouseCode);
                        }
                        if (item.Bin != null)
                        {
                            newdetails.Bin = newdetails.Session.GetObjectByKey<vwBin>(item.Bin.BinCode);
                        }
                        newdetails.Quantity = item.Quantity;
                        newdetails.Loaded = item.Quantity;
                        newdetails.Price = item.Price;
                        newdetails.BaseDoc = item.PortalNum;
                        newdetails.BaseId = item.LineOid.ToString();
                        newdetails.BaseDate = item.PostingDate;
                        newdetails.DOLine = count++;
                        newdetails.SODocNum = item.SODocNum;
                        newdetails.SOBaseID = item.SOBaseID;
                        if (item.ContactPerson != null)
                        {
                            newdetails.ContactPerson = newdetails.Session.GetObjectByKey<vwSalesPerson>(item.ContactPerson.SlpCode);
                        }
                        newdetails.PickListDocNum = item.PickListDocNum;
                        newdetails.PackListLine = item.PackListLine;
                        if (item.EIVClassification != null)
                        {
                            newdetails.EIVClassification = newdetails.Session.GetObjectByKey<vwEIVClass>(item.EIVClassification.Code);
                        }
                        trx.EMContainerDODetails.Add(newdetails);
                    }
                }
            }

            os.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }

        private void ImportEMDO_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            EMContainer trx = (EMContainer)View.CurrentObject;

            if (trx.DocNum == null)
            {
                string docprefix = genCon.GetDocPrefix();
                trx.DocNum = genCon.GenerateDocNum(DocTypeList.EMC, ObjectSpace, TransferType.NA, 0, docprefix);
            }

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            var os = Application.CreateObjectSpace();
            var solution = os.CreateObject<ImportData>();
            solution.Option = new ImportOption();

            solution.Option.UpdateProgress = (x) => solution.Progress = x;
            solution.Option.DocNum = trx.DocNum;
            solution.Option.ConnectionString = genCon.getConnectionString();
            solution.Option.Type = "EMContainerDO";

            solution.Option.MainTypeInfo = (this.View as DetailView).Model.ModelClass;
            var view = Application.CreateDetailView(os, solution, false);

            view.Closed += (sss, eee) =>
            {
                this.Frame.GetController<RefreshController>().RefreshAction.DoExecute();
            };

            e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            e.DialogController.AcceptAction.ActionMeaning = ActionMeaning.Unknown;
            //e.Maximized = true;

            e.View = view;
        }

        private void ImportExternalDO_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }

        private void ImportExternalDO_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            EMContainer trx = (EMContainer)View.CurrentObject;

            if (trx.DocNum == null)
            {
                string docprefix = genCon.GetDocPrefix();
                trx.DocNum = genCon.GenerateDocNum(DocTypeList.EMC, ObjectSpace, TransferType.NA, 0, docprefix);
            }

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            var os = Application.CreateObjectSpace();
            var solution = os.CreateObject<ImportData>();
            solution.Option = new ImportOption();

            solution.Option.UpdateProgress = (x) => solution.Progress = x;
            solution.Option.DocNum = trx.DocNum;
            solution.Option.ConnectionString = genCon.getConnectionString();
            solution.Option.Type = "EMContainerExtDO";

            solution.Option.MainTypeInfo = (this.View as DetailView).Model.ModelClass;
            var view = Application.CreateDetailView(os, solution, false);

            view.Closed += (sss, eee) =>
            {
                this.Frame.GetController<RefreshController>().RefreshAction.DoExecute();
            };

            e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            e.DialogController.AcceptAction.ActionMeaning = ActionMeaning.Unknown;
            //e.Maximized = true;

            e.View = view;
        }

        private void CopyFromDO_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count > 0)
            {
                try
                {
                    EMContainer container = (EMContainer)View.CurrentObject;

                    foreach (vwEMDO dtl in e.PopupWindowViewSelectedObjects)
                    {
                        IObjectSpace os = Application.CreateObjectSpace();
                        vwEMDO DOListing = os.FindObject<vwEMDO>(CriteriaOperator.Parse("PortalNum = ?", dtl.PortalNum));

                        if (DOListing == null)
                        {
                            showMsg("Error", "DO already created container, please refresh data.", InformationType.Error);
                            return;
                        }

                        if (container.EMContainerDO.Where(x => x.DONumber == DOListing.PortalNum).Count() > 0)
                        {
                            showMsg("Error", "DO (" + DOListing.PortalNum + ") already created container, please refresh data.", InformationType.Error);
                            return;
                        }

                        IList<vwEMDODetails> itemlist = os.GetObjects<vwEMDODetails>
                            (CriteriaOperator.Parse("PortalNum = ?", dtl.PortalNum));

                        if (itemlist.Count() > 0)
                        {
                            EMContainerDO newDO = ObjectSpace.CreateObject<EMContainerDO>();

                            newDO.DONumber = dtl.PortalNum;
                            container.EMContainerDO.Add(newDO);
                        }

                        int count = 1;

                        if (container.EMContainerDODetails.Where(x => x.BaseDoc == dtl.PortalNum).Count() > 0)
                        {
                            count = container.EMContainerDODetails.Where(x => x.BaseDoc == dtl.PortalNum).Max(pp => pp.DOLine);
                        }

                        foreach (vwEMDODetails item in itemlist)
                        {
                            EMContainerDODetails newdetails = ObjectSpace.CreateObject<EMContainerDODetails>();

                            newdetails.ItemCode = newdetails.Session.GetObjectByKey<vwItemMasters>(item.ItemCode);

                            if (item.Warehouse != null)
                            {
                                newdetails.Warehouse = newdetails.Session.GetObjectByKey<vwWarehouse>(item.Warehouse.WarehouseCode);
                            }
                            if (item.Bin != null)
                            {
                                newdetails.Bin = newdetails.Session.GetObjectByKey<vwBin>(item.Bin.BinCode);
                            }
                            newdetails.Quantity = item.Quantity;
                            newdetails.Loaded = item.Quantity;
                            newdetails.Price = item.Price;
                            newdetails.BaseDoc = item.PortalNum;
                            newdetails.BaseId = item.LineOid.ToString();
                            newdetails.BaseDate = item.PostingDate;
                            newdetails.DOLine = count++;
                            newdetails.SODocNum = item.SODocNum;
                            newdetails.SOBaseID = item.SOBaseID;
                            if (item.ContactPerson != null)
                            {
                                newdetails.ContactPerson = newdetails.Session.GetObjectByKey<vwSalesPerson>(item.ContactPerson.SlpCode);
                            }
                            newdetails.PickListDocNum = item.PickListDocNum;
                            newdetails.PackListLine = item.PackListLine;
                            if (item.EIVClassification != null)
                            {
                                newdetails.EIVClassification = newdetails.Session.GetObjectByKey<vwEIVClass>(item.EIVClassification.Code);
                            }
                            container.EMContainerDODetails.Add(newdetails);
                        }
                    }

                    if (container.DocNum == null)
                    {
                        string docprefix = genCon.GetDocPrefix();
                        container.DocNum = genCon.GenerateDocNum(DocTypeList.EMC, ObjectSpace, TransferType.NA, 0, docprefix);
                    }

                    ObjectSpace.CommitChanges();
                    ObjectSpace.Refresh();

                    showMsg("Success", "Copy Success.", InformationType.Success);
                }
                catch (Exception)
                {
                    showMsg("Fail", "Copy Fail.", InformationType.Error);
                }
            }
        }

        private void CopyFromDO_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            EMContainer trx = (EMContainer)View.CurrentObject;

            var os = Application.CreateObjectSpace();
            var viewId = Application.FindListViewId(typeof(vwEMDO));
            var cs = Application.CreateCollectionSource(os, typeof(vwEMDO), viewId);
            if (trx.Customer != null)
            {
                cs.Criteria["CardCode"] = new BinaryOperator("CardCode", trx.Customer.BPCode);
            }
            else
            {
                cs.Criteria["CardCode"] = new BinaryOperator("CardCode", "");
            }

            var lv1 = Application.CreateListView(viewId, cs, true);
            e.View = lv1;
        }

        private void DeleteDO_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            string query = "";
            SqlConnection conn = new SqlConnection(genCon.getConnectionString());

            foreach (EMContainerDO selectedObject in e.SelectedObjects)
            {
                query = "UPDATE EMContainerDO SET GCRecord = 666 WHERE OID = " + selectedObject.Oid;
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                cmd.Dispose();
                conn.Close();

                query = "UPDATE EMContainerDODetails SET GCRecord = 666 " +
                    "WHERE BaseDoc = '" + selectedObject.DONumber + "' AND EMContainer = " + selectedObject.EMContainer.Oid;
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmddetails = new SqlCommand(query, conn);
                SqlDataReader readerdetails = cmddetails.ExecuteReader();
                cmddetails.Dispose();
                conn.Close();

                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();
            }
        }

        private void PrintInvoiceEM_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count == 1)
            {
                string strServer;
                string strDatabase;
                string strUserID;
                string strPwd;
                string filename;
                // Start ver 1.0.30
                string reportprefix = "";
                // End ver 1.0.30

                SqlConnection conn = new SqlConnection(genCon.getConnectionString());
                EMContainer emcontainer = (EMContainer)View.CurrentObject;
                ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

                if (emcontainer.SAPINVDocNum != null)
                {
                    // Start ver 1.0.30
                    string query = "SELECT T1.BeginStr " +
                        "FROM [" + ConfigurationManager.AppSettings["SAPDB"].ToString() + "]..OINV T0 " +
                        "INNER JOIN [" + ConfigurationManager.AppSettings["SAPDB"].ToString() + "]..NNM1 T1 on T0.Series = T1.Series " +
                        "WHERE T0.DocNum = '" + emcontainer.SAPINVDocNum + "'";
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        reportprefix = reader.GetString(0);
                    }
                    cmd.Dispose();
                    conn.Close();
                    // End ver 1.0.30

                    try
                    {
                        ReportDocument doc = new ReportDocument();
                        strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                        doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\Invoice.rpt"));
                        strDatabase = conn.Database;
                        strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                        strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                        doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                        doc.Refresh();

                        doc.SetParameterValue("dockey@", emcontainer.Oid);
                        doc.SetParameterValue("dbName@", conn.Database);

                        // Start ver 1.0.30
                        //filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                        //    + "_" + delivery.Oid + "_" + user.UserName + "_Inv_"
                        //    + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";
                        filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + reportprefix + emcontainer.SAPINVDocNum + "_"
                            + conn.Database
                            + "_" + user.UserName + "_"
                            + DateTime.Parse(emcontainer.DocDate.ToString()).ToString("yyMMdd") + ".pdf";
                        // End ver 1.0.30

                        doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                        doc.Close();
                        doc.Dispose();

                        // Start ver 1.0.30
                        //string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                        //    ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                        //    + "_" + delivery.Oid + "_" + user.UserName + "_Inv_"
                        //    + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";
                        string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                            ConfigurationManager.AppSettings.Get("PrintPath").ToString() + reportprefix + emcontainer.SAPINVDocNum + "_"
                            + conn.Database
                            + "_" + user.UserName + "_"
                            + DateTime.Parse(emcontainer.DocDate.ToString()).ToString("yyMMdd") + ".pdf";
                        // End ver 1.0.30
                        var script = "window.open('" + url + "');";

                        WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);
                    }
                    catch (Exception ex)
                    {
                        showMsg("Fail", ex.Message, InformationType.Error);
                    }
                }
                else
                {
                    showMsg("Fail", "Invoice not found.", InformationType.Error);
                }
            }
            else
            {
                showMsg("Fail", "Please select one container only.", InformationType.Error);
            }
        }
    }
}
