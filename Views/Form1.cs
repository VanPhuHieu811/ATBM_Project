using System;
using System.Windows.Forms;
using ATBM_Project.Presenters; // Đảm bảo đã có dòng này

namespace ATBM_Project.Views
{
    public partial class Form1 : Form
    {
        private Button btnUserLoad;
        private DataGridView dataGridView1;
        private Button btnShowRoles;
        private Button btnUserLoadPatient;
        private Button btnRevokePrivilege;

        public Form1()
        {
            InitializeComponent();
        }

        // Toàn bộ hàm btnUserLoad_Click phải nằm trong ngoặc nhọn của Class
        private void btnUserLoad_Click(object sender, EventArgs e)
        {
            try
            {
                AccountPresenter presenter = new AccountPresenter();
                dataGridView1.DataSource = presenter.GetUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnUserLoad = new System.Windows.Forms.Button();
            this.btnUserLoadPatient = new System.Windows.Forms.Button();
            this.btnShowRoles = new System.Windows.Forms.Button();
            this.btnRevokePrivilege = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(20, 80);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.Size = new System.Drawing.Size(630, 300);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            // 
            // btnUserLoad
            // 
            this.btnUserLoad.Location = new System.Drawing.Point(20, 20);
            this.btnUserLoad.Name = "btnUserLoad";
            this.btnUserLoad.Size = new System.Drawing.Size(150, 40);
            this.btnUserLoad.TabIndex = 0;
            this.btnUserLoad.Text = "Tải danh sách User";
            this.btnUserLoad.Click += new System.EventHandler(this.btnUserLoad_Click);
            // 
            // btnUserLoadPatient
            // 
            this.btnUserLoadPatient.Location = new System.Drawing.Point(500, 20);
            this.btnUserLoadPatient.Name = "btnUserLoadPatient";
            this.btnUserLoadPatient.Size = new System.Drawing.Size(150, 40);
            this.btnUserLoadPatient.TabIndex = 1;
            this.btnUserLoadPatient.Text = "btnUserLoadPatient";
            this.btnUserLoadPatient.UseVisualStyleBackColor = true;
            this.btnUserLoadPatient.Click += new System.EventHandler(this.btnUserLoadPatient_Click);
            // 
            // btnShowRoles
            // 
            this.btnShowRoles.Location = new System.Drawing.Point(217, 20);
            this.btnShowRoles.Name = "btnShowRoles";
            this.btnShowRoles.Size = new System.Drawing.Size(147, 40);
            this.btnShowRoles.TabIndex = 2;
            this.btnShowRoles.Text = "Danh sách vai trò";
            this.btnShowRoles.UseVisualStyleBackColor = true;
            this.btnShowRoles.Click += new System.EventHandler(this.btnShowRoles_Click);
            // 
            // btnRevokePrivilege
            // 
            this.btnRevokePrivilege.Location = new System.Drawing.Point(370, 20);
            this.btnRevokePrivilege.Name = "btnRevokePrivilege";
            this.btnRevokePrivilege.Size = new System.Drawing.Size(120, 40);
            this.btnRevokePrivilege.TabIndex = 3;
            this.btnRevokePrivilege.Text = "Thu Hồi";
            this.btnRevokePrivilege.UseVisualStyleBackColor = true;
            this.btnRevokePrivilege.Click += new System.EventHandler(this.btnRevokePrivilege_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(680, 420);
            this.Controls.Add(this.btnRevokePrivilege);
            this.Controls.Add(this.btnShowRoles);
            this.Controls.Add(this.btnUserLoadPatient);
            this.Controls.Add(this.btnUserLoad);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Form1";
            this.Text = "Quản trị Oracle - Phân hệ 1";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }
        private void btnUserLoadPatient_Click(object sender, EventArgs e)
        {
            try
            {
                BenhNhanPresenter presenter = new BenhNhanPresenter();
                var data = presenter.GetBenhNhanList();

                if (data.Count > 0)
                {
                    dataGridView1.DataSource = data;
                    // Tự động căn chỉnh độ rộng cột cho đẹp
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
                else
                {
                    MessageBox.Show("Dữ liệu trống. Hãy kiểm tra lại bảng BENHNHAN trong Oracle.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thực thi: " + ex.Message);
            }
        }

        private void btnShowRoles_Click(object sender, EventArgs e)
        {
            try
            {
                AccountPresenter presenter = new AccountPresenter();
                dataGridView1.DataSource = presenter.GetRoles();
                this.Text = "Danh sách vai trò (Roles) hệ thống";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnRevokePrivilege_Click(object sender, EventArgs e)
        {
            FormRevoke f = new FormRevoke();
            f.ShowDialog();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // To do: implement this if needed
            }
        }
    }
}