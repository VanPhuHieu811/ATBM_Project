using ATBM_Project.Models;
using ATBM_Project.Presenters;
using ATBM_Project.Views.BN;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ATBM_Project.Views.BN
{
    public class FormBenhNhanProfile : Form
    {
        private BenhNhanPresenter presenter;
        private BenhNhanModel currentProfile;

        // Các GroupBox để gom nhóm thông tin
        private GroupBox grpHanhChinh, grpDiaChi, grpYTe;

        // Các TextBox hiển thị (Chỉ đọc)
        private TextBox txtMaBN, txtTenBN, txtPhai, txtNgaySinh, txtCccd;
        private TextBox txtSoNha, txtTenDuong, txtQuanHuyen, txtTinhTp;
        private TextBox txtTienSu, txtTienSuGD, txtDiUng;

        private Button btnEdit, btnRefresh;

        public FormBenhNhanProfile()
        {
            presenter = new BenhNhanPresenter();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.ClientSize = new Size(800, 600);
            this.BackColor = Color.FromArgb(245, 246, 250); // Màu nền xám trắng hiện đại
            Font fontLabel = new Font("Segoe UI", 9F, FontStyle.Bold);
            Font fontText = new Font("Segoe UI", 10F);

            Label lblTitle = new Label()
            {
                Text = "HỒ SƠ THÔNG TIN CÁ NHÂN",
                Location = new Point(20, 15),
                AutoSize = true,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 53, 65)
            };

            // --- 1. NHÓM THÔNG TIN HÀNH CHÍNH ---
            grpHanhChinh = new GroupBox() { Text = "Thông tin hành chính", Location = new Point(20, 60), Size = new Size(370, 210), Font = fontLabel, ForeColor = Color.SteelBlue };

            AddInputPair(grpHanhChinh, "Mã bệnh nhân:", out txtMaBN, 30, 80);
            AddInputPair(grpHanhChinh, "Họ và tên:", out txtTenBN, 65, 80);
            AddInputPair(grpHanhChinh, "Phái:", out txtPhai, 100, 80);
            AddInputPair(grpHanhChinh, "Ngày sinh:", out txtNgaySinh, 135, 80);
            AddInputPair(grpHanhChinh, "Số CCCD:", out txtCccd, 170, 80);

            // --- 2. NHÓM ĐỊA CHỈ THƯỜNG TRÚ ---
            grpDiaChi = new GroupBox() { Text = "Địa chỉ liên lạc", Location = new Point(410, 60), Size = new Size(360, 210), Font = fontLabel, ForeColor = Color.SteelBlue };

            AddInputPair(grpDiaChi, "Số nhà:", out txtSoNha, 30, 80);
            AddInputPair(grpDiaChi, "Tên đường:", out txtTenDuong, 65, 80);
            AddInputPair(grpDiaChi, "Quận / Huyện:", out txtQuanHuyen, 100, 80);
            AddInputPair(grpDiaChi, "Tỉnh / TP:", out txtTinhTp, 135, 80);

            // --- 3. NHÓM THÔNG TIN Y TẾ (CHIẾM CHIỀU RỘNG) ---
            grpYTe = new GroupBox() { Text = "Tiền sử bệnh và Dị ứng", Location = new Point(20, 280), Size = new Size(750, 220), Font = fontLabel, ForeColor = Color.DarkRed };

            Label l1 = new Label() { Text = "Tiền sử bệnh lý:", Location = new Point(15, 30), AutoSize = true };
            txtTienSu = new TextBox() { Location = new Point(15, 50), Width = 225, Height = 140, Multiline = true, ReadOnly = true, Font = fontText, BackColor = Color.White };

            Label l2 = new Label() { Text = "Tiền sử bệnh gia đình:", Location = new Point(260, 30), AutoSize = true };
            txtTienSuGD = new TextBox() { Location = new Point(260, 50), Width = 225, Height = 140, Multiline = true, ReadOnly = true, Font = fontText, BackColor = Color.White };

            Label l3 = new Label() { Text = "Dị ứng thuốc:", Location = new Point(505, 30), AutoSize = true };
            txtDiUng = new TextBox() { Location = new Point(505, 50), Width = 225, Height = 140, Multiline = true, ReadOnly = true, Font = fontText, BackColor = Color.White };

            grpYTe.Controls.AddRange(new Control[] { l1, txtTienSu, l2, txtTienSuGD, l3, txtDiUng });

            // --- NÚT BẤM ---
            btnEdit = new Button()
            {
                Text = "CẬP NHẬT THÔNG TIN",
                Location = new Point(20, 520),
                Size = new Size(200, 45),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnEdit.Click += BtnEdit_Click;

            btnRefresh = new Button()
            {
                Text = "Làm mới",
                Location = new Point(235, 520),
                Size = new Size(120, 45),
                BackColor = Color.White,
                ForeColor = Color.SteelBlue,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderColor = Color.SteelBlue;
            btnRefresh.Click += (s, e) => LoadData();

            this.Controls.AddRange(new Control[] { lblTitle, grpHanhChinh, grpDiaChi, grpYTe, btnEdit, btnRefresh });
        }

        // Helper để tạo cặp Label + TextBox nhanh và thẳng hàng
        private void AddInputPair(GroupBox box, string labelText, out TextBox txt, int y, int labelWidth)
        {
            Label lbl = new Label() { Text = labelText, Location = new Point(15, y + 3), Width = 110, Font = new Font("Segoe UI", 9F, FontStyle.Regular), ForeColor = Color.Black };
            txt = new TextBox()
            {
                Location = new Point(130, y),
                Width = box.Width - 150,
                ReadOnly = true,
                BackColor = Color.FromArgb(245, 246, 250), // Màu hơi xám nhẹ để báo là chỉ đọc
                BorderStyle = BorderStyle.None, // Bỏ viền cho đẹp
                Font = new Font("Segoe UI", 10F, FontStyle.Italic)
            };
            box.Controls.Add(lbl);
            box.Controls.Add(txt);
        }

        private void LoadData()
        {
            try
            {
                currentProfile = presenter.GetProfile();
                if (currentProfile != null)
                {
                    txtMaBN.Text = currentProfile.MaBN;
                    txtTenBN.Text = currentProfile.TenBN;
                    txtPhai.Text = currentProfile.Phai;
                    txtNgaySinh.Text = currentProfile.NgaySinh;
                    txtCccd.Text = currentProfile.Cccd;
                    txtSoNha.Text = currentProfile.SoNha;
                    txtTenDuong.Text = currentProfile.TenDuong;
                    txtQuanHuyen.Text = currentProfile.QuanHuyen;
                    txtTinhTp.Text = currentProfile.TinhTp;
                    txtTienSu.Text = currentProfile.TienSuBenh;
                    txtTienSuGD.Text = currentProfile.TienSuBenhGd;
                    txtDiUng.Text = currentProfile.DiUngThuoc;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (currentProfile == null) return;
            FormEditBenhNhan editForm = new FormEditBenhNhan(currentProfile, presenter);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }
    }
}