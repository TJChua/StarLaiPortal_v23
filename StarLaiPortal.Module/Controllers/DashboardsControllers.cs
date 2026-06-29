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
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Web;
using StarLaiPortal.Module.BusinessObjects;
using StarLaiPortal.Module.BusinessObjects.Advanced_Shipment_Notice;
using StarLaiPortal.Module.BusinessObjects.Dashboard;
using StarLaiPortal.Module.BusinessObjects.Delivery_Order;
using StarLaiPortal.Module.BusinessObjects.GRN;
using StarLaiPortal.Module.BusinessObjects.Load;
using StarLaiPortal.Module.BusinessObjects.Pack_List;
using StarLaiPortal.Module.BusinessObjects.Pick_List;
using StarLaiPortal.Module.BusinessObjects.Purchase_Order;
using StarLaiPortal.Module.BusinessObjects.Purchase_Return;
using StarLaiPortal.Module.BusinessObjects.Sales_Order;
using StarLaiPortal.Module.BusinessObjects.Sales_Order_Collection;
using StarLaiPortal.Module.BusinessObjects.Sales_Quotation;
using StarLaiPortal.Module.BusinessObjects.Sales_Refund;
using StarLaiPortal.Module.BusinessObjects.Sales_Return;
using StarLaiPortal.Module.BusinessObjects.Stock_Adjustment;
using StarLaiPortal.Module.BusinessObjects.View;
using StarLaiPortal.Module.BusinessObjects.Warehouse_Transfer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;

// 2023-09-08 - add dashboard sales/purchase/warehouse - ver 1.0.9 
// 2026-06-29 - add preview button - ver 1.0.30

