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
        private Button btnUsersSide, btnRolesSide, btnGrantPrivs, btnGrantRoles, btnRevoke;
        private Button btnLogout;
        
        private Panel pnlContent;
        private Form currentChildForm;

        public FormMain()
        {
            InitializeComponent();
            lblUsername.Text = DBConfig.user?.ToUpper();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // SIDEBAR
            pnlSidebar = new Panel();
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Width = 200;
            pnlSidebar.BackColor = Color.LightGray;

            lblUsername = new Label();
            lblUsername.Text = "ADMIN_DBSEC02";
            lblUsername.AutoSize = false;
            lblUsername.Width = 200;
            lblUsername.TextAlign = ContentAlignment.MiddleCenter;
            lblUsername.Location = new Point(0, 30);

            btnUsersSide = CreateSidebarButton("User", 100);
            btnUsersSide.Click += (s, e) => OpenChildForm(new FormUser());
            btnRolesSide = CreateSidebarButton("Role", 140);
            btnRolesSide.Click += (s, e) => OpenChildForm(new FormRole());
            btnGrantPrivs = CreateSidebarButton("Grant Privileges", 180);
            btnGrantPrivs.Click += (s, e) => OpenChildForm(new FormGrantPrivileges());
            btnGrantRoles = CreateSidebarButton("Grant Roles", 220);
            btnGrantRoles.Click += (s, e) => OpenChildForm(new FormGrantRoles());
            btnRevoke = CreateSidebarButton("Revoke", 260);
            btnRevoke.Click += (s, e) => OpenChildForm(new FormRevoke());

            btnLogout = CreateSidebarButton("Logout", 0);
            btnLogout.Dock = DockStyle.Bottom;
            btnLogout.Click += BtnLogout_Click;

            pnlSidebar.Controls.Add(btnLogout);
            pnlSidebar.Controls.Add(lblUsername);
            pnlSidebar.Controls.Add(btnUsersSide);
            pnlSidebar.Controls.Add(btnRolesSide);
            pnlSidebar.Controls.Add(btnGrantPrivs);
            pnlSidebar.Controls.Add(btnGrantRoles);
            pnlSidebar.Controls.Add(btnRevoke);

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
            btn.Text = text;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Location = new Point(10, yPos);
            btn.Size = new Size(180, 40);
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        private Button CreateActionButton(string text, int x, int y)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new Point(x, y);
            btn.Size = new Size(80, 30);
            return btn;
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            // Nếu bạn muốn làm lại Login cho dễ quản lý thì dùng Application.Restart() hoặc mở lại FormLogin
            FormLogin login = new FormLogin();
            login.ShowDialog();
            this.Close();
        }
    }
}