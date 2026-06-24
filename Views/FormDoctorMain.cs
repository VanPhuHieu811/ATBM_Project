using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Data;
using ATBM_Project.Presenters;

namespace ATBM_Project.Views
{
    public class FormDoctorMain : Form
    {
        private readonly DoctorPresenter presenter = new DoctorPresenter();
        private readonly string displayName;

        private Panel pnlHeader;
        private Panel pnlContent;
        private Panel pnlListPage;
        private Panel pnlDetailPage;
        private Panel pnlSearch;
        private Panel pnlDetailToolbar;
        private Panel pnlDetailHost;

        private Label lblTitle;
        private Label lblUser;
        private Label lblListTitle;
        private Label lblDetailTitle;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnThongBao;
        private Button btnClearSearch;
        private Button btnRefresh;
        private Button btnLogout;
        private Button btnBack;
        private DataGridView dgvMedicalRecords;
        private FormDoctorRecordDetail currentDetailForm;
        private FormThongBao currentThongBaoForm;

        public FormDoctorMain(string displayName)
        {
            this.displayName = string.IsNullOrWhiteSpace(displayName) ? DBConfig.User : displayName;
            InitializeComponent();
            LoadMedicalRecords();
            ShowListPage();
        }

        private void InitializeComponent()
        {
            this.Text = "Bác sĩ / Y sĩ";
            this.ClientSize = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 86,
                BackColor = Color.White
            };

            lblTitle = new Label
            {
                Text = "Bác sĩ / Y sĩ - Quản lý hồ sơ bệnh án",
                Location = new Point(20, 18),
                Size = new Size(560, 32),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 53, 65)
            };

            lblUser = new Label
            {
                Text = $"{displayName} ({DBConfig.User?.ToUpperInvariant()})",
                Location = new Point(20, 54),
                Size = new Size(560, 24),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.DimGray
            };

            btnRefresh = CreateButton("Tải lại", 760, 27, 100);
            btnRefresh.Click += (s, e) => LoadMedicalRecords();

            btnLogout = CreateButton("Đăng xuất", 875, 27, 100);
            btnLogout.Click += (s, e) => this.Close();

