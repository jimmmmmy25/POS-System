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
    public partial class frmPayment : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnections dbcon = new DBConnections();
        SqlDataReader dr;
        frmPOS fpos;
        public frmPayment(frmPOS frmpos)
        {
            InitializeComponent();
            fpos = frmpos;
            cn = new SqlConnection(dbcon.MyConnection());
        }

        private void txtPayment_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Prevent non-digit characters from being entered
                return;
            }

            if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrEmpty(txtPayment.Text))
            {
                if (decimal.TryParse(fpos.lblTotal.Text, out decimal total) &&
                    decimal.TryParse(txtPayment.Text, out decimal payment))
                {
                    if (payment < total)
                    {
                        MessageBox.Show("Invalid Input: Payment is less than total.");
                        this.Dispose();
                        return;
                    }

                    try
                    {

                        MessageBox.Show("Payment entered: ₱" + payment);
                        fpos.lblpayment.Text = string.Format("{0:0.00}", payment);
                        decimal change = payment - fpos.Total; // Consider discount
                        fpos.lblchange.Text = change.ToString();
                        // Refresh or reload the cart
                        fpos.LoadCart();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }

                    // Optionally dispose the current form
                    this.Dispose();
                }
                else
                {
                    MessageBox.Show("Invalid Input: Ensure both total and payment are valid numbers.");
                }
            }
        }
    }
}
