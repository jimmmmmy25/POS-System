using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace System
{
    public partial class FrmRegister : Form
    {
        public FrmRegister()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
        }
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;
        DBConnections dbcon = new DBConnections();
        string stitle = "Simple POS System";

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Check if any of the fields are umpty
            if (string.IsNullOrWhiteSpace(txtUserName.Text) ||

                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtConfirmPass.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtContactNo.Text) ||
                cmbGender.SelectedItem == null ||
                cmbQuestionnaire.SelectedItem == null ||  // Add this line for the Questionnaire
                string.IsNullOrWhiteSpace(txtAnswer.Text)) // Add this line for the Answer
            {
                MessageBox.Show("Please fill out all fields.");
                return;
            }

                if (txtPassword.Text != txtConfirmPass.Text)
             {
                 MessageBox.Show("Passwords do not match.");
                 return;
             }

           

            // Check if contact number length is 11 digits
            if (txtContactNo.Text.Length != 11)
            {
                MessageBox.Show("Contact number should be 11 digits long.");
                return;
            }

            // Check if email contains "@" and ".com"
            if (!txtEmail.Text.Contains("@") || !txtEmail.Text.EndsWith(".com"))
            {
                MessageBox.Show("Invalid email format. Email should contain @ and end with .com");
                return;
            }

            try
             {
                 // Connection string to your SQL Server
                 {
                     cn.Open();
                    // Check for username if it is already exist
                    string checkUsernameQuery = "SELECT COUNT(*) FROM tblaccounts WHERE username = @Username";
                    using (SqlCommand checkUsernameCmd = new SqlCommand(checkUsernameQuery, cn))
                    {
                        checkUsernameCmd.Parameters.AddWithValue("@Username", txtUserName.Text);
                        int count = (int)checkUsernameCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("Username already exists. Please choose a different username.");
                            return;
                        }
                    }

                        string query = "INSERT INTO tblaccounts (username, password, firstName, lastName, Email, ContactNo, Birthdate, Gender,questionnaire,answer) " +
                                    "VALUES (@Username, @Password, @FirstName, @LastName, @Email, @ContactNo, @Birthdate, @Gender, @questionnaire, @answer)";

                     using (SqlCommand cm = new SqlCommand(query, cn))
                     {
                         cm.Parameters.AddWithValue("@Username", txtUserName.Text);
                         cm.Parameters.AddWithValue("@Password", txtPassword.Text);
                         cm.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                         cm.Parameters.AddWithValue("@LastName", txtLastName.Text);
                         cm.Parameters.AddWithValue("@Email", txtEmail.Text);
                         cm.Parameters.AddWithValue("@ContactNo", txtContactNo.Text);
                         cm.Parameters.AddWithValue("@Birthdate", dtpBirthDate.Value);
                         cm.Parameters.AddWithValue("@Gender", cmbGender.SelectedItem.ToString());
                         cm.Parameters.AddWithValue("@Questionnaire", cmbQuestionnaire.SelectedItem.ToString());
                         cm.Parameters.AddWithValue("answer", txtAnswer.Text);

                         cm.ExecuteNonQuery();
                         MessageBox.Show("Register Successful");

                        this.Hide();
                        frmLogIn frm = new frmLogIn();
                        frm.Show();
                        frm.BringToFront();
                    }
                 }
             }

             catch (Exception ex)
             {
                 MessageBox.Show("General Error:" + ex.Message);
             }
         }

            private void FrmRegister_Load(object sender, EventArgs e)
            {

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

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtEmail_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the pressed key is a space
            if (e.KeyChar == ' ')
            {
                e.Handled = true; // Ignore the space
            }
        }

        private void txtContactNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtContactNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the pressed key is a space
            if (e.KeyChar == ' ')
            {
                e.Handled = true; // Ignore the space
            }
        }

        private void txtUserName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the pressed key is a space
            if (e.KeyChar == ' ')
            {
                e.Handled = true; // Ignore the space
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the pressed key is a space
            if (e.KeyChar == ' ')
            {
                e.Handled = true; // Ignore the space
            }
        }

        private void txtConfirmPass_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the pressed key is a space
            if (e.KeyChar == ' ')
            {
                e.Handled = true; // Ignore the space
            }
        }
    }
    }

