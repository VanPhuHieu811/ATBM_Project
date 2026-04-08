using ATBM_Project.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATBM_Project
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var login = new frmLogin())
            {
                if (login.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new frmUserRole(login.ConnectionString));
                }
            }
            //Application.Run(new Form1());
        }
    }
}
