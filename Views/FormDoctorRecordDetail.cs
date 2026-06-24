using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Presenters;

namespace ATBM_Project.Views
{
    public class FormDoctorRecordDetail : Form
    {
        private readonly string maHsba;
        private readonly DoctorPresenter presenter = new DoctorPresenter();

        private TabControl tabControl;
        private TextBox txtMaHsba, txtMaBn, txtNgay, txtMaBs, txtMaKhoa;
        private TextBox txtChanDoan, txtDieuTri, txtKetLuan;
        private Button btnSaveRecord;

        private DataGridView dgvServices;
        private TextBox txtServiceType, txtServiceTech, txtServiceResult;
        private Button btnAddService, btnDeleteService;

        private DataGridView dgvPrescriptions;
        private TextBox txtPrescriptionName, txtPrescriptionDose;
        private Button btnAddPrescription, btnUpdatePrescription, btnDeletePrescription;

        private TextBox txtBnMa, txtBnTen, txtBnPhai, txtBnNgaySinh, txtBnCccd;
        private TextBox txtBnDiaChi, txtTienSuBenh, txtTienSuBenhGd, txtDiUngThuoc;
        private Button btnSavePatient;

        public FormDoctorRecordDetail(string maHsba)
        {
            this.maHsba = maHsba;
            InitializeComponent();
            LoadAllData();
        }

        private void InitializeComponent()
        {
            this.Text = $"Chi tiết HSBA - {maHsba}";
            this.BackColor = Color.White;

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                Padding = new Point(12, 6)
            };

            tabControl.TabPages.Add(BuildRecordTab());
            tabControl.TabPages.Add(BuildServiceTab());
            tabControl.TabPages.Add(BuildPrescriptionTab());
            tabControl.TabPages.Add(BuildPatientTab());

            this.Controls.Add(tabControl);
        }

