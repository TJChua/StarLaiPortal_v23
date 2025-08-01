﻿using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo.DB;
using StarLaiPortal.Module.BusinessObjects;
using StarLaiPortal.Module.BusinessObjects.Item_Inquiry;
using StarLaiPortal.Module.BusinessObjects.Purchase_Order;
using StarLaiPortal.Module.BusinessObjects.Purchase_Return;
using StarLaiPortal.Module.BusinessObjects.Sales_Order;
using StarLaiPortal.Module.BusinessObjects.Sales_Quotation;
using StarLaiPortal.Module.BusinessObjects.Sales_Refund;
using StarLaiPortal.Module.BusinessObjects.Sales_Return;
using StarLaiPortal.Module.BusinessObjects.Setup;
using StarLaiPortal.Module.BusinessObjects.Stock_Adjustment;
using StarLaiPortal.Module.BusinessObjects.View;
using StarLaiPortal.Module.BusinessObjects.Warehouse_Transfer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

// 2023-07-28 add item button do not pop out ver 1.0.7
// 2023-08-16 - add stock 3 and stock 4 - ver 1.0.8
// 2023-12-04 - add order status - ver 1.0.13
// 2024-01-30 - orderstatus add new  field - ver 1.0.14
// 2024-04-01 - add catalog number and old code search - ver 1.0.15
// 2024-06-01 - add salesperson - ver 1.0.17
// 2024-07-29 - add DfltWhs - ver 1.0.19
// 2024-10-08 - add whse - ver 1.0.21
// 2025-02-04 - add global item inquiry - ver 1.0.22
// 2025-02-25 - block add item if not in draft - ver 1.0.22
// 2025-06-25 - change item inquiry to dataview mode - ver 1.0.23
// 2025-06-26 - Add check performance feature - ver 1.0.23
// 2025-06-30 - Not allow search with no keyword - ver 1.0.23

