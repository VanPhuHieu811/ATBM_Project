using System;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Models;
using ATBM_Project.Presenters;

namespace ATBM_Project.Views.DPV
{
    public class FormCoordinatorRecords : Form
    {
        private readonly CoordinatorPresenter presenter = new CoordinatorPresenter();
        private DataGridView dgvRecords;
        private DataGridView dgvDoctors;
        private TextBox txtSearch;
        private TextBox txtMaHSBA, txtMaBN, txtNgay, txtChanDoan, txtDieuTri, txtMaBS, txtMaKhoa, txtKetLuan;

        public FormCoordinatorRecords()
        {
            InitializeComponent();
            LoadData();
            LoadDoctors();
        }

        private void InitializeComponent()
        {
            this.ClientSize = new Size(880, 650);
            this.BackColor = Color.WhiteSmoke;

            Label title = new Label
            {
                Text = "Điều phối hồ sơ bệnh án",
                Location = new Point(20, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 53, 65)
            };

            txtSearch = new TextBox { Location = new Point(20, 62), Width = 300, Font = new Font("Segoe UI", 10F) };
            Button btnSearch = CreateButton("Tìm", 330, 58, 80);
            btnSearch.Click += (s, e) => LoadData();
            Button btnRefresh = CreateButton("Làm mới", 420, 58, 100);
            btnRefresh.Click += (s, e) =>
            {
                txtSearch.Clear();
                LoadData();
                LoadDoctors();
                ClearInputs();
                SetCreateMode();
            };

            dgvRecords = CreateGrid(20, 105, 820, 250);
            dgvRecords.SelectionChanged += (s, e) => FillInputsFromSelection();

            GroupBox grp = new GroupBox
            {
                Text = "Thông tin HSBA",
                Location = new Point(20, 370),
                Size = new Size(510, 205),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            txtMaHSBA = AddField(grp, "Mã HSBA", 15, 30, 120);
            txtMaBN = AddField(grp, "Mã BN", 255, 30, 120);
            txtNgay = AddField(grp, "Ngày", 15, 70, 120);
            txtMaKhoa = AddField(grp, "Mã khoa", 255, 70, 120);
            txtMaBS = AddField(grp, "Mã BS", 15, 110, 120);
            txtChanDoan = AddField(grp, "Chẩn đoán", 255, 110, 120);
            txtDieuTri = AddField(grp, "Điều trị", 15, 150, 120);
            txtKetLuan = AddField(grp, "Kết luận", 255, 150, 120);
            SetCreateMode();

            GroupBox grpDoctors = new GroupBox
            {
                Text = "Bác sĩ/Y sĩ",
                Location = new Point(545, 370),
                Size = new Size(295, 205),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            dgvDoctors = CreateGrid(10, 25, 275, 170);
            dgvDoctors.SelectionChanged += (s, e) =>
            {
                if (dgvDoctors.SelectedRows.Count == 0) return;
                txtMaBS.Text = Value(dgvDoctors.SelectedRows[0], "MANV");
                string khoa = Value(dgvDoctors.SelectedRows[0], "CHUYENKHOA");
                if (!string.IsNullOrWhiteSpace(khoa)) txtMaKhoa.Text = khoa;
            };
            grpDoctors.Controls.Add(dgvDoctors);

            Button btnAdd = CreateButton("Tạo HSBA", 20, 595, 115);
            btnAdd.Click += BtnAdd_Click;
            Button btnAssign = CreateButton("Cập nhật phân công", 150, 595, 165);
            btnAssign.Click += BtnAssign_Click;

            this.Controls.Add(title);
            this.Controls.Add(txtSearch);
            this.Controls.Add(btnSearch);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(dgvRecords);
            this.Controls.Add(grp);
            this.Controls.Add(grpDoctors);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnAssign);
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

        private TextBox AddField(Control parent, string label, int x, int y, int width)
        {
            Label lbl = new Label { Text = label + ":", Location = new Point(x, y + 3), Width = 80, Font = new Font("Segoe UI", 9F) };
            TextBox txt = new TextBox { Location = new Point(x + 85, y), Width = width, Font = new Font("Segoe UI", 9F) };
            parent.Controls.Add(lbl);
            parent.Controls.Add(txt);
            return txt;
        }

        private Button CreateButton(string text, int x, int y, int width)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 34),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void LoadData()
        {
            try
            {
                dgvRecords.DataSource = presenter.GetMedicalRecords(txtSearch.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải HSBA: " + ex.Message);
            }
        }

        private void LoadDoctors()
        {
            try
            {
                dgvDoctors.DataSource = presenter.GetDoctors();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách bác sĩ/y sĩ: " + ex.Message);
            }
        }

        private void FillInputsFromSelection()
        {
            if (dgvRecords.SelectedRows.Count == 0) return;
            DataGridViewRow row = dgvRecords.SelectedRows[0];
            txtMaHSBA.Text = Value(row, "MAHSBA");
            txtMaBN.Text = Value(row, "MABN");
            txtNgay.Text = Value(row, "NGAY");
            txtChanDoan.Text = Value(row, "CHANDOAN");
            txtDieuTri.Text = Value(row, "DIEUTRI");
            txtMaBS.Text = Value(row, "MABS");
            txtMaKhoa.Text = Value(row, "MAKHOA");
            txtKetLuan.Text = Value(row, "KETLUAN");
            SetAssignmentMode();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SetCreateMode();
                ValidateRecordInput(requireAssignment: false);
                presenter.AddMedicalRecord(BuildModel());
                MessageBox.Show("Đã tạo hồ sơ bệnh án.");
                LoadData();
                ClearInputs();
                SetCreateMode();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo HSBA: " + ex.Message);
            }
        }

        private void BtnAssign_Click(object sender, EventArgs e)
        {
            try
            {
                ValidateRecordInput(requireAssignment: true);
                presenter.UpdateMedicalRecordAssignment(BuildModel());
                MessageBox.Show("Đã cập nhật mã khoa và bác sĩ phụ trách.");
                LoadData();
                SetAssignmentMode();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật phân công: " + ex.Message);
            }
        }

        private CoordinatorMedicalRecordModel BuildModel()
        {
            return new CoordinatorMedicalRecordModel
            {
                MaHSBA = txtMaHSBA.Text.Trim().ToUpperInvariant(),
                MaBN = txtMaBN.Text.Trim().ToUpperInvariant(),
                Ngay = txtNgay.Text.Trim(),
                ChanDoan = txtChanDoan.Text.Trim(),
                DieuTri = txtDieuTri.Text.Trim(),
                MaBS = txtMaBS.Text.Trim().ToUpperInvariant(),
                MaKhoa = txtMaKhoa.Text.Trim(),
                KetLuan = txtKetLuan.Text.Trim()
            };
        }

        private void ValidateRecordInput(bool requireAssignment)
        {
            if (string.IsNullOrWhiteSpace(txtMaHSBA.Text))
            {
                throw new Exception("Vui lòng nhập mã HSBA.");
            }

            if (!requireAssignment &&
                (string.IsNullOrWhiteSpace(txtMaBN.Text) || string.IsNullOrWhiteSpace(txtNgay.Text)))
            {
                throw new Exception("Vui lòng nhập mã bệnh nhân và ngày lập HSBA.");
            }

            if (requireAssignment &&
                (string.IsNullOrWhiteSpace(txtMaKhoa.Text) || string.IsNullOrWhiteSpace(txtMaBS.Text)))
            {
                throw new Exception("Vui lòng nhập mã khoa và mã bác sĩ/y sĩ.");
            }
        }

        private string Value(DataGridViewRow row, string column)
        {
            return row.DataGridView.Columns.Contains(column) && row.Cells[column].Value != null
                ? row.Cells[column].Value.ToString()
                : string.Empty;
        }

        private void SetCreateMode()
        {
            SetEditable(txtMaHSBA, true);
            SetEditable(txtMaBN, true);
            SetEditable(txtNgay, true);
            SetEditable(txtChanDoan, true);
            SetEditable(txtDieuTri, true);
            SetEditable(txtKetLuan, true);
            SetEditable(txtMaBS, true);
            SetEditable(txtMaKhoa, true);
        }

        private void SetAssignmentMode()
        {
            SetEditable(txtMaHSBA, false);
            SetEditable(txtMaBN, false);
            SetEditable(txtNgay, false);
            SetEditable(txtChanDoan, false);
            SetEditable(txtDieuTri, false);
            SetEditable(txtKetLuan, false);
            SetEditable(txtMaBS, true);
            SetEditable(txtMaKhoa, true);
        }

        private void SetEditable(TextBox textBox, bool editable)
        {
            textBox.ReadOnly = !editable;
            textBox.BackColor = editable ? Color.White : SystemColors.Control;
            textBox.TabStop = editable;
        }

        private void ClearInputs()
        {
            txtMaHSBA.Clear();
            txtMaBN.Clear();
            txtNgay.Clear();
            txtChanDoan.Clear();
            txtDieuTri.Clear();
            txtMaBS.Clear();
            txtMaKhoa.Clear();
            txtKetLuan.Clear();
        }
    }
}
