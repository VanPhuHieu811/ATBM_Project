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

        private Panel pnlSidebar;
        private Panel pnlContent;
        private Panel pnlListPage;
        private Panel pnlDetailPage;
        private Panel pnlSearch;
        private Panel pnlGridHost;
        private Panel pnlDetailToolbar;
        private Panel pnlDetailHost;

        private Label lblListTitle;
        private Label lblDetailTitle;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnRecords;
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
            InitializeComponent();
            LoadMedicalRecords();
            ShowListPage();
        }

        private void InitializeComponent()
        {
            this.Text = "Bác sĩ / Y sĩ - Quản lý hồ sơ bệnh án";
            this.ClientSize = new Size(1100, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.MinimumSize = new Size(900, 600);

            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.FromArgb(41, 53, 65)
            };

            Label lblSidebarTitle = new Label
            {
                AutoSize = false,
                Width = 220,
                Height = 70,
                Location = new Point(0, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.White,
                Text = "Bác sĩ / Y sĩ"
            };

            btnRecords = CreateSidebarButton("Hồ sơ bệnh án", 110);
            btnRecords.Click += (s, e) => ShowListPage();

            btnThongBao = CreateSidebarButton("Thông báo", 160);
            btnThongBao.Click += (s, e) => ShowNotificationPage();

            btnLogout = CreateSidebarButton("Đăng xuất", 0);
            btnLogout.Dock = DockStyle.Bottom;
            btnLogout.Height = 50;
            btnLogout.BackColor = Color.FromArgb(31, 43, 55);
            btnLogout.Click += (s, e) => this.Close();

            pnlSidebar.Controls.Add(lblSidebarTitle);
            pnlSidebar.Controls.Add(btnRecords);
            pnlSidebar.Controls.Add(btnThongBao);
            pnlSidebar.Controls.Add(btnLogout);

            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            BuildListPage();
            BuildDetailPage();

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlSidebar);
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
                Height = 48,
                Padding = new Padding(20, 0, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 53, 65),
                BackColor = Color.FromArgb(245, 247, 250)
            };

            pnlSearch = new Panel
            {
                Dock = DockStyle.Top,
                Height = 58,
                BackColor = Color.White,
                Padding = new Padding(16, 12, 16, 8)
            };

            TableLayoutPanel searchLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 1
            };
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 88F));
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96F));
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96F));

            Label lblSearch = new Label
            {
                Text = "Tìm kiếm bệnh nhân / hồ sơ:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(41, 53, 65)
            };

            txtSearch = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                Margin = new Padding(0, 6, 8, 6)
            };
            txtSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadMedicalRecords();
                    e.SuppressKeyPress = true;
                }
            };

            btnSearch = CreateActionButton("Tìm");
            btnSearch.Click += (s, e) => LoadMedicalRecords();

            btnClearSearch = CreateActionButton("Xóa lọc");
            btnClearSearch.Click += (s, e) =>
            {
                txtSearch.Clear();
                LoadMedicalRecords();
            };

            btnRefresh = CreateActionButton("Tải lại");
            btnRefresh.Click += (s, e) => LoadMedicalRecords();

            searchLayout.Controls.Add(lblSearch, 0, 0);
            searchLayout.Controls.Add(txtSearch, 1, 0);
            searchLayout.Controls.Add(btnSearch, 2, 0);
            searchLayout.Controls.Add(btnClearSearch, 3, 0);
            searchLayout.Controls.Add(btnRefresh, 4, 0);
            pnlSearch.Controls.Add(searchLayout);

            pnlGridHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(16, 8, 16, 16)
            };

            dgvMedicalRecords = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(230, 235, 240),
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
            dgvMedicalRecords.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
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

            pnlGridHost.Controls.Add(dgvMedicalRecords);

            pnlListPage.Controls.Add(pnlGridHost);
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
                Height = 54,
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(12, 8, 12, 8)
            };

            btnBack = CreateActionButton("← Quay lại");
            btnBack.Width = 120;
            btnBack.Dock = DockStyle.Left;
            btnBack.Margin = new Padding(0, 4, 0, 4);
            btnBack.Click += (s, e) => ShowListPage();

            lblDetailTitle = new Label
            {
                Text = "Chi tiết hồ sơ bệnh án",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0),
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 53, 65)
            };

            pnlDetailToolbar.Controls.Add(lblDetailTitle);
            pnlDetailToolbar.Controls.Add(btnBack);

            pnlDetailHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(8)
            };

            pnlDetailPage.Controls.Add(pnlDetailHost);
            pnlDetailPage.Controls.Add(pnlDetailToolbar);
        }

        private Button CreateSidebarButton(string text, int yPos)
        {
            Button btn = new Button
            {
                Text = "  " + text,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(0, yPos),
                Size = new Size(220, 48),
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.Gainsboro,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(51, 63, 75);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(61, 73, 85);
            return btn;
        }

        private Button CreateActionButton(string text)
        {
            Button button = new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                Margin = new Padding(4, 4, 0, 4),
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

            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(pnlDetailPage);
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