namespace StarLaiPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ItemInquiryControllers : ViewController
    {
        GeneralControllers genCon;
        public ItemInquiryControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            this.Search_ItemInquiry.Active.SetItemValue("Enabled", false);
            this.ViewSales.Active.SetItemValue("Enabled", false);
            this.AddToCartPop.Active.SetItemValue("Enabled", false);
            this.OpenCart.Active.SetItemValue("Enabled", false);
            this.ViewItemPicture.Active.SetItemValue("Enabled", false);
            // Start ver 1.0.7
            this.AddToCartSimple.Active.SetItemValue("Enabled", false);
            // End ver 1.0.7
            // Start ver 1.0.13
            this.ViewOrderStatus.Active.SetItemValue("Enabled", false);
            // End ver 1.0.13
            // Start ver 1.0.22
            this.GlobalSearch_ItemInquiry.Active.SetItemValue("Enabled", false);
            // End ver 1.0.22

            if (typeof(ItemInquiry).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(ItemInquiry))
                {
                    this.Search_ItemInquiry.Active.SetItemValue("Enabled", true);
                    this.Search_ItemInquiry.ActionMeaning = ActionMeaning.Accept;
                    //this.OpenCart.Active.SetItemValue("Enabled", true);
                }
            }

            if (typeof(ItemInquiryDetails).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(ItemInquiryDetails))
                {
                    if (View.Id == "ItemInquiry_ItemInquiryDetails_ListView")
                    {
                        // Start ver 1.0.7
                        //this.AddToCartPop.Active.SetItemValue("Enabled", true);
                        this.AddToCartSimple.Active.SetItemValue("Enabled", true);
                        // End ver 1.0.7
                    }
                    else
                    {
                        // Start ver 1.0.7
                        //this.AddToCartPop.Active.SetItemValue("Enabled", false);
                        this.AddToCartSimple.Active.SetItemValue("Enabled", false);
                        // End ver 1.0.7
                    }
                    this.ViewSales.Active.SetItemValue("Enabled", true);
                    // Start ver 1.0.13
                    this.ViewOrderStatus.Active.SetItemValue("Enabled", true);
                    // End ver 1.0.13
                    this.ViewItemPicture.Active.SetItemValue("Enabled", true);
                    this.ViewItemPicture.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
                }
            }

            // Start ver 1.0.22
            if (typeof(GlobalItemInquiry).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(GlobalItemInquiry))
                {
                    this.GlobalSearch_ItemInquiry.Active.SetItemValue("Enabled", true);
                    this.GlobalSearch_ItemInquiry.ActionMeaning = ActionMeaning.Accept;
                }
            }

            if (typeof(GlobalItemInquiryDetails).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(GlobalItemInquiryDetails))
                {
                    this.ViewItemPicture.Active.SetItemValue("Enabled", true);
                    this.ViewItemPicture.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
                }
            }
            // End ver 1.0.22
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            genCon = Frame.GetController<GeneralControllers>();
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
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

        private void Search_ItemInquiry_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            string cardcode = "";
            ItemInquiry selectedObject = (ItemInquiry)e.CurrentObject;

            if (selectedObject.CardCode != null)
            {
                cardcode = selectedObject.CardCode.BPCode;
            }

            // Start ver 1.0.15
            if (selectedObject.Search != null && selectedObject.OldCode == null && selectedObject.CatalogNumber == null ||
                selectedObject.Search == null && selectedObject.OldCode != null && selectedObject.CatalogNumber == null ||
                selectedObject.Search == null && selectedObject.OldCode == null && selectedObject.CatalogNumber != null ||
                selectedObject.Search == null && selectedObject.OldCode == null && selectedObject.CatalogNumber == null)
            {
            // End ver 1.0.15

                if (selectedObject.PriceList1 != null && selectedObject.PriceList2 != null
                    && selectedObject.PriceList3 != null && selectedObject.PriceList4 != null
                    && selectedObject.Stock1 != null && selectedObject.Stock2 != null
                    // Start ver 1.0.8
                    && selectedObject.Stock3 != null && selectedObject.Stock4 != null)
                // End ver 1.0.8
                {
                    //for (int i = 0; selectedObject.ItemInquiryDetails.Count > i;)
                    //{
                    //    selectedObject.ItemInquiryDetails.Remove(selectedObject.ItemInquiryDetails[i]);
                    //}

                    // Start ver 1.0.15
                    // Start ver 1.0.23
                    if (selectedObject.Search != null)
                    {
                        if (!string.IsNullOrEmpty(selectedObject.Search.Trim(' ')))
                        // End ver 1.0.23
                        {
                            XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();
                            SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_GetItem", new OperandValue(selectedObject.Search),
                                new OperandValue(selectedObject.Exclude),
                                new OperandValue(selectedObject.PriceList1.ListNum), new OperandValue(selectedObject.PriceList2.ListNum),
                                new OperandValue(selectedObject.PriceList3.ListNum), new OperandValue(selectedObject.PriceList4.ListNum),
                                new OperandValue(selectedObject.Stock1.WarehouseCode), new OperandValue(selectedObject.Stock2.WarehouseCode),
                                // Start ver 1.0.8
                                new OperandValue(selectedObject.Stock3.WarehouseCode), new OperandValue(selectedObject.Stock4.WarehouseCode),
                                // End ver 1.0.8
                                new OperandValue(selectedObject.Method), new OperandValue(cardcode), new OperandValue(selectedObject.Oid));

                            persistentObjectSpace.Session.DropIdentityMap();
                            persistentObjectSpace.Dispose();
                        }
                        // Start ver 1.0.23
                        else
                        {
                            genCon.showMsg("Fail", "Not allow to search without any keyword.", InformationType.Error);
                            return;
                        }
                        // End ver 1.0.23
                    }
                    // End ver 1.0.15 

                    // Start ver 1.0.15
                    // Start ver 1.0.23
                    if (selectedObject.OldCode != null)
                    {
                        if (!string.IsNullOrEmpty(selectedObject.OldCode.Trim(' ')))
                        // End ver 1.0.23
                        {
                            XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();
                            SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_GetItemOldCode", new OperandValue(selectedObject.OldCode),
                                new OperandValue(selectedObject.Exclude),
                                new OperandValue(selectedObject.PriceList1.ListNum), new OperandValue(selectedObject.PriceList2.ListNum),
                                new OperandValue(selectedObject.PriceList3.ListNum), new OperandValue(selectedObject.PriceList4.ListNum),
                                new OperandValue(selectedObject.Stock1.WarehouseCode), new OperandValue(selectedObject.Stock2.WarehouseCode),
                                // Start ver 1.0.8
                                new OperandValue(selectedObject.Stock3.WarehouseCode), new OperandValue(selectedObject.Stock4.WarehouseCode),
                                // End ver 1.0.8
                                new OperandValue(selectedObject.Method), new OperandValue(cardcode), new OperandValue(selectedObject.Oid));

                            persistentObjectSpace.Session.DropIdentityMap();
                            persistentObjectSpace.Dispose();
                        }
                        // Start ver 1.0.23
                        else
                        {
                            genCon.showMsg("Fail", "Not allow to search without any keyword.", InformationType.Error);
                            return;
                        }
                        // End ver 1.0.23
                    }
                    // End ver 1.0.15 

                    // Start ver 1.0.15
                    // Start ver 1.0.23
                    if (selectedObject.CatalogNumber != null)
                    {
                        if (!string.IsNullOrEmpty(selectedObject.CatalogNumber.Trim(' ')))
                        // End ver 1.0.23
                        {
                            XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();
                            SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_GetItemCatalogNumber", new OperandValue(selectedObject.CatalogNumber),
                                new OperandValue(selectedObject.Exclude),
                                new OperandValue(selectedObject.PriceList1.ListNum), new OperandValue(selectedObject.PriceList2.ListNum),
                                new OperandValue(selectedObject.PriceList3.ListNum), new OperandValue(selectedObject.PriceList4.ListNum),
                                new OperandValue(selectedObject.Stock1.WarehouseCode), new OperandValue(selectedObject.Stock2.WarehouseCode),
                                // Start ver 1.0.8
                                new OperandValue(selectedObject.Stock3.WarehouseCode), new OperandValue(selectedObject.Stock4.WarehouseCode),
                                // End ver 1.0.8
                                new OperandValue(selectedObject.Method), new OperandValue(cardcode), new OperandValue(selectedObject.Oid));

                            persistentObjectSpace.Session.DropIdentityMap();
                            persistentObjectSpace.Dispose();
                        }
                        // Start ver 1.0.23
                        else
                        {
                            genCon.showMsg("Fail", "Not allow to search without any keyword.", InformationType.Error);
                            return;
                        }
                        // End ver 1.0.23
                    }
                    // End ver 1.0.15 

                    //if (sprocData.ResultSet.Count() > 0)
                    //{
                    //    if (sprocData.ResultSet[0].Rows.Count() > 0)
                    //    {
                    //        foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                    //        {
                    //            ItemInquiryDetails searchitem = ObjectSpace.CreateObject<ItemInquiryDetails>();
                    //            searchitem.ItemCode = row.Values[0].ToString();
                    //            searchitem.ItemDesc = row.Values[1].ToString();
                    //            searchitem.Model = row.Values[2].ToString();
                    //            searchitem.PriceList1 = selectedObject.PriceList1.ListName;
                    //            searchitem.Price1 = Convert.ToDecimal(row.Values[3]);
                    //            searchitem.PriceList2 = selectedObject.PriceList2.ListName;
                    //            searchitem.Price2 = Convert.ToDecimal(row.Values[4]);
                    //            searchitem.StockName1 = selectedObject.Stock1.WarehouseName;
                    //            searchitem.Stock1 = Convert.ToDecimal(row.Values[5]);
                    //            searchitem.StockName2 = selectedObject.Stock2.WarehouseName;
                    //            searchitem.Stock2 = Convert.ToDecimal(row.Values[6]);
                    //            searchitem.OriginalCatalog = row.Values[7].ToString();
                    //            searchitem.HierarchyLevel1 = row.Values[8].ToString();
                    //            searchitem.HierarchyLevel2 = row.Values[9].ToString();
                    //            searchitem.HierarchyLevel3 = row.Values[10].ToString();
                    //            searchitem.HierarchyLevel4 = row.Values[11].ToString();
                    //            searchitem.HierarchyLevel5 = row.Values[12].ToString();
                    //            searchitem.Remarks = row.Values[13].ToString();
                    //            searchitem.Barcode = row.Values[14].ToString();
                    //            searchitem.ForeignName = row.Values[15].ToString();
                    //            searchitem.BPCatalogNo = row.Values[16].ToString();

                    //            selectedObject.ItemInquiryDetails.Add(searchitem);
                    //        }
                    //    }
                    //}
                    ObjectSpace.CommitChanges();
                    ObjectSpace.Refresh();
                    View.Refresh();
                }
                else
                {
                    showMsg("Fail", "PriceList1/2/3/4 and Stock1/2 is empty.", InformationType.Error);
                }
            // Start ver 1.0.15
            }
            else
            {
                showMsg("Fail", "Not allow enter multiple searching field.", InformationType.Error);
            }
            // End ver 1.0.15
        }

        private void ViewSales_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {

        }

        private void ViewSales_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            // Start ver 1.0.23
            //ItemInquiryDetails iteminquiry = (ItemInquiryDetails)View.CurrentObject;

            ArrayList selectedHdr = new ArrayList();
            string OrderStatusItemCode = "";
            string OrderStatusCode = "";

            if (View.SelectedObjects.Count > 0)
            {
                foreach (var selectedObject in View.SelectedObjects)
                {
                    selectedHdr.Add((ItemInquiryDetails)ObjectSpace.GetObject(selectedObject));
                }
            }
            // End ver 1.0.23

            IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(SalesHistory));

            XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();
            var nonPersistentOS = Application.CreateObjectSpace(typeof(SalesHistoryList));
            SalesHistoryList saleslist = nonPersistentOS.CreateObject<SalesHistoryList>();

            // Start ver 1.0.23
            //if (iteminquiry != null)
            foreach (ItemInquiryDetails iteminquiry in selectedHdr)
            // End ver 1.0.23
            {
                // Start ver 1.0.23
                OrderStatusItemCode = iteminquiry.ItemCode + " - " + iteminquiry.ItemDesc;
                OrderStatusCode = iteminquiry.ItemCode;
                // End ver 1.0.23

                SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_GetSales", new OperandValue(iteminquiry.ItemCode),
                    new OperandValue(DateTime.Today.AddMonths(-3).Date), new OperandValue(DateTime.Today.AddDays(1).Date));

                int i = 1;

                if (sprocData.ResultSet.Count() > 0)
                {
                    if (sprocData.ResultSet[0].Rows.Count() > 0)
                    {
                        foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                        {
                            var itemos = Application.CreateObjectSpace(typeof(SalesHistory));
                            var item = itemos.CreateObject<SalesHistory>();
                            item.Id = i;
                            item.No = i;
                            item.Customer = row.Values[1].ToString();
                            item.SalesDate = (DateTime)row.Values[2];
                            item.Quantity = (decimal)row.Values[3];
                            item.UnitPrice = (decimal)row.Values[4];
                            item.SAPInvoiceNo = row.Values[5].ToString();
                            // Start ver 1.0.17
                            item.Salesperson = row.Values[6].ToString();
                            // End ver 1.0.17
                            // Start ver 1.0.21
                            item.Whse = row.Values[7].ToString();
                            // End ver 1.0.21
                            saleslist.Sales.Add(item);

                            i++;
                        }
                    }
                }
            }

            nonPersistentOS.CommitChanges();
            persistentObjectSpace.Session.DropIdentityMap();
            persistentObjectSpace.Dispose();

            DetailView detailView = Application.CreateDetailView(nonPersistentOS, saleslist);
            detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            // Start ver 1.0.23
            //if (iteminquiry != null)
            //{
            //    ((SalesHistoryList)detailView.CurrentObject).ItemCode = iteminquiry.ItemCode + " - " + iteminquiry.ItemDesc;
            //}
            if (OrderStatusItemCode != "")
            {
                ((SalesHistoryList)detailView.CurrentObject).ItemCode = OrderStatusItemCode;
            }
            if (OrderStatusCode != "")
            {
                ((SalesHistoryList)detailView.CurrentObject).Code = OrderStatusCode;
            }
            // End ver 1.0.23
            e.View = detailView;
            e.DialogController.SaveOnAccept = false;
            e.DialogController.CancelAction.Active["NothingToCancel"] = false;
        }

        private void OpenCart_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ItemInquiry selectedObject = (ItemInquiry)e.CurrentObject;

            IObjectSpace os = Application.CreateObjectSpace();
            SalesOrder trx = os.FindObject<SalesOrder>(new BinaryOperator("Cart", selectedObject.Cart));

            if (trx != null)
            {
                openNewView(os, trx, ViewEditMode.Edit);
            }
            else
            {
                showMsg("Error", "Cart not found.", InformationType.Error);
            }
        }

        private void AddToCartPop_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            //ItemInquiryDetails selectedObject = (ItemInquiryDetails)e.CurrentObject;
            SqlConnection conn = new SqlConnection(genCon.getConnectionString());
            int oid = 0;

            //if (selectedObject == null)
            //{
                foreach (ItemInquiryDetails dtl in e.SelectedObjects)
                {
                    oid = dtl.ItemInquiry.Oid;
                    if (dtl.ItemInquiry.DocType == DocTypeList.SQ)
                    {
                        IObjectSpace cos = Application.CreateObjectSpace();
                        SalesQuotation newSQ = cos.CreateObject<SalesQuotation>();
                        IObjectSpace os = Application.CreateObjectSpace();

                        if (dtl.ItemInquiry.Cart != null)
                        {
                            SalesQuotation trx = os.FindObject<SalesQuotation>(new BinaryOperator("DocNum", dtl.ItemInquiry.Cart));

                            if (trx != null)
                            {
                                SalesQuotationDetails newSQdetail = os.CreateObject<SalesQuotationDetails>();
                                newSQdetail.Postingdate = trx.PostingDate;
                                newSQdetail.Customer = newSQdetail.Session.GetObjectByKey<vwBusniessPartner>(trx.Customer.BPCode);
                                newSQdetail.Location = newSQdetail.Session.GetObjectByKey<vwWarehouse>(dtl.ItemInquiry.Stock1.WarehouseCode);
                                newSQdetail.ItemCode = newSQdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                                newSQdetail.ItemDesc = dtl.ItemDesc;
                                newSQdetail.Model = dtl.Model;
                                newSQdetail.CatalogNo = dtl.OriginalCatalog;
                                //newSQdetail.Price = dtl.Price1;
                                //newSQdetail.AdjustedPrice = dtl.Price1;
                                trx.SalesQuotationDetails.Add(newSQdetail);
                            }
                            else
                            {
                                newSQ.Priority = newSQ.Session.FindObject<PriorityType>
                                    (CriteriaOperator.Parse("PriorityName = ? and IsActive= ?",
                                    "Normal", "True"));

                                SalesQuotationDetails newSQdetail = cos.CreateObject<SalesQuotationDetails>();
                                newSQdetail.ItemCode = newSQdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                                newSQdetail.ItemDesc = dtl.ItemDesc;
                                newSQdetail.Model = dtl.Model;
                                newSQdetail.CatalogNo = dtl.OriginalCatalog;
                                newSQdetail.Location = newSQdetail.Session.GetObjectByKey<vwWarehouse>(dtl.ItemInquiry.Stock1.WarehouseCode);
                                //newSQdetail.Price = dtl.Price1;
                                //newSQdetail.AdjustedPrice = dtl.Price1;
                                newSQ.SalesQuotationDetails.Add(newSQdetail);
                            }
                        }
                        else
                        {
                            showMsg("Error", "Please specify cart no.", InformationType.Error);
                            return;
                        }

                        os.CommitChanges();

                        showMsg("Success", "Added to Sales Quoatation.", InformationType.Success);
                    }
                    else if (dtl.ItemInquiry.DocType == DocTypeList.PO)
                    {
                        IObjectSpace cos = Application.CreateObjectSpace();
                        PurchaseOrders newPO = cos.CreateObject<PurchaseOrders>();
                        IObjectSpace os = Application.CreateObjectSpace();

                        if (dtl.ItemInquiry.Cart != null)
                        {
                            PurchaseOrders trx = os.FindObject<PurchaseOrders>(new BinaryOperator("DocNum", dtl.ItemInquiry.Cart));

                            if (trx != null)
                            {
                                PurchaseOrderDetails newPOdetail = os.CreateObject<PurchaseOrderDetails>();
                                newPOdetail.Postingdate = trx.PostingDate;
                                newPOdetail.Supplier = newPOdetail.Session.GetObjectByKey<vwBusniessPartner>(trx.Supplier.BPCode);
                                newPOdetail.ItemCode = newPOdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                                newPOdetail.ItemDesc = dtl.ItemDesc;
                                newPOdetail.Model = dtl.Model;
                                newPOdetail.Location = newPOdetail.Session.GetObjectByKey<vwWarehouse>(trx.Warehouse.WarehouseCode);
                                newPOdetail.CatalogNo = dtl.OriginalCatalog;
                                //newPOdetail.Price = dtl.Price1;
                                //newPOdetail.AdjustedPrice = dtl.Price1;
                                trx.PurchaseOrderDetails.Add(newPOdetail);
                            }
                            else
                            {
                                PurchaseOrderDetails newPOdetail = cos.CreateObject<PurchaseOrderDetails>();
                                newPOdetail.ItemCode = newPOdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                                newPOdetail.ItemDesc = dtl.ItemDesc;
                                newPOdetail.Model = dtl.Model;
                                newPOdetail.Location = newPOdetail.Session.GetObjectByKey<vwWarehouse>(trx.Warehouse.WarehouseCode);
                                newPOdetail.CatalogNo = dtl.OriginalCatalog;
                                //newPOdetail.Price = dtl.Price1;
                                //newPOdetail.AdjustedPrice = dtl.Price1;
                                newPO.PurchaseOrderDetails.Add(newPOdetail);
                            }
                        }
                        else
                        {
                            showMsg("Error", "Please specify cart no.", InformationType.Error);
                            return;
                        }

                        os.CommitChanges();

                        showMsg("Success", "Added to Purchase Order.", InformationType.Success);
                    }
                    else if (dtl.ItemInquiry.DocType == DocTypeList.PRR)
                    {
                        IObjectSpace cos = Application.CreateObjectSpace();
                        PurchaseReturnRequests newprr = cos.CreateObject<PurchaseReturnRequests>();
                        IObjectSpace os = Application.CreateObjectSpace();

                        if (dtl.ItemInquiry.Cart != null)
                        {
                            PurchaseReturnRequests trx = os.FindObject<PurchaseReturnRequests>(new BinaryOperator("DocNum", dtl.ItemInquiry.Cart));

                            if (trx != null)
                            {
                                PurchaseReturnRequestDetails newPRRdetail = os.CreateObject<PurchaseReturnRequestDetails>();
                                newPRRdetail.ItemCode = newPRRdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                                newPRRdetail.ItemDesc = dtl.ItemDesc;
                                newPRRdetail.UOM = newPRRdetail.ItemCode.UOM;
                                newPRRdetail.Warehouse = newPRRdetail.Session.GetObjectByKey<vwWarehouse>(dtl.ItemInquiry.Stock1.WarehouseCode);
                                newPRRdetail.Price = dtl.Price1;
                                trx.PurchaseReturnRequestDetails.Add(newPRRdetail);
                            }
                            else
                            {
                                PurchaseReturnRequestDetails newPRRdetail = cos.CreateObject<PurchaseReturnRequestDetails>();
                                newPRRdetail.ItemCode = newPRRdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                                newPRRdetail.ItemDesc = dtl.ItemDesc;
                                newPRRdetail.UOM = newPRRdetail.ItemCode.UOM;
                                newPRRdetail.Warehouse = newPRRdetail.Session.GetObjectByKey<vwWarehouse>(dtl.ItemInquiry.Stock1.WarehouseCode);
                                newPRRdetail.Price = dtl.Price1;
                                newprr.PurchaseReturnRequestDetails.Add(newPRRdetail);
                            }
                        }
                        else
                        {
                            showMsg("Error", "Please specify cart no.", InformationType.Error);
                            return;
                        }

                        os.CommitChanges();

                        showMsg("Success", "Added to Purchase Return Request.", InformationType.Success);
                    }
                    else if (dtl.ItemInquiry.DocType == DocTypeList.SRR)
                    {
                        IObjectSpace cos = Application.CreateObjectSpace();
                        SalesReturnRequests newsrr = cos.CreateObject<SalesReturnRequests>();
                        IObjectSpace os = Application.CreateObjectSpace();

                        if (dtl.ItemInquiry.Cart != null)
                        {
                            SalesReturnRequests trx = os.FindObject<SalesReturnRequests>(new BinaryOperator("DocNum", dtl.ItemInquiry.Cart));

                            if (trx != null)
                            {
                                SalesReturnRequestDetails newSRRdetail = os.CreateObject<SalesReturnRequestDetails>();
                                newSRRdetail.ItemCode = newSRRdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                                newSRRdetail.ItemDesc = dtl.ItemDesc;
                                newSRRdetail.UOM = newSRRdetail.ItemCode.UOM;
                                newSRRdetail.Warehouse = newSRRdetail.Session.GetObjectByKey<vwWarehouse>(dtl.ItemInquiry.Stock1.WarehouseCode);
                                newSRRdetail.Price = dtl.Price1;
                                trx.SalesReturnRequestDetails.Add(newSRRdetail);
                            }
                            else
                            {
                                SalesReturnRequestDetails newSRRdetail = cos.CreateObject<SalesReturnRequestDetails>();
                                newSRRdetail.ItemCode = newSRRdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                                newSRRdetail.ItemDesc = dtl.ItemDesc;
                                newSRRdetail.UOM = newSRRdetail.ItemCode.UOM;
                                newSRRdetail.Warehouse = newSRRdetail.Session.GetObjectByKey<vwWarehouse>(dtl.ItemInquiry.Stock1.WarehouseCode);
                                newSRRdetail.Price = dtl.Price1;
                                newsrr.SalesReturnRequestDetails.Add(newSRRdetail);
                            }
                        }
                        else
                        {
                            showMsg("Error", "Please specify cart no.", InformationType.Error);
                            return;
                        }

                        os.CommitChanges();

                        showMsg("Success", "Added to Sales Return Request.", InformationType.Success);
                    }
                    else if (dtl.ItemInquiry.DocType == DocTypeList.WTR)
                    {
                        IObjectSpace cos = Application.CreateObjectSpace();
                        WarehouseTransferReq newwtr = cos.CreateObject<WarehouseTransferReq>();
                        IObjectSpace os = Application.CreateObjectSpace();

                        if (dtl.ItemInquiry.Cart != null)
                        {
                            WarehouseTransferReq trx = os.FindObject<WarehouseTransferReq>(new BinaryOperator("DocNum", dtl.ItemInquiry.Cart));

                            if (trx != null)
                            {
                                WarehouseTransferReqDetails newWTRdetail = os.CreateObject<WarehouseTransferReqDetails>();
                                newWTRdetail.FromWarehouse = newWTRdetail.Session.GetObjectByKey<vwWarehouse>(trx.FromWarehouse.WarehouseCode);
                                newWTRdetail.ToWarehouse = newWTRdetail.Session.GetObjectByKey<vwWarehouse>(trx.ToWarehouse.WarehouseCode);
                                newWTRdetail.ItemCode = newWTRdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                                newWTRdetail.ItemDesc = dtl.ItemDesc;
                                newWTRdetail.UOM = newWTRdetail.ItemCode.UOM;
                                newWTRdetail.CatalogNo = dtl.OriginalCatalog;
                                trx.WarehouseTransferReqDetails.Add(newWTRdetail);
                            }
                            else
                            {
                                WarehouseTransferReqDetails newWTRdetail = cos.CreateObject<WarehouseTransferReqDetails>();
                                newWTRdetail.FromWarehouse = newWTRdetail.Session.GetObjectByKey<vwWarehouse>(newwtr.FromWarehouse.WarehouseCode);
                                newWTRdetail.ToWarehouse = newWTRdetail.Session.GetObjectByKey<vwWarehouse>(newwtr.ToWarehouse.WarehouseCode);
                                newWTRdetail.ItemCode = newWTRdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                                newWTRdetail.ItemDesc = dtl.ItemDesc;
                                newWTRdetail.UOM = newWTRdetail.ItemCode.UOM;
                                newWTRdetail.CatalogNo = dtl.OriginalCatalog;
                                newwtr.WarehouseTransferReqDetails.Add(newWTRdetail);
                            }
                        }
                        else
                        {
                            showMsg("Error", "Please specify cart no.", InformationType.Error);
                            return;
                        }

                        os.CommitChanges();

                        showMsg("Success", "Added to Warehouse Transfer Request.", InformationType.Success);
                    }
                    else if (dtl.ItemInquiry.DocType == DocTypeList.SAR)
                    {
                        IObjectSpace cos = Application.CreateObjectSpace();
                        StockAdjustmentRequests newsar = cos.CreateObject<StockAdjustmentRequests>();
                        IObjectSpace os = Application.CreateObjectSpace();

                        if (dtl.ItemInquiry.Cart != null)
                        {
                            StockAdjustmentRequests trx = os.FindObject<StockAdjustmentRequests>(new BinaryOperator("DocNum", dtl.ItemInquiry.Cart));

                            if (trx != null)
                            {
                                StockAdjustmentReqDetails newSARdetail = os.CreateObject<StockAdjustmentReqDetails>();
                                newSARdetail.ItemCode = newSARdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                                newSARdetail.ItemDesc = dtl.ItemDesc;
                                newSARdetail.UOM = newSARdetail.ItemCode.UOM;
                                newSARdetail.Warehouse = newSARdetail.Session.GetObjectByKey<vwWarehouse>(dtl.ItemInquiry.Stock1.WarehouseCode);
                                newSARdetail.CatalogNo = dtl.OriginalCatalog;
                                trx.StockAdjustmentReqDetails.Add(newSARdetail);
                            }
                            else
                            {
                                StockAdjustmentReqDetails newSARdetail = cos.CreateObject<StockAdjustmentReqDetails>();
                                newSARdetail.ItemCode = newSARdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                                newSARdetail.ItemDesc = dtl.ItemDesc;
                                newSARdetail.UOM = newSARdetail.ItemCode.UOM;
                                newSARdetail.Warehouse = newSARdetail.Session.GetObjectByKey<vwWarehouse>(dtl.ItemInquiry.Stock1.WarehouseCode);
                                newSARdetail.CatalogNo = dtl.OriginalCatalog;
                                newsar.StockAdjustmentReqDetails.Add(newSARdetail);
                            }
                        }
                        else
                        {
                            showMsg("Error", "Please specify cart no.", InformationType.Error);
                            return;
                        }

                        os.CommitChanges();

                        showMsg("Success", "Added to Warehouse Transfer Request.", InformationType.Success);
                    }
                    else if (dtl.ItemInquiry.DocType == DocTypeList.SRF)
                    {
                        IObjectSpace cos = Application.CreateObjectSpace();
                        SalesRefundRequests newsfr = cos.CreateObject<SalesRefundRequests>();
                        IObjectSpace os = Application.CreateObjectSpace();

                        if (dtl.ItemInquiry.Cart != null)
                        {
                            SalesRefundRequests trx = os.FindObject<SalesRefundRequests>(new BinaryOperator("DocNum", dtl.ItemInquiry.Cart));

                            if (trx != null)
                            {
                                SalesRefundReqDetails newSARdetail = os.CreateObject<SalesRefundReqDetails>();
                                newSARdetail.Customer = newSARdetail.Session.GetObjectByKey<vwBusniessPartner>(trx.Customer.BPCode);
                                newSARdetail.ItemCode = newSARdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                                newSARdetail.ItemDesc = dtl.ItemDesc;
                                newSARdetail.UOM = newSARdetail.ItemCode.UOM;
                                newSARdetail.Warehouse = newSARdetail.Session.GetObjectByKey<vwWarehouse>(dtl.ItemInquiry.Stock1.WarehouseCode);
                                trx.SalesRefundReqDetails.Add(newSARdetail);
                            }
                            else
                            {
                                SalesRefundReqDetails newSARdetail = cos.CreateObject<SalesRefundReqDetails>();
                                newSARdetail.ItemCode = newSARdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                                newSARdetail.ItemDesc = dtl.ItemDesc;
                                newSARdetail.UOM = newSARdetail.ItemCode.UOM;
                                newSARdetail.Warehouse = newSARdetail.Session.GetObjectByKey<vwWarehouse>(dtl.ItemInquiry.Stock1.WarehouseCode);
                                newsfr.SalesRefundReqDetails.Add(newSARdetail);
                            }
                        }
                        else
                        {
                            showMsg("Error", "Please specify cart no.", InformationType.Error);
                            return;
                        }

                        os.CommitChanges();

                        showMsg("Success", "Added to Sales Refund Request.", InformationType.Success);
                    }

                    dtl.ItemInquiry.Search = null;
                    dtl.ItemInquiry.Exclude = null;
                    dtl.ItemInquiry.Method = SearchMethod.AND;
                }

                //selectedObject.ItemInquiry.Search = null;
                //selectedObject.ItemInquiry.Exclude = null;
                //selectedObject.ItemInquiry.Method = SearchMethod.AND;

                //View.ObjectSpace.Delete(((ListView)View).CollectionSource.List);

                string deleterecord = "DELETE FROM ItemInquiryDetails WHERE ItemInquiry = " + oid;
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(deleterecord, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                cmd.Dispose();
                conn.Close();

                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();
            //}
        }

        private void AddToCartPop_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            //ItemInquiryDetails iteminquiry = (ItemInquiryDetails)View.CurrentObject;

            var os = Application.CreateObjectSpace(typeof(Confirmation));
            Confirmation message = os.CreateObject<Confirmation>();

            DetailView dv = Application.CreateDetailView(os, message);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((Confirmation)dv.CurrentObject).Message = "Do you want to proceed to add in document?";

            //if (iteminquiry != null)
            //{
            //    if (iteminquiry.ItemInquiry.DocType == DocTypeList.SQ)
            //    {
            //        ((Confirmation)dv.CurrentObject).Message = "Do you want to proceed to add in this Sales Quotation (" + iteminquiry.ItemInquiry.Cart + ") ?";
            //    }
            //    else if (iteminquiry.ItemInquiry.DocType == DocTypeList.PO)
            //    {
            //        ((Confirmation)dv.CurrentObject).Message = "Do you want to proceed to add in this Purchase Order (" + iteminquiry.ItemInquiry.Cart + ") ?";
            //    }
            //    if (iteminquiry.ItemInquiry.DocType == DocTypeList.PRR)
            //    {
            //        ((Confirmation)dv.CurrentObject).Message = "Do you want to proceed to add in this Purchase Return Request (" + iteminquiry.ItemInquiry.Cart + ") ?";
            //    }
            //    if (iteminquiry.ItemInquiry.DocType == DocTypeList.SRR)
            //    {
            //        ((Confirmation)dv.CurrentObject).Message = "Do you want to proceed to add in this Sales Return Request (" + iteminquiry.ItemInquiry.Cart + ") ?";
            //    }
            //    if (iteminquiry.ItemInquiry.DocType == DocTypeList.WTR)
            //    {
            //        ((Confirmation)dv.CurrentObject).Message = "Do you want to proceed to add in this Warehouse Transfer Request (" + iteminquiry.ItemInquiry.Cart + ") ?";
            //    }
            //    if (iteminquiry.ItemInquiry.DocType == DocTypeList.SAR)
            //    {
            //        ((Confirmation)dv.CurrentObject).Message = "Do you want to proceed to add in this Stock Adjustment Request (" + iteminquiry.ItemInquiry.Cart + ") ?";
            //    }
            //    if (iteminquiry.ItemInquiry.DocType == DocTypeList.SRF)
            //    {
            //        ((Confirmation)dv.CurrentObject).Message = "Do you want to proceed to add in this Sales Refund Request (" + iteminquiry.ItemInquiry.Cart + ") ?";
            //    }
            //}
            //else
            //{
            //    ((Confirmation)dv.CurrentObject).Message = "No item selected.";
            //}

            e.View = dv;
        }

        private void ViewItemPicture_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            // Start ver 1.0.23
            ArrayList selectedHdr = new ArrayList();
            // End ver 1.0.23

            // Start ver 1.0.22
            if (View.ObjectTypeInfo.Type == typeof(ItemInquiryDetails))
            {
                // Start ver 1.0.23
                //ItemInquiryDetails iteminquiry = (ItemInquiryDetails)View.CurrentObject;

                if (View.SelectedObjects.Count > 0)
                {
                    foreach (var selectedObject in View.SelectedObjects)
                    {
                        selectedHdr.Add((ItemInquiryDetails)ObjectSpace.GetObject(selectedObject));
                    }
                }
                // End ver 1.0.23

                // Start ver 1.0.23
                foreach (ItemInquiryDetails iteminquiry in selectedHdr)
                {
                // End ver 1.0.23
                    if (iteminquiry.PictureName != null)
                    {
                        string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                            ConfigurationManager.AppSettings.Get("ItemPicturePath").ToString() + iteminquiry.PictureName;
                        var script = "window.open('" + url + "');";

                        WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);
                    }
                    else
                    {
                        showMsg("Error", "No picture file.", InformationType.Error);
                    }
                    // Start ver 1.0.23
                }
                // End ver 1.0.23
            }

            if (View.ObjectTypeInfo.Type == typeof(GlobalItemInquiryDetails))
            {
                // Start ver 1.0.23
                //GlobalItemInquiryDetails iteminquiry = (GlobalItemInquiryDetails)View.CurrentObject;

                if (View.SelectedObjects.Count > 0)
                {
                    foreach (var selectedObject in View.SelectedObjects)
                    {
                        selectedHdr.Add((GlobalItemInquiryDetails)ObjectSpace.GetObject(selectedObject));
                    }
                }
                // End ver 1.0.23

                // Start ver 1.0.23
                foreach (GlobalItemInquiryDetails iteminquiry in selectedHdr)
                {
                // End ver 1.0.23
                    if (iteminquiry.PictureName != null)
                    {
                        string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                            ConfigurationManager.AppSettings.Get("ItemPicturePath").ToString() + iteminquiry.PictureName;
                        var script = "window.open('" + url + "');";

                        WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);
                    }
                    else
                    {
                        showMsg("Error", "No picture file.", InformationType.Error);
                    }
                    // Start ver 1.0.23
                }
                // End ver 1.0.23
            }
            // End ver 1.0.22
        }

        // Start ver 1.0.7
        private void AddToCartSimple_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //ItemInquiryDetails selectedObject = (ItemInquiryDetails)e.CurrentObject;
            SqlConnection conn = new SqlConnection(genCon.getConnectionString());
            int oid = 0;

            // Start ver 1.0.23
            ArrayList selectedHdr = new ArrayList();

            if (View.SelectedObjects.Count > 0)
            {
                foreach (var selectedObject in View.SelectedObjects)
                {
                    selectedHdr.Add((ItemInquiryDetails)ObjectSpace.GetObject(selectedObject));
                }
            }
            // End ver 1.0.23

            // Start ver 1.0.23
            //foreach (ItemInquiryDetails dtl in e.SelectedObjects)
            foreach (ItemInquiryDetails dtl in selectedHdr)
            // End ver 1.0.23
            {
                oid = dtl.ItemInquiry.Oid;
                if (dtl.ItemInquiry.DocType == DocTypeList.SQ)
                {
                    IObjectSpace cos = Application.CreateObjectSpace();
                    SalesQuotation newSQ = cos.CreateObject<SalesQuotation>();
                    IObjectSpace os = Application.CreateObjectSpace();

                    if (dtl.ItemInquiry.Cart != null)
                    {
                        SalesQuotation trx = os.FindObject<SalesQuotation>(new BinaryOperator("DocNum", dtl.ItemInquiry.Cart));

                        if (trx != null)
                        {
                            // Start ver 1.0.22
                            if (trx.Status != DocStatus.Draft)
                            {
                                showMsg("Failed", "Not allow add item due to not draft document.", InformationType.Error);
                                return;
                            }
                            // End ver 1.0.22

                            SalesQuotationDetails newSQdetail = os.CreateObject<SalesQuotationDetails>();
                            newSQdetail.Postingdate = trx.PostingDate;
                            newSQdetail.Customer = newSQdetail.Session.GetObjectByKey<vwBusniessPartner>(trx.Customer.BPCode);
                            // Start ver 1.0.19
                            //newSQdetail.Location = newSQdetail.Session.GetObjectByKey<vwWarehouse>(dtl.ItemInquiry.Stock1.WarehouseCode);
                            if (newSQdetail.Customer.DfltWhs != null)
                            {
                                newSQdetail.Location = newSQdetail.Session.GetObjectByKey<vwWarehouse>(newSQdetail.Customer.DfltWhs);
                            }
                            else
                            {
                                newSQdetail.Location = newSQdetail.Session.GetObjectByKey<vwWarehouse>(dtl.ItemInquiry.Stock1.WarehouseCode);
                            }
                            // End ver 1.0.19
                            newSQdetail.ItemCode = newSQdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                            newSQdetail.ItemDesc = dtl.ItemDesc;
                            newSQdetail.Model = dtl.Model;
                            newSQdetail.CatalogNo = dtl.OriginalCatalog;
                            //newSQdetail.Price = dtl.Price1;
                            //newSQdetail.AdjustedPrice = dtl.Price1;
                            trx.SalesQuotationDetails.Add(newSQdetail);
                        }
                        else
                        {
                            newSQ.Priority = newSQ.Session.FindObject<PriorityType>
                                (CriteriaOperator.Parse("PriorityName = ? and IsActive= ?",
                                "Normal", "True"));

                            SalesQuotationDetails newSQdetail = cos.CreateObject<SalesQuotationDetails>();
                            newSQdetail.ItemCode = newSQdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                            newSQdetail.ItemDesc = dtl.ItemDesc;
                            newSQdetail.Model = dtl.Model;
                            newSQdetail.CatalogNo = dtl.OriginalCatalog;
                            newSQdetail.Location = newSQdetail.Session.GetObjectByKey<vwWarehouse>(dtl.ItemInquiry.Stock1.WarehouseCode);
                            //newSQdetail.Price = dtl.Price1;
                            //newSQdetail.AdjustedPrice = dtl.Price1;
                            newSQ.SalesQuotationDetails.Add(newSQdetail);
                        }
                    }
                    else
                    {
                        showMsg("Error", "Please specify cart no.", InformationType.Error);
                        return;
                    }

                    os.CommitChanges();

                    showMsg("Success", "Added to Sales Quoatation.", InformationType.Success);
                }
                else if (dtl.ItemInquiry.DocType == DocTypeList.PO)
                {
                    IObjectSpace cos = Application.CreateObjectSpace();
                    PurchaseOrders newPO = cos.CreateObject<PurchaseOrders>();
                    IObjectSpace os = Application.CreateObjectSpace();

                    if (dtl.ItemInquiry.Cart != null)
                    {
                        PurchaseOrders trx = os.FindObject<PurchaseOrders>(new BinaryOperator("DocNum", dtl.ItemInquiry.Cart));

                        if (trx != null)
                        {
                            // Start ver 1.0.22
                            if (trx.Status != DocStatus.Draft)
                            {
                                showMsg("Failed", "Not allow add item due to not draft document.", InformationType.Error);
                                return;
                            }
                            // End ver 1.0.22

                            PurchaseOrderDetails newPOdetail = os.CreateObject<PurchaseOrderDetails>();
                            newPOdetail.Postingdate = trx.PostingDate;
                            newPOdetail.Supplier = newPOdetail.Session.GetObjectByKey<vwBusniessPartner>(trx.Supplier.BPCode);
                            newPOdetail.ItemCode = newPOdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                            newPOdetail.ItemDesc = dtl.ItemDesc;
                            newPOdetail.Model = dtl.Model;
                            newPOdetail.Location = newPOdetail.Session.GetObjectByKey<vwWarehouse>(trx.Warehouse.WarehouseCode);
                            newPOdetail.CatalogNo = dtl.OriginalCatalog;
                            //newPOdetail.Price = dtl.Price1;
                            //newPOdetail.AdjustedPrice = dtl.Price1;
                            trx.PurchaseOrderDetails.Add(newPOdetail);
                        }
                        else
                        {
                            PurchaseOrderDetails newPOdetail = cos.CreateObject<PurchaseOrderDetails>();
                            newPOdetail.ItemCode = newPOdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                            newPOdetail.ItemDesc = dtl.ItemDesc;
                            newPOdetail.Model = dtl.Model;
                            newPOdetail.Location = newPOdetail.Session.GetObjectByKey<vwWarehouse>(trx.Warehouse.WarehouseCode);
                            newPOdetail.CatalogNo = dtl.OriginalCatalog;
                            //newPOdetail.Price = dtl.Price1;
                            //newPOdetail.AdjustedPrice = dtl.Price1;
                            newPO.PurchaseOrderDetails.Add(newPOdetail);
                        }
                    }
                    else
                    {
                        showMsg("Error", "Please specify cart no.", InformationType.Error);
                        return;
                    }

                    os.CommitChanges();

                    showMsg("Success", "Added to Purchase Order.", InformationType.Success);
                }
                else if (dtl.ItemInquiry.DocType == DocTypeList.PRR)
                {
                    IObjectSpace cos = Application.CreateObjectSpace();
                    PurchaseReturnRequests newprr = cos.CreateObject<PurchaseReturnRequests>();
                    IObjectSpace os = Application.CreateObjectSpace();

                    if (dtl.ItemInquiry.Cart != null)
                    {
                        PurchaseReturnRequests trx = os.FindObject<PurchaseReturnRequests>(new BinaryOperator("DocNum", dtl.ItemInquiry.Cart));

                        if (trx != null)
                        {
                            // Start ver 1.0.22
                            if (trx.Status != DocStatus.Draft)
                            {
                                showMsg("Failed", "Not allow add item due to not draft document.", InformationType.Error);
                                return;
                            }
                            // End ver 1.0.22

                            PurchaseReturnRequestDetails newPRRdetail = os.CreateObject<PurchaseReturnRequestDetails>();
                            newPRRdetail.ItemCode = newPRRdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                            newPRRdetail.ItemDesc = dtl.ItemDesc;
                            newPRRdetail.UOM = newPRRdetail.ItemCode.UOM;
                            newPRRdetail.Warehouse = newPRRdetail.Session.GetObjectByKey<vwWarehouse>(dtl.ItemInquiry.Stock1.WarehouseCode);
                            newPRRdetail.Price = dtl.Price1;
                            trx.PurchaseReturnRequestDetails.Add(newPRRdetail);
                        }
                        else
                        {
                            PurchaseReturnRequestDetails newPRRdetail = cos.CreateObject<PurchaseReturnRequestDetails>();
                            newPRRdetail.ItemCode = newPRRdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                            newPRRdetail.ItemDesc = dtl.ItemDesc;
                            newPRRdetail.UOM = newPRRdetail.ItemCode.UOM;
                            newPRRdetail.Warehouse = newPRRdetail.Session.GetObjectByKey<vwWarehouse>(dtl.ItemInquiry.Stock1.WarehouseCode);
                            newPRRdetail.Price = dtl.Price1;
                            newprr.PurchaseReturnRequestDetails.Add(newPRRdetail);
                        }
                    }
                    else
                    {
                        showMsg("Error", "Please specify cart no.", InformationType.Error);
                        return;
                    }

                    os.CommitChanges();

                    showMsg("Success", "Added to Purchase Return Request.", InformationType.Success);
                }
                else if (dtl.ItemInquiry.DocType == DocTypeList.SRR)
                {
                    IObjectSpace cos = Application.CreateObjectSpace();
                    SalesReturnRequests newsrr = cos.CreateObject<SalesReturnRequests>();
                    IObjectSpace os = Application.CreateObjectSpace();

                    if (dtl.ItemInquiry.Cart != null)
                    {
                        SalesReturnRequests trx = os.FindObject<SalesReturnRequests>(new BinaryOperator("DocNum", dtl.ItemInquiry.Cart));

                        if (trx != null)
                        {
                            // Start ver 1.0.22
                            if (trx.Status != DocStatus.Draft)
                            {
                                showMsg("Failed", "Not allow add item due to not draft document.", InformationType.Error);
                                return;
                            }
                            // End ver 1.0.22

                            SalesReturnRequestDetails newSRRdetail = os.CreateObject<SalesReturnRequestDetails>();
                            newSRRdetail.ItemCode = newSRRdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                            newSRRdetail.ItemDesc = dtl.ItemDesc;
                            newSRRdetail.UOM = newSRRdetail.ItemCode.UOM;
                            newSRRdetail.Warehouse = newSRRdetail.Session.GetObjectByKey<vwWarehouse>(dtl.ItemInquiry.Stock1.WarehouseCode);
                            newSRRdetail.Price = dtl.Price1;
                            trx.SalesReturnRequestDetails.Add(newSRRdetail);
                        }
                        else
                        {
                            SalesReturnRequestDetails newSRRdetail = cos.CreateObject<SalesReturnRequestDetails>();
                            newSRRdetail.ItemCode = newSRRdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                            newSRRdetail.ItemDesc = dtl.ItemDesc;
                            newSRRdetail.UOM = newSRRdetail.ItemCode.UOM;
                            newSRRdetail.Warehouse = newSRRdetail.Session.GetObjectByKey<vwWarehouse>(dtl.ItemInquiry.Stock1.WarehouseCode);
                            newSRRdetail.Price = dtl.Price1;
                            newsrr.SalesReturnRequestDetails.Add(newSRRdetail);
                        }
                    }
                    else
                    {
                        showMsg("Error", "Please specify cart no.", InformationType.Error);
                        return;
                    }

                    os.CommitChanges();

                    showMsg("Success", "Added to Sales Return Request.", InformationType.Success);
                }
                else if (dtl.ItemInquiry.DocType == DocTypeList.WTR)
                {
                    IObjectSpace cos = Application.CreateObjectSpace();
                    WarehouseTransferReq newwtr = cos.CreateObject<WarehouseTransferReq>();
                    IObjectSpace os = Application.CreateObjectSpace();

                    if (dtl.ItemInquiry.Cart != null)
                    {
                        WarehouseTransferReq trx = os.FindObject<WarehouseTransferReq>(new BinaryOperator("DocNum", dtl.ItemInquiry.Cart));

                        if (trx != null)
                        {
                            // Start ver 1.0.22
                            if (trx.Status != DocStatus.Draft)
                            {
                                showMsg("Failed", "Not allow add item due to not draft document.", InformationType.Error);
                                return;
                            }
                            // End ver 1.0.22

                            WarehouseTransferReqDetails newWTRdetail = os.CreateObject<WarehouseTransferReqDetails>();
                            newWTRdetail.FromWarehouse = newWTRdetail.Session.GetObjectByKey<vwWarehouse>(trx.FromWarehouse.WarehouseCode);
                            newWTRdetail.ToWarehouse = newWTRdetail.Session.GetObjectByKey<vwWarehouse>(trx.ToWarehouse.WarehouseCode);
                            newWTRdetail.ItemCode = newWTRdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                            newWTRdetail.ItemDesc = dtl.ItemDesc;
                            newWTRdetail.UOM = newWTRdetail.ItemCode.UOM;
                            newWTRdetail.CatalogNo = dtl.OriginalCatalog;
                            trx.WarehouseTransferReqDetails.Add(newWTRdetail);
                        }
                        else
                        {
                            WarehouseTransferReqDetails newWTRdetail = cos.CreateObject<WarehouseTransferReqDetails>();
                            newWTRdetail.FromWarehouse = newWTRdetail.Session.GetObjectByKey<vwWarehouse>(newwtr.FromWarehouse.WarehouseCode);
                            newWTRdetail.ToWarehouse = newWTRdetail.Session.GetObjectByKey<vwWarehouse>(newwtr.ToWarehouse.WarehouseCode);
                            newWTRdetail.ItemCode = newWTRdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                            newWTRdetail.ItemDesc = dtl.ItemDesc;
                            newWTRdetail.UOM = newWTRdetail.ItemCode.UOM;
                            newWTRdetail.CatalogNo = dtl.OriginalCatalog;
                            newwtr.WarehouseTransferReqDetails.Add(newWTRdetail);
                        }
                    }
                    else
                    {
                        showMsg("Error", "Please specify cart no.", InformationType.Error);
                        return;
                    }

                    os.CommitChanges();

                    showMsg("Success", "Added to Warehouse Transfer Request.", InformationType.Success);
                }
                else if (dtl.ItemInquiry.DocType == DocTypeList.SAR)
                {
                    IObjectSpace cos = Application.CreateObjectSpace();
                    StockAdjustmentRequests newsar = cos.CreateObject<StockAdjustmentRequests>();
                    IObjectSpace os = Application.CreateObjectSpace();

                    if (dtl.ItemInquiry.Cart != null)
                    {
                        StockAdjustmentRequests trx = os.FindObject<StockAdjustmentRequests>(new BinaryOperator("DocNum", dtl.ItemInquiry.Cart));

                        if (trx != null)
                        {
                            // Start ver 1.0.22
                            if (trx.Status != DocStatus.Draft)
                            {
                                showMsg("Failed", "Not allow add item due to not draft document.", InformationType.Error);
                                return;
                            }
                            // End ver 1.0.22

                            StockAdjustmentReqDetails newSARdetail = os.CreateObject<StockAdjustmentReqDetails>();
                            newSARdetail.ItemCode = newSARdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                            newSARdetail.ItemDesc = dtl.ItemDesc;
                            newSARdetail.UOM = newSARdetail.ItemCode.UOM;
                            newSARdetail.Warehouse = newSARdetail.Session.GetObjectByKey<vwWarehouse>(dtl.ItemInquiry.Stock1.WarehouseCode);
                            newSARdetail.CatalogNo = dtl.OriginalCatalog;
                            trx.StockAdjustmentReqDetails.Add(newSARdetail);
                        }
                        else
                        {
                            StockAdjustmentReqDetails newSARdetail = cos.CreateObject<StockAdjustmentReqDetails>();
                            newSARdetail.ItemCode = newSARdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                            newSARdetail.ItemDesc = dtl.ItemDesc;
                            newSARdetail.UOM = newSARdetail.ItemCode.UOM;
                            newSARdetail.Warehouse = newSARdetail.Session.GetObjectByKey<vwWarehouse>(dtl.ItemInquiry.Stock1.WarehouseCode);
                            newSARdetail.CatalogNo = dtl.OriginalCatalog;
                            newsar.StockAdjustmentReqDetails.Add(newSARdetail);
                        }
                    }
                    else
                    {
                        showMsg("Error", "Please specify cart no.", InformationType.Error);
                        return;
                    }

                    os.CommitChanges();

                    showMsg("Success", "Added to Warehouse Transfer Request.", InformationType.Success);
                }
                else if (dtl.ItemInquiry.DocType == DocTypeList.SRF)
                {
                    IObjectSpace cos = Application.CreateObjectSpace();
                    SalesRefundRequests newsfr = cos.CreateObject<SalesRefundRequests>();
                    IObjectSpace os = Application.CreateObjectSpace();

                    if (dtl.ItemInquiry.Cart != null)
                    {
                        SalesRefundRequests trx = os.FindObject<SalesRefundRequests>(new BinaryOperator("DocNum", dtl.ItemInquiry.Cart));

                        if (trx != null)
                        {
                            // Start ver 1.0.22
                            if (trx.Status != DocStatus.Draft)
                            {
                                showMsg("Failed", "Not allow add item due to not draft document.", InformationType.Error);
                                return;
                            }
                            // End ver 1.0.22

                            SalesRefundReqDetails newSARdetail = os.CreateObject<SalesRefundReqDetails>();
                            newSARdetail.Customer = newSARdetail.Session.GetObjectByKey<vwBusniessPartner>(trx.Customer.BPCode);
                            newSARdetail.ItemCode = newSARdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                            newSARdetail.ItemDesc = dtl.ItemDesc;
                            newSARdetail.UOM = newSARdetail.ItemCode.UOM;
                            newSARdetail.Warehouse = newSARdetail.Session.GetObjectByKey<vwWarehouse>(dtl.ItemInquiry.Stock1.WarehouseCode);
                            trx.SalesRefundReqDetails.Add(newSARdetail);
                        }
                        else
                        {
                            SalesRefundReqDetails newSARdetail = cos.CreateObject<SalesRefundReqDetails>();
                            newSARdetail.ItemCode = newSARdetail.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                            newSARdetail.ItemDesc = dtl.ItemDesc;
                            newSARdetail.UOM = newSARdetail.ItemCode.UOM;
                            newSARdetail.Warehouse = newSARdetail.Session.GetObjectByKey<vwWarehouse>(dtl.ItemInquiry.Stock1.WarehouseCode);
                            newsfr.SalesRefundReqDetails.Add(newSARdetail);
                        }
                    }
                    else
                    {
                        showMsg("Error", "Please specify cart no.", InformationType.Error);
                        return;
                    }

                    os.CommitChanges();

                    showMsg("Success", "Added to Sales Refund Request.", InformationType.Success);
                }

                dtl.ItemInquiry.Search = null;
                dtl.ItemInquiry.Exclude = null;
                dtl.ItemInquiry.Method = SearchMethod.AND;
            }

            //selectedObject.ItemInquiry.Search = null;
            //selectedObject.ItemInquiry.Exclude = null;
            //selectedObject.ItemInquiry.Method = SearchMethod.AND;

            //View.ObjectSpace.Delete(((ListView)View).CollectionSource.List);

            //string deleterecord = "DELETE FROM ItemInquiryDetails WHERE ItemInquiry = " + oid;
            //if (conn.State == ConnectionState.Open)
            //{
            //    conn.Close();
            //}
            //conn.Open();
            //SqlCommand cmd = new SqlCommand(deleterecord, conn);
            //SqlDataReader reader = cmd.ExecuteReader();
            //conn.Close();

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            //}
        }
        // End ver 1.0.7

        // Start ver 1.0.13
        private void ViewOrderStatus_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {

        }

        private void ViewOrderStatus_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            // Start ver 1.0.23
            //ItemInquiryDetails iteminquiry = (ItemInquiryDetails)View.CurrentObject;

            ArrayList selectedHdr = new ArrayList();
            string OrderStatusItemCode = "";

            if (View.SelectedObjects.Count > 0)
            {
                foreach (var selectedObject in View.SelectedObjects)
                {
                    selectedHdr.Add((ItemInquiryDetails)ObjectSpace.GetObject(selectedObject));
                }
            }
            // End ver 1.0.23

            IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(OrderStatus));

            XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();
            var nonPersistentOS = Application.CreateObjectSpace(typeof(OrderStatusList));
            OrderStatusList orderstatuslist = nonPersistentOS.CreateObject<OrderStatusList>();

            // Start ver 1.0.23
            //if (iteminquiry != null)
            foreach (ItemInquiryDetails iteminquiry in selectedHdr)
            // End ver 1.0.23
            {
                // Start ver 1.0.23
                OrderStatusItemCode = iteminquiry.ItemCode + " - " + iteminquiry.ItemDesc;
                // End ver 1.0.23

                SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_GetItemOrderStatus", new OperandValue(iteminquiry.ItemCode));

                int i = 1;

                if (sprocData.ResultSet.Count() > 0)
                {
                    if (sprocData.ResultSet[0].Rows.Count() > 0)
                    {
                        foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                        {
                            var itemos = Application.CreateObjectSpace(typeof(OrderStatusList));
                            var item = itemos.CreateObject<OrderStatus>();
                            item.Id = i;
                            item.No = i;
                            item.ItemCode = row.Values[1].ToString();
                            item.LegacyCode = row.Values[2].ToString();
                            item.ItemName = row.Values[3].ToString();
                            item.Origin = row.Values[4].ToString();
                            item.Warehouse = row.Values[5].ToString();
                            item.DocNo = row.Values[6].ToString();
                            item.Quantity = (decimal)row.Values[7];
                            item.ESRDate = (DateTime)row.Values[8];
                            // Start ver 1.0.14
                            item.PODate = (DateTime)row.Values[9];
                            item.PODelivery = (DateTime)row.Values[10];
                            // End ver 1.0.14

                            orderstatuslist.Orderstatus.Add(item);

                            i++;
                        }
                    }
                }
            }

            nonPersistentOS.CommitChanges();
            persistentObjectSpace.Session.DropIdentityMap();
            persistentObjectSpace.Dispose();

            DetailView detailView = Application.CreateDetailView(nonPersistentOS, orderstatuslist);
            detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            // Start ver 1.0.23
            //if (iteminquiry != null)
            //{
            //    ((OrderStatusList)detailView.CurrentObject).ItemCode = iteminquiry.ItemCode + " - " + iteminquiry.ItemDesc;
            //}
            if (OrderStatusItemCode != "")
            {
                ((OrderStatusList)detailView.CurrentObject).ItemCode = OrderStatusItemCode;
            }
            // End ver 1.0.23
            e.View = detailView;
            e.DialogController.SaveOnAccept = false;
            e.DialogController.CancelAction.Active["NothingToCancel"] = false;
        }
        // End ver 1.0.13

        // Start ver 1.0.22
        private void GlobalSearch_ItemInquiry_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            string cardcode = "";
            GlobalItemInquiry selectedObject = (GlobalItemInquiry)e.CurrentObject;

            if (selectedObject.Search != null && selectedObject.OldCode == null && selectedObject.CatalogNumber == null ||
                selectedObject.Search == null && selectedObject.OldCode != null && selectedObject.CatalogNumber == null ||
                selectedObject.Search == null && selectedObject.OldCode == null && selectedObject.CatalogNumber != null ||
                selectedObject.Search == null && selectedObject.OldCode == null && selectedObject.CatalogNumber == null)
            {
                // Start ver 1.0.23
                if (selectedObject.Search != null)
                {
                    if (!string.IsNullOrEmpty(selectedObject.Search.Trim(' ')))
                    // End ver 1.0.23
                    {
                        XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();
                        SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_GetItemGlobal", new OperandValue(selectedObject.Search),
                            new OperandValue(selectedObject.Exclude),
                            new OperandValue(selectedObject.Method), new OperandValue(cardcode), new OperandValue(selectedObject.Oid));

                        persistentObjectSpace.Session.DropIdentityMap();
                        persistentObjectSpace.Dispose();
                    }
                    // Start ver 1.0.23
                    else
                    {
                        genCon.showMsg("Fail", "Not allow to search without any keyword.", InformationType.Error);
                        return;
                    }
                    // End ver 1.0.23
                }

                // Start ver 1.0.23
                if (selectedObject.OldCode != null)
                {
                    if (!string.IsNullOrEmpty(selectedObject.OldCode.Trim(' ')))
                    // End ver 1.0.23
                    {
                        XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();
                        SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_GetItemOldCodeGlobal", new OperandValue(selectedObject.OldCode),
                            new OperandValue(selectedObject.Exclude),
                            new OperandValue(selectedObject.Method), new OperandValue(cardcode), new OperandValue(selectedObject.Oid));

                        persistentObjectSpace.Session.DropIdentityMap();
                        persistentObjectSpace.Dispose();
                    }
                    // Start ver 1.0.23
                    else
                    {
                        genCon.showMsg("Fail", "Not allow to search without any keyword.", InformationType.Error);
                        return;
                    }
                    // End ver 1.0.23
                }

                // Start ver 1.0.23
                if (selectedObject.CatalogNumber != null)
                {
                    if (!string.IsNullOrEmpty(selectedObject.CatalogNumber.Trim(' ')))
                    // End ver 1.0.23
                    {
                        XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();
                        SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_GetItemCatalogNumberGlobal", new OperandValue(selectedObject.CatalogNumber),
                            new OperandValue(selectedObject.Exclude),
                            new OperandValue(selectedObject.Method), new OperandValue(cardcode), new OperandValue(selectedObject.Oid));

                        persistentObjectSpace.Session.DropIdentityMap();
                        persistentObjectSpace.Dispose();
                    }
                    // Start ver 1.0.23
                    else
                    {
                        genCon.showMsg("Fail", "Not allow to search without any keyword.", InformationType.Error);
                        return;
                    }
                    // End ver 1.0.23
                }

                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();
                View.Refresh();
            }
            else
            {
                showMsg("Fail", "Not allow enter multiple searching field.", InformationType.Error);
            }
        }
        // End ver 1.0.22

        //// Start ver 1.0.23
        //#region Performance Optimization - Simple Caching Methods

        //private static readonly Dictionary<string, CacheItem> _simpleCache = new Dictionary<string, CacheItem>();
        //private static readonly object _cacheLock = new object();

        //private class CacheItem
        //{
        //    public SelectedData Data { get; set; }
        //    public DateTime ExpiryTime { get; set; }

        //    public bool IsExpired => DateTime.Now > ExpiryTime;
        //}

        ///// <summary>
        ///// Try to get cached sales data
        ///// </summary>
        //private SelectedData TryGetCachedSalesData(string cacheKey)
        //{
        //    try
        //    {
        //        lock (_cacheLock)
        //        {
        //            if (_simpleCache.TryGetValue(cacheKey, out CacheItem item))
        //            {
        //                if (!item.IsExpired)
        //                {
        //                    return item.Data;
        //                }
        //                else
        //                {
        //                    // Remove expired item
        //                    _simpleCache.Remove(cacheKey);
        //                }
        //            }
        //        }
        //        return null;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        ///// <summary>
        ///// Cache sales data with expiration
        ///// </summary>
        //private void CacheSalesData(string cacheKey, SelectedData data, TimeSpan expiry)
        //{
        //    try
        //    {
        //        lock (_cacheLock)
        //        {
        //            var cacheItem = new CacheItem
        //            {
        //                Data = data,
        //                ExpiryTime = DateTime.Now.Add(expiry)
        //            };

        //            _simpleCache[cacheKey] = cacheItem;

        //            // Clean up expired items (simple cleanup)
        //            if (_simpleCache.Count > 100)
        //            {
        //                CleanupExpiredItems();
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        // Silently fail if caching doesn't work
        //    }
        //}

        ///// <summary>
        ///// Clear cache for specific item
        ///// </summary>
        //private void ClearItemCache(string itemCode)
        //{
        //    try
        //    {
        //        string cacheKey = $"sales_history_{itemCode}";
        //        lock (_cacheLock)
        //        {
        //            _simpleCache.Remove(cacheKey);
        //        }
        //    }
        //    catch
        //    {
        //        // Silently fail
        //    }
        //}

        ///// <summary>
        ///// Clean up expired cache items
        ///// </summary>
        //private static void CleanupExpiredItems()
        //{
        //    var expiredKeys = new List<string>();
        //    foreach (var kvp in _simpleCache)
        //    {
        //        if (kvp.Value.IsExpired)
        //        {
        //            expiredKeys.Add(kvp.Key);
        //        }
        //    }

        //    foreach (var key in expiredKeys)
        //    {
        //        _simpleCache.Remove(key);
        //    }
        //}

        ///// <summary>
        ///// Get cache statistics for performance monitoring
        ///// </summary>
        //public static SimpleCacheStats GetCacheStats()
        //{
        //    lock (_cacheLock)
        //    {
        //        CleanupExpiredItems(); // Clean before counting
        //        return new SimpleCacheStats
        //        {
        //            ItemCount = _simpleCache.Count,
        //            EstimatedSizeMB = _simpleCache.Count * 10 // Rough estimate: 10KB per item
        //        };
        //    }
        //}

        ///// <summary>
        ///// Clear all cached items
        ///// </summary>
        //public static void ClearAllCache()
        //{
        //    lock (_cacheLock)
        //    {
        //        _simpleCache.Clear();
        //    }
        //}

        //public class SimpleCacheStats
        //{
        //    public int ItemCount { get; set; }
        //    public long EstimatedSizeMB { get; set; }
        //}

        //#endregion
        //// End ver 1.0.23
    }
}
