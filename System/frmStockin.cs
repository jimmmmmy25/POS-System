using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient; //
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace System
{
    public partial class frmStockin : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnections dbcon = new DBConnections();
        SqlDataReader dr;
        string stitle = "Simple POS System";    ///////////////////////////         papalitan pa....... /////////////

        public frmStockin()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
        }



        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void frmStockin_Load(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }


        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView2.Columns[e.ColumnIndex].Name;
           if (colName == "colDelete")
            {
                if (MessageBox.Show("Remove this item?", stitle,  MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new SqlCommand("delete from tblStockIn where id = '" + dataGridView2.Rows[e.RowIndex].Cells[1].Value.ToString() + "'", cn);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Item has been successfully removed", stitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadstockIn();
                }
                
            }
        }

        public void LoadstockIn()
        {
            int i = 0;
            dataGridView2.Rows.Clear();
            cn.Open();
            cm = new SqlCommand("SELECT tblStockIn.id, tblStockIn.pcode, tblProduct.pdesc, tblStockIn.qty, tblStockIn.sdate, tblStockIn.stockinby FROM tblStockIn INNER JOIN tblProduct ON tblStockIn.pcode = tblProduct.pcode WHERE tblStockIn.pcode = '" + txtRefNo.Text + "' AND  tblStockIn.status = 'Pending'", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView2.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString());
            }
            dr.Close();
            cn.Close();


        }

        private void txtSearch_Click(object sender, EventArgs e)
        {

        }

        private void txtRefNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        public void Clear()
        {
            txtBy.Clear();
            txtRefNo.Clear();
            dt1.Value = DateTime.Now;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView2.Rows.Count > 0)
                {
                    if (MessageBox.Show("Are you sure you want to save this record?", stitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        cn.Open(); // Open the connection before the loop

                        for (int i = 0; i < dataGridView2.Rows.Count; i++)
                        {
                            int quantityToAdd;
                            if (int.TryParse(dataGridView2.Rows[i].Cells[4].Value?.ToString(), out quantityToAdd))
                            {
                                string pcode = dataGridView2.Rows[i].Cells[2].Value?.ToString();
                                Console.WriteLine("Updating tblproduct and tblstockin for pcode: " + pcode + ", QuantityToAdd: " + quantityToAdd);

                                // Update tblproduct qty
                                string updateProductQuery = "UPDATE tblproduct SET qty = qty + @QuantityToAdd WHERE pcode = @PCode";
                                using (SqlCommand cm = new SqlCommand(updateProductQuery, cn))
                                {
                                    cm.Parameters.AddWithValue("@QuantityToAdd", quantityToAdd);
                                    cm.Parameters.AddWithValue("@PCode", pcode);
                                    cm.ExecuteNonQuery();
                                }

                                // Update tblstockin qty
                                string updateStockInQuery = "UPDATE tblstockin SET qty = qty + @QuantityToAdd, status = 'Done' WHERE pcode = @PCode";
                                using (SqlCommand cm = new SqlCommand(updateStockInQuery, cn))
                                {
                                    cm.Parameters.AddWithValue("@QuantityToAdd", quantityToAdd);
                                    cm.Parameters.AddWithValue("@PCode", pcode);
                                    cm.ExecuteNonQuery();
                                }
                            }
                        }

                        cn.Close(); // Close the connection after the loop

                        Clear();
                        LoadstockIn();
                        MessageBox.Show("Records updated successfully.", stitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmSearchProductStockin frm = new frmSearchProductStockin(this);
            frm.LoadProduct();
            frm.ShowDialog();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void LoadStockInHistory()
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            string query = "SELECT tblStockIn.id, tblStockIn.pcode, tblProduct.pdesc, tblStockIn.qty, tblStockIn.sdate, tblStockIn.stockinby FROM tblStockIn INNER JOIN tblProduct ON tblStockIn.pcode = tblProduct.pcode WHERE CAST(sdate AS DATE) BETWEEN @StartDate AND @EndDate AND status LIKE 'Done'";
            cm = new SqlCommand(query, cn);
            cm.Parameters.AddWithValue("@StartDate", date1.Value.Date);
            cm.Parameters.AddWithValue("@EndDate", date2.Value.Date);

            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                string dateString1 = dr[4].ToString();

                try
                {
                    DateTime date1 = DateTime.Parse(dateString1);

                    dataGridView1.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), date1.ToShortDateString(), dr[5].ToString());
                }
                catch (FormatException ex)
                {
                    // Log or display the error message
                    Console.WriteLine("Error parsing date: " + ex.Message);
                    // Optionally, you can add default values to the DataGridView or handle the error in another way
                }
            }
            dr.Close();
            cn.Close();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadStockInHistory();
        }

        private void date1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
    }
}
