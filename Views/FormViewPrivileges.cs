using System;
using System.Drawing;
using System.Windows.Forms;

namespace ATBM_Project.Views
{
    public class FormViewPrivileges : Form
    {
        public FormViewPrivileges()
        {
            this.Text = "FormViewPrivileges";
            this.BackColor = Color.White;
            Label lbl = new Label() { Text = "View Privileges", Location = new Point(20, 20), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            this.Controls.Add(lbl);
        }
    }
}