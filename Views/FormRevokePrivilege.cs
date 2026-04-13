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
        private DataGridView dgvPrivileges;
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
                dgvPrivileges.DataSource = privileges;

                if (privileges.Count > 0)
                {
                    dgvPrivileges.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
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
            this.dgvPrivileges = new System.Windows.Forms.DataGridView();
            this.btnRevoke = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.dgvPrivileges)).BeginInit();
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
            this.btnRevoke.Location = new System.Drawing.Point(380, 15);
            this.btnRevoke.Name = "btnRevoke";
            this.btnRevoke.Size = new System.Drawing.Size(120, 30);
            this.btnRevoke.Text = "Thu Hồi";
            this.btnRevoke.Click += new System.EventHandler(this.btnRevoke_Click);

            // dgvPrivileges
            this.dgvPrivileges.Location = new System.Drawing.Point(20, 60);
            this.dgvPrivileges.Name = "dgvPrivileges";
            this.dgvPrivileges.Size = new System.Drawing.Size(480, 280);
            this.dgvPrivileges.AllowUserToAddRows = false;
            this.dgvPrivileges.AllowUserToDeleteRows = false;
            this.dgvPrivileges.ReadOnly = true;
            this.dgvPrivileges.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPrivileges.MultiSelect = false;

            // FormRevokePrivilege
            this.ClientSize = new System.Drawing.Size(520, 360);
            this.Controls.Add(this.lblGrantee);
            this.Controls.Add(this.txtGrantee);
            this.Controls.Add(this.btnRevoke);
            this.Controls.Add(this.dgvPrivileges);
            this.Name = "FormRevokePrivilege";
            this.Text = "Danh Sách Các Quyền Đã Cấp - Thu Hồi";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

            ((System.ComponentModel.ISupportInitialize)(this.dgvPrivileges)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void btnRevoke_Click(object sender, EventArgs e)
        {
            if (dgvPrivileges.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một quyền từ danh sách để thu hồi.");
                return;
            }

            string grantee = txtGrantee.Text.Trim();
            DataGridViewRow row = dgvPrivileges.SelectedRows[0];

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