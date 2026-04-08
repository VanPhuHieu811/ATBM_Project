// ============================================================
// frmUserRole.Designer.cs
// Định nghĩa layout giao diện WinForm cho Màn hình 1
// ============================================================

namespace ATBM_Project.Views
{
    partial class frmUserRole
    {
        private System.ComponentModel.IContainer components = null;

        // ── Controls khu vực TẠO USER ──────────────────────────
        private System.Windows.Forms.GroupBox grpUser;
        private System.Windows.Forms.Label lblUsername, lblPassword, lblTablespace, lblProfile;
        private System.Windows.Forms.TextBox txtUsername, txtPassword;
        private System.Windows.Forms.ComboBox cboTablespace, cboProfile;
        private System.Windows.Forms.Button btnCreateUser, btnEditUser, btnDropUser;

        // ── Controls khu vực TẠO ROLE ──────────────────────────
        private System.Windows.Forms.GroupBox grpRole;
        private System.Windows.Forms.Label lblRoleName, lblRoleAuth, lblRolePassword;
        private System.Windows.Forms.TextBox txtRoleName, txtRolePassword;
        private System.Windows.Forms.ComboBox cboRoleAuth;
        private System.Windows.Forms.Button btnCreateRole, btnDropRole;

        // ── Controls khu vực GÁN ROLE ──────────────────────────
        private System.Windows.Forms.GroupBox grpGrantRole;
        private System.Windows.Forms.Label lblGrantUser, lblGrantRole;
        private System.Windows.Forms.ComboBox cboGrantUser, cboGrantRole;
        private System.Windows.Forms.CheckBox chkWithAdmin;
        private System.Windows.Forms.Button btnGrantRole;
        private System.Windows.Forms.DataGridView dgvCurrentRoles;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // ── Form ────────────────────────────────────────────
            this.Text = "Quản lý User / Role — Oracle Admin";
            this.Size = new System.Drawing.Size(900, 640);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.BackColor = System.Drawing.Color.FromArgb(245, 246, 250);

            // ============================================================
            // GROUP: TẠO USER (cột trái, trên)
            // ============================================================
            grpUser = new System.Windows.Forms.GroupBox
            {
                Text = "Quản lý User",
                Location = new System.Drawing.Point(12, 12),
                Size = new System.Drawing.Size(410, 220),
                Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold),
            };

            // Username
            lblUsername = MakeLabel("Username:", new System.Drawing.Point(12, 28));
            txtUsername = MakeTextBox(new System.Drawing.Point(110, 25), 280);

            // Password
            lblPassword = MakeLabel("Password:", new System.Drawing.Point(12, 60));
            txtPassword = MakeTextBox(new System.Drawing.Point(110, 57), 280);
            txtPassword.UseSystemPasswordChar = true;

            // Tablespace
            lblTablespace = MakeLabel("Tablespace:", new System.Drawing.Point(12, 92));
            cboTablespace = MakeCombo(new System.Drawing.Point(110, 89), 280);

            // Profile
            lblProfile = MakeLabel("Profile:", new System.Drawing.Point(12, 124));
            cboProfile = MakeCombo(new System.Drawing.Point(110, 121), 280);

            // Buttons
            btnCreateUser = MakeButton("Tạo User", new System.Drawing.Point(12, 165), System.Drawing.Color.FromArgb(24, 95, 165), System.Drawing.Color.White);
            btnEditUser = MakeButton("Sửa User", new System.Drawing.Point(118, 165), System.Drawing.Color.FromArgb(63, 84, 95), System.Drawing.Color.White);
            btnDropUser = MakeButton("Xóa User", new System.Drawing.Point(224, 165), System.Drawing.Color.FromArgb(163, 45, 45), System.Drawing.Color.White);

            btnCreateUser.Click += btnCreateUser_Click;
            btnEditUser.Click += btnEditUser_Click;
            btnDropUser.Click += btnDropUser_Click;

