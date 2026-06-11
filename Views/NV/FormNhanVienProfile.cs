using System;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Presenters;
using ATBM_Project.Models;

namespace ATBM_Project.Views.NV
{
    public class FormNhanVienProfile : Form
    {
        private NhanVienPresenter presenter;
        private NhanVienModel currentProfile;

        private GroupBox grpHanhChinh, grpLienHe;
        private TextBox txtManV, txtHoTen, txtPhai, txtNgaySinh, txtCmnd, txtVaiTro, txtChuyenKhoa;
        private TextBox txtQueQuan, txtSoDt;
        private Button btnEdit, btnRefresh;

        public FormNhanVienProfile()
        {
            presenter = new NhanVienPresenter();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.ClientSize = new Size(800, 600);
            this.BackColor = Color.FromArgb(245, 246, 250);
            Font fontLabel = new Font("Segoe UI", 9F, FontStyle.Bold);
            Font fontText = new Font("Segoe UI", 10F);

            Label lblTitle = new Label()
            {
                Text = "THÔNG TIN CÁ NHÂN NHÂN VIÊN",
                Location = new Point(20, 15),
                AutoSize = true,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 53, 65)
            };

            // --- 1. NHÓM THÔNG TIN HÀNH CHÍNH & CHUYÊN MÔN ---
            grpHanhChinh = new GroupBox() { Text = "Thông tin nhân sự", Location = new Point(20, 60), Size = new Size(370, 280), Font = fontLabel, ForeColor = Color.SteelBlue };

            AddProfileRow(grpHanhChinh, "Mã nhân viên:", out txtManV, 30);
            AddProfileRow(grpHanhChinh, "Họ và tên:", out txtHoTen, 65);
            AddProfileRow(grpHanhChinh, "Phái:", out txtPhai, 100);
            AddProfileRow(grpHanhChinh, "Ngày sinh:", out txtNgaySinh, 135);
            AddProfileRow(grpHanhChinh, "Số CMND/CCCD:", out txtCmnd, 170);
            AddProfileRow(grpHanhChinh, "Vai trò:", out txtVaiTro, 205);
            AddProfileRow(grpHanhChinh, "Chuyên khoa:", out txtChuyenKhoa, 240);

            // --- 2. NHÓM THÔNG TIN LIÊN LẠC (ĐƯỢC SỬA) ---
            grpLienHe = new GroupBox() { Text = "Thông tin liên lạc & Cư trú", Location = new Point(410, 60), Size = new Size(360, 150), Font = fontLabel, ForeColor = Color.DarkGreen };

            AddProfileRow(grpLienHe, "Số điện thoại:", out txtSoDt, 35);
            AddProfileRow(grpLienHe, "Quê quán:", out txtQueQuan, 80);
            txtQueQuan.Multiline = true;
            txtQueQuan.Height = 45;

            // --- NÚT BẤM ---
            btnEdit = new Button()
            {
                Text = "THAY ĐỔI THÔNG TIN",
                Location = new Point(20, 500),
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
                Location = new Point(235, 500),
                Size = new Size(120, 45),
                BackColor = Color.White,
                ForeColor = Color.SteelBlue,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderColor = Color.SteelBlue;
            btnRefresh.Click += (s, e) => LoadData();

            this.Controls.AddRange(new Control[] { lblTitle, grpHanhChinh, grpLienHe, btnEdit, btnRefresh });
        }

        private void AddProfileRow(GroupBox box, string labelText, out TextBox txt, int y)
        {
            Label lbl = new Label() { Text = labelText, Location = new Point(15, y + 3), Width = 110, Font = new Font("Segoe UI", 9F, FontStyle.Regular), ForeColor = Color.Black };
            txt = new TextBox()
            {
                Location = new Point(130, y),
                Width = box.Width - 150,
                ReadOnly = true,
                BackColor = Color.FromArgb(245, 246, 250),
                BorderStyle = BorderStyle.None,
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
                    txtManV.Text = currentProfile.ManV;
                    txtHoTen.Text = currentProfile.HoTen;
                    txtPhai.Text = currentProfile.Phai;
                    txtNgaySinh.Text = currentProfile.NgaySinh;
                    txtCmnd.Text = currentProfile.Cmnd;
                    txtVaiTro.Text = currentProfile.VaiTro;
                    txtChuyenKhoa.Text = string.IsNullOrEmpty(currentProfile.ChuyenKhoa) ? "Không có" : currentProfile.ChuyenKhoa;
                    txtSoDt.Text = currentProfile.SoDt;
                    txtQueQuan.Text = currentProfile.QueQuan;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải hồ sơ nhân viên: " + ex.Message);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (currentProfile == null) return;
            FormEditNhanVien editForm = new FormEditNhanVien(currentProfile, presenter);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }
    }
}