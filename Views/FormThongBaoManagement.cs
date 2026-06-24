using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using ATBM_Project.Data;
using ATBM_Project.Utilities;

namespace ATBM_Project.Views
{
    /// <summary>
    /// Form quản lý thông báo OLS cho Admin.
    /// Chỉ cho phép chọn từ danh sách 13 nhãn đã được CREATE_LABEL,
    /// tránh hoàn toàn việc tạo nhãn mới ngoài whitelist.
    /// </summary>
    public class FormThongBaoManagement : Form
    {
        // ── Controls ────────────────────────────────────────────────────────
        private TextBox txtNoiDung;
        private TextBox txtDiaDiem;
        private DateTimePicker dtpNgayGio;
        private ComboBox cboNhanOLS;   // ← 1 combo thay cho 3 combo cũ
        private Label lblNhanPreview;
        private Label lblSQLPreview;
        private Button btnGui;

        // ── Whitelist: 13 nhãn đã CREATE_LABEL trong OLS_Setup.sql ─────────
        private class OlsLabel
        {
            public string Tag { get; }   // chuỗi nhãn truyền vào FN_BUILD_LABEL/SP
            public string Desc { get; }   // mô tả hiển thị cho người dùng
            // Parse ngược ra 3 thành phần để truyền vào SP
            public string Level { get; }
            public string Comp { get; }
            public string Group { get; }

            public OlsLabel(string tag, string desc)
            {
                Tag = tag;
                Desc = desc;

                // Parse: "LEVEL:COMP:GROUP" hoặc "LEVEL:COMP" hoặc "LEVEL"
                string[] parts = tag.Split(':');
                Level = parts.Length > 0 ? parts[0] : "NV";
                Comp = parts.Length > 1 ? parts[1] : "";
                Group = parts.Length > 2 ? parts[2] : "";
            }

            // Hiển thị trong ComboBox
            public override string ToString() => $"{Tag}   —   {Desc}";
        }

                    private static readonly List<OlsLabel> LABEL_WHITELIST = new List<OlsLabel>
            {
                new OlsLabel("NV",                   "[t1] Toàn thể nhân viên"),
                new OlsLabel("BGD",                  "[t2] Ban Giám Đốc"),
                new OlsLabel("LDK",                  "[t3] Tất cả lãnh đạo khoa"),
                new OlsLabel("LDK:C_TH",            "[t4] Lãnh đạo Khoa Tiêu Hóa"),
                new OlsLabel("NV:C_TH:G_HCM",       "[t5] Nhân viên Tiêu Hóa – TP.HCM"),
                new OlsLabel("NV:C_TH:G_HN",        "[t6] Nhân viên Tiêu Hóa – Hà Nội"),
                new OlsLabel("LDK:C_TH,C_TK:G_HP", "[t7] LĐ Tiêu Hóa & Thần Kinh – Hải Phòng"),
            };

        // ── Constructor ─────────────────────────────────────────────────────
        public FormThongBaoManagement()
        {
            InitializeComponent();
            PopulateLabelCombo();
            cboNhanOLS.SelectedIndex = 0;
        }

        // ── UI Setup ─────────────────────────────────────────────────────────
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Quản lý Thông báo – Oracle Label Security (OLS)";
            this.Size = new Size(760, 560);
            this.BackColor = Color.White;
            this.AutoScroll = true;

            int left = 20;
            int width = 700;
            int y = 15;

            // ── Tiêu đề ────────────────────────────────────────────────────
            var lblTitle = MakeLabel(
                "Quản lý Thông báo — Oracle Label Security (OLS)",
                left, y, width, 30,
                new Font("Segoe UI", 13F, FontStyle.Bold), Color.SteelBlue);
            y += 42;

            // ── Nội dung ───────────────────────────────────────────────────
            this.Controls.Add(MakeLabel("Nội dung thông báo:", left, y));
            y += 20;
            txtNoiDung = new TextBox
            {
                Multiline = true,
                Location = new Point(left, y),
                Width = width,
                Height = 80,
                Font = new Font("Segoe UI", 9F),
                ScrollBars = ScrollBars.Vertical
            };
            txtNoiDung.TextChanged += (s, e) => UpdateSQLPreview();
            this.Controls.Add(txtNoiDung);
            y += 90;

