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
            this.ClientSize = new Size(1000, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F)
            };

            tabControl.TabPages.Add(BuildRecordTab());
            tabControl.TabPages.Add(BuildServiceTab());
            tabControl.TabPages.Add(BuildPrescriptionTab());
            tabControl.TabPages.Add(BuildPatientTab());

            this.Controls.Add(tabControl);
        }

        private TabPage BuildRecordTab()
        {
            TabPage page = new TabPage("Hồ sơ");

            txtMaHsba = AddTextField(page, "Mã HSBA", 30, 30, true);
            txtMaBn = AddTextField(page, "Mã bệnh nhân", 30, 75, true);
            txtNgay = AddTextField(page, "Ngày", 30, 120, true);
            txtMaBs = AddTextField(page, "Mã bác sĩ", 30, 165, true);
            txtMaKhoa = AddTextField(page, "Mã khoa", 30, 210, true);

            txtChanDoan = AddTextArea(page, "Chẩn đoán", 420, 30, false);
            txtDieuTri = AddTextArea(page, "Điều trị", 420, 155, false);
            txtKetLuan = AddTextArea(page, "Kết luận", 420, 280, false);

            btnSaveRecord = CreateButton("Lưu hồ sơ", 420, 420);
            btnSaveRecord.Click += BtnSaveRecord_Click;
            page.Controls.Add(btnSaveRecord);

            return page;
        }

        private TabPage BuildServiceTab()
        {
            TabPage page = new TabPage("Dịch vụ");

            txtServiceType = AddInlineTextBox(page, "Loại dịch vụ", 20, 20, 230);
            txtServiceTech = AddInlineTextBox(page, "Mã KTV", 270, 20, 130);
            txtServiceResult = AddInlineTextBox(page, "Kết quả", 420, 20, 280);

            btnAddService = CreateButton("Thêm", 725, 20);
            btnAddService.Click += BtnAddService_Click;

            btnDeleteService = CreateButton("Xóa dòng", 855, 20);
            btnDeleteService.Click += BtnDeleteService_Click;

            dgvServices = CreateGrid(20, 92, 940, 400);

            page.Controls.Add(btnAddService);
            page.Controls.Add(btnDeleteService);
            page.Controls.Add(dgvServices);

            return page;
        }

        private TabPage BuildPrescriptionTab()
        {
            TabPage page = new TabPage("Đơn thuốc");

            txtPrescriptionName = AddInlineTextBox(page, "Tên thuốc", 20, 20, 240);
            txtPrescriptionDose = AddInlineTextBox(page, "Liều dùng", 280, 20, 300);

            btnAddPrescription = CreateButton("Thêm", 610, 45);
            btnAddPrescription.Click += BtnAddPrescription_Click;

            btnUpdatePrescription = CreateButton("Sửa liều", 740, 45);
            btnUpdatePrescription.Click += BtnUpdatePrescription_Click;

            btnDeletePrescription = CreateButton("Xóa", 870, 45);
            btnDeletePrescription.Click += BtnDeletePrescription_Click;

            dgvPrescriptions = CreateGrid(20, 100, 940, 392);
            dgvPrescriptions.SelectionChanged += (s, e) => FillPrescriptionInputsFromSelection();

            page.Controls.Add(btnAddPrescription);
            page.Controls.Add(btnUpdatePrescription);
            page.Controls.Add(btnDeletePrescription);
            page.Controls.Add(dgvPrescriptions);

            return page;
        }

        private TabPage BuildPatientTab()
        {
            TabPage page = new TabPage("Bệnh nhân");

            txtBnMa = AddTextField(page, "Mã BN", 30, 30, true);
            txtBnTen = AddTextField(page, "Tên BN", 30, 75, true);
            txtBnPhai = AddTextField(page, "Phái", 30, 120, true);
            txtBnNgaySinh = AddTextField(page, "Ngày sinh", 30, 165, true);
            txtBnCccd = AddTextField(page, "CCCD", 30, 210, true);
            txtBnDiaChi = AddTextArea(page, "Địa chỉ", 30, 255, true);
            txtBnDiaChi.Size = new Size(340, 76);

            txtTienSuBenh = AddTextArea(page, "Tiền sử bệnh", 420, 30, false);
            txtTienSuBenhGd = AddTextArea(page, "Tiền sử bệnh GĐ", 420, 155, false);
            txtDiUngThuoc = AddTextArea(page, "Dị ứng thuốc", 420, 270, false);

            btnSavePatient = CreateButton("Lưu bệnh nhân", 420, 390);
            btnSavePatient.Click += BtnSavePatient_Click;
            page.Controls.Add(btnSavePatient);

            return page;
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

        private TextBox AddTextField(Control parent, string labelText, int x, int y, bool readOnly)
        {
            Label label = new Label { Text = labelText, Location = new Point(x, y), Size = new Size(120, 24), Font = new Font("Segoe UI", 9F) };
            TextBox textBox = new TextBox { Location = new Point(x + 130, y), Size = new Size(210, 24), ReadOnly = readOnly, Font = new Font("Segoe UI", 9F) };
            parent.Controls.Add(label);
            parent.Controls.Add(textBox);
            return textBox;
        }

        private TextBox AddTextArea(Control parent, string labelText, int x, int y, bool readOnly)
        {
            Label label = new Label { Text = labelText, Location = new Point(x, y), Size = new Size(150, 24), Font = new Font("Segoe UI", 9F) };
            TextBox textBox = new TextBox
            {
                Location = new Point(x, y + 28),
                Size = new Size(520, 76),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = readOnly,
                Font = new Font("Segoe UI", 9F)
            };
            parent.Controls.Add(label);
            parent.Controls.Add(textBox);
            return textBox;
        }

        private TextBox AddInlineTextBox(Control parent, string labelText, int x, int y, int width)
        {
            Label label = new Label { Text = labelText, Location = new Point(x, y), Size = new Size(width, 22), Font = new Font("Segoe UI", 9F) };
            TextBox textBox = new TextBox { Location = new Point(x, y + 25), Size = new Size(width, 24), Font = new Font("Segoe UI", 9F) };
            parent.Controls.Add(label);
            parent.Controls.Add(textBox);
            return textBox;
        }

        private DataGridView CreateGrid(int x, int y, int width, int height)
        {
            DataGridView grid = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            return grid;
        }

        private Button CreateButton(string text, int x, int y)
        {
            Button button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(120, 34),
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
