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
            // EMContainerControllers
            // 
            this.Actions.Add(this.SubmitEMC);
            this.Actions.Add(this.CancelEMC);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction SubmitEMC;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CancelEMC;
    }
}
