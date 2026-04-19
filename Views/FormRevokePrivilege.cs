using System;
using System.Windows.Forms;
using ATBM_Project.Presenters;
using ATBM_Project.Models;

namespace ATBM_Project.Views
{
    public class FormRevokePrivilege : Form
    {
        private Label lblGrantee;
        private TextBox txtGrantee;
        private TabControl tabControlPrivs;
        private TabPage tabGeneral;
        private TabPage tabColumn;
        private DataGridView dgvGeneral;
        private DataGridView dgvColumn;
        private Button btnRevoke;

        public FormRevokePrivilege()
        {
            InitializeComponent();
        }

        public FormRevokePrivilege(string granteeName) : this()
        {
            txtGrantee.Text = granteeName;
            LoadPrivileges(granteeName);
        }

        private void LoadPrivileges(string grantee)
        {
            try
            {
                PrivilegePresenter presenter = new PrivilegePresenter();
                var privileges = presenter.GetPrivileges(grantee);

                var generalPrivs = new System.Collections.Generic.List<PrivilegeInfo>();
                var colPrivs = new System.Collections.Generic.List<PrivilegeInfo>();

                foreach (var priv in privileges)
                {
                    if (priv.Type == "COLUMN") colPrivs.Add(priv);
                    else generalPrivs.Add(priv);
                }

                dgvGeneral.DataSource = generalPrivs;
                dgvColumn.DataSource = colPrivs;

                if (generalPrivs.Count > 0) dgvGeneral.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                if (colPrivs.Count > 0) dgvColumn.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách quyền: " + ex.Message);
            }
        }

        private void InitializeComponent()
        {
            this.lblGrantee = new System.Windows.Forms.Label();
            this.txtGrantee = new System.Windows.Forms.TextBox();
            this.btnRevoke = new System.Windows.Forms.Button();

            this.tabControlPrivs = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.tabColumn = new System.Windows.Forms.TabPage();
            this.dgvGeneral = new System.Windows.Forms.DataGridView();
            this.dgvColumn = new System.Windows.Forms.DataGridView();

            ((System.ComponentModel.ISupportInitialize)(this.dgvGeneral)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvColumn)).BeginInit();
            this.tabControlPrivs.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabColumn.SuspendLayout();
            this.SuspendLayout();

            // lblGrantee
            this.lblGrantee.Location = new System.Drawing.Point(20, 20);
            this.lblGrantee.Name = "lblGrantee";
            this.lblGrantee.Size = new System.Drawing.Size(100, 20);
            this.lblGrantee.Text = "Tên User / Role:";
            this.lblGrantee.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // txtGrantee
            this.txtGrantee.Location = new System.Drawing.Point(120, 20);
            this.txtGrantee.Name = "txtGrantee";
            this.txtGrantee.Size = new System.Drawing.Size(200, 22);
            this.txtGrantee.ReadOnly = true;

            // btnRevoke
            this.btnRevoke.Location = new System.Drawing.Point(540, 15);
            this.btnRevoke.Name = "btnRevoke";
            this.btnRevoke.Size = new System.Drawing.Size(120, 30);
            this.btnRevoke.Text = "Thu Hồi";
            this.btnRevoke.Click += new System.EventHandler(this.btnRevoke_Click);

            // tabControlPrivs
            this.tabControlPrivs.Controls.Add(this.tabGeneral);
            this.tabControlPrivs.Controls.Add(this.tabColumn);
            this.tabControlPrivs.Location = new System.Drawing.Point(20, 60);
            this.tabControlPrivs.Name = "tabControlPrivs";
            this.tabControlPrivs.SelectedIndex = 0;
            this.tabControlPrivs.Size = new System.Drawing.Size(640, 320);

            // tabGeneral
            this.tabGeneral.Controls.Add(this.dgvGeneral);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(632, 294);
            this.tabGeneral.Text = "Quyền Hệ Thống / Đối Tượng";
            this.tabGeneral.UseVisualStyleBackColor = true;

            // dgvGeneral
            this.dgvGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvGeneral.Name = "dgvGeneral";
            this.dgvGeneral.AllowUserToAddRows = false;
            this.dgvGeneral.AllowUserToDeleteRows = false;
            this.dgvGeneral.AllowUserToResizeColumns = false;
            this.dgvGeneral.AllowUserToResizeRows = false;
            this.dgvGeneral.ReadOnly = true;
            this.dgvGeneral.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvGeneral.MultiSelect = false;

            // tabColumn
            this.tabColumn.Controls.Add(this.dgvColumn);
            this.tabColumn.Location = new System.Drawing.Point(4, 22);
            this.tabColumn.Name = "tabColumn";
            this.tabColumn.Padding = new System.Windows.Forms.Padding(3);
            this.tabColumn.Size = new System.Drawing.Size(632, 294);
            this.tabColumn.Text = "Quyền Trên Cột";
            this.tabColumn.UseVisualStyleBackColor = true;

            // dgvColumn
            this.dgvColumn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvColumn.Name = "dgvColumn";
            this.dgvColumn.AllowUserToAddRows = false;
            this.dgvColumn.AllowUserToDeleteRows = false;
            this.dgvColumn.AllowUserToResizeColumns = false;
            this.dgvColumn.AllowUserToResizeRows = false;
            this.dgvColumn.ReadOnly = true;
            this.dgvColumn.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvColumn.MultiSelect = false;

            // FormRevokePrivilege
            this.ClientSize = new System.Drawing.Size(680, 400);
            this.Controls.Add(this.lblGrantee);
            this.Controls.Add(this.txtGrantee);
            this.Controls.Add(this.btnRevoke);
            this.Controls.Add(this.tabControlPrivs);
            this.Name = "FormRevokePrivilege";
            this.Text = "Danh Sách Các Quyền Đã Cấp - Thu Hồi";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

            ((System.ComponentModel.ISupportInitialize)(this.dgvGeneral)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvColumn)).EndInit();
            this.tabControlPrivs.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabColumn.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void btnRevoke_Click(object sender, EventArgs e)
        {
            DataGridView activeGrid = tabControlPrivs.SelectedTab == tabGeneral ? dgvGeneral : dgvColumn;

            if (activeGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một quyền từ danh sách để thu hồi.");
                return;
            }

            string grantee = txtGrantee.Text.Trim();
            DataGridViewRow row = activeGrid.SelectedRows[0];

            string privName = row.Cells["PrivilegeName"].Value?.ToString().Trim();
            string privType = row.Cells["Type"].Value?.ToString().Trim();
            string tableName = row.Cells["TableName"].Value?.ToString().Trim();

            if (string.IsNullOrEmpty(grantee) || string.IsNullOrEmpty(privName))
            {
                MessageBox.Show("Dữ liệu dòng chọn không hợp lệ.");
                return;
            }

            DialogResult confirm = MessageBox.Show($"Bạn có chắc muốn thu hồi quyền '{privName}' từ '{grantee}'?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    RevokePresenter presenter = new RevokePresenter();
                    if (presenter.RevokePrivilege(privName, privType, tableName, grantee))
                    {
                        MessageBox.Show("Thu hồi quyền thành công!");
                        LoadPrivileges(grantee);
                    }
                }
                catch (Exception ex)
                {
                    // Hiển thị nguyên chuỗi lệnh gặp lỗi từ Presenter
                    MessageBox.Show(ex.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}