using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    internal class DBConnections
    {
        public string MyConnection()
        {
            string con = @"Data Source=(LocalDB)\LocalDB;Initial Catalog=Test;Integrated Security=True";
            return con;
        }
    }
}
