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
    public partial class frmSearchProductStockin : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnections dbcon = new DBConnections();
        SqlDataReader dr;
        string stitle = "Simple POS System";
        frmStockin slist = new frmStockin();
        public frmSearchProductStockin(frmStockin flist)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            slist = flist;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
            string colName = dataGridView1.Columns[e.ColumnIndex].Name;
            if (colName == "Select")
            {
                if (slist.txtRefNo.Text == string.Empty)
                {
                    MessageBox.Show("Please enter Product Code.", stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    slist.txtRefNo.Focus();
                    return;
                }
                if (slist.txtBy.Text == string.Empty)
                {
                    MessageBox.Show("Please enter stock in by.", stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    slist.txtBy.Focus();
                    return;
                }
                if (MessageBox.Show("Add this item?", stitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new SqlCommand("insert into tblstockin(pcode, sdate, stockinby)values(@pcode, @sdate, @stockinby)", cn);
                    cm.Parameters.AddWithValue("@pcode", dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                    cm.Parameters.AddWithValue("@sdate", slist.dt1.Value);
                    cm.Parameters.AddWithValue("@stockinby", slist.txtBy.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();

                    MessageBox.Show("Successfully added!", stitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    slist.LoadstockIn();
                }


            }
        }

       public void LoadProduct()
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new SqlCommand("Select pcode, pdesc, qty from tblproduct where pdesc like '%" + txtSearch.Text + "%' order by pdesc ", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void txtSearch_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text != string.Empty)
            {
                cn.Open();
                cm = new SqlCommand("select * from tblproduct where pcode LIKE '" + txtSearch.Text.ToString() + "%'", cn);
                cm.ExecuteNonQuery();
                int i = 0;
                dataGridView1.Rows.Clear();
                dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    i++;
                    dataGridView1.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString());
                }
                dr.Close();
                cn.Close();
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
