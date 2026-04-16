using System;
using System.Drawing;
using System.Windows.Forms;

namespace ATBM_Project.Views
{
    public class FormGrantRoles : Form
    {
        public FormGrantRoles()
        {
            this.Text = "FormGrantRoles";
            this.BackColor = Color.White;
            Label lbl = new Label() { Text = "Grant Roles View", Location = new Point(20, 20), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            this.Controls.Add(lbl);
        }
    }
}