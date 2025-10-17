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
    public partial class frmProduct : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnections dbcon = new DBConnections();
        SqlDataReader dr;
        public frmProductList flist;
        string stitle = "Product Listing";
        public frmProduct(frmProductList frm)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            flist = frm;
            try
            {
                cn.Open();
                cm = new SqlCommand("SELECT dpcode FROM tblCategory", cn);
                dr = cm.ExecuteReader();
                comboBox1.Items.Clear();
                while (dr.Read())
                {
                    comboBox1.Items.Add(dr["dpcode"].ToString());
                }
                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                cn.Close();
            }

            
        }

        private void frmProduct_Load(object sender, EventArgs e)
        {
            
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            while(txtPcode.Text == "")
            {
                MessageBox.Show("Please choose a category.", stitle, MessageBoxButtons.OK);
                return;
            }
            while (txtpdesc.Text == "")
            {
                MessageBox.Show("Please enter the description.", stitle, MessageBoxButtons.OK);
                return;
            }
            while (txtPrice.Text == "")
            {
                MessageBox.Show("Please enter a price.", stitle, MessageBoxButtons.OK);
                return;
            }
            try
            {
                
                if (MessageBox.Show("Are you sure you want to save this product?" , "Save Product" , MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    
                        cn.Close();
                        cn.Open();
                        cm = new SqlCommand("INSERT INTO tblproduct (pcode, pdesc, price,category) VALUES(@pcode, @pdesc, @price,@category)", cn);
                        cm.Parameters.AddWithValue("@category", comboBox1.SelectedItem);
                        cm.Parameters.AddWithValue("@pcode", txtPcode.Text);
                        cm.Parameters.AddWithValue("@pdesc", txtpdesc.Text);//
                        cm.Parameters.AddWithValue("@price", txtPrice.Text);
                        cm.ExecuteNonQuery();
                        cn.Close();
                        MessageBox.Show("Product has been successfully saved.");
                        Clear();
                        flist.LoadRecords();
                }
            }catch(Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message);
            }
        }

        public void Clear()
        {
            txtPrice.Clear();
            txtPcode.Clear();
            txtPcode.Focus();
            btnSave.Enabled = true;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void txtPrice_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
                if (e.KeyChar == 46){
                //accept .character
            }
                else if (e.KeyChar == 8) { 
                   //accept backspace
            }
                else if ((e.KeyChar < 48) || (e.KeyChar > 57)) // ascii code 48 - 57 between 0 - 9
            {
                e.Handled = true;
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}