namespace StarLaiPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class DashboardsControllers : ViewController<ListView>
    {
        ASPxGridListEditor gridListEditor = null;
        // Start ver 1.0.30
        GeneralControllers genCon;
        // End ver 1.0.30
        public DashboardsControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            this.DashboardWarehouse.Active.SetItemValue("Enabled", false);
            this.ViewDoc.Active.SetItemValue("Enabled", false);
            this.ViewDashboardDoc.Active.SetItemValue("Enabled", false);
            // Start ver 1.0.9
            this.ViewDocSales.Active.SetItemValue("Enabled", false);
            this.ViewDashboardDocSales.Active.SetItemValue("Enabled", false);
            this.ViewDocPurchase.Active.SetItemValue("Enabled", false);
            this.ViewDashboardDocPurchase.Active.SetItemValue("Enabled", false);
            this.ViewDocWhs.Active.SetItemValue("Enabled", false);
            this.ViewDashboardDocWhs.Active.SetItemValue("Enabled", false);
            // End ver 1.0.9
            // Start ver 1.0.30
            this.PreviewSalesDoc.Active.SetItemValue("Enabled", false);
            // End ver 1.0.30

            if (typeof(Dashboards).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(Dashboards))
                {
                    DashboardWarehouse.Items.Clear();

                    foreach (vwWarehouse warehouse in View.ObjectSpace.CreateCollection(typeof(vwWarehouse), null))
                    {
                        DashboardWarehouse.Items.Add(new ChoiceActionItem(warehouse.WarehouseCode, warehouse.WarehouseCode));
                    }

                    this.DashboardWarehouse.Active.SetItemValue("Enabled", true);
                    DashboardWarehouse.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
                    DashboardWarehouse.CustomizeControl += action_CustomizeControl;

                    //this.ViewDoc.Active.SetItemValue("Enabled", true);
                    //this.ViewDoc.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;

                    this.ViewDashboardDoc.Active.SetItemValue("Enabled", true);
                    this.ViewDashboardDoc.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
                }
            }

            // Start ver 1.0.9
            if (typeof(DashboardsSales).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(DashboardsSales))
                {
                    DashboardWarehouse.Items.Clear();

                    foreach (vwWarehouse warehouse in View.ObjectSpace.CreateCollection(typeof(vwWarehouse), null))
                    {
                        DashboardWarehouse.Items.Add(new ChoiceActionItem(warehouse.WarehouseCode, warehouse.WarehouseCode));
                    }

                    this.DashboardWarehouse.Active.SetItemValue("Enabled", true);
                    DashboardWarehouse.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
                    DashboardWarehouse.CustomizeControl += action_CustomizeControl;

                    //this.ViewDocSales.Active.SetItemValue("Enabled", true);
                    //this.ViewDocSales.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;

                    this.ViewDashboardDocSales.Active.SetItemValue("Enabled", true);
                    this.ViewDashboardDocSales.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;

                    // Start ver 1.0.30
                    //this.PreviewSalesDoc.Active.SetItemValue("Enabled", true);
                    //this.PreviewSalesDoc.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
                    // End ver 1.0.30
                }
            }

            if (typeof(DashboardsPurchase).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(DashboardsPurchase))
                {
                    DashboardWarehouse.Items.Clear();

                    foreach (vwWarehouse warehouse in View.ObjectSpace.CreateCollection(typeof(vwWarehouse), null))
                    {
                        DashboardWarehouse.Items.Add(new ChoiceActionItem(warehouse.WarehouseCode, warehouse.WarehouseCode));
                    }

                    this.DashboardWarehouse.Active.SetItemValue("Enabled", true);
                    DashboardWarehouse.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
                    DashboardWarehouse.CustomizeControl += action_CustomizeControl;

                    //this.ViewDocPurchase.Active.SetItemValue("Enabled", true);
                    //this.ViewDocPurchase.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;

                    this.ViewDashboardDocPurchase.Active.SetItemValue("Enabled", true);
                    this.ViewDashboardDocPurchase.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
                }
            }

            if (typeof(DashboardsWarehouse).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(DashboardsWarehouse))
                {
                    DashboardWarehouse.Items.Clear();

                    foreach (vwWarehouse warehouse in View.ObjectSpace.CreateCollection(typeof(vwWarehouse), null))
                    {
                        DashboardWarehouse.Items.Add(new ChoiceActionItem(warehouse.WarehouseCode, warehouse.WarehouseCode));
                    }

                    this.DashboardWarehouse.Active.SetItemValue("Enabled", true);
                    DashboardWarehouse.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
                    DashboardWarehouse.CustomizeControl += action_CustomizeControl;

                    //this.ViewDocWhs.Active.SetItemValue("Enabled", true);
                    //this.ViewDocWhs.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;

                    this.ViewDashboardDocWhs.Active.SetItemValue("Enabled", true);
                    this.ViewDashboardDocWhs.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
                }
            }
            // End ver 1.0.9
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            //gridListEditor = View.Editor as ASPxGridListEditor;
            //if (gridListEditor != null && gridListEditor.Grid != null)
            //{
            //    gridListEditor.Grid.HtmlRowPrepared += Grid_HtmlRowPrepared;
            //}

            // Start ver 1.0.30
            genCon = Frame.GetController<GeneralControllers>();
            // End ver 1.0.30
        }

        void Grid_HtmlRowPrepared(object sender, DevExpress.Web.ASPxGridViewTableRowEventArgs e)
        {
            if (e.RowType == GridViewRowType.Group)
            {
                e.Row.BackColor = Color.FromArgb(228, 241, 254);
                e.Row.ForeColor = Color.Black;

                //string targetColumnName1 = "DashboardType";
                //string board = (string)e.GetValue(targetColumnName1);
                //if (board.ToString().ToUpper() == "INBOUND")
                //{
                //    e.Row.BackColor = Color.FromArgb(255, 0, 0);
                //}
                //else if (board.ToString().ToUpper() == "OUTBOUND")
                //{
                //    e.Row.BackColor = Color.FromArgb(255, 111, 0);
                //}
            }
        }

        protected override void OnDeactivated()
        {
            if (gridListEditor != null && gridListEditor.Grid != null)
            {
                gridListEditor.Grid.HtmlRowPrepared -= Grid_HtmlRowPrepared;
            }
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        void action_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
            SingleChoiceActionAsModeMenuActionItem actionItem = e.Control as SingleChoiceActionAsModeMenuActionItem;
            if (actionItem != null && actionItem.Action.PaintStyle == DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption)
            {
                DropDownSingleChoiceActionControlBase control = (DropDownSingleChoiceActionControlBase)actionItem.Control;
                control.Label.Text = actionItem.Action.Caption;
                control.Label.Style["padding-right"] = "5px";
            }
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

        private void DashboardWarehouse_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            if (e.SelectedChoiceActionItem.Id != null)
            {
                ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Warehouse] = ?",
                    e.SelectedChoiceActionItem.Id);
            }
        }

        private void ViewDoc_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            Dashboards selectedObject = (Dashboards)e.CurrentObject;

            if (selectedObject.TransactionType == DocTypeList.SQ)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesQuotation trx = os.FindObject<SalesQuotation>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.ASN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                ASN trx = os.FindObject<ASN>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PAL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PackList trx = os.FindObject<PackList>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturnRequests trx = os.FindObject<PurchaseReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.ARD)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrderCollection trx = os.FindObject<SalesOrderCollection>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.DO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.GRN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                GRN trx = os.FindObject<GRN>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.Load)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                Load trx = os.FindObject<Load>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PickList trx = os.FindObject<PickList>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseOrders trx = os.FindObject<PurchaseOrders>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturns trx = os.FindObject<PurchaseReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SA)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustments trx = os.FindObject<StockAdjustments>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SAR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustmentRequests trx = os.FindObject<StockAdjustmentRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrder trx = os.FindObject<SalesOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturns trx = os.FindObject<SalesReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRefund)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefunds trx = os.FindObject<SalesRefunds>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRF)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefundRequests trx = os.FindObject<SalesRefundRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturnRequests trx = os.FindObject<SalesReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.WT)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransfers trx = os.FindObject<WarehouseTransfers>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.WTR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransferReq trx = os.FindObject<WarehouseTransferReq>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }
        }

        private void ViewDashboardDoc_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            Dashboards selectedObject = (Dashboards)e.CurrentObject;

            if (selectedObject.TransactionType == DocTypeList.SQ)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesQuotation trx = os.FindObject<SalesQuotation>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.ASN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                ASN trx = os.FindObject<ASN>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PAL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PackList trx = os.FindObject<PackList>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturnRequests trx = os.FindObject<PurchaseReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.ARD)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrderCollection trx = os.FindObject<SalesOrderCollection>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.DO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.GRN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                GRN trx = os.FindObject<GRN>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.Load)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                Load trx = os.FindObject<Load>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PickList trx = os.FindObject<PickList>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseOrders trx = os.FindObject<PurchaseOrders>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturns trx = os.FindObject<PurchaseReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SA)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustments trx = os.FindObject<StockAdjustments>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SAR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustmentRequests trx = os.FindObject<StockAdjustmentRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrder trx = os.FindObject<SalesOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturns trx = os.FindObject<SalesReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRefund)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefunds trx = os.FindObject<SalesRefunds>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRF)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefundRequests trx = os.FindObject<SalesRefundRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturnRequests trx = os.FindObject<SalesReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.WT)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransfers trx = os.FindObject<WarehouseTransfers>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.WTR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransferReq trx = os.FindObject<WarehouseTransferReq>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }
        }

        private void ViewDashboardDoc_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            Dashboards selectedObject = (Dashboards)View.CurrentObject;

            if (selectedObject.TransactionType == DocTypeList.SQ)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesQuotation trx = os.FindObject<SalesQuotation>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesQuotation_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.ARD)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrderCollection trx = os.FindObject<SalesOrderCollection>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesOrderCollection_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.ASN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                ASN trx = os.FindObject<ASN>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "ASN_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.DO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "DeliveryOrder_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.GRN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                GRN trx = os.FindObject<GRN>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "GRN_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.Load)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                Load trx = os.FindObject<Load>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "Load_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.PAL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PackList trx = os.FindObject<PackList>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "PackList_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.PL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PickList trx = os.FindObject<PickList>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "PickList_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.PO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseOrders trx = os.FindObject<PurchaseOrders>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "PurchaseOrders_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.PR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturns trx = os.FindObject<PurchaseReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "PurchaseReturns_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.PRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturnRequests trx = os.FindObject<PurchaseReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "PurchaseReturnRequests_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SA)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustments trx = os.FindObject<StockAdjustments>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "StockAdjustments_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SAR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustmentRequests trx = os.FindObject<StockAdjustmentRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "StockAdjustmentRequests_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrder trx = os.FindObject<SalesOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesOrder_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturns trx = os.FindObject<SalesReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesReturns_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SRefund)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefunds trx = os.FindObject<SalesRefunds>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesRefunds_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SRF)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefundRequests trx = os.FindObject<SalesRefundRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesRefundRequests_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturnRequests trx = os.FindObject<SalesReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesReturnRequests_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.WT)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransfers trx = os.FindObject<WarehouseTransfers>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "WarehouseTransfers_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.WTR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransferReq trx = os.FindObject<WarehouseTransferReq>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "WarehouseTransferReq_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }
        }

        // Start ver 1.0.9
        private void ViewDashboardDocSales_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            DashboardsSales selectedObject = (DashboardsSales)e.CurrentObject;

            if (selectedObject.TransactionType == DocTypeList.SQ)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesQuotation trx = os.FindObject<SalesQuotation>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.ASN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                ASN trx = os.FindObject<ASN>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PAL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PackList trx = os.FindObject<PackList>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturnRequests trx = os.FindObject<PurchaseReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.ARD)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrderCollection trx = os.FindObject<SalesOrderCollection>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.DO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.GRN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                GRN trx = os.FindObject<GRN>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.Load)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                Load trx = os.FindObject<Load>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PickList trx = os.FindObject<PickList>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseOrders trx = os.FindObject<PurchaseOrders>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturns trx = os.FindObject<PurchaseReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SA)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustments trx = os.FindObject<StockAdjustments>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SAR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustmentRequests trx = os.FindObject<StockAdjustmentRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrder trx = os.FindObject<SalesOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturns trx = os.FindObject<SalesReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRefund)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefunds trx = os.FindObject<SalesRefunds>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRF)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefundRequests trx = os.FindObject<SalesRefundRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturnRequests trx = os.FindObject<SalesReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.WT)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransfers trx = os.FindObject<WarehouseTransfers>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.WTR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransferReq trx = os.FindObject<WarehouseTransferReq>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            // Start ver 1.0.30
            if (selectedObject.TransactionType == DocTypeList.INV)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }
            // End ver 1.0.30
        }

        private void ViewDocSales_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            DashboardsSales selectedObject = (DashboardsSales)e.CurrentObject;

            if (selectedObject.TransactionType == DocTypeList.SQ)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesQuotation trx = os.FindObject<SalesQuotation>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.ASN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                ASN trx = os.FindObject<ASN>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PAL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PackList trx = os.FindObject<PackList>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturnRequests trx = os.FindObject<PurchaseReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.ARD)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrderCollection trx = os.FindObject<SalesOrderCollection>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.DO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.GRN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                GRN trx = os.FindObject<GRN>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.Load)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                Load trx = os.FindObject<Load>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PickList trx = os.FindObject<PickList>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseOrders trx = os.FindObject<PurchaseOrders>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturns trx = os.FindObject<PurchaseReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SA)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustments trx = os.FindObject<StockAdjustments>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SAR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustmentRequests trx = os.FindObject<StockAdjustmentRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrder trx = os.FindObject<SalesOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturns trx = os.FindObject<SalesReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRefund)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefunds trx = os.FindObject<SalesRefunds>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRF)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefundRequests trx = os.FindObject<SalesRefundRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturnRequests trx = os.FindObject<SalesReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.WT)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransfers trx = os.FindObject<WarehouseTransfers>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.WTR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransferReq trx = os.FindObject<WarehouseTransferReq>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }
        }

        private void ViewDashboardDocSales_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            DashboardsSales selectedObject = (DashboardsSales)View.CurrentObject;

            if (selectedObject.TransactionType == DocTypeList.SQ)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesQuotation trx = os.FindObject<SalesQuotation>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesQuotation_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.ARD)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrderCollection trx = os.FindObject<SalesOrderCollection>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesOrderCollection_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.ASN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                ASN trx = os.FindObject<ASN>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "ASN_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.DO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "DeliveryOrder_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.GRN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                GRN trx = os.FindObject<GRN>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "GRN_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.Load)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                Load trx = os.FindObject<Load>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "Load_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.PAL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PackList trx = os.FindObject<PackList>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "PackList_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.PL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PickList trx = os.FindObject<PickList>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "PickList_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.PO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseOrders trx = os.FindObject<PurchaseOrders>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "PurchaseOrders_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.PR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturns trx = os.FindObject<PurchaseReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "PurchaseReturns_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.PRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturnRequests trx = os.FindObject<PurchaseReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "PurchaseReturnRequests_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SA)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustments trx = os.FindObject<StockAdjustments>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "StockAdjustments_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SAR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustmentRequests trx = os.FindObject<StockAdjustmentRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "StockAdjustmentRequests_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrder trx = os.FindObject<SalesOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesOrder_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturns trx = os.FindObject<SalesReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesReturns_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SRefund)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefunds trx = os.FindObject<SalesRefunds>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesRefunds_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SRF)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefundRequests trx = os.FindObject<SalesRefundRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesRefundRequests_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturnRequests trx = os.FindObject<SalesReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesReturnRequests_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.WT)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransfers trx = os.FindObject<WarehouseTransfers>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "WarehouseTransfers_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.WTR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransferReq trx = os.FindObject<WarehouseTransferReq>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "WarehouseTransferReq_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }
        }

        private void ViewDashboardDocPurchase_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            DashboardsPurchase selectedObject = (DashboardsPurchase)e.CurrentObject;

            if (selectedObject.TransactionType == DocTypeList.SQ)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesQuotation trx = os.FindObject<SalesQuotation>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.ASN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                ASN trx = os.FindObject<ASN>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PAL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PackList trx = os.FindObject<PackList>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturnRequests trx = os.FindObject<PurchaseReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.ARD)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrderCollection trx = os.FindObject<SalesOrderCollection>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.DO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.GRN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                GRN trx = os.FindObject<GRN>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.Load)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                Load trx = os.FindObject<Load>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PickList trx = os.FindObject<PickList>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseOrders trx = os.FindObject<PurchaseOrders>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturns trx = os.FindObject<PurchaseReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SA)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustments trx = os.FindObject<StockAdjustments>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SAR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustmentRequests trx = os.FindObject<StockAdjustmentRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrder trx = os.FindObject<SalesOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturns trx = os.FindObject<SalesReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRefund)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefunds trx = os.FindObject<SalesRefunds>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRF)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefundRequests trx = os.FindObject<SalesRefundRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturnRequests trx = os.FindObject<SalesReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.WT)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransfers trx = os.FindObject<WarehouseTransfers>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.WTR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransferReq trx = os.FindObject<WarehouseTransferReq>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }
        }

        private void ViewDashboardDocPurchase_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            DashboardsPurchase selectedObject = (DashboardsPurchase)View.CurrentObject;

            if (selectedObject.TransactionType == DocTypeList.SQ)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesQuotation trx = os.FindObject<SalesQuotation>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesQuotation_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.ARD)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrderCollection trx = os.FindObject<SalesOrderCollection>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesOrderCollection_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.ASN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                ASN trx = os.FindObject<ASN>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "ASN_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.DO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "DeliveryOrder_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.GRN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                GRN trx = os.FindObject<GRN>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "GRN_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.Load)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                Load trx = os.FindObject<Load>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "Load_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.PAL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PackList trx = os.FindObject<PackList>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "PackList_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.PL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PickList trx = os.FindObject<PickList>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "PickList_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.PO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseOrders trx = os.FindObject<PurchaseOrders>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "PurchaseOrders_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.PR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturns trx = os.FindObject<PurchaseReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "PurchaseReturns_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.PRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturnRequests trx = os.FindObject<PurchaseReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "PurchaseReturnRequests_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SA)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustments trx = os.FindObject<StockAdjustments>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "StockAdjustments_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SAR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustmentRequests trx = os.FindObject<StockAdjustmentRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "StockAdjustmentRequests_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrder trx = os.FindObject<SalesOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesOrder_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturns trx = os.FindObject<SalesReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesReturns_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SRefund)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefunds trx = os.FindObject<SalesRefunds>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesRefunds_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SRF)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefundRequests trx = os.FindObject<SalesRefundRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesRefundRequests_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturnRequests trx = os.FindObject<SalesReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesReturnRequests_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.WT)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransfers trx = os.FindObject<WarehouseTransfers>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "WarehouseTransfers_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.WTR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransferReq trx = os.FindObject<WarehouseTransferReq>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "WarehouseTransferReq_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }
        }

        private void ViewDocPurchase_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            DashboardsPurchase selectedObject = (DashboardsPurchase)e.CurrentObject;

            if (selectedObject.TransactionType == DocTypeList.SQ)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesQuotation trx = os.FindObject<SalesQuotation>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.ASN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                ASN trx = os.FindObject<ASN>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PAL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PackList trx = os.FindObject<PackList>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturnRequests trx = os.FindObject<PurchaseReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.ARD)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrderCollection trx = os.FindObject<SalesOrderCollection>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.DO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.GRN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                GRN trx = os.FindObject<GRN>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.Load)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                Load trx = os.FindObject<Load>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PickList trx = os.FindObject<PickList>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseOrders trx = os.FindObject<PurchaseOrders>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturns trx = os.FindObject<PurchaseReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SA)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustments trx = os.FindObject<StockAdjustments>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SAR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustmentRequests trx = os.FindObject<StockAdjustmentRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrder trx = os.FindObject<SalesOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturns trx = os.FindObject<SalesReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRefund)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefunds trx = os.FindObject<SalesRefunds>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRF)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefundRequests trx = os.FindObject<SalesRefundRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturnRequests trx = os.FindObject<SalesReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.WT)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransfers trx = os.FindObject<WarehouseTransfers>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.WTR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransferReq trx = os.FindObject<WarehouseTransferReq>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }
        }

        private void ViewDashboardDocWhs_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            DashboardsWarehouse selectedObject = (DashboardsWarehouse)e.CurrentObject;

            if (selectedObject.TransactionType == DocTypeList.SQ)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesQuotation trx = os.FindObject<SalesQuotation>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.ASN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                ASN trx = os.FindObject<ASN>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PAL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PackList trx = os.FindObject<PackList>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturnRequests trx = os.FindObject<PurchaseReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.ARD)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrderCollection trx = os.FindObject<SalesOrderCollection>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.DO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.GRN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                GRN trx = os.FindObject<GRN>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.Load)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                Load trx = os.FindObject<Load>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PickList trx = os.FindObject<PickList>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseOrders trx = os.FindObject<PurchaseOrders>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturns trx = os.FindObject<PurchaseReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SA)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustments trx = os.FindObject<StockAdjustments>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SAR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustmentRequests trx = os.FindObject<StockAdjustmentRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrder trx = os.FindObject<SalesOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturns trx = os.FindObject<SalesReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRefund)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefunds trx = os.FindObject<SalesRefunds>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRF)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefundRequests trx = os.FindObject<SalesRefundRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturnRequests trx = os.FindObject<SalesReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.WT)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransfers trx = os.FindObject<WarehouseTransfers>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.WTR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransferReq trx = os.FindObject<WarehouseTransferReq>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }
        }

        private void ViewDashboardDocWhs_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            DashboardsWarehouse selectedObject = (DashboardsWarehouse)View.CurrentObject;

            if (selectedObject.TransactionType == DocTypeList.SQ)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesQuotation trx = os.FindObject<SalesQuotation>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesQuotation_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.ARD)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrderCollection trx = os.FindObject<SalesOrderCollection>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesOrderCollection_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.ASN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                ASN trx = os.FindObject<ASN>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "ASN_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.DO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "DeliveryOrder_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.GRN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                GRN trx = os.FindObject<GRN>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "GRN_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.Load)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                Load trx = os.FindObject<Load>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "Load_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.PAL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PackList trx = os.FindObject<PackList>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "PackList_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.PL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PickList trx = os.FindObject<PickList>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "PickList_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.PO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseOrders trx = os.FindObject<PurchaseOrders>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "PurchaseOrders_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.PR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturns trx = os.FindObject<PurchaseReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "PurchaseReturns_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.PRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturnRequests trx = os.FindObject<PurchaseReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "PurchaseReturnRequests_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SA)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustments trx = os.FindObject<StockAdjustments>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "StockAdjustments_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SAR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustmentRequests trx = os.FindObject<StockAdjustmentRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "StockAdjustmentRequests_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrder trx = os.FindObject<SalesOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesOrder_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturns trx = os.FindObject<SalesReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesReturns_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SRefund)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefunds trx = os.FindObject<SalesRefunds>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesRefunds_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SRF)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefundRequests trx = os.FindObject<SalesRefundRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesRefundRequests_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.SRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturnRequests trx = os.FindObject<SalesReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "SalesReturnRequests_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.WT)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransfers trx = os.FindObject<WarehouseTransfers>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "WarehouseTransfers_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }

            if (selectedObject.TransactionType == DocTypeList.WTR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransferReq trx = os.FindObject<WarehouseTransferReq>(new BinaryOperator("DocNum", selectedObject.DocNum));

                DetailView detailView = Application.CreateDetailView(os, "WarehouseTransferReq_DetailView_Dashboard", true, trx);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
                e.View = detailView;
                e.DialogController.AcceptAction.Caption = "Go To Document";
                e.Maximized = true;
                //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            }
        }

        private void ViewDocWhs_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            DashboardsWarehouse selectedObject = (DashboardsWarehouse)e.CurrentObject;

            if (selectedObject.TransactionType == DocTypeList.SQ)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesQuotation trx = os.FindObject<SalesQuotation>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.ASN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                ASN trx = os.FindObject<ASN>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PAL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PackList trx = os.FindObject<PackList>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturnRequests trx = os.FindObject<PurchaseReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.ARD)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrderCollection trx = os.FindObject<SalesOrderCollection>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.DO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.GRN)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                GRN trx = os.FindObject<GRN>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.Load)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                Load trx = os.FindObject<Load>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PL)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PickList trx = os.FindObject<PickList>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseOrders trx = os.FindObject<PurchaseOrders>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.PR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseReturns trx = os.FindObject<PurchaseReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SA)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustments trx = os.FindObject<StockAdjustments>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SAR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                StockAdjustmentRequests trx = os.FindObject<StockAdjustmentRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SO)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrder trx = os.FindObject<SalesOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturns trx = os.FindObject<SalesReturns>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRefund)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefunds trx = os.FindObject<SalesRefunds>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRF)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesRefundRequests trx = os.FindObject<SalesRefundRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.SRR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                SalesReturnRequests trx = os.FindObject<SalesReturnRequests>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.WT)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransfers trx = os.FindObject<WarehouseTransfers>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }

            if (selectedObject.TransactionType == DocTypeList.WTR)
            {
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransferReq trx = os.FindObject<WarehouseTransferReq>(new BinaryOperator("DocNum", selectedObject.DocNum));
                openNewView(os, trx, ViewEditMode.View);
            }
        }
        // End ver 1.0.9

        // Start ver 1.0.30
        private void PreviewSalesDoc_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            DashboardsSales selectedObject = (DashboardsSales)e.CurrentObject;

            if (selectedObject.TransactionType == DocTypeList.SQ)
            {
                string strServer;
                string strDatabase;
                string strUserID;
                string strPwd;
                string filename;

                SqlConnection conn = new SqlConnection(genCon.getConnectionString());
                ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

                if (selectedObject.DocNum == "")
                {
                    showMsg("Fail", "SQ number not found.", InformationType.Error);
                    return;
                }

                IObjectSpace os = Application.CreateObjectSpace();
                SalesQuotation trx = os.FindObject<SalesQuotation>(new BinaryOperator("DocNum", selectedObject.DocNum));

                try
                {
                    ReportDocument doc = new ReportDocument();
                    strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                    doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\Quotation.rpt"));
                    strDatabase = conn.Database;
                    strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                    strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                    doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                    doc.Refresh();

                    doc.SetParameterValue("dockey@", trx.Oid);
                    doc.SetParameterValue("dbName@", conn.Database);

                    filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                        + "_" + trx.Oid + "_" + user.UserName + "_SQ_"
                        + DateTime.Parse(trx.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";

                    doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                    doc.Close();
                    doc.Dispose();

                    string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                        ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                        + "_" + trx.Oid + "_" + user.UserName + "_SQ_"
                        + DateTime.Parse(trx.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";
                    var script = "window.open('" + url + "');";

                    WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);
                }
                catch (Exception ex)
                {
                    showMsg("Fail", ex.Message, InformationType.Error);
                }
            }
            else if (selectedObject.TransactionType == DocTypeList.SO)
            {
                string strServer;
                string strDatabase;
                string strUserID;
                string strPwd;
                string filename;

                SqlConnection conn = new SqlConnection(genCon.getConnectionString());
                ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

                if (selectedObject.DocNum == "")
                {
                    showMsg("Fail", "SO number not found.", InformationType.Error);
                    return;
                }

                IObjectSpace os = Application.CreateObjectSpace();
                SalesOrder trx = os.FindObject<SalesOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));

                try
                {
                    ReportDocument doc = new ReportDocument();
                    strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                    doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\SalesOrder.rpt"));
                    strDatabase = conn.Database;
                    strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                    strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                    doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                    doc.Refresh();

                    doc.SetParameterValue("dockey@", trx.Oid);
                    doc.SetParameterValue("dbName@", conn.Database);

                    filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                        + "_" + trx.Oid + "_" + user.UserName + "_SO_"
                        + DateTime.Parse(trx.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";

                    doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                    doc.Close();
                    doc.Dispose();

                    string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                        ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                        + "_" + trx.Oid + "_" + user.UserName + "_SO_"
                        + DateTime.Parse(trx.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";
                    var script = "window.open('" + url + "');";

                    WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);
                }
                catch (Exception ex)
                {
                    showMsg("Fail", ex.Message, InformationType.Error);
                }
            }
            else if (selectedObject.TransactionType == DocTypeList.DO)
            {
                string strServer;
                string strDatabase;
                string strUserID;
                string strPwd;
                string filename;

                SqlConnection conn = new SqlConnection(genCon.getConnectionString());
                ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

                if (selectedObject.DocNum == "")
                {
                    showMsg("Fail", "DO number not found.", InformationType.Error);
                    return;
                }

                IObjectSpace os = Application.CreateObjectSpace();
                DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));

                try
                {
                    ReportDocument doc = new ReportDocument();
                    strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                    doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\DeliveryOrder.rpt"));
                    strDatabase = conn.Database;
                    strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                    strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                    doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                    doc.Refresh();

                    doc.SetParameterValue("dockey@", trx.Oid);
                    doc.SetParameterValue("dbName@", conn.Database);

                    filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                        + "_" + trx.Oid + "_" + user.UserName + "_DO_"
                        + DateTime.Parse(trx.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";

                    doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                    doc.Close();
                    doc.Dispose();

                    string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                        ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                        + "_" + trx.Oid + "_" + user.UserName + "_DO_"
                        + DateTime.Parse(trx.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";
                    var script = "window.open('" + url + "');";

                    WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);
                }
                catch (Exception ex)
                {
                    showMsg("Fail", ex.Message, InformationType.Error);
                }
            }
            else if (selectedObject.TransactionType == DocTypeList.INV)
            {
                string strServer;
                string strDatabase;
                string strUserID;
                string strPwd;
                string filename;

                SqlConnection conn = new SqlConnection(genCon.getConnectionString());
                ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

                if (selectedObject.DocNum == "")
                {
                    showMsg("Fail", "Invoice number not found.", InformationType.Error);
                    return;
                }

                IObjectSpace os = Application.CreateObjectSpace();
                DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("DocNum", selectedObject.DocNum));

                if (trx.SAPDocNum != null)
                {
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

                        doc.SetParameterValue("dockey@", trx.Oid);
                        doc.SetParameterValue("dbName@", conn.Database);

                        filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                            + "_" + trx.Oid + "_" + user.UserName + "_Inv_"
                            + DateTime.Parse(trx.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";

                        doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                        doc.Close();
                        doc.Dispose();

                        string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                            ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                            + "_" + trx.Oid + "_" + user.UserName + "_Inv_"
                            + DateTime.Parse(trx.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";
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
                genCon.showMsg("Warning", "No printing.", InformationType.Warning);
            }
        }
        // End ver 1.0.30
    }
}
