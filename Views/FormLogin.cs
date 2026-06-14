using System;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Data;

namespace ATBM_Project.Views
{
    public class FormLogin : Form
    {
        private Label lblHost, lblPort, lblSid, lblUser, lblPass;
        private TextBox txtHost, txtPort, txtSid, txtUser, txtPass;
        private Button btnLogin;

        public FormLogin()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Đăng nhập hệ thống bệnh viện";
            this.ClientSize = new Size(450, 455);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.WhiteSmoke;

            Font headerFont = new Font("Segoe UI", 16F, FontStyle.Bold);
            Font labelFont = new Font("Segoe UI", 10F, FontStyle.Regular);
            Font textFont = new Font("Segoe UI", 10F, FontStyle.Regular);

            Label lblTitle = new Label() { Text = "Đăng nhập hệ thống bệnh viện", Location = new Point(0, 25), AutoSize = false, Width = 450, Height = 42, TextAlign = ContentAlignment.MiddleCenter, Font = headerFont, ForeColor = Color.SteelBlue };
            Label lblDemo = new Label() { Text = "Chế độ demo giao diện - chưa kết nối Oracle", Location = new Point(0, 67), AutoSize = false, Width = 450, Height = 24, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9F, FontStyle.Italic), ForeColor = Color.DimGray };

            int startY = 115;
            int gapY = 45;
            int lx = 80;
            int tx = 180;
            int tw = 180;

            this.lblHost = new Label() { Text = "Host:", Location = new Point(lx, startY), AutoSize = true, Font = labelFont };
            this.txtHost = new TextBox() { Text = "localhost", Location = new Point(tx, startY - 3), Width = tw, Font = textFont, ReadOnly = true };

            this.lblPort = new Label() { Text = "Port:", Location = new Point(lx, startY + gapY), AutoSize = true, Font = labelFont };
            this.txtPort = new TextBox() { Text = "1521", Location = new Point(tx, startY + gapY - 3), Width = tw, Font = textFont, ReadOnly = true };

            this.lblSid = new Label() { Text = "Service/PDB:", Location = new Point(lx, startY + gapY * 2), AutoSize = true, Font = labelFont };
            this.txtSid = new TextBox() { Text = "xepdb1", Location = new Point(tx, startY + gapY * 2 - 3), Width = tw, Font = textFont, ReadOnly = true };

            this.lblUser = new Label() { Text = "Username:", Location = new Point(lx, startY + gapY * 3), AutoSize = true, Font = labelFont };
            this.txtUser = new TextBox() { Text = "NV005", Location = new Point(tx, startY + gapY * 3 - 3), Width = tw, Font = textFont };

            this.lblPass = new Label() { Text = "Password:", Location = new Point(lx, startY + gapY * 4), AutoSize = true, Font = labelFont };
            this.txtPass = new TextBox() { Text = "123", Location = new Point(tx, startY + gapY * 4 - 3), Width = tw, UseSystemPasswordChar = true, Font = textFont };

            this.btnLogin = new Button() { Text = "ĐĂNG NHẬP", Location = new Point(tx, startY + gapY * 5), Width = tw, Height = 40 };
            this.btnLogin.BackColor = Color.SteelBlue;
            this.btnLogin.ForeColor = Color.White;
            this.btnLogin.FlatStyle = FlatStyle.Flat;
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnLogin.Cursor = Cursors.Hand;
            this.btnLogin.Click += BtnLogin_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblDemo);
            this.Controls.Add(lblHost); this.Controls.Add(txtHost);
            this.Controls.Add(lblPort); this.Controls.Add(txtPort);
            this.Controls.Add(lblSid);  this.Controls.Add(txtSid);
            this.Controls.Add(lblUser); this.Controls.Add(txtUser);
            this.Controls.Add(lblPass); this.Controls.Add(txtPass);
            this.Controls.Add(btnLogin);
            this.AcceptButton = this.btnLogin;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string host = txtHost.Text.Trim();
            string port = txtPort.Text.Trim();
            string sid = txtSid.Text.Trim();
            string user = txtUser.Text.Trim();
            string pass = txtPass.Text;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Vui lòng nhập Username và Password!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DBConfig.UpdateConfig(host, port, sid, user, pass);

            try
            {
                Form nextForm = ResolveNextForm(user);
                MoForm(nextForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi đăng nhập demo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Form ResolveNextForm(string username)
        {
            string normalizedUser = (username ?? string.Empty).Trim().ToUpperInvariant();
            if (normalizedUser == "ADMIN" || normalizedUser == "SYS" || normalizedUser == "SYSTEM")
            {
                return new FormMain();
            }

            // 8 user OLS test - mở FormThongBao để xem thông báo
            if (normalizedUser == "U1_BGD" || normalizedUser == "U2_LDK" || normalizedUser == "U3_LDK" ||
                normalizedUser == "U4_NV" || normalizedUser == "U5_NV" || normalizedUser == "U6_LDK" ||
                normalizedUser == "U7_LDK" || normalizedUser == "U8_NV")
            {
                return new FormThongBao(DBConfig.ConnectionString);
            }

            if (normalizedUser == "BS" || normalizedUser == "BACSI" || normalizedUser == "NV005" || normalizedUser == "NV009")
            {
                return new FormDoctorMain(GetDemoDisplayName(normalizedUser, "Bác sĩ/Y sĩ demo"));
            }

            if (normalizedUser == "DPV" || normalizedUser == "DIEUPHOI" || normalizedUser == "NV001")
            {
                return new FormCoordinatorMain(GetDemoDisplayName(normalizedUser, "Điều phối viên demo"));
            }

            if (normalizedUser == "KTV" || normalizedUser == "NV015")
            {
                return new KTV.FormKTVMain();
            }

            if (normalizedUser == "BN" || normalizedUser.StartsWith("BN"))
            {
                return new BN.FormBenhNhanMain();
            }

            return new FormDoctorMain(GetDemoDisplayName(normalizedUser, "Bác sĩ/Y sĩ demo"));
        }

        private string GetDemoDisplayName(string username, string fallback)
        {
            if (username == "NV005") return "Đặng Thu Hà";
            if (username == "NV009") return "Đỗ Mỹ Linh";
            if (username == "NV001") return "Nguyễn Văn An";
            if (username == "NV015") return "Nguyễn Tử Quảng";
            if (username.StartsWith("BN")) return "Nguyễn Văn Khách";
            return fallback;
        }

        private void MoForm(Form nextForm)
        {
            if (nextForm is ILogoutSupport logoutForm)
            {
                logoutForm.LogoutRequested += (s, e) =>
                {
                    nextForm.Hide();
                    txtPass.Clear();
                    txtUser.Focus();
                    this.Show();
                };

                nextForm.FormClosed += (s, e) =>
                {
                    txtPass.Clear();
                    this.Show();
                };

                this.Hide();
                nextForm.Show();
            }
            else
            {
                this.Hide();
                nextForm.ShowDialog();
                this.Show();
            }
        }
    }
}