using System;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Data;

namespace ATBM_Project.Views
{
    public class FormMain : Form
    {
        private Panel pnlSidebar;
        private Label lblUsername;
        private Button btnUsersSide, btnRolesSide, btnGrantPrivs, btnGrantRoles, btnRevoke, btnViewPrivs;
        private Button btnLogout;
        
        private Panel pnlContent;
        private Form currentChildForm;

        public FormMain()
        {
            InitializeComponent();
            lblUsername.Text = DBConfig.User?.ToUpper();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // SIDEBAR
            pnlSidebar = new Panel();
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Width = 220;
            pnlSidebar.BackColor = Color.FromArgb(41, 53, 65);

            lblUsername = new Label();
            lblUsername.AutoSize = false;
            lblUsername.Width = 220;
            lblUsername.Height = 60;
            lblUsername.TextAlign = ContentAlignment.MiddleCenter;
            lblUsername.Location = new Point(0, 20);
            lblUsername.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblUsername.ForeColor = Color.White;

            btnUsersSide = CreateSidebarButton("User", 90);
            btnUsersSide.Click += (s, e) => OpenChildForm(new FormUser());
            btnRolesSide = CreateSidebarButton("Role", 140);
            btnRolesSide.Click += (s, e) => OpenChildForm(new FormRole());
            btnGrantPrivs = CreateSidebarButton("Grant Privileges", 190);
            btnGrantPrivs.Click += (s, e) => OpenChildForm(new FormGrantPrivileges());
            btnGrantRoles = CreateSidebarButton("Grant Roles", 240);
            btnGrantRoles.Click += (s, e) => OpenChildForm(new FormGrantRoles());
            btnRevoke = CreateSidebarButton("Revoke", 290);
            btnRevoke.Click += (s, e) => OpenChildForm(new FormRevoke());

            btnViewPrivs = CreateSidebarButton("View Privileges", 340);
            btnViewPrivs.Click += (s, e) => OpenChildForm(new FormViewPrivileges());

            btnLogout = CreateSidebarButton("Logout", 0);
            btnLogout.Dock = DockStyle.Bottom;
            btnLogout.Height = 50;
            btnLogout.BackColor = Color.FromArgb(31, 43, 55);
            btnLogout.Click += BtnLogout_Click;

            pnlSidebar.Controls.Add(btnLogout);
            pnlSidebar.Controls.Add(lblUsername);
            pnlSidebar.Controls.Add(btnUsersSide);
            pnlSidebar.Controls.Add(btnRolesSide);
            pnlSidebar.Controls.Add(btnGrantPrivs);
            pnlSidebar.Controls.Add(btnGrantRoles);
            pnlSidebar.Controls.Add(btnRevoke);
            pnlSidebar.Controls.Add(btnViewPrivs);

            // CONTENT
            pnlContent = new Panel();
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.BackColor = Color.White;

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlSidebar);

            // MAIN FORM
            this.Text = "Admin Main";
            this.ClientSize = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

            OpenChildForm(new FormUser());
        }

        private void OpenChildForm(Form childForm)
        {
            if (currentChildForm != null)
            {
                currentChildForm.Close();
            }
            currentChildForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(childForm);
            pnlContent.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        private Button CreateSidebarButton(string text, int yPos)
        {
            Button btn = new Button();
            btn.Text = "  " + text;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(51, 63, 75);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(61, 73, 85);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Location = new Point(0, yPos);
            btn.Size = new Size(220, 50);
            btn.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            btn.ForeColor = Color.Gainsboro;
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormLogin login = new FormLogin();
            login.ShowDialog();
            this.Close();
        }
    }
}