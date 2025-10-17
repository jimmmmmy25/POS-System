using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace System
{
    public partial class UserSetting : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;
        DBConnections dbcon = new DBConnections();
        private string username;
        public UserSetting(string username)
        {
            this.username = username; // Set the label text to the username from Form1
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            txtUserName.Enabled = false;
            txtFirstName.Enabled = false;
            txtLastName.Enabled = false;
            txtEmail.Enabled = false;
            txtContactNo.Enabled = false;
            txtGender.Enabled = false;
            txtBirthDate.Enabled = false;
            SetTextBoxesFromDatabase(username);
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void frmUserSetting_Load(object sender, EventArgs e)
        {

        }
        private void SetTextBoxesFromDatabase(string username)
        {
            
            frmLogIn frm = new frmLogIn();
            try
            {
                cn.Open();
                cm = new SqlCommand("SELECT * FROM tblAccounts WHERE username = @Username", cn);
                cm.Parameters.AddWithValue("@Username", username);
                dr = cm.ExecuteReader();

                if (dr.Read())
                {
                    // Assuming 'txtUserID', 'txtFullName', 'txtEmail' are TextBox controls
                    txtUserName.Text = dr["username"].ToString(); // Replace 'userID' with the actual column name
                    txtFirstName.Text = dr["firstName"].ToString(); // Replace 'FirstName' with the actual column name
                    txtLastName.Text = dr["lastName"].ToString(); // Replace 'LastName' with the actual column name
                    txtEmail.Text = dr["email"].ToString(); // Replace 'email' with the actual column name
                    txtContactNo.Text = dr["contactNo"].ToString();
                    txtGender.Text = dr["gender"].ToString();
                    txtBirthDate.Text = dr["birthdate"].ToString();
                    

                    // Set other TextBoxes as needed
                }
                else
                {
                    MessageBox.Show("User not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                cn.Close();
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void txtContactNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtGender_TextChanged(object sender, EventArgs e)
        {

        }

        private void UserSetting_Load(object sender, EventArgs e)
        {

        }
    }
}
