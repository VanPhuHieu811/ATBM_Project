// ============================================================
// OracleHelper.cs
// Lớp tiện ích kết nối Oracle – dùng chung toàn ứng dụng
// ============================================================

using System;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ATBM_Project.Views
{
    // ============================================================
    // A. CONNECTION MANAGER
    // ============================================================
    public static class OracleHelper
    {
        /// <summary>
        /// Build connection string từ các thông số đăng nhập.
        /// Host: vd "localhost", Port: 1521, ServiceName: "ORCL"
        /// </summary>
        public static string BuildConnectionString(
            string host, int port, string serviceName,
            string username, string password)
        {
            return $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)" +
                   $"(HOST={host})(PORT={port}))(CONNECT_DATA=(SERVICE_NAME={serviceName})))" +
                   $";User Id={username};Password={password};";
        }

        /// <summary>Kiểm tra kết nối có thành công không</summary>
        public static bool TestConnection(string connStr, out string errorMsg)
        {
            errorMsg = string.Empty;
            try
            {
                using (var conn = new OracleConnection(connStr))
                {
                    conn.Open();
                }
                return true;
            }
            catch (OracleException ex)
            {
                errorMsg = $"ORA-{ex.Number}: {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }
    }

    // ============================================================
    // B. FORM ĐĂNG NHẬP
    // ============================================================
    public class frmLogin : Form
    {
        private System.Windows.Forms.TextBox txtHost, txtPort, txtService, txtUser, txtPass;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblStatus;

        public string ConnectionString { get; private set; }

        public frmLogin()
        {
            this.Text = "Kết nối Oracle DB";
            this.Size = new System.Drawing.Size(380, 310);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);

            int lx = 12, tx = 120, tw = 220, dy = 35;

            System.Windows.Forms.Control[] items =
            {
                MakeLabel("Host:",         lx, 20),
                txtHost    = MakeTxt(tx, 18,  tw, "localhost"),

                MakeLabel("Port:",         lx, 20 + dy),
                txtPort    = MakeTxt(tx, 18 + dy, tw, "1521"),

                MakeLabel("Service Name:", lx, 20 + dy*2),
                txtService = MakeTxt(tx, 18 + dy*2, tw, "XE"),

                MakeLabel("Username:",     lx, 20 + dy*3),
                txtUser    = MakeTxt(tx, 18 + dy*3, tw, "SYSTEM"),

                MakeLabel("Password:",     lx, 20 + dy*4),
                txtPass    = MakeTxt(tx, 18 + dy*4, tw, ""),
            };
            txtPass.UseSystemPasswordChar = true;

            btnConnect = new System.Windows.Forms.Button
            {
                Text = "Kết nối",
                Location = new System.Drawing.Point(tx, 20 + dy * 5),
                Size = new System.Drawing.Size(100, 30),
                BackColor = System.Drawing.Color.FromArgb(24, 95, 165),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
            };
            btnConnect.Click += BtnConnect_Click;

            lblStatus = new System.Windows.Forms.Label
            {
                Location = new System.Drawing.Point(12, 230),
                Size = new System.Drawing.Size(340, 40),
                ForeColor = System.Drawing.Color.FromArgb(163, 45, 45),
                Font = new System.Drawing.Font("Segoe UI", 8.5F),
            };

            this.Controls.AddRange(items);
            this.Controls.Add(btnConnect);
            this.Controls.Add(lblStatus);
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                int port = int.Parse(txtPort.Text.Trim());
                string connStr = OracleHelper.BuildConnectionString(
                    txtHost.Text.Trim(), port, txtService.Text.Trim(),
                    txtUser.Text.Trim(), txtPass.Text);

                if (OracleHelper.TestConnection(connStr, out string err))
                {
                    ConnectionString = connStr;
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
                else
                {
                    lblStatus.Text = $"Kết nối thất bại: {err}";
                }
            }
            catch (FormatException)
            {
                lblStatus.Text = "Port phải là số nguyên.";
            }
        }

        private static System.Windows.Forms.Label MakeLabel(string text, int x, int y)
            => new System.Windows.Forms.Label
            {
                Text = text,
                Location = new System.Drawing.Point(x, y + 3),
                Size = new System.Drawing.Size(105, 22),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
            };

        private static System.Windows.Forms.TextBox MakeTxt(int x, int y, int w, string def)
            => new System.Windows.Forms.TextBox
            {
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(w, 24),
                Text = def,
            };
    }
}

    // ============================================================
    // C. ENTRY POINT
    // ============================================================
    
