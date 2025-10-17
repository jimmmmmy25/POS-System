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
using System.Collections;
using System.Reflection;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography;

namespace System
{
    public partial class frmPOS : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;
        DBConnections dbcon = new DBConnections();
        string stitle = "Simple POS System";
        string sdate = DateTime.Now.ToString("yyyyMMdd");
        public decimal Total { get { return decimal.Parse(lblTotal.Text); } }
        public decimal Discount { get { return decimal.Parse(lblDiscount.Text); } }


        // MAY NULL SA TOTAL AMT MEANING HINDI NAGSESETTLE PAYMENT TAPOS NEW TRANSACTION AGAD

        public frmPOS()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            this.KeyPreview = true;
            LoadWidgetsFromDatabase();
            lblDate.Text = DateTime.Now.ToString();
            label16.Text = DateTime.Now.ToLongDateString();

        }
        private void GetTransNo()
        {
            try
            {
                using (var cn = new SqlConnection(dbcon.MyConnection()))
                {
                    cn.Open();
                    string transno = GetTransNoFromDatabase(cn, "tblcart") ?? GetTransNoFromDatabase(cn, "tblsale");

                    if (transno != null)
                    {
                        lblTransno.Text = IncrementTransNo(transno);
                    }
                }

                LoadCart();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private string GetTransNoFromDatabase(SqlConnection cn, string tblsale)
        {
            using (var cm = new SqlCommand($"SELECT TOP 1 transno FROM tblsale ORDER BY transno DESC", cn))
            {
                using (var dr = cm.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return dr["transno"].ToString();
                    }
                }
            }

            return null;
        }

        private string IncrementTransNo(string transno)
        {
            // Split the transno into date and count parts
            string datePart = transno.Substring(0, 8);
            string countPart = transno.Substring(8);

            // Increment the count
            int count = int.Parse(countPart);
            count++;

            // Combine the date part and the incremented count part
            return datePart + count.ToString("D4");  // D4 ensures the count is always 4 digits
        }



        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

            /*try
            {
                if (txtSearch.Text == string.Empty)
                {
                    return;
                }
                else
                {
                    cn.Open();
                    cm = new SqlCommand("select * from tblproduct where pcode like '" + txtSearch.Text + "'", cn);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        frmQty frm = new frmQty(this);
                        frm.ProductDetails(dr["pcode"].ToString(), double.Parse(dr["price"].ToString()), lblTransno.Text);
                        dr.Close();
                        cn.Close();
                        frm.ShowDialog();
                    }
                    else
                    {
                        dr.Close();
                        cn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            */
        }
        public void LoadCart()
        {
            try
            {
                using (var cn = new SqlConnection(dbcon.MyConnection()))
                {
                    cn.Open();
                    LoadDataToGrid(cn);
                }

                UpdateLabels();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoadDataToGrid(SqlConnection cn)
        {
            string query = "SELECT c.id, c.pcode, p.pdesc, c.price, c.qty, c.disc, c.total " +
                           "FROM tblcart AS c " +
                           "INNER JOIN tblProduct AS p ON c.pcode = p.pcode " +
                           "WHERE transno LIKE @transno";

            using (var cm = new SqlCommand(query, cn))
            {
                cm.Parameters.AddWithValue("@transno", lblTransno.Text + "%");

                using (var dr = cm.ExecuteReader())
                {
                    dataGridView1.Rows.Clear();
                    int i = 0;

                    while (dr.Read())
                    {
                        i++;
                        dataGridView1.Rows.Add(i, dr["id"].ToString(), dr["pdesc"].ToString(), dr["price"].ToString(), dr["qty"].ToString(), dr["disc"].ToString(), dr["total"].ToString());
                    }
                }
            }
        }

        private void UpdateLabels()
        {
            VAT();
            showsum();
            showdisc();
            showsubtotal();

            if (lblTotal.Text == "0" && lblDiscount.Text == "0")
            {
                lblchange.Text = "0";
                lblpayment.Text = "0";
            }
        }

        private void showdisc()
        {
            try
            {
                using (var cn = new SqlConnection(dbcon.MyConnection()))
                {
                    cn.Open();
                    object result = new SqlCommand("SELECT SUM(disc) FROM tblCart WHERE transno = @transno", cn)
                    {
                        Parameters = { new SqlParameter("@transno", lblTransno.Text) }
                    }.ExecuteScalar();

                    lblDiscount.Text = result != null ? result.ToString() : "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void showsubtotal()
        {
            try
            {
                using (var cn = new SqlConnection(dbcon.MyConnection()))
                {
                    cn.Open();
                    using (var cm = new SqlCommand("SELECT SUM(total) AS total, SUM(disc) AS disc FROM tblCart WHERE transno = @transno", cn))
                    {
                        cm.Parameters.AddWithValue("@transno", lblTransno.Text);

                        using (var dr = cm.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                decimal total = dr["total"] != DBNull.Value ? Convert.ToDecimal(dr["total"]) : 0;
                                decimal disc = dr["disc"] != DBNull.Value ? Convert.ToDecimal(dr["disc"]) : 0;
                                decimal subtotal = total + disc;

                                lblSubTotalval.Text = subtotal.ToString();
                            }
                            else
                            {
                                lblSubTotalval.Text = "0";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void showsum()
        {
            try
            {
                using (var cn = new SqlConnection(dbcon.MyConnection()))
                {
                    cn.Open();
                    object result = new SqlCommand("SELECT SUM(total) FROM tblCart WHERE transno = @transno", cn)
                    {
                        Parameters = { new SqlParameter("@transno", lblTransno.Text) }
                    }.ExecuteScalar();

                    lblTotal.Text = result != null ? result.ToString() : "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoadWidgetsFromDatabase()
        {
            try
            {
                using (var cn = new SqlConnection(dbcon.MyConnection()))
                {
                    // Open the connection
                    cn.Open();

                    // Query to fetch categories from tblCategory
                    string categoryQuery = "SELECT dpcode, description FROM tblCategory";

                    // Create a SqlCommand for the category query
                    using (SqlCommand categoryCommand = new SqlCommand(categoryQuery, cn))
                    {
                        // Execute the category query and read the results
                        using (SqlDataReader categoryReader = categoryCommand.ExecuteReader())
                        {
                            while (categoryReader.Read())
                            {
                                string dpcode = categoryReader["dpcode"].ToString();
                                string description = categoryReader["description"].ToString();

                                // Create an instance of Widget2 for the category
                                Widget2 categoryWidget = new Widget2();

                                // Set label values for categoryWidget
                                categoryWidget.label.Text = dpcode;
                                categoryWidget.label1.Text = description;

                                // Add click event handler for categoryWidget label
                                categoryWidget.panel1.Click += (sender, e) => CategoryWidget_Click(sender, e, dpcode);
                                categoryWidget.label.Click += (sender, e) => CategoryWidgetLabel_Click(sender, e, dpcode);
                                categoryWidget.label1.Click += (sender, e) => CategoryWidgetLabel_Click(sender, e, dpcode);
                                // Add categoryWidget to your form or a container like a panel
                                flowLayoutPanel1.Controls.Add(categoryWidget); // Add the categoryWidget to your container
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                // Close the connection if it's open
                if (cn.State == ConnectionState.Open)
                    cn.Close();
            }
        }

        private void CategoryWidget_Click(object sender, EventArgs e, string category)
        {
            // Clear existing products
            flowLayoutPanel1.Controls.Clear();

            try
            {
                // Open the connection
                cn.Open();

                // Query to fetch products for the selected category from tblProduct
                string productQuery = "SELECT pcode, price, pdesc FROM tblproduct WHERE category = @category";

                // Create a SqlCommand for the product query
                using (SqlCommand command = new SqlCommand(productQuery, cn))
                {
                    // Add parameter for the selected category
                    command.Parameters.AddWithValue("@category", category);

                    // Execute the product query and read the results
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            label15.Visible = true;
                            string pcode = reader["pcode"].ToString();
                            string price = reader["price"].ToString();
                            string pdesc = reader["pdesc"].ToString();

                            // Create an instance of the user control
                            Widget widget = new Widget();

                            // Set label values from database columns
                            widget.label1.Text = pcode;
                            widget.label2.Text = price;
                            widget.label3.Text = pdesc;

                            widget.panel1.Click += (panelSender, panelEvent) => Widget_Click(panelSender, panelEvent, widget);
                            widget.label1.Click += (panelSender, panelEvent) => Widget_Click(panelSender, panelEvent, widget);
                            widget.label2.Click += (panelSender, panelEvent) => Widget_Click(panelSender, panelEvent, widget);
                            widget.label3.Click += (panelSender, panelEvent) => Widget_Click(panelSender, panelEvent, widget);

                            // Add the user control to your form or a container like a panel
                            flowLayoutPanel1.Controls.Add(widget); // Add the widget to your container
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                // Close the connection if it's open
                if (cn.State == ConnectionState.Open)
                    cn.Close();
            }
        }
        private void CategoryWidgetLabel_Click(object sender, EventArgs e, string category)
        {
            CategoryWidget_Click(sender, e, category);
        }







        private void Widget_Click(object sender, EventArgs e, Widget widget)
        {
            // Handle the click event of the widget
            if (lblTransno.Text != "0")
            {
                cn.Open();
                cm = new SqlCommand("select * from tblproduct where pcode like '" + widget.label1.Text + "'", cn);
                dr = cm.ExecuteReader();
                dr.Read();
                int qty = int.Parse(dr["qty"].ToString());

                // Check if quantity is less than or equal to 0
                if (qty <= 0)
                {
                    MessageBox.Show("Product is out of stock.", stitle, MessageBoxButtons.OK);
                }
                else
                {
                    frmQty frm = new frmQty(this);
                    frm.ProductDetails(dr["pcode"].ToString(), double.Parse(dr["price"].ToString()), lblTransno.Text);
                    frm.ShowDialog();
                }

                dr.Close();
                cn.Close();
            }
            else
            {
                MessageBox.Show("Transaction No is empty. Please click the New Transaction.", stitle, MessageBoxButtons.OK);
            }
        }


        private void btnNew_Click(object sender, EventArgs e)
        {
            txtSearch.Enabled = true;
            GetTransNo();

            try
            {
                cn.Open();
                cm = new SqlCommand("select * from tblcart where transno like '" + lblTransno.Text + "%'", cn);
                dr = cm.ExecuteReader();
                dr.Read();

                if (dr.HasRows)
                {
                    MessageBox.Show("There is a current order in the cart.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                dr.Close();
                cn.Close();
            }

            LoadCart();
        }


        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView1.Columns[e.ColumnIndex].Name;
            if (colName == "colDelete")
            {
                if (MessageBox.Show("Are you sure you want to delete this record?", "Delete Record ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new SqlCommand("delete from tblcart where id like '" + dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString() + "'", cn);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    LoadCart();
                }
            }
        }

        private void frmPOS_Load(object sender, EventArgs e)
        {
            timer1.Start();
            LoadCart();


        }

        private void txtSearch_Click(object sender, EventArgs e)//
        {
            
        }

    

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            LoadWidgetsFromDatabase();
            label15.Visible = false;
        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = DateTime.Now.ToString("hh:mm:ss");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to go back?", stitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {//
                string username = Form1.LoggedInUsername;
                this.Close();      
                Form1 frm = new Form1(username);
                frm.Show();
                frm.BringToFront();

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lblpayment.Text) || lblpayment.Text == "0")
            {
                MessageBox.Show("Payment is not provided", stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var cn = new SqlConnection(dbcon.MyConnection()))
            {
                cn.Open();

                using (var cm = new SqlCommand("select transno from tblcart", cn))
                {
                    using (var dr = cm.ExecuteReader())
                    {
                        if (!dr.HasRows)
                        {
                            MessageBox.Show("Empty cart", stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }

                if (MessageBox.Show("Are you sure you want to settle payment?", stitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                using (var cm = new SqlCommand("INSERT INTO tblSale (id, transno, pcode, price, qty, disc, total, sdate) SELECT c.id, c.transno, c.pcode, c.price, c.qty, c.disc, c.total, c.sdate FROM tblcart AS c INNER JOIN tblProduct AS p ON c.pcode = p.pcode", cn))
                {
                    cm.ExecuteNonQuery();
                }

                using (var cm = new SqlCommand("UPDATE tblSale SET totalamt = (SELECT SUM(total) FROM tblSale WHERE transno = @transno) WHERE transno = @transno;", cn))
                {
                    cm.Parameters.AddWithValue("@transno", lblTransno.Text);
                    cm.ExecuteNonQuery();
                }

                using (var cm = new SqlCommand("UPDATE tblProduct SET qty = qty - (SELECT SUM(qty) FROM tblCart WHERE pcode = tblProduct.pcode) WHERE pcode IN (SELECT pcode FROM tblCart)", cn))
                {
                    cm.ExecuteNonQuery();
                }
                ShowInvoice();

                using (var cm = new SqlCommand("delete from tblCart", cn))
                {
                    cm.ExecuteNonQuery();
                }

                MessageBox.Show("Successfully added!", stitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                GetTransNo();
                lblDiscount.Text = "0";
                lblVATval.Text = "0";
                lblTotal.Text = "0";
                lblpayment.Text = "0";
                lblchange.Text = "0";
            }
            
        }


        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to cancel order?", stitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new SqlCommand("select * from tblcart where transno like '" + lblTransno.Text + "%'", cn);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        dr.Close();
                        cm = new SqlCommand("delete from tblcart where transno like '" + lblTransno.Text + "%'", cn);
                        cm.ExecuteNonQuery();
                        cn.Close();
                    }
                    else
                    {

                        MessageBox.Show("Empty cart", stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    cn.Close();
                }

                LoadCart();
            }
            catch (Exception ex)
            {
                dr.Close();
                cn.Close();
                MessageBox.Show(ex.Message, stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void applydiscount()
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    if (MessageBox.Show("Are you sure you want add the discount/s?", stitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        cn.Open(); // Open the connection before the loop
                        for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        {

                            string discountValue = dataGridView1.Rows[i].Cells[5].Value?.ToString();
                            string price = dataGridView1.Rows[i].Cells[3].Value?.ToString();
                            string quantity = dataGridView1.Rows[i].Cells[4].Value?.ToString();
                            string pdesc = dataGridView1.Rows[i].Cells[2].Value?.ToString();
                            if (decimal.TryParse(discountValue, out decimal discount))
                            {

                                if (decimal.TryParse(price, out decimal price1))
                                {
                                    if (decimal.TryParse(quantity, out decimal qty))
                                    {

                                        decimal price2 = price1 * qty;
                                        if (discount > price2 || discount < 0)
                                        {
                                            MessageBox.Show("Discount must be less than or equal to the total price for \n " + pdesc);
                                            break;
                                        }

                                        string id = dataGridView1.Rows[i].Cells[1].Value?.ToString();

                                        // Update tblcart  peso discount
                                        string updateProductQuery = "UPDATE tblcart SET disc = @discount WHERE id = @id";
                                        using (SqlCommand cm = new SqlCommand(updateProductQuery, cn))
                                        {
                                            cm.Parameters.AddWithValue("@discount", discount);
                                            cm.Parameters.AddWithValue("@id", id);
                                            cm.ExecuteNonQuery();
                                        }

                                    }
                                }
                            }
                        }

                        cn.Close(); // Close the connection after the loop
                        LoadCart();
                        MessageBox.Show("Discount updated successfully.", stitle, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDiscount2_Click(object sender, EventArgs e)
        {
            frmDiscount frm = new frmDiscount(this);
            frm.ShowDialog();
        }

        private void btnPercDiscount_Click(object sender, EventArgs e)
        {

            try
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    if (MessageBox.Show("Are you sure you want add the discount/s?", stitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        cn.Open(); // Open the connection before the loop
                        for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        {

                            string discountValue = dataGridView1.Rows[i].Cells[5].Value?.ToString();
                            string price = dataGridView1.Rows[i].Cells[3].Value?.ToString();
                            string quantity = dataGridView1.Rows[i].Cells[4].Value?.ToString();
                            if (decimal.TryParse(discountValue, out decimal discount))
                            {
                                if (discount < 0 || discount > 100)
                                {
                                    MessageBox.Show("Please enter valid input on " + dataGridView1.Rows[i].Cells[2].Value?.ToString() + ". 0 - 100 only");
                                    break;
                                }

                                if (decimal.TryParse(price, out decimal price1))
                                {
                                    if (decimal.TryParse(quantity, out decimal qty))
                                    {
                                        decimal price2 = price1 * qty;
                                        decimal discountPercentage = discount;
                                        decimal discountAmount = (discountPercentage / 100) * price2;


                                        string id = dataGridView1.Rows[i].Cells[1].Value?.ToString();
                                        Console.WriteLine("Updating discount: " + id + ", Quantity To Add: " + discountAmount + ", Value of Discount: " + discount);

                                        // Update tblcart percentage discount
                                        string updateProductQuery = "UPDATE tblcart SET disc = @discount WHERE id = @id";
                                        using (SqlCommand cm = new SqlCommand(updateProductQuery, cn))
                                        {
                                            cm.Parameters.AddWithValue("@discount", discountAmount);
                                            cm.Parameters.AddWithValue("@id", id);
                                            cm.ExecuteNonQuery();
                                            MessageBox.Show("Discount updated successfully.", stitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }

                                    }
                                }
                            }

                        }
                        cn.Close();
                        LoadCart();

                    }
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show("An error occurred: " + ex.Message, stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }


        private void btnDiscount3_Click(object sender, EventArgs e)
        {
            frmDiscount2 frm = new frmDiscount2(this);
            frm.ShowDialog();
        }

        private void btnVAT_Click(object sender, EventArgs e)
        {
            VAT();
        }
        private void VAT()
        {
            
            try
            {
                cn.Open();

                // Get VAT percentage from label text
                decimal vatPercentage = GetVATPercentage(lblVAT.Text);

                // Execute the SQL command to get the sum of "total"
                object result = new SqlCommand($"SELECT SUM(total) FROM tblCart WHERE transno = '{lblTransno.Text}'", cn).ExecuteScalar();

                // Check if result is not DBNull before converting it to decimal
                decimal total = result != DBNull.Value ? Convert.ToDecimal(result) : 0;

                // Calculate VAT amount and update label
                lblVATval.Text = CalculateVATAmount(total, vatPercentage).ToString("F2");

                cn.Close();
            }
            catch (Exception ex)
            {
                cn.Close();
            }
        }

        private decimal GetVATPercentage(string vatLabelText)
        {
            // Extract VAT percentage from label text
            string vatNumberString = vatLabelText.Split(' ')[1].TrimEnd('%');

            // Try to parse VAT percentage and return it as a decimal
            return decimal.TryParse(vatNumberString, out decimal vat) ? vat / 100 : 0;
        }

        private decimal CalculateVATAmount(decimal total, decimal vatPercentage)
        {
            // Calculate VAT amount
            return total * vatPercentage;
        }


        private void btnDiscount3_Click_1(object sender, EventArgs e)
        {
            frmDiscount2 frm = new frmDiscount2(this);
            frm.ShowDialog();
        }

        private void label14_Click(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            frmPayment frm = new frmPayment(this);
            frm.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ShowInvoice();
        }
        private void ShowInvoice()
        {

            using (var cn = new SqlConnection(dbcon.MyConnection()))
            {
                SqlCommand cm = new SqlCommand("SELECT * FROM tblcart WHERE transno = @TransactionNumber", cn);
                cm.Parameters.AddWithValue("@TransactionNumber", lblTransno.Text);

                try
                {
                    cn.Open();
                    SqlDataReader dr = cm.ExecuteReader();

                    if (dr.HasRows)
                    {
                        StringBuilder invoice = new StringBuilder();
                        invoice.AppendLine(new string('*', 40));
                        invoice.AppendLine("RESTAURANT INVOICE");
                        invoice.AppendLine(new string('*', 40));
                        invoice.AppendLine($"Transaction Number: {lblTransno.Text}");
                        invoice.AppendLine(new string('*', 40));
                        invoice.AppendLine($"{"PCode",-10} {"Price",-10} {"Quantity",-10} {"Discount",-10} {"Total",-10} ");
                        invoice.AppendLine(new string('*', 40));

                        decimal grandTotal = 0;
                        decimal payment = 0;
                        decimal change = 0;
                        while (dr.Read())
                        {
                            string pCode = dr["PCode"].ToString();
                            decimal price = Convert.ToDecimal(dr["Price"]);
                            int quantity = Convert.ToInt32(dr["qty"]);
                            decimal discount = Convert.ToDecimal(dr["disc"]);
                            decimal total = Convert.ToDecimal(dr["Total"]);

                            invoice.AppendLine($"{pCode,-10} {price,-10:C} {quantity,-10} {discount,-10:C} {total,-10:C} ");
                        }

                        invoice.AppendLine(new string('*', 40));
                        invoice.AppendLine($"{"Grand Total:",-30} {lblTotal.Text}");
                        invoice.AppendLine($"{"Payment:",-30} {lblpayment.Text}");
                        invoice.AppendLine($"{"Change:",-30} {lblchange.Text}");
                        invoice.AppendLine(new string('*', 40));
                        MessageBox.Show(invoice.ToString(), "Invoice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Transaction not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    dr.Close(); // Ensure the data reader is closed
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show("An operational error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("A SQL error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



    }
}
    

    




