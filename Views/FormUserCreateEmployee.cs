using System;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Models;
using ATBM_Project.Presenters;

namespace ATBM_Project.Views
{
    public class FormUserCreateEmployee : Form
    {
        private readonly UserPresenter presenter = new UserPresenter();

        private TextBox txtManv;
        private TextBox txtHoTen;
        private ComboBox cboPhai;
        private TextBox txtNgaySinh;
        private TextBox txtCmnd;
        private TextBox txtQueQuan;
        private TextBox txtSoDt;
        private ComboBox cboVaiTro;
        private TextBox txtChuyenKhoa;
        private TextBox txtPassword;

        public FormUserCreateEmployee()
        {
            InitializeComponent();
            txtManv.Text = presenter.SuggestNextEmployeeId();
        }

        private void InitializeComponent()
        {
            this.Text = "Thêm nhân viên mới";
            this.ClientSize = new Size(520, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.WhiteSmoke;

            Font labelFont = new Font("Segoe UI", 10F);
            Font headerFont = new Font("Segoe UI", 13F, FontStyle.Bold);

            Label lblTitle = new Label
            {
                Text = "Nhập thông tin nhân viên và tạo tài khoản",
                Location = new Point(20, 15),
                AutoSize = true,
                Font = headerFont,
                ForeColor = Color.FromArgb(41, 53, 65)
            };

            GroupBox grp = new GroupBox
            {
                Text = "Thông tin nhân viên",
                Location = new Point(20, 50),
                Size = new Size(470, 340),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            txtManv = AddField(grp, "Mã NV", 20, 30, 120);
            Button btnSuggest = new Button
            {
                Text = "Gợi ý mã",
                Location = new Point(320, 28),
                Size = new Size(90, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Font = labelFont,
                Cursor = Cursors.Hand
            };
            btnSuggest.Click += (s, e) => txtManv.Text = presenter.SuggestNextEmployeeId();
            grp.Controls.Add(btnSuggest);

            txtHoTen = AddField(grp, "Họ tên", 20, 70, 390);
            cboPhai = AddComboField(grp, "Phái", 20, 110, 120, new[] { "Nam", "Nữ" });
            txtNgaySinh = AddField(grp, "Ngày sinh", 260, 110, 150);
            Label lblDateHint = new Label
            {
                Text = "(DD/MM/YYYY)",
                Location = new Point(350, 133),
                AutoSize = true,
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.Gray
            };
            grp.Controls.Add(lblDateHint);

            txtCmnd = AddField(grp, "CMND/CCCD", 20, 150, 390);
            txtQueQuan = AddField(grp, "Quê quán", 20, 190, 390);
            txtSoDt = AddField(grp, "Số ĐT", 20, 230, 180);
            cboVaiTro = AddComboField(grp, "Vai trò", 260, 230, 150,
                new[] { "Điều phối viên", "Bác sĩ/Y sĩ", "Kỹ thuật viên" });
            txtChuyenKhoa = AddField(grp, "Chuyên khoa", 20, 270, 390);

            lblPassword = new Label
            {
                Text = "Mật khẩu tài khoản:",
                Location = new Point(20, 405),
                AutoSize = true,
                Font = labelFont
            };
            txtPassword = new TextBox
            {
                Location = new Point(170, 401),
                Width = 150,
                Font = labelFont,
                Text = "123"
            };

            Button btnSave = CreateActionButton("Lưu và tạo tài khoản", 20, 445, 200);
            btnSave.Click += BtnSave_Click;

            Button btnCancel = CreateActionButton("Hủy", 240, 445, 100);
            btnCancel.BackColor = Color.Gray;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lblTitle);
            this.Controls.Add(grp);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private Label lblPassword;

        private TextBox AddField(Control parent, string label, int x, int y, int width)
        {
            Label lbl = new Label
            {
                Text = label + ":",
                Location = new Point(x, y + 3),
                Width = 95,
                Font = new Font("Segoe UI", 9F)
            };
            TextBox txt = new TextBox
            {
                Location = new Point(x + 100, y),
                Width = width,
                Font = new Font("Segoe UI", 9F)
            };
            parent.Controls.Add(lbl);
            parent.Controls.Add(txt);
            return txt;
        }

        private ComboBox AddComboField(Control parent, string label, int x, int y, int width, string[] items)
        {
            Label lbl = new Label
            {
                Text = label + ":",
                Location = new Point(x, y + 3),
                Width = 95,
                Font = new Font("Segoe UI", 9F)
            };
            ComboBox cbo = new ComboBox
            {
                Location = new Point(x + 100, y),
                Width = width,
                Font = new Font("Segoe UI", 9F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbo.Items.AddRange(items);
            if (items.Length > 0)
            {
                cbo.SelectedIndex = 0;
            }
            parent.Controls.Add(lbl);
            parent.Controls.Add(cbo);
            return cbo;
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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ValidateInput();

                NhanVienModel model = new NhanVienModel
                {
                    ManV = txtManv.Text.Trim(),
                    HoTen = txtHoTen.Text.Trim(),
                    Phai = cboPhai.SelectedItem?.ToString(),
                    NgaySinh = txtNgaySinh.Text.Trim(),
                    Cmnd = txtCmnd.Text.Trim(),
                    QueQuan = txtQueQuan.Text.Trim(),
                    SoDt = txtSoDt.Text.Trim(),
                    VaiTro = cboVaiTro.SelectedItem?.ToString(),
                    ChuyenKhoa = string.IsNullOrWhiteSpace(txtChuyenKhoa.Text) ? null : txtChuyenKhoa.Text.Trim()
                };

                string password = txtPassword.Text;
                string manv = model.ManV.ToUpperInvariant();

                if (MessageBox.Show(
                    $"Thêm nhân viên {manv} - {model.HoTen} và tạo tài khoản Oracle?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                presenter.CreateEmployeeWithDataAndAccount(model, password);

                MessageBox.Show(
                    $"Đã thêm nhân viên {manv} và tạo tài khoản thành công.",
                    "Hoàn tất",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtManv.Text))
            {
                throw new Exception("Vui lòng nhập mã nhân viên.");
            }

            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                throw new Exception("Vui lòng nhập họ tên.");
            }

            if (cboPhai.SelectedIndex < 0)
            {
                throw new Exception("Vui lòng chọn phái.");
            }

            if (string.IsNullOrWhiteSpace(txtNgaySinh.Text))
            {
                throw new Exception("Vui lòng nhập ngày sinh (DD/MM/YYYY).");
            }

            if (string.IsNullOrWhiteSpace(txtCmnd.Text))
            {
                throw new Exception("Vui lòng nhập CMND/CCCD.");
            }

            if (cboVaiTro.SelectedIndex < 0)
            {
                throw new Exception("Vui lòng chọn vai trò.");
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                throw new Exception("Vui lòng nhập mật khẩu tài khoản.");
            }
        }
    }
}
