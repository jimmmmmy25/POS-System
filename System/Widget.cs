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

namespace System
{
    public partial class Widget : UserControl
    {
        public Widget()
        {
            InitializeComponent();
            label1.Click += Label_Click;
            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_MouseClick(object sender, EventArgs e)
        {

        }

        private void Label_Click(object sender, EventArgs e)
    {
        }
    }
}
