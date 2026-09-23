namespace StarLaiPortal.Module.Controllers
{
    partial class EMContainerControllers
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SubmitEMC = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.CancelEMC = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.ImportEMDO = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.ImportExternalDO = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.CopyFromDO = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.DeleteDO = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.PrintInvoiceEM = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // SubmitEMC
            // 
            this.SubmitEMC.Caption = "Submit";
            this.SubmitEMC.Category = "ObjectsCreation";
            this.SubmitEMC.ConfirmationMessage = null;
            this.SubmitEMC.Id = "SubmitEMC";
            this.SubmitEMC.ToolTip = null;
            this.SubmitEMC.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.SubmitEMC_Execute);
            // 
            // CancelEMC
            // 
            this.CancelEMC.AcceptButtonCaption = null;
            this.CancelEMC.CancelButtonCaption = null;
            this.CancelEMC.Caption = "Cancel";
            this.CancelEMC.Category = "ObjectsCreation";
            this.CancelEMC.ConfirmationMessage = null;
            this.CancelEMC.Id = "CancelEMC";
            this.CancelEMC.ToolTip = null;
            this.CancelEMC.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.CancelEMC_CustomizePopupWindowParams);
            this.CancelEMC.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.CancelEMC_Execute);
            // 
            // ImportEMDO
            // 
            this.ImportEMDO.AcceptButtonCaption = null;
            this.ImportEMDO.CancelButtonCaption = null;
            this.ImportEMDO.Caption = "Import DO       ";
            this.ImportEMDO.Category = "ListView";
            this.ImportEMDO.ConfirmationMessage = null;
            this.ImportEMDO.Id = "ImportEMDO";
            this.ImportEMDO.ToolTip = null;
            this.ImportEMDO.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.ImportEMDO_CustomizePopupWindowParams);
            this.ImportEMDO.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.ImportEMDO_Execute);
            // 
            // ImportExternalDO
            // 
            this.ImportExternalDO.AcceptButtonCaption = null;
            this.ImportExternalDO.CancelButtonCaption = null;
            this.ImportExternalDO.Caption = "Import Ext. DO";
            this.ImportExternalDO.Category = "ListView";
            this.ImportExternalDO.ConfirmationMessage = null;
            this.ImportExternalDO.Id = "ImportExternalDO";
            this.ImportExternalDO.ToolTip = null;
            this.ImportExternalDO.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.ImportExternalDO_CustomizePopupWindowParams);
            this.ImportExternalDO.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.ImportExternalDO_Execute);
            // 
            // CopyFromDO
            // 
            this.CopyFromDO.AcceptButtonCaption = null;
            this.CopyFromDO.CancelButtonCaption = null;
            this.CopyFromDO.Caption = "Copy From DO";
            this.CopyFromDO.Category = "ObjectsCreation";
            this.CopyFromDO.ConfirmationMessage = null;
            this.CopyFromDO.Id = "CopyFromDO";
            this.CopyFromDO.ToolTip = null;
            this.CopyFromDO.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.CopyFromDO_CustomizePopupWindowParams);
            this.CopyFromDO.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.CopyFromDO_Execute);
            // 
            // DeleteDO
            // 
            this.DeleteDO.Caption = "Delete";
            this.DeleteDO.Category = "Edit";
            this.DeleteDO.ConfirmationMessage = null;
            this.DeleteDO.Id = "DeleteDO";
            this.DeleteDO.ImageName = "Action_Delete";
            this.DeleteDO.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireMultipleObjects;
            this.DeleteDO.ToolTip = null;
            this.DeleteDO.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.DeleteDO_Execute);
            // 
            // PrintInvoiceEM
            // 
            this.PrintInvoiceEM.Caption = "Print Invoice";
            this.PrintInvoiceEM.Category = "ObjectsCreation";
            this.PrintInvoiceEM.ConfirmationMessage = null;
            this.PrintInvoiceEM.Id = "PrintInvoiceEM";
            this.PrintInvoiceEM.ToolTip = null;
            this.PrintInvoiceEM.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.PrintInvoiceEM_Execute);
            // 
            // EMContainerControllers
            // 
            this.Actions.Add(this.SubmitEMC);
            this.Actions.Add(this.CancelEMC);
            this.Actions.Add(this.ImportEMDO);
            this.Actions.Add(this.ImportExternalDO);
            this.Actions.Add(this.CopyFromDO);
            this.Actions.Add(this.DeleteDO);
            this.Actions.Add(this.PrintInvoiceEM);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction SubmitEMC;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CancelEMC;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction ImportEMDO;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction ImportExternalDO;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CopyFromDO;
        private DevExpress.ExpressApp.Actions.SimpleAction DeleteDO;
        private DevExpress.ExpressApp.Actions.SimpleAction PrintInvoiceEM;
    }
}
