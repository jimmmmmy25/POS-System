using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace System
{
    public partial class frmDiscount : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnections dbcon = new DBConnections();
        SqlDataReader dr;
        frmPOS fpos;
        private double discount;
        public frmDiscount(frmPOS frmpos)
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
            if (int.TryParse(txtDiscount.Text, out int discount) && (discount < 0 || discount > 10))
            {
                e.Handled = true;
            }
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Prevent the character from being entered
            }
            // If Enter key is pressed and txtQty is not empty
            else if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrEmpty(txtDiscount.Text))
            {

                using (SqlConnection cn = new SqlConnection(dbcon.MyConnection()))
                {
                    cn.Open();

                    List<(int id, decimal price, decimal qty)> cartItems = new List<(int id, decimal price, decimal qty)>();

                    using (SqlCommand selectCmd = new SqlCommand("SELECT id, price, qty FROM tblcart WHERE transno = @transno", cn))
                    {
                        selectCmd.Parameters.AddWithValue("@transno", fpos.lblTransno.Text);

                        using (SqlDataReader reader = selectCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = Convert.ToInt32(reader["id"]);
                                decimal price = Convert.ToDecimal(reader["price"]);
                                decimal qty = Convert.ToDecimal(reader["qty"]);
                                cartItems.Add((id, price, qty));
                            }
                        }
                    }

                    foreach (var item in cartItems)
                    {
                        decimal discountPercentage = decimal.Parse(txtDiscount.Text);
                        decimal discountAmount = (discountPercentage / 100) * (item.price * item.qty);

                        using (SqlCommand updateCmd = new SqlCommand("UPDATE tblcart SET disc = @disc WHERE transno = @transno AND id = @id", cn))
                        {
                            updateCmd.Parameters.AddWithValue("@disc", discountAmount);
                            updateCmd.Parameters.AddWithValue("@transno", fpos.lblTransno.Text);
                            updateCmd.Parameters.AddWithValue("@id", item.id);

                            updateCmd.ExecuteNonQuery();
                        }
                    }
                }


                MessageBox.Show("Discount entered: " + discount + "%");
                fpos.LoadCart();
                this.Dispose();

            }
        }
    }
}
