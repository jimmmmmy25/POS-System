using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace System
{
    public partial class frmForgotPass : Form
    {
        public frmForgotPass()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
        }
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;
        DBConnections dbcon = new DBConnections();
        string stitle = "Simple POS System";
        private void frmForgotPass_Load(object sender, EventArgs e)
        {

        }

        private void lblConfirmPass_Click(object sender, EventArgs e)
        {

        }

        private void cmbQuestionnaire_SelectedIndexChanged(object sender, EventArgs e)
        {

            cmbQuestionnaire.Items.AddRange(new string[]{

            "What is your childhood nickname?",
            "What is the name of your first pet?",
            "What is your mother's maiden name?",
            "What is your favorite color?",
            "What is your favorite subject?",
            "What is your favorite food?"
            });
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string newPassword = txtNewPass.Text;
            string confirmNewPassword = txtConfirmPass.Text;
            string selectedSecurityQuestion = cmbQuestionnaire.SelectedItem?.ToString();
            string securityAnswer = txtAnswer.Text;
            string username = txtUserName.Text;
            // Perform validation checks on the entered passwords and security answer
            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmNewPassword) ||
                string.IsNullOrWhiteSpace(selectedSecurityQuestion) || string.IsNullOrWhiteSpace(securityAnswer))
            {
                MessageBox.Show("Please fill out all fields.");
                return;
            }

            if (newPassword != confirmNewPassword)
            {
                MessageBox.Show("New passwords do not match.");
                return;
            }
            try
            {
                cn.Open();

                // Verify the security answer
                string verifyAnswerQuery = "SELECT Answer FROM tblaccounts WHERE username = @Username AND questionnaire = @questionnaire";
                using (SqlCommand verifyAnswerCmd = new SqlCommand(verifyAnswerQuery, cn))
                {
                    verifyAnswerCmd.Parameters.AddWithValue("@Username", username);
                    verifyAnswerCmd.Parameters.AddWithValue("@questionnaire", selectedSecurityQuestion);
                    string storedSecurityAnswer = (string)verifyAnswerCmd.ExecuteScalar();
                    if (securityAnswer != storedSecurityAnswer)
                    {
                        MessageBox.Show("Security answer is incorrect.");
                        return;
                    }
                }

                // Update the password
                string updatePasswordQuery = "UPDATE tblaccounts SET password = @NewPassword WHERE username = @Username";
                using (SqlCommand updatePasswordCmd = new SqlCommand(updatePasswordQuery, cn))
                {
                    updatePasswordCmd.Parameters.AddWithValue("@NewPassword", newPassword);
                    updatePasswordCmd.Parameters.AddWithValue("@Username", username);
                    updatePasswordCmd.ExecuteNonQuery();
                    MessageBox.Show("Password changed successfully.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                cn.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to go back?", stitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
                frmLogIn frm = new frmLogIn();
                frm.Show();
                frm.BringToFront();

            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
  
       
                    