            grpUser.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblUsername, txtUsername, lblPassword, txtPassword,
                lblTablespace, cboTablespace, lblProfile, cboProfile,
                btnCreateUser, btnEditUser, btnDropUser
            });

            // ============================================================
            // GROUP: TẠO ROLE (cột phải, trên)
            // ============================================================
            grpRole = new System.Windows.Forms.GroupBox
            {
                Text = "Quản lý Role",
                Location = new System.Drawing.Point(436, 12),
                Size = new System.Drawing.Size(440, 220),
                Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold),
            };

            lblRoleName = MakeLabel("Tên Role:", new System.Drawing.Point(12, 28));
            txtRoleName = MakeTextBox(new System.Drawing.Point(120, 25), 300);

            lblRoleAuth = MakeLabel("Xác thực:", new System.Drawing.Point(12, 60));
            cboRoleAuth = MakeCombo(new System.Drawing.Point(120, 57), 300);
            cboRoleAuth.Items.AddRange(new[] { "None", "Password", "External" });
            cboRoleAuth.SelectedIndex = 0;
            cboRoleAuth.SelectedIndexChanged += cboRoleAuth_SelectedIndexChanged;

            lblRolePassword = MakeLabel("Role Password:", new System.Drawing.Point(12, 92));
            txtRolePassword = MakeTextBox(new System.Drawing.Point(120, 89), 300);
            txtRolePassword.UseSystemPasswordChar = true;
            txtRolePassword.Enabled = false;

            btnCreateRole = MakeButton("Tạo Role", new System.Drawing.Point(12, 165), System.Drawing.Color.FromArgb(24, 95, 165), System.Drawing.Color.White);
            btnDropRole = MakeButton("Xóa Role", new System.Drawing.Point(118, 165), System.Drawing.Color.FromArgb(163, 45, 45), System.Drawing.Color.White);

            btnCreateRole.Click += btnCreateRole_Click;
            btnDropRole.Click += btnDropRole_Click;

            grpRole.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblRoleName, txtRoleName, lblRoleAuth, cboRoleAuth,
                lblRolePassword, txtRolePassword,
                btnCreateRole, btnDropRole
            });

            // ============================================================
            // GROUP: GÁN ROLE CHO USER (dưới, full width)
            // ============================================================
            grpGrantRole = new System.Windows.Forms.GroupBox
            {
                Text = "Gán Role cho User",
                Location = new System.Drawing.Point(12, 248),
                Size = new System.Drawing.Size(864, 340),
                Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold),
            };

            lblGrantUser = MakeLabel("Chọn User:", new System.Drawing.Point(12, 28));
            cboGrantUser = MakeCombo(new System.Drawing.Point(110, 25), 250);
            cboGrantUser.SelectedIndexChanged += cboGrantUser_SelectedIndexChanged;

            lblGrantRole = MakeLabel("Chọn Role:", new System.Drawing.Point(390, 28));
            cboGrantRole = MakeCombo(new System.Drawing.Point(480, 25), 200);

            chkWithAdmin = new System.Windows.Forms.CheckBox
            {
                Text = "WITH ADMIN OPTION",
                Location = new System.Drawing.Point(12, 65),
                Size = new System.Drawing.Size(200, 22),
                Font = new System.Drawing.Font("Segoe UI", 9F),
            };

            btnGrantRole = MakeButton("Thực hiện GRANT", new System.Drawing.Point(230, 60),
                System.Drawing.Color.FromArgb(24, 95, 165), System.Drawing.Color.White);
            btnGrantRole.Size = new System.Drawing.Size(150, 28);
            btnGrantRole.Click += btnGrantRole_Click;

            // DataGridView hiển thị role hiện tại của user đã chọn
            dgvCurrentRoles = new System.Windows.Forms.DataGridView
            {
                Location = new System.Drawing.Point(12, 100),
                Size = new System.Drawing.Size(836, 220),
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = System.Drawing.Color.White,
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect,
                Font = new System.Drawing.Font("Segoe UI", 9F),
            };

            grpGrantRole.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblGrantUser, cboGrantUser, lblGrantRole, cboGrantRole,
                chkWithAdmin, btnGrantRole, dgvCurrentRoles
            });

            // ── Thêm tất cả vào Form ────────────────────────────
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                grpUser, grpRole, grpGrantRole
            });

            this.Load += frmUserRole_Load;
        }

        // ── Factory helpers (giảm boilerplate) ─────────────────
        private static System.Windows.Forms.Label MakeLabel(string text, System.Drawing.Point loc)
            => new System.Windows.Forms.Label
            {
                Text = text,
                Location = loc,
                Size = new System.Drawing.Size(96, 22),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Font = new System.Drawing.Font("Segoe UI", 9F),
            };

        private static System.Windows.Forms.TextBox MakeTextBox(System.Drawing.Point loc, int width)
            => new System.Windows.Forms.TextBox
            {
                Location = loc,
                Size = new System.Drawing.Size(width, 24),
                Font = new System.Drawing.Font("Segoe UI", 9F),
            };

        private static System.Windows.Forms.ComboBox MakeCombo(System.Drawing.Point loc, int width)
            => new System.Windows.Forms.ComboBox
            {
                Location = loc,
                Size = new System.Drawing.Size(width, 24),
                DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList,
                Font = new System.Drawing.Font("Segoe UI", 9F),
            };

        private static System.Windows.Forms.Button MakeButton(
            string text, System.Drawing.Point loc,
            System.Drawing.Color backColor, System.Drawing.Color foreColor)
            => new System.Windows.Forms.Button
            {
                Text = text,
                Location = loc,
                Size = new System.Drawing.Size(100, 28),
                BackColor = backColor,
                ForeColor = foreColor,
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 9F),
                Cursor = System.Windows.Forms.Cursors.Hand,
            };
    }
}