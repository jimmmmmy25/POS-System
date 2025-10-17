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
    public partial class frmQty : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnections dbcon = new DBConnections();
        SqlDataReader dr;
        private String pcode;
        private double price;
        private String transno;
        string stitle = "Simple POS System";
        frmPOS fpos;
        public frmQty(frmPOS frmpos)
        {
            InitializeComponent();
            fpos = frmpos;
            cn = new SqlConnection(dbcon.MyConnection());
        }

        private void txtQty_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if (txtQty.Text == "0")
            {
                MessageBox.Show("Please enter a quantity greater than 0."); // Inform user about invalid quantity
            }
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Prevent the character from being entered
            }
            // If Enter key is pressed and txtQty is not empty
            else if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrEmpty(txtQty.Text) && int.Parse(txtQty.Text) > 0)
            {
                cn.Open();

                // Fetch the available quantity from tblproduct
                SqlCommand cmd = new SqlCommand("SELECT qty FROM tblproduct WHERE pcode = @pcode", cn);
                cmd.Parameters.AddWithValue("@pcode", pcode);
                int availableQty = (int)cmd.ExecuteScalar();

                // Check if entered quantity is less than available quantity
                if (int.Parse(txtQty.Text) > availableQty)
                {
                    MessageBox.Show("Entered quantity is greater than available quantity."); // Inform user about invalid quantity
                }
                else
                {
                    cm = new SqlCommand("INSERT into tblcart (transno, pcode, price, qty, sdate)VALUES (@transno, @pcode, @price, @qty, @sdate)", cn);
                    cm.Parameters.AddWithValue("@transno", transno);
                    cm.Parameters.AddWithValue("@pcode", pcode);
                    cm.Parameters.AddWithValue("@price", price);
                    cm.Parameters.AddWithValue("@qty", int.Parse(txtQty.Text));
                    cm.Parameters.AddWithValue("@sdate", DateTime.Now);
                    cm.ExecuteNonQuery();
                    fpos.txtSearch.Clear();
                    fpos.txtSearch.Focus();
                    fpos.LoadCart();
                    this.Dispose();
                }

                cn.Close();
            }

        }

        public void ProductDetails(String pcode, double price, String transno)
        {
            this.pcode = pcode;
            this.price = price;
            this.transno = transno;
        }
        private void frmQty_Load(object sender, EventArgs e)
        {

        }
    }
}
