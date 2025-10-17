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
using System.Data.Common;
using System.Data.Odbc;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
namespace System
{

    public partial class Form1 : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnections dbcon = new DBConnections();
        string stitle = "Simple POS System";
                    
        public static string LoggedInUsername { get; private set; }
        public Form1(string username)
        {
            InitializeComponent();
            LoggedInUsername = username;
            lblUsername.Text = username;
            

        }
        //
        private void ClosePreviousForm<T>() where T : Form
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm is T)
                {
                    frm.Close();
                    break;
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }


        private void btnBrand_Click(object sender, EventArgs e)
        {

            // Close any open forms of type frmBrand
            ClosePreviousForm<frmBrand>();

            frmBrand frm = new frmBrand();
            frm.TopLevel = false;
            panel4.Controls.Add(frm);
            frm.BringToFront();
            frm.Show();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Dashboard frm = new Dashboard();
            frm.TopLevel = false;
            panel4.Controls.Add(frm);
            frm.BringToFront();
            frm.Show();
        }

        private void btnCategory_Click(object sender, EventArgs e)
        {
            {
                // Collect all forms to close except Form1
                List<Form> formsToClose = new List<Form>();

                foreach (Form frm in Application.OpenForms)
                {
                    if (frm != this && !(frm is frmCategoryList)) // Exclude Form1
                    {
                        formsToClose.Add(frm);
                    }
                }

                // Close the collected forms
                foreach (Form frm in formsToClose)
                {
                    frm.Hide();
                }

                // Create and show the new form
                frmCategoryList frm1 = new frmCategoryList();
                frm1.TopLevel = false;
                panel4.Controls.Add(frm1);
                frm1.Show();
                frm1.BringToFront();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            {
                // Collect all forms to close except Form1
                List<Form> formsToClose = new List<Form>();

                foreach (Form frm in Application.OpenForms)
                {
                    if (frm != this && !(frm is frmProductList)) // Exclude Form1
                    {
                        formsToClose.Add(frm);
                    }
                }

                // Close the collected forms
                foreach (Form frm in formsToClose)
                {
                    frm.Hide();
                }

                // Create and show the new form
                frmProductList frm1 = new frmProductList();
                frm1.TopLevel = false;
                panel4.Controls.Add(frm1);
                frm1.Show();
                frm1.BringToFront();
            }
        }

        public void btnStockIn_Click(object sender, EventArgs e)
        {
            {
                // Collect all forms to close except Form1
                List<Form> formsToClose = new List<Form>();

                foreach (Form frm in Application.OpenForms)
                {
                    if (frm != this && !(frm is frmStockin)) // Exclude Form1
                    {
                        formsToClose.Add(frm);
                    }
                }

                // Close the collected forms
                foreach (Form frm in formsToClose)
                {
                    frm.Hide();
                }

                // Create and show the new form
                frmStockin frm1 = new frmStockin();
                frm1.TopLevel = false;
                panel4.Controls.Add(frm1);
                frm1.Show();
                frm1.BringToFront();
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnPOS_Click(object sender, EventArgs e)
        {


            {
                // Collect all forms to close except Form1
                List<Form> formsToClose = new List<Form>();

                foreach (Form frm in Application.OpenForms)
                {
                    if (!(frm is frmPOS)) // Exclude Form1
                    {
                        formsToClose.Add(frm);
                    }
                }

                // Close the collected forms
                foreach (Form frm in formsToClose)
                {
                    frm.Hide();
                }

                // Create and show the new form
                frmPOS frm1 = new frmPOS();
                frm1.Show();
            }



        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<Form> formsToClose = new List<Form>();

            foreach (Form frm in Application.OpenForms)
            {
                if (frm != this && !(frm is frmRecord)) // Exclude Form1
                {
                    formsToClose.Add(frm);
                }
            }

            // Close the collected forms
            foreach (Form frm in formsToClose)
            {
                frm.Hide();
            }
            // Create and show the new form
            frmRecord frm1 = new frmRecord();
            frm1.TopLevel = false;
            panel4.Controls.Add(frm1);
            frm1.Show();
            frm1.BringToFront();

        }

        public void button1_Click(object sender, EventArgs e)
        {
            // Collect all forms to close except Form1
            List<Form> formsToClose = new List<Form>();

            foreach (Form frm in Application.OpenForms)
            {
                if (frm != this && !(frm is frmCategoryList)) // Exclude Form1
                {
                    formsToClose.Add(frm);
                }
            }

            // Close the collected forms
            foreach (Form frm in formsToClose)
            {
                frm.Hide();
            }

            // Create and show the new form
            frmCategoryList frm1 = new frmCategoryList();
            frm1.TopLevel = false;
            panel4.Controls.Add(frm1);
            frm1.Show();
            frm1.BringToFront();
        }


        public void button3_Click(object sender, EventArgs e)
        {
            // Collect all forms to close except Form1
            List<Form> formsToClose = new List<Form>();

            foreach (Form frm in Application.OpenForms)
            {
                if (frm != this && !(frm is Dashboard)) // Exclude Form1
                {
                    formsToClose.Add(frm);
                }
            }

            // Close the collected forms
            foreach (Form frm in formsToClose)
            {
                frm.Hide();
            }

            // Create and show the new form
            Dashboard frm1 = new Dashboard();
            frm1.TopLevel = false;
            panel4.Controls.Add(frm1);
            frm1.Show();
            frm1.BringToFront();
        }

        private void button9_Click(object sender, EventArgs e)
        {


            if (MessageBox.Show("Are you sure you want to go back?", stitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
                frmLogIn frm = new frmLogIn();
                frm.Show();
                frm.BringToFront();

            }

        }

        private void label1_Click_1(object sender, EventArgs e)
        {
           


        }

        private void button7_Click(object sender, EventArgs e)
        {
                
                
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            // Collect all forms to close except Form1
            List<Form> formsToClose = new List<Form>();

            foreach (Form frm in Application.OpenForms)
            {
                if (frm != this && !(frm is UserSetting)) // Exclude Form1
                {
                    formsToClose.Add(frm);
                }
            }

            // Close the collected forms
            foreach (Form frm in formsToClose)
            {
                frm.Hide();
            }
            string username = lblUsername.Text;
            // Create and show the new form
            UserSetting frm1 = new UserSetting(username);
            frm1.TopLevel = false;
            panel4.Controls.Add(frm1);
            frm1.Show();
            frm1.BringToFront();
        }
    }
}





        


    

