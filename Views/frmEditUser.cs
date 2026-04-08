// ============================================================
// frmEditUser.cs
// Dialog sửa thông tin user (password / tablespace / trạng thái)
// ============================================================
using System;
using System.Data;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ATBM_Project.Views
{
    public class frmEditUser : Form
    {
        private readonly string _connStr;
        private readonly string _username;

        private TextBox txtNewPassword;
        private ComboBox cboTablespace;
        private ComboBox cboStatus;    // OPEN / LOCKED
        private Button btnSave, btnCancel;

        public frmEditUser(string connStr, string username)
        {
            _connStr = connStr;
            _username = username;
            BuildUI();
            LoadCurrentInfo();
        }

        private void BuildUI()
        {
            this.Text = $"Sửa User: {_username}";
            this.Size = new System.Drawing.Size(380, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);

            int lw = 130, tx = 145, tw = 200;

            var lblPwd = new Label { Text = "Password mới:", Location = new System.Drawing.Point(12, 20), Size = new System.Drawing.Size(lw, 24), TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            txtNewPassword = new TextBox { Location = new System.Drawing.Point(tx, 18), Size = new System.Drawing.Size(tw, 24), UseSystemPasswordChar = true };

            var lblTs = new Label { Text = "Default Tablespace:", Location = new System.Drawing.Point(12, 55), Size = new System.Drawing.Size(lw, 24), TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            cboTablespace = new ComboBox { Location = new System.Drawing.Point(tx, 53), Size = new System.Drawing.Size(tw, 24), DropDownStyle = ComboBoxStyle.DropDownList };

            var lblStat = new Label { Text = "Trạng thái:", Location = new System.Drawing.Point(12, 90), Size = new System.Drawing.Size(lw, 24), TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            cboStatus = new ComboBox { Location = new System.Drawing.Point(tx, 88), Size = new System.Drawing.Size(tw, 24), DropDownStyle = ComboBoxStyle.DropDownList };
            cboStatus.Items.AddRange(new[] { "OPEN (Mở khóa)", "LOCKED (Khóa)" });

            btnSave = new Button { Text = "Lưu", Location = new System.Drawing.Point(tx, 150), Size = new System.Drawing.Size(90, 30), BackColor = System.Drawing.Color.FromArgb(24, 95, 165), ForeColor = System.Drawing.Color.White, FlatStyle = FlatStyle.Flat, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Hủy", Location = new System.Drawing.Point(tx + 100, 150), Size = new System.Drawing.Size(90, 30), FlatStyle = FlatStyle.Flat };

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblPwd, txtNewPassword, lblTs, cboTablespace,
                lblStat, cboStatus, btnSave, btnCancel
            });
        }

        private void LoadCurrentInfo()
        {
            try
            {
                using (var conn = new OracleConnection(_connStr))
                {
                    conn.Open();

                    // Load tablespace list
                    var dtTs = new DataTable();
                    using (var cmd = new OracleCommand(
                        "SELECT TABLESPACE_NAME FROM DBA_TABLESPACES WHERE CONTENTS IN ('PERMANENT','UNDO') ORDER BY 1", conn))
                    {
                        using (var adp = new OracleDataAdapter(cmd))
                        {
                            adp.Fill(dtTs);
                        }
                    }

                    foreach (DataRow r in dtTs.Rows)
                        cboTablespace.Items.Add(r[0].ToString());

                    // Load current user info
                    using (var cmd = new OracleCommand(
                        "SELECT DEFAULT_TABLESPACE, ACCOUNT_STATUS FROM DBA_USERS WHERE USERNAME = :u", conn))
                    {
                        cmd.Parameters.Add("u", OracleDbType.Varchar2).Value = _username;
                        using (var rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                string ts = rdr["DEFAULT_TABLESPACE"].ToString();
                                string status = rdr["ACCOUNT_STATUS"].ToString();
                                cboTablespace.SelectedItem = ts;
                                cboStatus.SelectedIndex = status.Contains("LOCKED") ? 1 : 0;
                            }
                        }
                    }
                } // Kết thúc using connection tại đây
            }
            catch (OracleException ex)
            {
                MessageBox.Show(ex.Message, "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (var conn = new OracleConnection(_connStr))
                {
                    conn.Open();

                    // Đổi password nếu có nhập
                    if (!string.IsNullOrWhiteSpace(txtNewPassword.Text))
                    {
                        string pwd = txtNewPassword.Text.Trim();
                        using (var cmd = new OracleCommand($@"ALTER USER {_username} IDENTIFIED BY ""{pwd}""", conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Đổi tablespace nếu chọn
                    if (cboTablespace.SelectedItem != null)
                    {
                        string ts = cboTablespace.SelectedItem.ToString();
                        using (var cmd = new OracleCommand($"ALTER USER {_username} DEFAULT TABLESPACE {ts}", conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Đổi trạng thái LOCK / UNLOCK
                    if (cboStatus.SelectedIndex == 1) // Chọn LOCKED
                    {
                        using (var cmd = new OracleCommand($"ALTER USER {_username} ACCOUNT LOCK", conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else // Chọn OPEN
                    {
                        using (var cmd = new OracleCommand($"ALTER USER {_username} ACCOUNT UNLOCK", conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                } // Đóng connection sau khi thực hiện xong tất cả lệnh

                MessageBox.Show($"Đã cập nhật user [{_username}] thành công.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (OracleException ex)
            {
                MessageBox.Show($"Lỗi Oracle [{ex.Number}]: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}