using System;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Models;
using ATBM_Project.Presenters;

namespace ATBM_Project.Views.DPV
{
    public class FormCoordinatorServices : Form
    {
        private readonly CoordinatorPresenter presenter = new CoordinatorPresenter();
        private DataGridView dgvServices;
        private DataGridView dgvTechnicians;
        private TextBox txtSearch;
        private TextBox txtMaHSBA, txtLoaiDV, txtNgayDV, txtMaKTV, txtKetQua;

        public FormCoordinatorServices()
        {
            InitializeComponent();
            LoadData();
            LoadTechnicians();
        }

        private void InitializeComponent()
        {
            this.ClientSize = new Size(880, 650);
            this.BackColor = Color.WhiteSmoke;

            Label title = new Label
            {
                Text = "Điều phối kỹ thuật viên thực hiện dịch vụ",
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
                LoadTechnicians();
                ClearInputs();
            };

            dgvServices = CreateGrid(20, 105, 820, 270);
            dgvServices.SelectionChanged += (s, e) => FillInputsFromSelection();

            GroupBox grp = new GroupBox
            {
                Text = "Dịch vụ hỗ trợ chẩn đoán",
                Location = new Point(20, 390),
                Size = new Size(510, 155),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            txtMaHSBA = AddField(grp, "Mã HSBA", 15, 30, 120);
            txtLoaiDV = AddField(grp, "Loại DV", 255, 30, 120);
            txtNgayDV = AddField(grp, "Ngày DV", 15, 70, 120);
            txtMaKTV = AddField(grp, "Mã KTV", 255, 70, 120);
            txtKetQua = AddField(grp, "Kết quả", 15, 110, 360);
            SetAssignmentMode();

            GroupBox grpTech = new GroupBox
            {
                Text = "Kỹ thuật viên",
                Location = new Point(545, 390),
                Size = new Size(295, 155),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            dgvTechnicians = CreateGrid(10, 25, 275, 120);
            dgvTechnicians.SelectionChanged += (s, e) =>
            {
                if (dgvTechnicians.SelectedRows.Count == 0) return;
                txtMaKTV.Text = Value(dgvTechnicians.SelectedRows[0], "MANV");
            };
            grpTech.Controls.Add(dgvTechnicians);

            Button btnAssign = CreateButton("Cập nhật KTV", 20, 565, 135);
            btnAssign.Click += BtnAssign_Click;

            this.Controls.Add(title);
            this.Controls.Add(txtSearch);
            this.Controls.Add(btnSearch);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(dgvServices);
            this.Controls.Add(grp);
            this.Controls.Add(grpTech);
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
                dgvServices.DataSource = presenter.GetServices(txtSearch.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dịch vụ: " + ex.Message);
            }
        }

        private void LoadTechnicians()
        {
            try
            {
                dgvTechnicians.DataSource = presenter.GetTechnicians();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải kỹ thuật viên: " + ex.Message);
            }
        }

        private void FillInputsFromSelection()
        {
            if (dgvServices.SelectedRows.Count == 0) return;
            DataGridViewRow row = dgvServices.SelectedRows[0];
            txtMaHSBA.Text = Value(row, "MAHSBA");
            txtLoaiDV.Text = Value(row, "LOAIDV");
            txtNgayDV.Text = Value(row, "NGAYDV");
            txtMaKTV.Text = Value(row, "MAKTV");
            txtKetQua.Text = Value(row, "KETQUA");
        }

        private void BtnAssign_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMaHSBA.Text) ||
                    string.IsNullOrWhiteSpace(txtLoaiDV.Text) ||
                    string.IsNullOrWhiteSpace(txtNgayDV.Text) ||
                    string.IsNullOrWhiteSpace(txtMaKTV.Text))
                {
                    throw new Exception("Vui lòng chọn dịch vụ và nhập/chọn mã kỹ thuật viên.");
                }

                presenter.UpdateServiceTechnician(new CoordinatorServiceModel
                {
                    MaHSBA = txtMaHSBA.Text.Trim(),
                    LoaiDV = txtLoaiDV.Text.Trim(),
                    NgayDV = txtNgayDV.Text.Trim(),
                    MaKTV = txtMaKTV.Text.Trim().ToUpperInvariant()
                });

                MessageBox.Show("Đã cập nhật kỹ thuật viên thực hiện dịch vụ.");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật kỹ thuật viên: " + ex.Message);
            }
        }

        private string Value(DataGridViewRow row, string column)
        {
            return row.DataGridView.Columns.Contains(column) && row.Cells[column].Value != null
                ? row.Cells[column].Value.ToString()
                : string.Empty;
        }

        private void SetAssignmentMode()
        {
            SetEditable(txtMaHSBA, false);
            SetEditable(txtLoaiDV, false);
            SetEditable(txtNgayDV, false);
            SetEditable(txtKetQua, false);
            SetEditable(txtMaKTV, true);
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
            txtLoaiDV.Clear();
            txtNgayDV.Clear();
            txtMaKTV.Clear();
            txtKetQua.Clear();
        }
    }
}
