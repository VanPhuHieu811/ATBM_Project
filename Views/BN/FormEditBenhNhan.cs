using System;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Presenters;
using ATBM_Project.Models;

namespace ATBM_Project.Views.BN
{
    public class FormEditBenhNhan : Form
    {
        private TextBox txtMaBN, txtTenBN, txtCccd, txtSoNha, txtTenDuong, txtQuanHuyen, txtTinhTp, txtTienSu, txtTienSuGD, txtDiUng;
        private Button btnSave, btnCancel;
        private BenhNhanPresenter _presenter;
        private BenhNhanModel _model;

        public FormEditBenhNhan(BenhNhanModel model, BenhNhanPresenter presenter)
        {
            _model = model;
            _presenter = presenter;
            InitializeComponent();
            FillData();
        }

        private void InitializeComponent()
        {
            this.Text = "Cập nhật thông tin cá nhân bệnh nhân";
            this.Size = new Size(700, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Font font = new Font("Segoe UI", 10F);
            int lx1 = 30, tx1 = 140, tw = 180;
            int lx2 = 360, tx2 = 480;

            // Cột 1: Thông tin cố định (ReadOnly)
            this.Controls.Add(new Label() { Text = "Mã bệnh nhân:", Location = new Point(lx1, 30), AutoSize = true, Font = font });
            txtMaBN = new TextBox() { Location = new Point(tx1, 27), Width = tw, Font = font, ReadOnly = true };

            this.Controls.Add(new Label() { Text = "Họ và tên:", Location = new Point(lx1, 75), AutoSize = true, Font = font });
            txtTenBN = new TextBox() { Location = new Point(tx1, 72), Width = tw, Font = font, ReadOnly = true };

            this.Controls.Add(new Label() { Text = "Số CCCD:", Location = new Point(lx1, 120), AutoSize = true, Font = font });
            txtCccd = new TextBox() { Location = new Point(tx1, 117), Width = tw, Font = font, ReadOnly = true };

            // Cột 2: Thông tin địa chỉ (Cho phép sửa)
            this.Controls.Add(new Label() { Text = "Số nhà:", Location = new Point(lx2, 30), AutoSize = true, Font = font });
            txtSoNha = new TextBox() { Location = new Point(tx2, 27), Width = tw, Font = font };

            this.Controls.Add(new Label() { Text = "Tên đường:", Location = new Point(lx2, 75), AutoSize = true, Font = font });
            txtTenDuong = new TextBox() { Location = new Point(tx2, 72), Width = tw, Font = font };

            this.Controls.Add(new Label() { Text = "Quận / Huyện:", Location = new Point(lx2, 120), AutoSize = true, Font = font });
            txtQuanHuyen = new TextBox() { Location = new Point(tx2, 117), Width = tw, Font = font };

            this.Controls.Add(new Label() { Text = "Tỉnh / TP:", Location = new Point(lx2, 165), AutoSize = true, Font = font });
            txtTinhTp = new TextBox() { Location = new Point(tx2, 162), Width = tw, Font = font };

            // Hàng dưới: Tiền sử bệnh và Dị ứng (Multiline textboxes rộng rãi)
            this.Controls.Add(new Label() { Text = "Tiền sử bệnh lý:", Location = new Point(lx1, 215), AutoSize = true, Font = font });
            txtTienSu = new TextBox() { Location = new Point(tx1, 212), Width = 500, Height = 60, Multiline = true, Font = font };

            this.Controls.Add(new Label() { Text = "Tiền sử bệnh GD:", Location = new Point(lx1, 290), AutoSize = true, Font = font });
            txtTienSuGD = new TextBox() { Location = new Point(tx1, 287), Width = 500, Height = 60, Multiline = true, Font = font };

            this.Controls.Add(new Label() { Text = "Dị ứng thuốc:", Location = new Point(lx1, 365), AutoSize = true, Font = font });
            txtDiUng = new TextBox() { Location = new Point(tx1, 362), Width = 500, Height = 60, Multiline = true, Font = font };

            // Nút bấm cứu dữ liệu
            btnSave = new Button() { Text = "LƯU THÔNG TIN", Location = new Point(200, 450), Size = new Size(140, 40), BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button() { Text = "Hủy bỏ", Location = new Point(360, 450), Size = new Size(100, 40), FlatStyle = FlatStyle.Flat, Font = font, Cursor = Cursors.Hand };
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { txtMaBN, txtTenBN, txtCccd, txtSoNha, txtTenDuong, txtQuanHuyen, txtTinhTp, txtTienSu, txtTienSuGD, txtDiUng, btnSave, btnCancel });
        }

        private void FillData()
        {
            txtMaBN.Text = _model.MaBN;
            txtTenBN.Text = _model.TenBN;
            txtCccd.Text = _model.Cccd;
            txtSoNha.Text = _model.SoNha;
            txtTenDuong.Text = _model.TenDuong;
            txtQuanHuyen.Text = _model.QuanHuyen;
            txtTinhTp.Text = _model.TinhTp;
            txtTienSu.Text = _model.TienSuBenh;
            txtTienSuGD.Text = _model.TienSuBenhGd;
            txtDiUng.Text = _model.DiUngThuoc;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Thu thập thông tin chỉnh sửa đưa vào Model
                _model.SoNha = txtSoNha.Text.Trim();
                _model.TenDuong = txtTenDuong.Text.Trim();
                _model.QuanHuyen = txtQuanHuyen.Text.Trim();
                _model.TinhTp = txtTinhTp.Text.Trim();
                _model.TienSuBenh = txtTienSu.Text.Trim();
                _model.TienSuBenhGd = txtTienSuGD.Text.Trim();
                _model.DiUngThuoc = txtDiUng.Text.Trim();

                bool success = _presenter.UpdateProfile(_model);
                if (success)
                {
                    MessageBox.Show("Cập nhật thông tin cá nhân thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại. Hãy kiểm tra lại phân quyền hệ thống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Oracle: " + ex.Message, "Lỗi thực thi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}