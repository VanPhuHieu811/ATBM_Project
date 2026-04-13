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
            this.lblHost = new Label() { Text = "Host:", Location = new Point(30, 30), AutoSize = true };
            this.txtHost = new TextBox() { Location = new Point(120, 27), Width = 150 };

            this.lblPort = new Label() { Text = "Port:", Location = new Point(30, 70), AutoSize = true };
            this.txtPort = new TextBox() { Text = "1521", Location = new Point(120, 67), Width = 150, ReadOnly = true };

            this.lblSid = new Label() { Text = "SID:", Location = new Point(30, 110), AutoSize = true };
            this.txtSid = new TextBox() { Text = "xe", Location = new Point(120, 107), Width = 150, ReadOnly = true };

            this.lblUser = new Label() { Text = "Username:", Location = new Point(30, 150), AutoSize = true };
            this.txtUser = new TextBox() { Location = new Point(120, 147), Width = 150 };

            this.lblPass = new Label() { Text = "Password:", Location = new Point(30, 190), AutoSize = true };
            this.txtPass = new TextBox() { Location = new Point(120, 187), Width = 150, UseSystemPasswordChar = true };

            this.btnLogin = new Button() { Text = "Login", Location = new Point(120, 230), Width = 80, Height = 30 };
            this.btnLogin.Click += BtnLogin_Click;

            this.Controls.Add(lblHost); this.Controls.Add(txtHost);
            this.Controls.Add(lblPort); this.Controls.Add(txtPort);
            this.Controls.Add(lblSid);  this.Controls.Add(txtSid);
            this.Controls.Add(lblUser); this.Controls.Add(txtUser);
            this.Controls.Add(lblPass); this.Controls.Add(txtPass);
            this.Controls.Add(btnLogin);

            this.Text = "Login to Oracle DB";
            this.ClientSize = new Size(320, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
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