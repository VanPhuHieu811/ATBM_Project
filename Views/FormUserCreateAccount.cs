using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Models;
using ATBM_Project.Presenters;

namespace ATBM_Project.Views
{
    public class FormUserCreateAccount : Form
    {
        private readonly UserPresenter presenter = new UserPresenter();

        private TabControl tabControl;
        private TabPage tabEmployees;
        private TabPage tabPatients;
        private DataGridView dgvEmployees;
        private DataGridView dgvPatients;
        private TextBox txtPassword;
        private Button btnCreate;
        private Button btnAddEmployee;
        private Button btnRefresh;
        private Button btnClose;
        private Label lblPassword;

        public FormUserCreateAccount()
        {
            InitializeComponent();
            LoadPendingAccounts();
        }

        private void InitializeComponent()
        {
            this.Text = "Tạo tài khoản Oracle";
            this.ClientSize = new Size(820, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.WhiteSmoke;

            Font headerFont = new Font("Segoe UI", 14F, FontStyle.Bold);
            Font labelFont = new Font("Segoe UI", 10F);

            Label lblTitle = new Label
            {
                Text = "Danh sách nhân viên / bệnh nhân chưa có tài khoản",
                Location = new Point(20, 15),
                AutoSize = true,
                Font = headerFont,
                ForeColor = Color.FromArgb(41, 53, 65)
            };

            tabControl = new TabControl
            {
                Location = new Point(20, 55),
                Size = new Size(780, 340),
                Font = labelFont
            };

            tabEmployees = new TabPage("Nhân viên");
            tabPatients = new TabPage("Bệnh nhân");

            dgvEmployees = CreateGrid();
            dgvPatients = CreateGrid();

            tabEmployees.Controls.Add(dgvEmployees);
            tabPatients.Controls.Add(dgvPatients);
            tabControl.TabPages.Add(tabEmployees);
            tabControl.TabPages.Add(tabPatients);

            lblPassword = new Label
            {
                Text = "Mật khẩu mặc định:",
                Location = new Point(20, 410),
                AutoSize = true,
                Font = labelFont
            };

            txtPassword = new TextBox
            {
                Location = new Point(160, 406),
                Width = 180,
                Font = labelFont,
                Text = "123"
            };

            btnCreate = CreateActionButton("Tạo tài khoản", 360, 402, 130);
            btnCreate.Click += BtnCreate_Click;

            btnAddEmployee = CreateActionButton("Thêm nhân viên mới", 500, 402, 150);
            btnAddEmployee.BackColor = Color.FromArgb(46, 125, 50);
            btnAddEmployee.Click += BtnAddEmployee_Click;

            btnRefresh = CreateActionButton("Làm mới", 660, 402, 70);
            btnRefresh.Click += (s, e) => LoadPendingAccounts();

            btnClose = CreateActionButton("Đóng", 740, 402, 60);
            btnClose.BackColor = Color.Gray;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(lblTitle);
            this.Controls.Add(tabControl);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnCreate);
            this.Controls.Add(btnAddEmployee);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnClose);
        }

        private DataGridView CreateGrid()
        {
            DataGridView grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 10F)
            };
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 53, 65);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.EnableHeadersVisualStyles = false;
            return grid;
        }

        private Button CreateActionButton(string text, int x, int y, int width)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 36),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }

        private void LoadPendingAccounts()
        {
            try
            {
                BindGrid(dgvEmployees, presenter.GetEmployeesWithoutAccount());
                BindGrid(dgvPatients, presenter.GetPatientsWithoutAccount());
                tabEmployees.Text = $"Nhân viên ({dgvEmployees.Rows.Count})";
                tabPatients.Text = $"Bệnh nhân ({dgvPatients.Rows.Count})";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được danh sách: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindGrid(DataGridView grid, List<PendingAccountItem> items)
        {
            grid.DataSource = null;
            grid.Columns.Clear();
            grid.AutoGenerateColumns = false;

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AccountId",
                HeaderText = "Mã",
                FillWeight = 80
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DisplayName",
                HeaderText = "Họ tên",
                FillWeight = 160
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "RoleOrType",
                HeaderText = "Vai trò / Loại",
                FillWeight = 120
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Gender",
                HeaderText = "Phái",
                FillWeight = 60
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "BirthDate",
                HeaderText = "Ngày sinh",
                FillWeight = 90
            });

            grid.DataSource = items;
        }

        private void BtnAddEmployee_Click(object sender, EventArgs e)
        {
            using (FormUserCreateEmployee createEmployeeForm = new FormUserCreateEmployee())
            {
                if (createEmployeeForm.ShowDialog(this) == DialogResult.OK)
                {
                    this.DialogResult = DialogResult.OK;
                    LoadPendingAccounts();
                }
            }
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            DataGridView activeGrid = tabControl.SelectedIndex == 0 ? dgvEmployees : dgvPatients;
            bool isEmployee = tabControl.SelectedIndex == 0;

            if (activeGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một dòng trong danh sách.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string accountId = activeGrid.SelectedRows[0].Cells[0].Value?.ToString();
            string displayName = activeGrid.SelectedRows[0].Cells[1].Value?.ToString();
            string roleOrType = activeGrid.SelectedRows[0].Cells[2].Value?.ToString();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string confirmMessage = isEmployee
                ? $"Tạo tài khoản Oracle cho nhân viên {accountId} - {displayName} ({roleOrType})?"
                : $"Tạo tài khoản Oracle cho bệnh nhân {accountId} - {displayName}?";

            if (MessageBox.Show(confirmMessage, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                if (isEmployee)
                {
                    presenter.CreateEmployeeAccount(accountId, password);
                }
                else
                {
                    presenter.CreatePatientAccount(accountId, password);
                }

                MessageBox.Show($"Đã tạo tài khoản {accountId.ToUpperInvariant()} thành công.", "Hoàn tất",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                LoadPendingAccounts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
