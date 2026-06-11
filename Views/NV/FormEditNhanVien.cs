using System;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Presenters;
using ATBM_Project.Models;

namespace ATBM_Project.Views.NV
{
    public class FormEditNhanVien : Form
    {
        private TextBox txtManV, txtHoTen, txtVaiTro, txtSoDt, txtQueQuan;
        private Button btnSave, btnCancel;
        private NhanVienPresenter _presenter;
        private NhanVienModel _model;

        public FormEditNhanVien(NhanVienModel model, NhanVienPresenter presenter)
        {
            _model = model;
            _presenter = presenter;
            InitializeComponent();
            FillData();
        }

        private void InitializeComponent()
        {
            this.Text = "Cập nhật thông tin liên lạc nhân viên";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Font font = new Font("Segoe UI", 10F);
            int lx = 30, tx = 150, tw = 280;

            this.Controls.Add(new Label() { Text = "Mã nhân viên:", Location = new Point(lx, 30), AutoSize = true, Font = font });
            txtManV = new TextBox() { Location = new Point(tx, 27), Width = tw, Font = font, ReadOnly = true };

            this.Controls.Add(new Label() { Text = "Họ và tên:", Location = new Point(lx, 75), AutoSize = true, Font = font });
            txtHoTen = new TextBox() { Location = new Point(tx, 72), Width = tw, Font = font, ReadOnly = true };

            this.Controls.Add(new Label() { Text = "Vai trò:", Location = new Point(lx, 120), AutoSize = true, Font = font });
            txtVaiTro = new TextBox() { Location = new Point(tx, 117), Width = tw, Font = font, ReadOnly = true };

            // Các ô được phép chỉnh sửa
            this.Controls.Add(new Label() { Text = "Số điện thoại:", Location = new Point(lx, 165), AutoSize = true, Font = font });
            txtSoDt = new TextBox() { Location = new Point(tx, 162), Width = tw, Font = font };

            this.Controls.Add(new Label() { Text = "Quên quán:", Location = new Point(lx, 210), AutoSize = true, Font = font });
            txtQueQuan = new TextBox() { Location = new Point(tx, 207), Width = tw, Height = 60, Multiline = true, Font = font };

            // Hệ thống nút bấm
            btnSave = new Button() { Text = "LƯU THAY ĐỔI", Location = new Point(150, 295), Size = new Size(130, 40), BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button() { Text = "Hủy bỏ", Location = new Point(295, 295), Size = new Size(100, 40), FlatStyle = FlatStyle.Flat, Font = font, Cursor = Cursors.Hand };
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { txtManV, txtHoTen, txtVaiTro, txtSoDt, txtQueQuan, btnSave, btnCancel });
        }

        private void FillData()
        {
            txtManV.Text = _model.ManV;
            txtHoTen.Text = _model.HoTen;
            txtVaiTro.Text = _model.VaiTro;
            txtSoDt.Text = _model.SoDt;
            txtQueQuan.Text = _model.QueQuan;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _model.SoDt = txtSoDt.Text.Trim();
                _model.QueQuan = txtQueQuan.Text.Trim();

                if (string.IsNullOrEmpty(_model.SoDt))
                {
                    MessageBox.Show("Số điện thoại không được bỏ trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_presenter.UpdateProfile(_model))
                {
                    MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Oracle: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}