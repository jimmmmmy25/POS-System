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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;

namespace System
{
    public partial class Dashboard : Form
    {
        SqlCommand cm = new SqlCommand();
        DBConnections dbcon = new DBConnections();
        SqlDataReader dr;

        public Dashboard()
        {
            InitializeComponent();

        }
        //first
        void FillChart()
        {
            string connectionString = "Data Source=(LocalDB)\\LocalDB;Initial Catalog=Test;Integrated Security=True;Encrypt=False;";

            // Create a new SqlConnection object with the provided connection string
            SqlConnection cn = new SqlConnection(connectionString);

            try
            {
                DateTime startDate = dtpStartDate.Value.Date;
                DateTime endDate = dtpEndDate.Value.Date;
                cn.Open();
                cm.Connection = cn;
                cm.CommandText = "SELECT SUM(totalamt) as totalamt, sdate FROM tblSale WHERE CAST(sdate AS DATE) >= @StartDate AND CAST(sdate AS DATE) <= @EndDate GROUP BY sdate";
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@StartDate", startDate);
                cm.Parameters.AddWithValue("@EndDate", endDate);
                dr = cm.ExecuteReader();

                chart1.Series["GrossRev"].Points.Clear(); // Clear previous data points
                decimal total = 0;
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        // Process data as needed, e.g., display in chart
                        decimal profit = dr.GetDecimal(dr.GetOrdinal("totalamt"));
                        DateTime date = dr.GetDateTime(dr.GetOrdinal("sdate"));

                        // Add the data point to the chart series
                        chart1.Series["GrossRev"].Points.AddXY(date, profit);
                        total += profit;
                    }
                    chart1.Titles.Clear(); // Clear previous titles if any
                }
                else
                {
                    Console.WriteLine("No data found.");
                }
                chart1.Refresh(); // Refresh the chart to update the display
                Console.WriteLine("Total of totalamt: " + total);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                if (dr != null)
                    dr.Close();

                if (cn != null && cn.State == System.Data.ConnectionState.Open)
                    cn.Close();
            }
        }


        private void LoadData()
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            FillChart();
            FillChart2();
            FillDataGridView();
        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }
        void FillChart2()
        {
            string connectionString = "Data Source=(LocalDB)\\LocalDB;Initial Catalog=Test;Integrated Security=True;Encrypt=False;";

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand cm = new SqlCommand();
                SqlDataReader dr = null;

                try
                {
                    cn.Open();
                    cm.Connection = cn;
                    cm.CommandText = "SELECT TOP 5 pcode, SUM(qty) AS 'Total Quantity' FROM tblSale GROUP BY pcode ORDER BY MAX(sdate) DESC";
                    dr = cm.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            string pcode = dr.GetString(dr.GetOrdinal("pcode"));
                            int total = dr.GetInt32(dr.GetOrdinal("Total Quantity"));

                            chart2.Series["TopProducts"].Points.AddXY(pcode, total);
                        }

                        chart2.Titles.Add("Top 5 Products by Total Sales");
                    }
                    else
                    {
                        Console.WriteLine("No data found.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
                finally
                {
                    if (dr != null)
                        dr.Close();
                }
            }
        }

        // Method to fill DataGridView with data
        private void FillDataGridView()
        {
            string connectionString = "Data Source=(LocalDB)\\LocalDB;Initial Catalog=Test;Integrated Security=True;Encrypt=False;";
            string query = "SELECT * from tblproduct WHERE qty <= 5 ORDER BY qty ASC"; // Replace YourTableName with your actual table name

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    try
                    {
                        cn.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        dgvUnderstock.DataSource = dataTable; // Set data source to dgvUnderstock
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }
        }
        private void dgvUnderstock_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is not the header row
            if (e.RowIndex >= 0)
            {
                // Get the value of the clicked cell
                DataGridViewCell cell = dgvUnderstock.Rows[e.RowIndex].Cells[e.ColumnIndex];
                string cellValue = cell.Value.ToString();

                // Display the cell value in a message box
                MessageBox.Show("Clicked cell value: " + cellValue);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void DisableCustomDates()
        {
            dtpStartDate.Enabled = true;
            dtpEndDate.Enabled = true;
            btnCustomDate.Visible = true;
        }
        //Events Methods
        private void btnToday_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today;
            dtpEndDate.Value = DateTime.Now;
            FillChart();
            DisableCustomDates();
        }

        private void btnLast7Days_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today.AddDays(-7);
            dtpEndDate.Value = DateTime.Now;
            FillChart();
            DisableCustomDates();
        }

        private void btnLast30Days_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today.AddDays(-30);
            dtpEndDate.Value = DateTime.Now;
            FillChart();
            DisableCustomDates();
        }

        private void btnThisMonth_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpEndDate.Value = DateTime.Now;
            FillChart();
            DisableCustomDates();
        }

        private void btnCustomDate_Click(object sender, EventArgs e)
        {
            dtpStartDate.Enabled = true;
            dtpEndDate.Enabled = true;
            btnCustomDate.Visible = true;
        }

        private void btnOkCustomDate_Click(object sender, EventArgs e)
        {
            FillChart();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
        ///
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Connection string for MySQL
            string connectionString = "Data Source=(LocalDB)\\LocalDB;Initial Catalog=Test;Integrated Security=True;Encrypt=False;";

            // Query to count distinct values of transno
            string query = "SELECT COUNT(DISTINCT transno) AS TotalDistinctTransNo FROM tblSale";

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand cm = new SqlCommand(query, cn);
                SqlDataReader dr = null;

                try
                {
                    cn.Open();
                    object result = cm.ExecuteScalar();

                    if (result != null)
                    {
                        int totalDistinctTransNo = Convert.ToInt32(result);
                        Console.WriteLine("Total distinct transno: " + totalDistinctTransNo);
                        lblNumOrders.Text = totalDistinctTransNo.ToString();
                    }
                    else
                    {
                        Console.WriteLine("No data found.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            // Connection string for MySQL
            string connectionString = "Data Source=(LocalDB)\\LocalDB;Initial Catalog=Test;Integrated Security=True;Encrypt=False;";

            // Query to count distinct values of transno
            string queryDistinctTransNo = "SELECT COUNT(DISTINCT transno) AS TotalDistinctTransNo FROM tblSale";

            // Query to calculate total revenue
            string queryTotalRevenue = "SELECT SUM(totalamt) AS TotalRevenue FROM tblSale";

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand cmdDistinctTransNo = new SqlCommand(queryDistinctTransNo, cn);
                SqlCommand cmdTotalRevenue = new SqlCommand(queryTotalRevenue, cn);
                SqlDataReader dr = null;

                try
                {
                    cn.Open();
                    object resultDistinctTransNo = cmdDistinctTransNo.ExecuteScalar();
                    object resultTotalRevenue = cmdTotalRevenue.ExecuteScalar();

                    if (resultDistinctTransNo != null)
                    {
                        int totalDistinctTransNo = Convert.ToInt32(resultDistinctTransNo);
                        lblNumOrders.Text = totalDistinctTransNo.ToString();
                    }
                    else
                    {
                        Console.WriteLine("No data found for distinct transno count.");
                    }

                    if (resultTotalRevenue != null)
                    {
                        decimal totalRevenue = Convert.ToDecimal(resultTotalRevenue);
                        lblTotalRevenue.Text = totalRevenue.ToString("C"); // Format as currency
                    }
                    else
                    {
                        Console.WriteLine("No data found for total revenue.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }
    }
        }
    

        

      