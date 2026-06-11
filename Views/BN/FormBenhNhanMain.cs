using System;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Data;

namespace ATBM_Project.Views.BN
{
    public class FormBenhNhanMain : Form
    {
        private Panel pnlSidebar;
        private Panel pnlContent;
        private Label lblUsername;
        private Button btnLogout;
        private Button btnProfile;
        private Form currentChildForm;

        public FormBenhNhanMain()
        {
            InitializeComponent();
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
            lblUsername.Text = $"{DBConfig.User?.ToUpper()}\n(Bệnh nhân)";

            btnProfile = CreateSidebarButton("Thông tin cá nhân", 90);
            btnProfile.Click += (s, e) => OpenChildForm(new FormBenhNhanProfile());

            btnLogout = CreateSidebarButton("Đăng xuất", 0);
            btnLogout.Dock = DockStyle.Bottom;
            btnLogout.Height = 50;
            btnLogout.BackColor = Color.FromArgb(31, 43, 55);
            btnLogout.Click += BtnLogout_Click;

            pnlSidebar.Controls.Add(lblUsername);
            pnlSidebar.Controls.Add(btnProfile);
            pnlSidebar.Controls.Add(btnLogout);

            // PANEL NỘI DUNG
            pnlContent = new Panel();
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.BackColor = Color.White;

            // SETTING MAIN FORM
            this.Text = "Phân hệ Bệnh nhân - Hệ thống Y tế";
            this.ClientSize = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlSidebar);
            this.ResumeLayout(false);

            // Mặc định mở Form thông tin cá nhân khi đăng nhập
            OpenChildForm(new FormBenhNhanProfile());
        }

        private void OpenChildForm(Form childForm)
        {
            if (currentChildForm != null) currentChildForm.Close();
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