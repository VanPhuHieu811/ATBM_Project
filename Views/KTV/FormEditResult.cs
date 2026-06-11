using ATBM_Project.Models;
using ATBM_Project.Presenters;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ATBM_Project.Views
{
    public class FormEditResult : Form
    {
        private TextBox txtMaHSBA, txtLoaiDV, txtNgayDV, txtKetQua;
        private Button btnSave, btnCancel;
        private KTVPresenter _presenter;
        private KTVServiceModel _model;

        public FormEditResult(KTVServiceModel model, KTVPresenter presenter)
        {
            _model = model;
            _presenter = presenter;
            InitializeComponent();
            FillData();
        }

        private void InitializeComponent()
        {
            this.Text = "Cập nhật kết quả dịch vụ";

            // THAY ĐỔI: Thu nhỏ kích thước Form lại cho vừa vặn
            this.ClientSize = new Size(420, 310);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false; // Ẩn luôn nút thu nhỏ cho đúng chuẩn Popup

            Font font = new Font("Segoe UI", 10F);
            int labelX = 35;
            int inputX = 145;
            int inputWidth = 235;

            // Nhãn và ô nhập Mã HSBA
            Label lblMa = new Label() { Text = "Mã HSBA:", Location = new Point(labelX, 30), AutoSize = true, Font = font };
            txtMaHSBA = new TextBox() { Location = new Point(inputX, 27), Width = inputWidth, Font = font, ReadOnly = true, BackColor = Color.WhiteSmoke };

            // Nhãn và ô nhập Loại dịch vụ
            Label lblLoai = new Label() { Text = "Loại dịch vụ:", Location = new Point(labelX, 75), AutoSize = true, Font = font };
            txtLoaiDV = new TextBox() { Location = new Point(inputX, 72), Width = inputWidth, Font = font, ReadOnly = true, BackColor = Color.WhiteSmoke };

            // Nhãn và ô nhập Ngày thực hiện
            Label lblNgay = new Label() { Text = "Ngày DV:", Location = new Point(labelX, 120), AutoSize = true, Font = font };
            txtNgayDV = new TextBox() { Location = new Point(inputX, 117), Width = inputWidth, Font = font, ReadOnly = true, BackColor = Color.WhiteSmoke };

            // Nhãn và ô nhập KẾT QUẢ
            Label lblKQ = new Label() { Text = "Kết quả:", Location = new Point(labelX, 165), AutoSize = true, Font = font };
            txtKetQua = new TextBox() { Location = new Point(inputX, 162), Width = inputWidth, Font = font, Multiline = true, Height = 60 };

            // Căn chỉnh lại vị trí nút bấm
            btnSave = new Button() { Text = "LƯU KẾT QUẢ", Location = new Point(145, 245), Size = new Size(115, 35), BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button() { Text = "Hủy", Location = new Point(275, 245), Size = new Size(105, 35), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.Add(lblMa); this.Controls.Add(txtMaHSBA);
            this.Controls.Add(lblLoai); this.Controls.Add(txtLoaiDV);
            this.Controls.Add(lblNgay); this.Controls.Add(txtNgayDV);
            this.Controls.Add(lblKQ); this.Controls.Add(txtKetQua);
            this.Controls.Add(btnSave); this.Controls.Add(btnCancel);
        }

        private void FillData()
        {
            txtMaHSBA.Text = _model.MaHSBA;
            txtLoaiDV.Text = _model.LoaiDV;
            txtNgayDV.Text = _model.NgayDV;
            txtKetQua.Text = _model.KetQua;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string ketqua = txtKetQua.Text.Trim();
            if (string.IsNullOrEmpty(ketqua))
            {
                MessageBox.Show("Vui lòng không để trống ô kết quả dịch vụ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _model.KetQua = ketqua;
                bool success = _presenter.UpdateResult(_model);

                if (success)
                {
                    MessageBox.Show("Cập nhật kết quả thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại. Kiểm tra lại phân quyền!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Oracle: " + ex.Message, "Lỗi phân quyền", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}