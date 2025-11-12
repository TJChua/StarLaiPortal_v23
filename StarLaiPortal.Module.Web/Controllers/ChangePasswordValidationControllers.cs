using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;

namespace StarLaiPortal.Module.Web.Controllers
{
    public partial class ChangePasswordValidationControllers : ObjectViewController<DetailView, ChangePasswordOnLogonParameters>
    {
        public ChangePasswordValidationControllers()
        {
            InitializeComponent();
        }
        protected override void OnActivated()
        {
            base.OnActivated();

            DialogController dialogController = Frame.GetController<DialogController>();
            if (dialogController != null)
            {
                dialogController.AcceptAction.Executing += AcceptAction_Executing;
            }
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
        }
        protected override void OnDeactivated()
        {
            DialogController dialogController = Frame.GetController<DialogController>();
            if (dialogController != null)
            {
                dialogController.AcceptAction.Executing -= AcceptAction_Executing;
            }
            base.OnDeactivated();
        }

        private void AcceptAction_Executing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ChangePasswordOnLogonParameters parameters = View.CurrentObject as ChangePasswordOnLogonParameters;
            string query = "";

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());

            query = "SELECT ISNULL(PwdMinLen, 0), ISNULL(MinUppers, 0), ISNULL(MinLowCase, 0), " +
                "ISNULL(MinDigits, 0), ISNULL(MinNonAlph, 0) FROM [" + ConfigurationManager.AppSettings["SAPDB"].ToString() + "]..OPPA";
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open();
            SqlCommand cmdsecurity = new SqlCommand(query, conn);
            SqlDataReader readersecurity = cmdsecurity.ExecuteReader();
            while (readersecurity.Read())
            {
                if (parameters != null && parameters.NewPassword.Length < readersecurity.GetInt32(0))
                {
                    throw new UserFriendlyException("Password too short.");
                }

                if (parameters != null && parameters.NewPassword.Count(char.IsUpper) < readersecurity.GetInt32(1))
                {
                    throw new UserFriendlyException("Password must contain at least " + readersecurity.GetInt32(1) + " upper case.");
                }

                if (parameters != null && parameters.NewPassword.Count(char.IsLower) < readersecurity.GetInt32(2))
                {
                    throw new UserFriendlyException("Password must contain at least " + readersecurity.GetInt32(2) + " lower case.");
                }

                if (parameters != null && parameters.NewPassword.Count(char.IsDigit) < readersecurity.GetInt32(3))
                {
                    throw new UserFriendlyException("Password must contain at least " + readersecurity.GetInt32(3) + " digit.");
                }

                if (parameters != null && parameters.NewPassword.Where(x => !char.IsLetterOrDigit(x)).Count() < readersecurity.GetInt32(4))
                {
                    throw new UserFriendlyException("Password must contain at least " + readersecurity.GetInt32(4) + " special character.");
                }
            }
            cmdsecurity.Dispose();
            conn.Close();
        }
    }
}
