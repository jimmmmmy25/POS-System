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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace System
{
    public partial class frmLogIn : Form
    {
        public frmLogIn()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            txtPass.PasswordChar = '*'; // Hide characters with asterisks
            txtUser.KeyPress += new KeyPressEventHandler(txtUser_KeyPress);
            txtPass.KeyPress += new KeyPressEventHandler(txtPass_KeyPress);


        }
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;
        DBConnections dbcon = new DBConnections();
        private void frmLogIn_Load(object sender, EventArgs e)
        {

        }
        private void txtUser_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the pressed key is a space
            if (e.KeyChar == ' ')
            {
                e.Handled = true; // Ignore the space
            }
        }

        private void txtPass_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the pressed key is a space
            if (e.KeyChar == ' ')
            {
                e.Handled = true; // Ignore the space
            }
        }
        public void btnLogIn_Click(object sender, EventArgs e)
        {
            string Username = txtUser.Text;
            string Password = txtPass.Text;

          
            try
            {
                cn.Open();

                // case-insensitive by default, but you can enforce case sensitivity
                cm = new SqlCommand("SELECT * FROM tblAccounts WHERE username = @Username COLLATE Latin1_General_CS_AS AND password = @Password COLLATE Latin1_General_CS_AS", cn);
                cm.Parameters.AddWithValue("@Username", Username);
                cm.Parameters.AddWithValue("@Password", Password);
                dr = cm.ExecuteReader();
                if (dr.HasRows)
                {

                    Username = txtUser.Text;
                    Password = txtPass.Text;
                     // Assuming this is an instance of frmLogIn
                    // proceed to next form
                    Form1 frm = new Form1(Username);
                    frm.Show();
                    this.Hide();
                }
                else
                {
                    // If login fails, display error message
                    MessageBox.Show("Invalid login details", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtUser.Clear();
                    txtPass.Clear();
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Close the connection
                cn.Close();


            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtPass.Clear();
            txtUser.Clear();
            txtUser.Focus();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult res;
            res = MessageBox.Show("Do you want to exit", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                Application.Exit();
            }
            else
            {
                this.Show();
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {

            FrmRegister frm = new FrmRegister();
            frm.Show();
            frm.BringToFront();

            this.Hide();
        }

        private void btnForgot_Click(object sender, EventArgs e)
        {

            frmForgotPass frm = new frmForgotPass();
            frm.Show();
            frm.BringToFront();

            this.Hide();
        }

        private void chkShow_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShow.Checked)
            {
                txtPass.PasswordChar = '\0'; // Display characters as plain text
            }
            else
            {
                txtPass.PasswordChar = '*'; // Hide characters with asterisks
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}


