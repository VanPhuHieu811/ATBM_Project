using System;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Models;
using ATBM_Project.Presenters;

namespace ATBM_Project.Views.DPV
{
    public class FormCoordinatorPatients : Form
    {
        private readonly CoordinatorPresenter presenter = new CoordinatorPresenter();
        private DataGridView dgvPatients;
        private TextBox txtSearch;
        private TextBox txtMaBN, txtTenBN, txtPhai, txtNgaySinh, txtCccd;
        private TextBox txtSoNha, txtTenDuong, txtQuanHuyen, txtTinhTp;
        private TextBox txtTienSu, txtTienSuGd, txtDiUng;

        public FormCoordinatorPatients()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.ClientSize = new Size(880, 650);
            this.BackColor = Color.WhiteSmoke;

            Label title = new Label
            {
                Text = "Quản lý bệnh nhân",
                Location = new Point(20, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 53, 65)
            };

            txtSearch = new TextBox { Location = new Point(20, 62), Width = 300, Font = new Font("Segoe UI", 10F) };
            Button btnSearch = CreateButton("Tìm", 330, 58, 80);
            btnSearch.Click += (s, e) => LoadData();
            Button btnRefresh = CreateButton("Làm mới", 420, 58, 100);
            btnRefresh.Click += (s, e) => { txtSearch.Clear(); LoadData(); };

            dgvPatients = new DataGridView
            {
                Location = new Point(20, 105),
                Size = new Size(820, 255),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvPatients.SelectionChanged += (s, e) => FillInputsFromSelection();

            GroupBox grp = new GroupBox
            {
                Text = "Thông tin bệnh nhân",
                Location = new Point(20, 375),
                Size = new Size(820, 210),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            txtMaBN = AddField(grp, "Mã BN", 20, 30, 95);
            txtTenBN = AddField(grp, "Họ tên", 290, 30, 170);
            txtPhai = AddField(grp, "Phái", 600, 30, 80);
            txtNgaySinh = AddField(grp, "Ngày sinh", 20, 70, 95);
            txtCccd = AddField(grp, "CCCD", 290, 70, 170);
            txtSoNha = AddField(grp, "Số nhà", 600, 70, 80);
            txtTenDuong = AddField(grp, "Tên đường", 20, 110, 220);
            txtQuanHuyen = AddField(grp, "Quận/Huyện", 410, 110, 120);
            txtTinhTp = AddField(grp, "Tỉnh/TP", 640, 110, 100);
            txtTienSu = AddField(grp, "Tiền sử", 20, 150, 220);
            txtTienSuGd = AddField(grp, "Tiền sử GĐ", 410, 150, 120);
            txtDiUng = AddField(grp, "Dị ứng", 640, 150, 100);

            Button btnAdd = CreateButton("Thêm", 20, 600, 100);
            btnAdd.Click += BtnAdd_Click;
            Button btnUpdate = CreateButton("Cập nhật", 135, 600, 110);
            btnUpdate.Click += BtnUpdate_Click;

            this.Controls.Add(title);
            this.Controls.Add(txtSearch);
            this.Controls.Add(btnSearch);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(dgvPatients);
            this.Controls.Add(grp);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnUpdate);
        }

        private TextBox AddField(Control parent, string label, int x, int y, int width)
        {
            Label lbl = new Label { Text = label + ":", Location = new Point(x, y + 3), Width = 85, Font = new Font("Segoe UI", 9F) };
            TextBox txt = new TextBox { Location = new Point(x + 90, y), Width = width, Font = new Font("Segoe UI", 9F) };
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
                dgvPatients.DataSource = presenter.GetPatients(txtSearch.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách bệnh nhân: " + ex.Message);
            }
        }

        private void FillInputsFromSelection()
        {
            if (dgvPatients.SelectedRows.Count == 0) return;
            DataGridViewRow row = dgvPatients.SelectedRows[0];
            txtMaBN.Text = Value(row, "MABN");
            txtTenBN.Text = Value(row, "TENBN");
            txtPhai.Text = Value(row, "PHAI");
            txtNgaySinh.Text = Value(row, "NGAYSINH");
            txtCccd.Text = Value(row, "CCCD");
            txtSoNha.Text = Value(row, "SONHA");
            txtTenDuong.Text = Value(row, "TENDUONG");
            txtQuanHuyen.Text = Value(row, "QUANHUYEN");
            txtTinhTp.Text = Value(row, "TINHTP");
            txtTienSu.Text = Value(row, "TIENSUBENH");
            txtTienSuGd.Text = Value(row, "TIENSUBENHGD");
            txtDiUng.Text = Value(row, "DIUNGTHUOC");
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ValidatePatientInput();
                presenter.AddPatient(BuildModel());
                MessageBox.Show("Đã thêm bệnh nhân.");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm bệnh nhân: " + ex.Message);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                ValidatePatientInput();
                presenter.UpdatePatient(BuildModel());
                MessageBox.Show("Đã cập nhật bệnh nhân.");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật bệnh nhân: " + ex.Message);
            }
        }

        private CoordinatorPatientModel BuildModel()
        {
            return new CoordinatorPatientModel
            {
                MaBN = txtMaBN.Text.Trim().ToUpperInvariant(),
                TenBN = txtTenBN.Text.Trim(),
                Phai = txtPhai.Text.Trim(),
                NgaySinh = txtNgaySinh.Text.Trim(),
                Cccd = txtCccd.Text.Trim(),
                SoNha = txtSoNha.Text.Trim(),
                TenDuong = txtTenDuong.Text.Trim(),
                QuanHuyen = txtQuanHuyen.Text.Trim(),
                TinhTp = txtTinhTp.Text.Trim(),
                TienSuBenh = txtTienSu.Text.Trim(),
                TienSuBenhGd = txtTienSuGd.Text.Trim(),
                DiUngThuoc = txtDiUng.Text.Trim()
            };
        }

        private void ValidatePatientInput()
        {
            if (string.IsNullOrWhiteSpace(txtMaBN.Text) ||
                string.IsNullOrWhiteSpace(txtTenBN.Text) ||
                string.IsNullOrWhiteSpace(txtPhai.Text) ||
                string.IsNullOrWhiteSpace(txtNgaySinh.Text) ||
                string.IsNullOrWhiteSpace(txtCccd.Text))
            {
                throw new Exception("Vui lòng nhập Mã BN, Họ tên, Phái, Ngày sinh và CCCD.");
            }
        }

        private string Value(DataGridViewRow row, string column)
        {
            return row.DataGridView.Columns.Contains(column) && row.Cells[column].Value != null
                ? row.Cells[column].Value.ToString()
                : string.Empty;
        }
    }
}
