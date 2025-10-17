using System;
using System.Collections;
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
using System.IO;

namespace System
{
    public partial class frmProduct2 : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnections dbcon = new DBConnections();
        SqlDataReader dr;
        public frmProductList flist;
        string stitle = "Simple POS System";
        public frmProduct2(frmProductList frm, string pcode)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            flist = frm;
            txtPcode.Text = pcode;
            OutputComboBox();
            txtPcode.Enabled = false;
            comboBox1.Enabled = false;

        }
        private void OutputComboBox()
        {
            try
            {
                comboBox1.Items.Clear();
                cn.Open();
                cm = new SqlCommand("SELECT category FROM tblproduct WHERE pcode = '" + txtPcode.Text + "'", cn);
                dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    string category = dr["category"].ToString();
                    comboBox1.Items.Add(category);
                }
                dr.Close();
                if (comboBox1.Items.Count > 0)
                {
                    comboBox1.SelectedIndex = 0; // Select the first item
                }

                cn.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                cn.Close();
            }
        }



        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtPcode_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to update this product?", "Update Product", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string sqc = "UPDATE tblProduct SET pdesc=@pdesc, price=@price where pcode like @pcode";
                    cn.Open();
                    cm = new SqlCommand(sqc, cn);
                    cm.Parameters.AddWithValue("@pcode", txtPcode.Text);
                    cm.Parameters.AddWithValue("@pdesc", txtpdesc.Text);
                    cm.Parameters.AddWithValue("@price", txtPrice.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Product has been successfully updated.");
                    frmProduct frm = new frmProduct(flist);
                    frm.Clear();
                    flist.LoadRecords();
                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtPrice.Clear();
            txtPcode.Clear();
            txtpdesc.Clear();
            txtPcode.Focus();
        }

        private void frmProduct2_Load(object sender, EventArgs e)
        {
        
        }
    }
}