            // ── Địa điểm ───────────────────────────────────────────────────
            this.Controls.Add(MakeLabel("Địa điểm:", left, y));
            y += 20;
            txtDiaDiem = new TextBox
            {
                Location = new Point(left, y),
                Width = width,
                Font = new Font("Segoe UI", 9F)
            };
            txtDiaDiem.TextChanged += (s, e) => UpdateSQLPreview();
            this.Controls.Add(txtDiaDiem);
            y += 35;

            // ── Ngày giờ ───────────────────────────────────────────────────
            this.Controls.Add(MakeLabel("Ngày giờ:", left, y));
            y += 20;
            dtpNgayGio = new DateTimePicker
            {
                Location = new Point(left, y),
                Width = 220,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm",
                Value = DateTime.Now,
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(dtpNgayGio);
            y += 38;

            // ── Nhãn OLS (whitelist combo) ─────────────────────────────────
            this.Controls.Add(MakeLabel("Nhãn OLS (chọn từ danh sách đã định nghĩa):", left, y));
            y += 20;

            cboNhanOLS = new ComboBox
            {
                Location = new Point(left, y),
                Width = width,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Courier New", 9F),
                DropDownWidth = width
            };
            cboNhanOLS.SelectedIndexChanged += (s, e) =>
            {
                UpdateLabelPreview();
                UpdateSQLPreview();
            };
            this.Controls.Add(cboNhanOLS);
            y += 32;

            // ── Preview nhãn ───────────────────────────────────────────────
            lblNhanPreview = new Label
            {
                Location = new Point(left, y),
                AutoSize = false,
                Width = width,
                Height = 28,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.DarkGreen,
                BackColor = Color.FromArgb(236, 248, 240),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(6, 4, 0, 0)
            };
            this.Controls.Add(lblNhanPreview);
            y += 36;

            // ── SQL Preview ────────────────────────────────────────────────
            this.Controls.Add(MakeLabel("Preview lệnh gọi SP:", left, y));
            y += 20;
            lblSQLPreview = new Label
            {
                Location = new Point(left, y),
                AutoSize = false,
                Width = width,
                Height = 75,
                Font = new Font("Courier New", 8F),
                BackColor = Color.FromArgb(245, 245, 245),
                ForeColor = Color.FromArgb(60, 60, 60),
                Padding = new Padding(6),
                BorderStyle = BorderStyle.Fixed3D
            };
            this.Controls.Add(lblSQLPreview);
            y += 85;

            // ── Nút Gửi ───────────────────────────────────────────────────
            btnGui = new Button
            {
                Text = "✉  Gửi thông báo",
                Location = new Point(left, y),
                Width = width,
                Height = 42,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnGui.FlatAppearance.BorderSize = 0;
            btnGui.Click += BtnGui_Click;
            this.Controls.Add(btnGui);

            this.Controls.Add(lblTitle);
            this.ResumeLayout(false);
        }

        // ── Helpers tạo Label ────────────────────────────────────────────────
        private Label MakeLabel(string text, int x, int y,
            int width = 0, int height = 0,
            Font font = null, Color? color = null)
        {
            var lbl = new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = (width == 0),
                Font = font ?? new Font("Segoe UI", 9F)
            };
            if (width > 0) lbl.Width = width;
            if (height > 0) lbl.Height = height;
            if (color.HasValue) lbl.ForeColor = color.Value;
            return lbl;
        }

        // ── Populate combo với 13 nhãn whitelist ────────────────────────────
        private void PopulateLabelCombo()
        {
            cboNhanOLS.Items.Clear();
            foreach (var lbl in LABEL_WHITELIST)
                cboNhanOLS.Items.Add(lbl);
        }

        // ── Cập nhật preview nhãn ────────────────────────────────────────────
        private void UpdateLabelPreview()
        {
            if (cboNhanOLS.SelectedItem is OlsLabel selected)
                lblNhanPreview.Text = $"✓  {selected.Tag}   —   {selected.Desc}";
            else
                lblNhanPreview.Text = "";
        }

