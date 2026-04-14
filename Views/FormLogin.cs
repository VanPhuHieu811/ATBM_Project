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
            this.Text = "Login to Oracle DB";
            this.ClientSize = new Size(450, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.WhiteSmoke;

            Font headerFont = new Font("Segoe UI", 16F, FontStyle.Bold);
            Font labelFont = new Font("Segoe UI", 10F, FontStyle.Regular);
            Font textFont = new Font("Segoe UI", 10F, FontStyle.Regular);

            Label lblTitle = new Label() { Text = "Oracle Database Login", Location = new Point(0, 30), AutoSize = false, Width = 450, Height = 50, TextAlign = ContentAlignment.MiddleCenter, Font = headerFont, ForeColor = Color.SteelBlue };

            int startY = 100;
            int gapY = 45;
            int lx = 80;
            int tx = 180;
            int tw = 180;

            this.lblHost = new Label() { Text = "Host:", Location = new Point(lx, startY), AutoSize = true, Font = labelFont };
            this.txtHost = new TextBox() { Text = "localhost", Location = new Point(tx, startY - 3), Width = tw, Font = textFont };

            this.lblPort = new Label() { Text = "Port:", Location = new Point(lx, startY + gapY), AutoSize = true, Font = labelFont };
            this.txtPort = new TextBox() { Text = "1521", Location = new Point(tx, startY + gapY - 3), Width = tw, Font = textFont }; // Cho phép chỉnh sửa

            this.lblSid = new Label() { Text = "Service/PDB:", Location = new Point(lx, startY + gapY * 2), AutoSize = true, Font = labelFont };
            this.txtSid = new TextBox() { Text = "xepdb1", Location = new Point(tx, startY + gapY * 2 - 3), Width = tw, Font = textFont };

            this.lblUser = new Label() { Text = "Username:", Location = new Point(lx, startY + gapY * 3), AutoSize = true, Font = labelFont };
            this.txtUser = new TextBox() { Location = new Point(tx, startY + gapY * 3 - 3), Width = tw, Font = textFont };

            this.lblPass = new Label() { Text = "Password:", Location = new Point(lx, startY + gapY * 4), AutoSize = true, Font = labelFont };
            this.txtPass = new TextBox() { Location = new Point(tx, startY + gapY * 4 - 3), Width = tw, UseSystemPasswordChar = true, Font = textFont };

            this.btnLogin = new Button() { Text = "ĐĂNG NHẬP", Location = new Point(tx, startY + gapY * 5), Width = tw, Height = 40 };
            this.btnLogin.BackColor = Color.SteelBlue;
            this.btnLogin.ForeColor = Color.White;
            this.btnLogin.FlatStyle = FlatStyle.Flat;
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnLogin.Cursor = Cursors.Hand;
            this.btnLogin.Click += BtnLogin_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblHost); this.Controls.Add(txtHost);
            this.Controls.Add(lblPort); this.Controls.Add(txtPort);
            this.Controls.Add(lblSid);  this.Controls.Add(txtSid);
            this.Controls.Add(lblUser); this.Controls.Add(txtUser);
            this.Controls.Add(lblPass); this.Controls.Add(txtPass);
            this.Controls.Add(btnLogin);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string host = txtHost.Text.Trim();
            string port = txtPort.Text.Trim();
            string sid = txtSid.Text.Trim();
            string user = txtUser.Text.Trim();
            string pass = txtPass.Text;

            if (string.IsNullOrEmpty(sid) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Vui lòng nhập Service/PDB, Username và Password!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Chặn connect root service mặc định vì sẽ gây lỗi scope khi revoke.
            if (sid.Equals("xe", StringComparison.OrdinalIgnoreCase) ||
                sid.Equals("cdb$root", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Vui lòng dùng service của PDB (ví dụ: xepdb1), không dùng root service 'xe'.", "Sai Service/PDB", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Update configuration logic
            DBConfig.UpdateConfig(host, port, sid, user, pass);

            try
            {
                if (DBConfig.TestConnection())
                {
                    // Chuyển sang Form Main
                    FormMain fm = new FormMain();
                    this.Hide();
                    fm.ShowDialog();
                    this.Close(); // Kết thúc app khi FormMain đóng
                }
                else
                {
                    MessageBox.Show("Kết nối thất bại. Vui lòng kiểm tra lại thông tin đăng nhập!", "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}