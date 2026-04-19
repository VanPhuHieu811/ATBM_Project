// ============================================================
// PHÂN HỆ 1 - MÀN HÌNH 1: Quản lý User / Role
// File: frmUserRole.cs
// Yêu cầu NuGet: Oracle.ManagedDataAccess.Core
// ============================================================

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ATBM_Project.Views
{
    public partial class frmUserRole : Form
    {
        // --------------------------------------------------------
        // Connection string – đọc từ config hoặc truyền vào
        // --------------------------------------------------------
        private readonly string _connStr;

        public frmUserRole(string connectionString)
        {
            InitializeComponent();
            _connStr = connectionString;
        }

        // ============================================================
        // KHỞI TẠO FORM
        // ============================================================
        private void frmUserRole_Load(object sender, EventArgs e)
        {
            LoadTablespaces();   // Nạp combobox Tablespace
            LoadProfiles();      // Nạp combobox Profile
            LoadUsers();         // Nạp combobox User (cho phần Gán role)
            LoadRoles();         // Nạp combobox Role (cho phần Gán role)
        }

        // ============================================================
        // A. TẠO USER MỚI
        // ============================================================
        private void btnCreateUser_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim().ToUpper();
            string password = txtPassword.Text.Trim();
            string tablespace = cboTablespace.SelectedItem?.ToString() ?? "USERS";
            string profile = cboProfile.SelectedItem?.ToString() ?? "DEFAULT";

            // Validate
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập Username và Password.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidOracleIdentifier(username))
            {
                MessageBox.Show("Username không hợp lệ (chỉ dùng chữ, số, dấu _).", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (var conn = new OracleConnection(_connStr))
                {
                    conn.Open();

                    // DDL không hỗ trợ bind param → dùng whitelist validation
                    string sqlCreate = $@"CREATE USER {username} IDENTIFIED BY ""{password}""
                    DEFAULT TABLESPACE {tablespace}
                    TEMPORARY TABLESPACE TEMP
                    PROFILE {profile}";

                    string sqlGrant = $"GRANT CREATE SESSION TO {username}";

                    using (var cmd = new OracleCommand(sqlCreate, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new OracleCommand(sqlGrant, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                } // Đóng ngoặc using connection ở đây

                MessageBox.Show($"Đã tạo user [{username}] thành công.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearUserForm();
                LoadUsers();   // Refresh combobox
            }
            catch (OracleException ex)
            {
                ShowOracleError(ex);
            }
        }

        // ============================================================
        // B. SỬA USER (đổi password / tablespace / profile)
        // ============================================================
        private void btnEditUser_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(username)) { ShowSelectFirst(); return; }

            using (var dlg = new frmEditUser(_connStr, username))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    MessageBox.Show($"Đã cập nhật user [{username}].", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ============================================================
        // C. XÓA USER
        // ============================================================
        private void btnDropUser_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(username)) { ShowSelectFirst(); return; }

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn XÓA user [{username}]?\nHành động này không thể hoàn tác!",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            ExecuteDDL($"DROP USER {username} CASCADE",
                $"Đã xóa user [{username}] thành công.");

            ClearUserForm();
            LoadUsers();
        }

        // ============================================================
        // D. TẠO ROLE MỚI
        // ============================================================
        private void btnCreateRole_Click(object sender, EventArgs e)
        {
            string roleName = txtRoleName.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(roleName))
            {
                MessageBox.Show("Vui lòng nhập tên Role.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidOracleIdentifier(roleName))
            {
                MessageBox.Show("Tên role không hợp lệ.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string authType = cboRoleAuth.SelectedItem?.ToString() ?? "None";
            string rolePassword = txtRolePassword.Text.Trim();

            string sql;
            if (authType == "Password" && !string.IsNullOrEmpty(rolePassword))
                sql = $@"CREATE ROLE {roleName} IDENTIFIED BY ""{rolePassword}""";
            else if (authType == "External")
                sql = $"CREATE ROLE {roleName} IDENTIFIED EXTERNALLY";
            else
                sql = $"CREATE ROLE {roleName}";

            ExecuteDDL(sql, $"Đã tạo role [{roleName}] thành công.");
            ClearRoleForm();
            LoadRoles();
        }

        // ============================================================
        // E. XÓA ROLE
        // ============================================================
        private void btnDropRole_Click(object sender, EventArgs e)
        {
            string roleName = txtRoleName.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(roleName)) { ShowSelectFirst(); return; }

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn XÓA role [{roleName}]?",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            ExecuteDDL($"DROP ROLE {roleName}",
                $"Đã xóa role [{roleName}] thành công.");

            ClearRoleForm();
            LoadRoles();
        }

        // ============================================================
        // F. GÁN ROLE CHO USER
        // ============================================================
        private void btnGrantRole_Click(object sender, EventArgs e)
        {
            string user = cboGrantUser.SelectedItem?.ToString();
            string role = cboGrantRole.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Vui lòng chọn User và Role.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Có WITH ADMIN OPTION không?
            bool withAdmin = chkWithAdmin.Checked;
            string sql = withAdmin
                ? $"GRANT {role} TO {user} WITH ADMIN OPTION"
                : $"GRANT {role} TO {user}";

            ExecuteDDL(sql, $"Đã gán role [{role}] cho user [{user}].");
            LoadCurrentRoles(user);   // Refresh danh sách role hiện tại của user
        }

        // ============================================================
        // G. XEM DANH SÁCH ROLE HIỆN CÓ CỦA USER
        // ============================================================
        private void LoadCurrentRoles(string username)
        {
            try
            {
                using (var conn = new OracleConnection(_connStr))
                {
                    conn.Open();

                    string sql = @"
                    SELECT GRANTED_ROLE, ADMIN_OPTION, DEFAULT_ROLE
                    FROM DBA_ROLE_PRIVS
                    WHERE GRANTEE = :username
                    ORDER BY GRANTED_ROLE";

                    using (var cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add("username", OracleDbType.Varchar2).Value = username;

                        using (var adapter = new OracleDataAdapter(cmd))
                        {
                            var dt = new DataTable();
                            adapter.Fill(dt);

                            dgvCurrentRoles.DataSource = dt;
                            dgvCurrentRoles.Columns["GRANTED_ROLE"].HeaderText = "Role";
                            dgvCurrentRoles.Columns["ADMIN_OPTION"].HeaderText = "Admin Option";
                            dgvCurrentRoles.Columns["DEFAULT_ROLE"].HeaderText = "Default";
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                ShowOracleError(ex);
            }
        }

        // ============================================================
        // H. NẠP COMBOBOX HELPER
        // ============================================================
        private void LoadTablespaces()
        {
            try
            {
                var dt = GetDataTable(@"
                    SELECT TABLESPACE_NAME FROM DBA_TABLESPACES
                    WHERE CONTENTS IN ('PERMANENT','UNDO')
                    ORDER BY TABLESPACE_NAME");

                cboTablespace.Items.Clear();
                foreach (DataRow row in dt.Rows)
                    cboTablespace.Items.Add(row["TABLESPACE_NAME"].ToString());

                if (cboTablespace.Items.Count > 0)
                    cboTablespace.SelectedItem = "USERS";
            }
            catch { cboTablespace.Items.AddRange(new[] { "USERS", "TEMP" }); }
        }

        private void LoadProfiles()
        {
            try
            {
                var dt = GetDataTable(@"
                    SELECT DISTINCT PROFILE FROM DBA_PROFILES
                    ORDER BY PROFILE");

                cboProfile.Items.Clear();
                foreach (DataRow row in dt.Rows)
                    cboProfile.Items.Add(row["PROFILE"].ToString());

                if (cboProfile.Items.Count > 0)
                    cboProfile.SelectedItem = "DEFAULT";
            }
            catch { cboProfile.Items.Add("DEFAULT"); }
        }

        private void LoadUsers()
        {
            try
            {
                var dt = GetDataTable(@"
                    SELECT USERNAME FROM DBA_USERS
                    WHERE USERNAME NOT IN (
                        'SYS','SYSTEM','DBSNMP','OUTLN','MDSYS','ORDSYS','CTXSYS',
                        'ANONYMOUS','XDB','PUBLIC','LBACSYS'
                    )
                    ORDER BY USERNAME");

                cboGrantUser.Items.Clear();
                foreach (DataRow row in dt.Rows)
                    cboGrantUser.Items.Add(row["USERNAME"].ToString());
            }
            catch (OracleException ex) { ShowOracleError(ex); }
        }

        private void LoadRoles()
        {
            try
            {
                var dt = GetDataTable(@"
                    SELECT ROLE FROM DBA_ROLES
                    WHERE ROLE NOT IN (
                        'CONNECT','RESOURCE','DBA','SELECT_CATALOG_ROLE',
                        'EXECUTE_CATALOG_ROLE','DELETE_CATALOG_ROLE',
                        'EXP_FULL_DATABASE','IMP_FULL_DATABASE'
                    )
                    ORDER BY ROLE");

                cboGrantRole.Items.Clear();
                foreach (DataRow row in dt.Rows)
                    cboGrantRole.Items.Add(row["ROLE"].ToString());
            }
            catch (OracleException ex) { ShowOracleError(ex); }
        }

        // ============================================================
        // HELPER METHODS
        // ============================================================

        /// <summary>Thực thi DDL (CREATE/DROP/GRANT/REVOKE) và hiển thị kết quả</summary>
        private void ExecuteDDL(string sql, string successMessage)
        {
            try
            {
                using (var conn = new OracleConnection(_connStr))
                {
                    conn.Open();
                    using (var cmd = new OracleCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show(successMessage, "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (OracleException ex)
            {
                ShowOracleError(ex);
            }
        }

        /// <summary>Trả về DataTable từ câu SELECT</summary>
        private DataTable GetDataTable(string sql)
        {
            using (var conn = new OracleConnection(_connStr))
            {
                conn.Open();
                using (var cmd = new OracleCommand(sql, conn))
                {
                    using (var adapter = new OracleDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        /// <summary>Kiểm tra identifier Oracle hợp lệ (chống SQL injection cho DDL)</summary>
        private static bool IsValidOracleIdentifier(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length > 30) return false;
            foreach (char c in name)
                if (!char.IsLetterOrDigit(c) && c != '_' && c != '#' && c != '$')
                    return false;
            return char.IsLetter(name[0]);
        }

        private static void ShowOracleError(OracleException ex)
        {
            string msg = "";
            switch (ex.Number)
            {
                case 1:
                    msg = "Tên đã tồn tại trong hệ thống.";
                    break;
                case 1918:
                    msg = "User không tồn tại.";
                    break;
                case 1919:
                    msg = "Role không tồn tại.";
                    break;
                case 1924:
                    msg = "Role đã được gán cho user này rồi.";
                    break;
                case 28003:
                    msg = "Password không đáp ứng yêu cầu bảo mật.";
                    break;
                default:
                    msg = ex.Message;
                    break;
            }
            MessageBox.Show($"Lỗi Oracle [{ex.Number}]: {msg}", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void ShowSelectFirst() =>
            MessageBox.Show("Vui lòng chọn hoặc nhập tên đối tượng cần thao tác.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void ClearUserForm()
        {
            txtUsername.Clear();
            txtPassword.Clear();
            if (cboTablespace.Items.Count > 0) cboTablespace.SelectedIndex = 0;
            if (cboProfile.Items.Count > 0) cboProfile.SelectedIndex = 0;
        }

        private void ClearRoleForm()
        {
            txtRoleName.Clear();
            txtRolePassword.Clear();
            if (cboRoleAuth.Items.Count > 0) cboRoleAuth.SelectedIndex = 0;
        }

        // ============================================================
        // EVENT: Khi chọn loại xác thực role → ẩn/hiện ô password
        // ============================================================
        private void cboRoleAuth_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtRolePassword.Enabled = cboRoleAuth.SelectedItem?.ToString() == "Password";
        }

        // ============================================================
        // EVENT: Khi chọn user trong combobox Gán role → load role hiện tại
        // ============================================================
        private void cboGrantUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboGrantUser.SelectedItem != null)
                LoadCurrentRoles(cboGrantUser.SelectedItem.ToString());
        }
    }
}