        private TabPage BuildRecordTab()
        {
            TabPage page = new TabPage("Hồ sơ")
            {
                BackColor = Color.White,
                Padding = new Padding(12)
            };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            Panel pnlLeft = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 4, 12, 0) };
            txtMaHsba = AddStackedField(pnlLeft, "Mã HSBA", true, 0);
            txtMaBn = AddStackedField(pnlLeft, "Mã bệnh nhân", true, 1);
            txtNgay = AddStackedField(pnlLeft, "Ngày", true, 2);
            txtMaBs = AddStackedField(pnlLeft, "Mã bác sĩ", true, 3);
            txtMaKhoa = AddStackedField(pnlLeft, "Mã khoa", true, 4);

            Panel pnlRight = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4, 0, 0, 0) };
            TableLayoutPanel rightLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4
            };
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.34F));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));

            txtChanDoan = AddDockedTextArea(rightLayout, "Chẩn đoán", false, 0);
            txtDieuTri = AddDockedTextArea(rightLayout, "Điều trị", false, 1);
            txtKetLuan = AddDockedTextArea(rightLayout, "Kết luận", false, 2);

            Panel pnlSave = new Panel { Dock = DockStyle.Fill };
            btnSaveRecord = CreateButton("Lưu hồ sơ");
            btnSaveRecord.Dock = DockStyle.Right;
            btnSaveRecord.Width = 130;
            btnSaveRecord.Click += BtnSaveRecord_Click;
            pnlSave.Controls.Add(btnSaveRecord);
            rightLayout.Controls.Add(pnlSave, 0, 3);

            pnlRight.Controls.Add(rightLayout);

            layout.Controls.Add(pnlLeft, 0, 0);
            layout.Controls.Add(pnlRight, 1, 0);
            page.Controls.Add(layout);

            return page;
        }

        private TabPage BuildServiceTab()
        {
            TabPage page = new TabPage("Dịch vụ")
            {
                BackColor = Color.White,
                Padding = new Padding(12)
            };

            Panel pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 88,
                Padding = new Padding(0, 0, 0, 8)
            };

            TableLayoutPanel topLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 2
            };
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 240F));
            topLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            topLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            AddInlineField(topLayout, "Loại dịch vụ", out txtServiceType, 0, 0);
            AddInlineField(topLayout, "Mã KTV", out txtServiceTech, 1, 0);
            AddInlineField(topLayout, "Kết quả", out txtServiceResult, 2, 0);

            Panel pnlButtons = new Panel { Dock = DockStyle.Fill };
            btnAddService = CreateButton("Thêm");
            btnAddService.Location = new Point(0, 4);
            btnAddService.Width = 110;
            btnAddService.Click += BtnAddService_Click;

            btnDeleteService = CreateButton("Xóa dòng");
            btnDeleteService.Location = new Point(118, 4);
            btnDeleteService.Width = 110;
            btnDeleteService.Click += BtnDeleteService_Click;

            pnlButtons.Controls.Add(btnAddService);
            pnlButtons.Controls.Add(btnDeleteService);
            topLayout.Controls.Add(pnlButtons, 3, 0);
            topLayout.SetRowSpan(pnlButtons, 2);

            pnlTop.Controls.Add(topLayout);

            dgvServices = CreateGrid();
            dgvServices.Dock = DockStyle.Fill;

            page.Controls.Add(dgvServices);
            page.Controls.Add(pnlTop);

            return page;
        }

        private TabPage BuildPrescriptionTab()
        {
            TabPage page = new TabPage("Đơn thuốc")
            {
                BackColor = Color.White,
                Padding = new Padding(12)
            };

            Panel pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 88,
                Padding = new Padding(0, 0, 0, 8)
            };

            TableLayoutPanel topLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 2
            };
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 360F));
            topLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            topLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            AddInlineField(topLayout, "Tên thuốc", out txtPrescriptionName, 0, 0);
            AddInlineField(topLayout, "Liều dùng", out txtPrescriptionDose, 1, 0);

            Panel pnlButtons = new Panel { Dock = DockStyle.Fill };
            btnAddPrescription = CreateButton("Thêm");
            btnAddPrescription.Location = new Point(0, 4);
            btnAddPrescription.Width = 100;
            btnAddPrescription.Click += BtnAddPrescription_Click;

            btnUpdatePrescription = CreateButton("Sửa liều");
            btnUpdatePrescription.Location = new Point(108, 4);
            btnUpdatePrescription.Width = 110;
            btnUpdatePrescription.Click += BtnUpdatePrescription_Click;

            btnDeletePrescription = CreateButton("Xóa");
            btnDeletePrescription.Location = new Point(226, 4);
            btnDeletePrescription.Width = 100;
            btnDeletePrescription.Click += BtnDeletePrescription_Click;

            pnlButtons.Controls.Add(btnAddPrescription);
            pnlButtons.Controls.Add(btnUpdatePrescription);
            pnlButtons.Controls.Add(btnDeletePrescription);
            topLayout.Controls.Add(pnlButtons, 2, 0);
            topLayout.SetRowSpan(pnlButtons, 2);

            pnlTop.Controls.Add(topLayout);

            dgvPrescriptions = CreateGrid();
            dgvPrescriptions.Dock = DockStyle.Fill;
            dgvPrescriptions.SelectionChanged += (s, e) => FillPrescriptionInputsFromSelection();

            page.Controls.Add(dgvPrescriptions);
            page.Controls.Add(pnlTop);

            return page;
        }

        private TabPage BuildPatientTab()
        {
            TabPage page = new TabPage("Bệnh nhân")
            {
                BackColor = Color.White,
                Padding = new Padding(12)
            };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 320F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            Panel pnlLeft = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 4, 12, 0) };
            txtBnMa = AddStackedField(pnlLeft, "Mã BN", true, 0);
            txtBnTen = AddStackedField(pnlLeft, "Tên BN", true, 1);
            txtBnPhai = AddStackedField(pnlLeft, "Phái", true, 2);
            txtBnNgaySinh = AddStackedField(pnlLeft, "Ngày sinh", true, 3);
            txtBnCccd = AddStackedField(pnlLeft, "CCCD", true, 4);
            txtBnDiaChi = AddStackedField(pnlLeft, "Địa chỉ", true, 5, multiline: true, height: 72);

            Panel pnlRight = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4, 0, 0, 0) };
            TableLayoutPanel rightLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4
            };
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.34F));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));

            txtTienSuBenh = AddDockedTextArea(rightLayout, "Tiền sử bệnh", false, 0);
            txtTienSuBenhGd = AddDockedTextArea(rightLayout, "Tiền sử bệnh GĐ", false, 1);
            txtDiUngThuoc = AddDockedTextArea(rightLayout, "Dị ứng thuốc", false, 2);

            Panel pnlSave = new Panel { Dock = DockStyle.Fill };
            btnSavePatient = CreateButton("Lưu bệnh nhân");
            btnSavePatient.Dock = DockStyle.Right;
            btnSavePatient.Width = 140;
            btnSavePatient.Click += BtnSavePatient_Click;
            pnlSave.Controls.Add(btnSavePatient);
            rightLayout.Controls.Add(pnlSave, 0, 3);

            pnlRight.Controls.Add(rightLayout);

            layout.Controls.Add(pnlLeft, 0, 0);
            layout.Controls.Add(pnlRight, 1, 0);
            page.Controls.Add(layout);

            return page;
        }

        private TextBox AddStackedField(Panel parent, string labelText, bool readOnly, int index, bool multiline = false, int height = 28)
        {
            int top = 4 + index * (multiline ? height + 34 : 52);

            Label label = new Label
            {
                Text = labelText,
                Location = new Point(0, top),
                Size = new Size(120, 22),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 53, 65)
            };

            TextBox textBox = new TextBox
            {
                Location = new Point(0, top + 24),
                Size = new Size(parent.ClientSize.Width > 0 ? parent.ClientSize.Width - 4 : 280, height),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = readOnly,
                Multiline = multiline,
                ScrollBars = multiline ? ScrollBars.Vertical : ScrollBars.None,
                BackColor = readOnly ? Color.FromArgb(245, 247, 250) : Color.White,
                Font = new Font("Segoe UI", 10F)
            };

            parent.Controls.Add(label);
            parent.Controls.Add(textBox);
            return textBox;
        }

        private TextBox AddDockedTextArea(TableLayoutPanel parent, string labelText, bool readOnly, int row)
        {
            Panel wrapper = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 0, 0, 8)
            };

            Label label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Top,
                Height = 24,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 53, 65)
            };

            TextBox textBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = readOnly,
                BackColor = readOnly ? Color.FromArgb(245, 247, 250) : Color.White,
                Font = new Font("Segoe UI", 10F)
            };

            wrapper.Controls.Add(textBox);
            wrapper.Controls.Add(label);
            parent.Controls.Add(wrapper, 0, row);
            return textBox;
        }

        private void AddInlineField(TableLayoutPanel parent, string labelText, out TextBox textBox, int column, int row)
        {
            Panel wrapper = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 0, 8, 0)
            };

            Label label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Top,
                Height = 22,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 53, 65)
            };

            textBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F)
            };

            wrapper.Controls.Add(textBox);
            wrapper.Controls.Add(label);
            parent.Controls.Add(wrapper, column, row);
            parent.SetRowSpan(wrapper, 2);
        }

        private void LoadAllData()
        {
            LoadRecord();
            LoadServices();
            LoadPrescriptions();
            LoadPatient();
        }

        private void LoadRecord()
        {
            DataTable data = presenter.GetMedicalRecord(maHsba);
            if (data.Rows.Count == 0)
            {
                MessageBox.Show("Không tìm thấy hồ sơ bệnh án hoặc không có quyền xem.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            DataRow row = data.Rows[0];
            txtMaHsba.Text = Value(row, "MAHSBA");
            txtMaBn.Text = Value(row, "MABN");
            txtNgay.Text = FormatDate(row, "NGAY");
            txtChanDoan.Text = Value(row, "CHANDOAN");
            txtDieuTri.Text = Value(row, "DIEUTRI");
            txtMaBs.Text = Value(row, "MABS");
            txtMaKhoa.Text = Value(row, "MAKHOA");
            txtKetLuan.Text = Value(row, "KETLUAN");
        }

        private void LoadServices()
        {
            dgvServices.DataSource = presenter.GetServices(maHsba);
            FormatGrid(dgvServices);
        }

        private void LoadPrescriptions()
        {
            dgvPrescriptions.DataSource = presenter.GetPrescriptions(maHsba);
            FormatGrid(dgvPrescriptions);
        }

        private void LoadPatient()
        {
            DataTable data = presenter.GetPatientByMedicalRecord(maHsba);
            if (data.Rows.Count == 0)
            {
                return;
            }

            DataRow row = data.Rows[0];
            txtBnMa.Text = Value(row, "MABN");
            txtBnTen.Text = Value(row, "TENBN");
            txtBnPhai.Text = Value(row, "PHAI");
            txtBnNgaySinh.Text = FormatDate(row, "NGAYSINH");
            txtBnCccd.Text = Value(row, "CCCD");
            txtBnDiaChi.Text = $"{Value(row, "SONHA")} {Value(row, "TENDUONG")}, {Value(row, "QUANHUYEN")}, {Value(row, "TINHTP")}".Trim();
            txtTienSuBenh.Text = Value(row, "TIENSUBENH");
            txtTienSuBenhGd.Text = Value(row, "TIENSUBENHGD");
            txtDiUngThuoc.Text = Value(row, "DIUNGTHUOC");
        }

        private void BtnSaveRecord_Click(object sender, EventArgs e)
        {
            try
            {
                presenter.UpdateMedicalRecord(maHsba, txtChanDoan.Text, txtDieuTri.Text, txtKetLuan.Text);
                MessageBox.Show("Đã lưu hồ sơ.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadRecord();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void BtnAddService_Click(object sender, EventArgs e)
        {
            string loaiDv = txtServiceType.Text.Trim();
            if (string.IsNullOrWhiteSpace(loaiDv))
            {
                MessageBox.Show("Vui lòng nhập loại dịch vụ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                presenter.AddService(maHsba, loaiDv, DateTime.Today, txtServiceTech.Text, txtServiceResult.Text);
                txtServiceType.Clear();
                txtServiceTech.Clear();
                txtServiceResult.Clear();
                LoadServices();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void BtnDeleteService_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = GetSelectedRow(dgvServices);
            if (row == null) return;

            if (MessageBox.Show("Xóa dịch vụ đã chọn?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                presenter.DeleteService(Value(row, "MAHSBA"), Value(row, "LOAIDV"), ToDate(row, "NGAYDV"));
                LoadServices();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void BtnAddPrescription_Click(object sender, EventArgs e)
        {
            string tenThuoc = txtPrescriptionName.Text.Trim();
            if (string.IsNullOrWhiteSpace(tenThuoc))
            {
                MessageBox.Show("Vui lòng nhập tên thuốc.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                presenter.AddPrescription(maHsba, DateTime.Today, tenThuoc, txtPrescriptionDose.Text);
                txtPrescriptionName.Clear();
                txtPrescriptionDose.Clear();
                LoadPrescriptions();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void BtnUpdatePrescription_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = GetSelectedRow(dgvPrescriptions);
            if (row == null) return;

            string oldDose = Value(row, "LIEUDUNG");
            string newDose = txtPrescriptionDose.Text.Trim();
            if (string.IsNullOrWhiteSpace(newDose) || newDose == oldDose) return;

            try
            {
                presenter.UpdatePrescriptionDose(Value(row, "MAHSBA"), ToDate(row, "NGAYDT"), Value(row, "TENTHUOC"), newDose);
                LoadPrescriptions();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void FillPrescriptionInputsFromSelection()
        {
            if (dgvPrescriptions.SelectedRows.Count == 0 || txtPrescriptionName == null || txtPrescriptionDose == null)
            {
                return;
            }

            DataGridViewRow row = dgvPrescriptions.SelectedRows[0];
            txtPrescriptionName.Text = Value(row, "TENTHUOC");
            txtPrescriptionDose.Text = Value(row, "LIEUDUNG");
        }

        private void BtnDeletePrescription_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = GetSelectedRow(dgvPrescriptions);
            if (row == null) return;

            if (MessageBox.Show("Xóa thuốc đã chọn?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                presenter.DeletePrescription(Value(row, "MAHSBA"), ToDate(row, "NGAYDT"), Value(row, "TENTHUOC"));
                LoadPrescriptions();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void BtnSavePatient_Click(object sender, EventArgs e)
        {
            try
            {
                presenter.UpdatePatientHistory(txtBnMa.Text, txtTienSuBenh.Text, txtTienSuBenhGd.Text, txtDiUngThuoc.Text);
                MessageBox.Show("Đã lưu thông tin bệnh nhân.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadPatient();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private DataGridView CreateGrid()
        {
            DataGridView grid = new DataGridView
            {
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 53, 65);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.EnableHeadersVisualStyles = false;
            return grid;
        }

        private Button CreateButton(string text)
        {
            Button button = new Button
            {
                Text = text,
                Height = 34,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private DataGridViewRow GetSelectedRow(DataGridView grid)
        {
            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một dòng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            return grid.SelectedRows[0];
        }

        private string Value(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value ? row[columnName].ToString() : string.Empty;
        }

        private string Value(DataGridViewRow row, string columnName)
        {
            return row.DataGridView.Columns.Contains(columnName) && row.Cells[columnName].Value != null ? row.Cells[columnName].Value.ToString() : string.Empty;
        }

        private string FormatDate(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
            {
                return string.Empty;
            }

            return Convert.ToDateTime(row[columnName]).ToString("dd/MM/yyyy");
        }

        private DateTime ToDate(DataGridViewRow row, string columnName)
        {
            object value = row.Cells[columnName].Value;
            return Convert.ToDateTime(value);
        }

        private void FormatGrid(DataGridView grid)
        {
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