        // ── Cập nhật preview SQL ─────────────────────────────────────────────
        private void UpdateSQLPreview()
        {
            if (!(cboNhanOLS.SelectedItem is OlsLabel selected))
            {
                lblSQLPreview.Text = "";
                return;
            }

            string nd = txtNoiDung.Text.Replace("'", "''");
            string dd = txtDiaDiem.Text.Replace("'", "''");

            lblSQLPreview.Text =
                $"EXEC ADMIN.SP_INSERT_THONGBAO(\r\n" +
                $"  p_noidung => N'{nd}',\r\n" +
                $"  p_level   => '{selected.Level}',\r\n" +
                $"  p_comp    => '{selected.Comp}',\r\n" +
                $"  p_group   => '{selected.Group}'\r\n" +
                $")";
        }

        // ── Gửi thông báo ────────────────────────────────────────────────────
        private void BtnGui_Click(object sender, EventArgs e)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(txtNoiDung.Text))
            {
                MessageBox.Show("Vui lòng nhập nội dung thông báo!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!(cboNhanOLS.SelectedItem is OlsLabel selected))
            {
                MessageBox.Show("Vui lòng chọn nhãn OLS!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Xác nhận
            var confirm = MessageBox.Show(
                $"Gửi thông báo với nhãn OLS:\n\n" +
                $"  {selected.Tag}\n  ({selected.Desc})\n\n" +
                $"Tiếp tục?",
                "Xác nhận gửi thông báo",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                btnGui.Enabled = false;
                btnGui.Text = "Đang gửi...";

                var parameters = new OracleParameter[]
                {
                    new OracleParameter { ParameterName = ":p_noidung",
                        OracleDbType = OracleDbType.NVarchar2,
                        Value = txtNoiDung.Text,      Direction = ParameterDirection.Input },

                    new OracleParameter { ParameterName = ":p_ngaygio",
                        OracleDbType = OracleDbType.TimeStamp,
                        Value = dtpNgayGio.Value,     Direction = ParameterDirection.Input },

                    new OracleParameter { ParameterName = ":p_diadiem",
                        OracleDbType = OracleDbType.NVarchar2,
                        Value = txtDiaDiem.Text,      Direction = ParameterDirection.Input },

                    new OracleParameter { ParameterName = ":p_level",
                        OracleDbType = OracleDbType.Varchar2,
                        Value = selected.Level,       Direction = ParameterDirection.Input },

                    new OracleParameter { ParameterName = ":p_comp",
                        OracleDbType = OracleDbType.Varchar2,
                        Value = selected.Comp,        Direction = ParameterDirection.Input },

                    new OracleParameter { ParameterName = ":p_group",
                        OracleDbType = OracleDbType.Varchar2,
                        Value = selected.Group,       Direction = ParameterDirection.Input }
                };

                OracleHelper.ExecuteNonQuery(
                    DBConfig.ConnectionString,
                    "ADMIN.SP_INSERT_THONGBAO",
                    parameters);

                MessageBox.Show(
                    $"✓ Gửi thông báo thành công!\n\nNhãn OLS: {selected.Tag}",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ResetForm();
            }
            catch (OracleException ex)
            {
                // Lỗi -20001: nhãn chưa định nghĩa (từ SP)
                // Lỗi -20002: CHAR_TO_LABEL trả NULL (phòng thủ thêm)
                string msg = ex.Message.Contains("-20001") || ex.Message.Contains("-20002")
                    ? $"Nhãn OLS không hợp lệ.\n\n{ex.Message}"
                    : $"Lỗi Oracle: {ex.Message}";

                MessageBox.Show(msg, "Lỗi gửi thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi không xác định: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnGui.Enabled = true;
                btnGui.Text = "✉  Gửi thông báo";
            }
        }

        // ── Reset form về trạng thái ban đầu ────────────────────────────────
        private void ResetForm()
        {
            txtNoiDung.Clear();
            txtDiaDiem.Clear();
            dtpNgayGio.Value = DateTime.Now;
            cboNhanOLS.SelectedIndex = 0;
            UpdateLabelPreview();
            UpdateSQLPreview();
        }
    }
}