            btnThongBao = CreateButton("Thông báo", 635, 27, 100);
            btnThongBao.Click += (s, e) => ShowNotificationPage();

            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.WhiteSmoke
            };

            BuildListPage();
            BuildDetailPage();

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblUser);
            pnlHeader.Controls.Add(btnRefresh);
            pnlHeader.Controls.Add(btnLogout);
            pnlHeader.Controls.Add(btnThongBao);

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlHeader);
        }

        private void BuildListPage()
        {
            pnlListPage = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            lblListTitle = new Label
            {
                Text = "Danh sách hồ sơ bệnh án",
                Dock = DockStyle.Top,
                Height = 45,
                Padding = new Padding(20, 0, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 53, 65),
                BackColor = Color.FromArgb(245, 247, 250)
            };

            pnlSearch = new Panel
            {
                Dock = DockStyle.Top,
                Height = 62,
                BackColor = Color.White
            };

            Label lblSearch = new Label
            {
                Text = "Tìm kiếm bệnh nhân / hồ sơ:",
                Location = new Point(20, 20),
                Size = new Size(180, 24),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(41, 53, 65)
            };

            txtSearch = new TextBox
            {
                Location = new Point(210, 18),
                Size = new Size(360, 24),
                Font = new Font("Segoe UI", 10F)
            };
            txtSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadMedicalRecords();
                    e.SuppressKeyPress = true;
                }
            };

            btnSearch = CreateButton("Tìm", 590, 14, 80);
            btnSearch.Click += (s, e) => LoadMedicalRecords();

            btnClearSearch = CreateButton("Xóa lọc", 680, 14, 90);
            btnClearSearch.Click += (s, e) =>
            {
                txtSearch.Clear();
                LoadMedicalRecords();
            };

            dgvMedicalRecords = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoGenerateColumns = false
            };
            dgvMedicalRecords.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvMedicalRecords.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 248);
            dgvMedicalRecords.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvMedicalRecords.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvMedicalRecords.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 53, 65);
            dgvMedicalRecords.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvMedicalRecords.EnableHeadersVisualStyles = false;
            dgvMedicalRecords.CellContentClick += DgvMedicalRecords_CellContentClick;
            dgvMedicalRecords.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    NavigateToDetailFromRow(e.RowIndex);
                }
            };
            ConfigureMedicalRecordColumns();

            pnlSearch.Controls.Add(lblSearch);
            pnlSearch.Controls.Add(txtSearch);
            pnlSearch.Controls.Add(btnSearch);
            pnlSearch.Controls.Add(btnClearSearch);

            pnlListPage.Controls.Add(dgvMedicalRecords);
            pnlListPage.Controls.Add(pnlSearch);
            pnlListPage.Controls.Add(lblListTitle);
        }

        private void BuildDetailPage()
        {
            pnlDetailPage = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Visible = false
            };

            pnlDetailToolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 58,
                BackColor = Color.FromArgb(245, 247, 250)
            };

            btnBack = CreateButton("← Quay lại", 20, 12, 120);
            btnBack.Click += (s, e) => ShowListPage();

            lblDetailTitle = new Label
            {
                Text = "Chi tiết hồ sơ bệnh án",
                Location = new Point(160, 10),
                Size = new Size(600, 35),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 53, 65)
            };

            pnlDetailToolbar.Controls.Add(btnBack);
            pnlDetailToolbar.Controls.Add(lblDetailTitle);

            pnlDetailHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            pnlDetailPage.Controls.Add(pnlDetailHost);
            pnlDetailPage.Controls.Add(pnlDetailToolbar);
        }

        private Button CreateButton(string text, int x, int y, int width)
        {
            Button button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 35),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private void LoadMedicalRecords()
        {
            try
            {
                DataTable data = presenter.GetMedicalRecords(txtSearch?.Text);
                dgvMedicalRecords.DataSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được danh sách HSBA: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureMedicalRecordColumns()
        {
            dgvMedicalRecords.Columns.Clear();

            DataGridViewButtonColumn detailColumn = new DataGridViewButtonColumn
            {
                Name = "DETAIL_ACTION",
                HeaderText = "Thao tác",
                Text = "Xem chi tiết",
                UseColumnTextForButtonValue = true,
                Width = 120,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };
            dgvMedicalRecords.Columns.Add(detailColumn);

            AddTextColumn("MAHSBA", "Mã HSBA", 90);
            AddTextColumn("MABN", "Mã BN", 90);
            AddTextColumn("TENBN", "Tên bệnh nhân", 260);
            AddTextColumn("NGAY", "Ngày", 100, "dd/MM/yyyy");
            AddTextColumn("MAKHOA", "Mã khoa", 90);
        }

        private void DgvMedicalRecords_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (dgvMedicalRecords.Columns[e.ColumnIndex].Name == "DETAIL_ACTION")
            {
                NavigateToDetailFromRow(e.RowIndex);
            }
        }

        private void NavigateToDetailFromRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dgvMedicalRecords.Rows.Count)
            {
                return;
            }

            string maHsba = dgvMedicalRecords.Rows[rowIndex].Cells["MAHSBA"].Value?.ToString();
            string maBn = dgvMedicalRecords.Rows[rowIndex].Cells["MABN"].Value?.ToString();
            string tenBn = dgvMedicalRecords.Rows[rowIndex].Cells["TENBN"].Value?.ToString();
            if (string.IsNullOrWhiteSpace(maHsba))
            {
                return;
            }

            ShowDetailPage(maHsba, maBn, tenBn);
        }

        private void ShowDetailPage(string maHsba, string maBn, string tenBn)
        {
            if (currentDetailForm != null)
            {
                pnlDetailHost.Controls.Remove(currentDetailForm);
                currentDetailForm.Close();
                currentDetailForm.Dispose();
                currentDetailForm = null;
            }

            currentDetailForm = new FormDoctorRecordDetail(maHsba)
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill
            };

            lblDetailTitle.Text = $"Chi tiết HSBA {maHsba} - {maBn} - {tenBn}";
            pnlDetailHost.Controls.Clear();
            pnlDetailHost.Controls.Add(currentDetailForm);
            currentDetailForm.Show();

            pnlListPage.Visible = false;
            pnlDetailPage.Visible = true;
            pnlDetailPage.BringToFront();
        }

        private void ShowListPage()
        {
            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(pnlListPage);
            pnlContent.Controls.Add(pnlDetailPage);
            pnlDetailPage.Visible = false;
            pnlListPage.Visible = true;
            pnlListPage.BringToFront();
            LoadMedicalRecords();
        }

        private void ShowNotificationPage()
        {
            if (currentThongBaoForm == null || currentThongBaoForm.IsDisposed)
            {
                currentThongBaoForm = new FormThongBao(DBConfig.ConnectionString)
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };
            }

            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(currentThongBaoForm);
            currentThongBaoForm.BringToFront();
            currentThongBaoForm.Show();
        }

        private void AddTextColumn(string name, string headerText, int width, string format = null)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn
            {
                Name = name,
                DataPropertyName = name,
                HeaderText = headerText,
                Width = width,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };

            if (!string.IsNullOrWhiteSpace(format))
            {
                column.DefaultCellStyle.Format = format;
            }

            dgvMedicalRecords.Columns.Add(column);
        }
    }
}
