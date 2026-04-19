using ATBM_Project.Data;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace ATBM_Project.Views
{
    public class FormViewPrivileges : Form
    {
        private Label label1;
        private TextBox txtTargetName;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private DataGridView dgvObjectPrivs;
        private TabPage tabPage2;
        private DataGridView dgvColumnPrivs;
        private Button btnSearch;

        public FormViewPrivileges()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtTargetName = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dgvObjectPrivs = new System.Windows.Forms.DataGridView();
            this.dgvColumnPrivs = new System.Windows.Forms.DataGridView();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvObjectPrivs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvColumnPrivs)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nhập tên User/Role";
            // 
            // txtTargetName
            // 
            this.txtTargetName.Location = new System.Drawing.Point(143, 6);
            this.txtTargetName.Name = "txtTargetName";
            this.txtTargetName.Size = new System.Drawing.Size(100, 20);
            this.txtTargetName.TabIndex = 1;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(277, 6);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Tìm kiếm";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(15, 47);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(677, 347);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dgvObjectPrivs);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(669, 321);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Quyền Đối tượng";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dgvColumnPrivs);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(669, 321);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Quyền trên Cột";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dgvObjectPrivs
            // 
            this.dgvObjectPrivs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvObjectPrivs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvObjectPrivs.Location = new System.Drawing.Point(3, 3);
            this.dgvObjectPrivs.Name = "dgvObjectPrivs";
            this.dgvObjectPrivs.Size = new System.Drawing.Size(663, 315);
            this.dgvObjectPrivs.TabIndex = 0;
            this.dgvObjectPrivs.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvObjectPrivs_CellContentClick);
            // 
            // dgvColumnPrivs
            // 
            this.dgvColumnPrivs.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvColumnPrivs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvColumnPrivs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvColumnPrivs.Location = new System.Drawing.Point(3, 3);
            this.dgvColumnPrivs.Name = "dgvColumnPrivs";
            this.dgvColumnPrivs.Size = new System.Drawing.Size(663, 315);
            this.dgvColumnPrivs.TabIndex = 0;
            // 
            // FormViewPrivileges
            // 
            this.ClientSize = new System.Drawing.Size(704, 461);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtTargetName);
            this.Controls.Add(this.label1);
            this.Name = "FormViewPrivileges";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvObjectPrivs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvColumnPrivs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void dgvObjectPrivs_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            // Lấy tên User/Role người dùng nhập vào
            string targetName = txtTargetName.Text.Trim();

            if (string.IsNullOrEmpty(targetName))
            {
                MessageBox.Show("Vui lòng nhập tên User hoặc Role cần xem quyền!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // TẠO KẾT NỐI: Đổi DBConfig.ConnectionString thành chuỗi kết nối thực tế của nhóm bạn
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                try
                {
                    conn.Open();

                    // ==========================================
                    // 1. Lấy dữ liệu quyền trên ĐỐI TƯỢNG (Bảng/View)
                    // ==========================================
                    string sqlObject = @"SELECT GRANTEE, OWNER, TABLE_NAME, PRIVILEGE, GRANTABLE 
                                 FROM DBA_TAB_PRIVS 
                                 WHERE GRANTEE = UPPER(:targetName)";

                    using (OracleCommand cmdObj = new OracleCommand(sqlObject, conn))
                    {
                        // Dùng Parameter để tránh SQL Injection
                        cmdObj.Parameters.Add(new OracleParameter("targetName", targetName));
                        using (OracleDataAdapter daObj = new OracleDataAdapter(cmdObj))
                        {
                            DataTable dtObjectPrivs = new DataTable();
                            daObj.Fill(dtObjectPrivs);
                            // Đổ dữ liệu vào DataGridView ở Tab 1
                            dgvObjectPrivs.DataSource = dtObjectPrivs;
                        }
                    }

                    // ==========================================
                    // 2. Lấy dữ liệu quyền trên CỘT
                    // ==========================================
                    string sqlCol = @"SELECT GRANTEE, OWNER, TABLE_NAME, COLUMN_NAME, PRIVILEGE, GRANTABLE 
                              FROM DBA_COL_PRIVS 
                              WHERE GRANTEE = UPPER(:targetName)";

                    using (OracleCommand cmdCol = new OracleCommand(sqlCol, conn))
                    {
                        cmdCol.Parameters.Add(new OracleParameter("targetName", targetName));
                        using (OracleDataAdapter daCol = new OracleDataAdapter(cmdCol))
                        {
                            DataTable dtColPrivs = new DataTable();
                            daCol.Fill(dtColPrivs);
                            // Đổ dữ liệu vào DataGridView ở Tab 2
                            dgvColumnPrivs.DataSource = dtColPrivs;
                        }
                    }

                    // Format lại tên cột cho dễ đọc (Tùy chọn)
                    FormatDataGridView(dgvObjectPrivs);
                    FormatDataGridView(dgvColumnPrivs);

                    MessageBox.Show("Đã tải dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi truy xuất dữ liệu quyền: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FormatDataGridView(DataGridView dgv)
        {
            if (dgv.Columns.Contains("GRANTEE")) dgv.Columns["GRANTEE"].HeaderText = "Người/Vai trò nhận";
            if (dgv.Columns.Contains("OWNER")) dgv.Columns["OWNER"].HeaderText = "Người sở hữu";
            if (dgv.Columns.Contains("TABLE_NAME")) dgv.Columns["TABLE_NAME"].HeaderText = "Tên Đối Tượng";
            if (dgv.Columns.Contains("COLUMN_NAME")) dgv.Columns["COLUMN_NAME"].HeaderText = "Tên Cột";
            if (dgv.Columns.Contains("PRIVILEGE")) dgv.Columns["PRIVILEGE"].HeaderText = "Quyền hạn";
            if (dgv.Columns.Contains("GRANTABLE")) dgv.Columns["GRANTABLE"].HeaderText = "Có quyền Grant (YES/NO)";

            // Auto-size các cột để vừa vặn nội dung
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

    }
}