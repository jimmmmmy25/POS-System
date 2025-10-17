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
namespace System
{
    public partial class frmDiscount2 : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnections dbcon = new DBConnections();
        SqlDataReader dr;
        frmPOS fpos;
        public frmDiscount2(frmPOS frmpos)
        {
            InitializeComponent();
            fpos = frmpos;
            cn = new SqlConnection(dbcon.MyConnection());
        }

        private void txtDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (int.TryParse(txtDiscount.Text, out int discount) && (discount < 0 || discount > 10000))
            {
                MessageBox.Show("Invalid Input");
                this.Dispose();
            }
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Prevent the character from being entered
            }
            else if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrEmpty(txtDiscount.Text))
            {
                try
                {
                   cn.Open(); // Open the connection before the loop

                   // Update tblproduct qty
                   string updatediscount = "UPDATE tblcart SET disc = @disc WHERE transno = @transno";
                   using (SqlCommand cm = new SqlCommand(updatediscount, cn))
                   {
                                        
                     cm.Parameters.AddWithValue("@transno", fpos.lblTransno.Text);
                     cm.Parameters.AddWithValue("@disc", discount);
                     cm.ExecuteNonQuery();
                   }

                   cn.Close(); // Close the connection after the loop
                   fpos.LoadCart();
                    MessageBox.Show("Discount entered: ₱" + discount);


                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
                this.Dispose();
            }
        }
    }
}
