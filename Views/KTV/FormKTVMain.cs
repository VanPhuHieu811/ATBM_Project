using System;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Data;
using ATBM_Project.Views;


namespace ATBM_Project.Views.KTV
{
    public class FormKTVMain : Form
    {
        private Panel pnlSidebar;
        private Panel pnlContent;
        private Label lblUsername;
        private Button btnLogout;
        private Button btnProfile;
        private Button btnViewAssigned;
        private Form currentChildForm;
        private Button btnThongBao;

        public FormKTVMain()
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

            //lblUsername = new Label();
            //lblUsername.AutoSize = false;
            //lblUsername.Width = 220;
            //lblUsername.Height = 60;
            //lblUsername.TextAlign = ContentAlignment.MiddleCenter;
            //lblUsername.Location = new Point(0, 20);
            //lblUsername.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            //lblUsername.ForeColor = Color.LightGreen;
            //lblUsername.Text = $"{DBConfig.User?.ToUpper()}\n(Kỹ thuật viên)";

            lblUsername = new Label();
            lblUsername.AutoSize = false;
            lblUsername.Width = 220;
            lblUsername.Height = 60;
            lblUsername.TextAlign = ContentAlignment.MiddleCenter;
            lblUsername.Location = new Point(0, 20);
            lblUsername.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblUsername.ForeColor = Color.White;
            lblUsername.Text = $"{DBConfig.User?.ToUpper()}\n(Kỹ thuật viên)";

            // CÁC NÚT CHỨC NĂNG
            btnProfile = CreateSidebarButton("Cá nhân", 90);
            btnProfile.Click += (s, e) => OpenChildForm(new NV.FormNhanVienProfile());

            btnViewAssigned = CreateSidebarButton("Dịch vụ điều phối", 140);
            btnViewAssigned.Click += (s, e) => OpenChildForm(new FormKTVServices()); // Mở form danh sách dịch vụ

            btnLogout = CreateSidebarButton("Đăng xuất", 0);
            btnLogout.Dock = DockStyle.Bottom;
            btnLogout.Height = 50;
            btnLogout.BackColor = Color.FromArgb(31, 43, 55);
            btnLogout.Click += BtnLogout_Click;

            btnThongBao = CreateSidebarButton("Thông báo", 190);
            btnThongBao.Click += (s, e) => OpenChildForm(new FormThongBao(DBConfig.ConnectionString));

            // NẠP VÀO SIDEBAR
            pnlSidebar.Controls.Add(lblUsername);
            pnlSidebar.Controls.Add(btnProfile);
            pnlSidebar.Controls.Add(btnViewAssigned);
            pnlSidebar.Controls.Add(btnLogout);
            pnlSidebar.Controls.Add(btnThongBao);

            // PANEL NỘI DUNG (Khung trống bên phải để nhúng form con)
            pnlContent = new Panel();
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.BackColor = Color.White;

            // THIẾT LẬP FORM CHÍNH
            this.Text = "Phân hệ Kỹ thuật viên - Quản lý bệnh viện";
            this.ClientSize = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlSidebar);
            this.ResumeLayout(false);

            // Mặc định mở Form danh sách dịch vụ lên khi vừa đăng nhập
            OpenChildForm(new FormKTVServices());
        }

        // HÀM NHÚNG FORM CON (Giống hệt FormMain của DBA)
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
            pnlContent.Controls.Clear();
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
            this.Close();
        }
    }
}