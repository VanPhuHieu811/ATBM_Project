    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using ATBM_Project.Data;
    using ATBM_Project.Presenters;

    namespace ATBM_Project.Views
    {
        public class FormLogin : Form
        {
            private Label lblUser, lblPass;
            private TextBox txtUser, txtPass;
            private Button btnLogin;

            public FormLogin()
            {
                InitializeComponent();
            }

            private void InitializeComponent()
            {
                this.Text = "Đăng nhập hệ thống bệnh viện";
                this.ClientSize = new Size(450, 305);
                this.StartPosition = FormStartPosition.CenterScreen;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.BackColor = Color.WhiteSmoke;

                Font headerFont = new Font("Segoe UI", 16F, FontStyle.Bold);
                Font labelFont = new Font("Segoe UI", 10F, FontStyle.Regular);
                Font textFont = new Font("Segoe UI", 10F, FontStyle.Regular);

                Label lblTitle = new Label() { Text = "Đăng nhập hệ thống bệnh viện", Location = new Point(0, 25), AutoSize = false, Width = 450, Height = 42, TextAlign = ContentAlignment.MiddleCenter, Font = headerFont, ForeColor = Color.SteelBlue };

                int startY = 115;
                int gapY = 45;
                int lx = 80;
                int tx = 180;
                int tw = 180;

                this.lblUser = new Label() { Text = "Username:", Location = new Point(lx, startY), AutoSize = true, Font = labelFont };
                this.txtUser = new TextBox() { Location = new Point(tx, startY - 3), Width = tw, Font = textFont };

                this.lblPass = new Label() { Text = "Password:", Location = new Point(lx, startY + gapY), AutoSize = true, Font = labelFont };
                this.txtPass = new TextBox() { Location = new Point(tx, startY + gapY - 3), Width = tw, UseSystemPasswordChar = true, Font = textFont };

                this.btnLogin = new Button() { Text = "ĐĂNG NHẬP", Location = new Point(tx, startY + gapY * 2 + 15), Width = tw, Height = 40 };
                this.btnLogin.BackColor = Color.SteelBlue;
                this.btnLogin.ForeColor = Color.White;
                this.btnLogin.FlatStyle = FlatStyle.Flat;
                this.btnLogin.FlatAppearance.BorderSize = 0;
                this.btnLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                this.btnLogin.Cursor = Cursors.Hand;
                this.btnLogin.Click += BtnLogin_Click;

                this.Controls.Add(lblTitle);
                this.Controls.Add(lblUser); this.Controls.Add(txtUser);
                this.Controls.Add(lblPass); this.Controls.Add(txtPass);
                this.Controls.Add(btnLogin);
                this.AcceptButton = this.btnLogin;
            }

            private void BtnLogin_Click(object sender, EventArgs e)
            {
                string user = txtUser.Text.Trim();
                string pass = txtPass.Text;

                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                {
                    MessageBox.Show("Vui lòng nhập Username và Password!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DBConfig.UpdateConfig(DBConfig.Host, DBConfig.Port, DBConfig.ServiceName, user, pass);

                try
                {
                    if (!DBConfig.TestConnection())
                    {
                        MessageBox.Show("Sai Username/Password hoặc không kết nối được CSDL.", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    Form nextForm = ResolveNextForm();
                    MoForm(nextForm);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi đăng nhập demo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private Form ResolveNextForm()

            {
                SessionPresenter sessionPresenter = new SessionPresenter();
                string role = (sessionPresenter.GetCurrentRole() ?? string.Empty).Trim();
                string normalizedRole = role.ToUpperInvariant();
                     

            if (string.IsNullOrWhiteSpace(role))
                {
                    throw new Exception("Không xác định được vai trò của tài khoản trong hệ thống.");
                }

                if (normalizedRole == "DBA")
                {
                    return new FormMain();
                }

                if (normalizedRole == "ĐIỀU PHỐI VIÊN")
                {
                    return new DPV.FormCoordinatorMain();
                }

                if (normalizedRole == "BÁC SĨ/Y SĨ")
                {
                    return new FormDoctorMain(sessionPresenter.GetCurrentDisplayName());
                }

                if (normalizedRole == "KỸ THUẬT VIÊN")
                {
                    return new KTV.FormKTVMain();
                }

                if (normalizedRole == "BỆNH NHÂN")
                {
                    return new BN.FormBenhNhanMain();
                }

                
                    throw new Exception("Vai trò chưa được hỗ trợ: " + role);
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
                    txtPass.Clear();
                    txtUser.Focus();
                    this.Show();
                }
            }
        }
    }
