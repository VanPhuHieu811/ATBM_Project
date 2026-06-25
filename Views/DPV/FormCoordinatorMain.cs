using System;
using System.Drawing;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using ATBM_Project.Data;

namespace ATBM_Project.Views.DPV
{
    public class FormCoordinatorMain : Form
    {
        private Panel pnlSidebar;
        private Panel pnlContent;
        private Label lblUsername;
        private Button btnPatients;
        private Button btnRecords;
        private Button btnServices;
        private Button btnLogout;
        private Form currentChildForm;
        private Button btnThongBao;

        public FormCoordinatorMain()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.FromArgb(41, 53, 65)
            };

            lblUsername = new Label
            {
                AutoSize = false,
                Width = 220,
                Height = 70,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 15),
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.White,
                Text = $"{DBConfig.User?.ToUpper()}\n(Điều phối viên)"
            };

            btnPatients = CreateSidebarButton("Bệnh nhân", 95);
            btnPatients.Click += (s, e) => OpenChildForm(new FormCoordinatorPatients());

            btnRecords = CreateSidebarButton("Hồ sơ bệnh án", 145);
            btnRecords.Click += (s, e) => OpenChildForm(new FormCoordinatorRecords());

            btnServices = CreateSidebarButton("Điều phối KTV", 195);
            btnServices.Click += (s, e) => OpenChildForm(new FormCoordinatorServices());

            btnLogout = CreateSidebarButton("Đăng xuất", 0);
            btnLogout.Dock = DockStyle.Bottom;
            btnLogout.Height = 50;
            btnLogout.BackColor = Color.FromArgb(31, 43, 55);
            btnLogout.Click += BtnLogout_Click;

            btnThongBao = CreateSidebarButton("Thông báo", 245);
            btnThongBao.Click += (s, e) => OpenChildForm(new FormThongBao(DBConfig.ConnectionString));


            pnlSidebar.Controls.Add(lblUsername);
            pnlSidebar.Controls.Add(btnPatients);
            pnlSidebar.Controls.Add(btnRecords);
            pnlSidebar.Controls.Add(btnServices);
            pnlSidebar.Controls.Add(btnLogout);
            pnlSidebar.Controls.Add(btnThongBao);

            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            this.Text = "Phân hệ Điều phối viên - Hệ thống Y tế";
            this.ClientSize = new Size(1100, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlSidebar);
            this.Shown += FormCoordinatorMain_Shown;
            this.ResumeLayout(false);
        }

        private void FormCoordinatorMain_Shown(object sender, EventArgs e)
        {
            if (!HasCoordinatorRole())
            {
                MessageBox.Show(
                    "Không có quyền truy cập. Vui lòng liên hệ admin để cấp role/quyền tương ứng rồi đăng nhập lại.",
                    "Không có quyền",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            OpenChildForm(new FormCoordinatorPatients());
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
            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(childForm);
            childForm.BringToFront();
            childForm.Show();
        }

        private Button CreateSidebarButton(string text, int yPos)
        {
            Button btn = new Button
            {
                Text = "  " + text,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(0, yPos),
                Size = new Size(220, 50),
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.Gainsboro,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(51, 63, 75);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(61, 73, 85);
            return btn;
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool HasCoordinatorRole()
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "SELECT COUNT(*) FROM SESSION_ROLES WHERE ROLE = 'ROLE_DIEUPHOIVIEN'";
                object value = cmd.ExecuteScalar();
                return Convert.ToInt32(value) > 0;
            }
        }
    }
}
