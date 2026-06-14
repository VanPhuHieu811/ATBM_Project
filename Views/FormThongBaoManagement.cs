using System;
using System.Data; // Đã thêm để sử dụng ParameterDirection
using System.Drawing;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using ATBM_Project.Data;
using ATBM_Project.Utilities;

namespace ATBM_Project.Views
{
    /// <summary>
    /// Form quản lý thông báo OLS cho Admin
    /// Cho phép tạo thông báo với các nhãn OLS khác nhau
    /// </summary>
    public class FormThongBaoManagement : Form
    {
        private TextBox txtNoiDung;
        private TextBox txtDiaDiem;
        private DateTimePicker dtpNgayGio;
        private ComboBox cboCap;
        private ComboBox cboKhoa;
        private ComboBox cboCoso;
        private Label lblNhanOLS;
        private Label lblSQLPreview;
        private Button btnGui;
        private Label lblTitle;

        public FormThongBaoManagement()
        {
            InitializeComponent();
            SetupEventHandlers();
            UpdateOLSLabel();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Title
            lblTitle = new Label();
            lblTitle.Text = "Quản lý Thông báo - Oracle Label Security (OLS)";
            lblTitle.Location = new Point(15, 15);
            lblTitle.AutoSize = false;
            lblTitle.Width = 700;
            lblTitle.Height = 30;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.SteelBlue;

            // Nội dung
            Label lblNoiDung = new Label();
            lblNoiDung.Text = "Nội dung thông báo:";
            lblNoiDung.Location = new Point(15, 55);
            lblNoiDung.AutoSize = true;
            lblNoiDung.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            txtNoiDung = new TextBox();
            txtNoiDung.Multiline = true;
            txtNoiDung.Location = new Point(15, 75);
            txtNoiDung.Width = 700;
            txtNoiDung.Height = 80;
            txtNoiDung.Font = new Font("Segoe UI", 9F);
            txtNoiDung.TextChanged += (s, e) => UpdateSQLPreview();

            // Địa điểm
            Label lblDiaDiem = new Label();
            lblDiaDiem.Text = "Địa điểm:";
            lblDiaDiem.Location = new Point(15, 165);
            lblDiaDiem.AutoSize = true;
            lblDiaDiem.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            txtDiaDiem = new TextBox();
            txtDiaDiem.Location = new Point(15, 185);
            txtDiaDiem.Width = 700;
            txtDiaDiem.Font = new Font("Segoe UI", 9F);
            txtDiaDiem.TextChanged += (s, e) => UpdateSQLPreview();

            // Ngày giờ
            Label lblNgayGio = new Label();
            lblNgayGio.Text = "Ngày giờ:";
            lblNgayGio.Location = new Point(15, 215);
            lblNgayGio.AutoSize = true;
            lblNgayGio.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            dtpNgayGio = new DateTimePicker();
            dtpNgayGio.Location = new Point(15, 235);
            dtpNgayGio.Width = 200;
            dtpNgayGio.Format = DateTimePickerFormat.Custom;
            dtpNgayGio.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpNgayGio.Value = DateTime.Now;
            dtpNgayGio.Font = new Font("Segoe UI", 9F);

            // Cấp bậc
            Label lblCap = new Label();
            lblCap.Text = "Cấp bậc:";
            lblCap.Location = new Point(15, 270);
            lblCap.AutoSize = true;
            lblCap.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            cboCap = new ComboBox();
            cboCap.Location = new Point(15, 290);
            cboCap.Width = 220;
            cboCap.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCap.Font = new Font("Segoe UI", 9F);
            cboCap.Items.AddRange(new object[]
            {
                "NV - Nhân viên",
                "LDK - Lãnh đạo khoa",
                "BGD - Ban giám đốc"
            });
            cboCap.SelectedIndex = 0;
            cboCap.SelectedIndexChanged += (s, e) => UpdateOLSLabel();

            // Khoa
            Label lblKhoa = new Label();
            lblKhoa.Text = "Khoa:";
            lblKhoa.Location = new Point(250, 270);
            lblKhoa.AutoSize = true;
            lblKhoa.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            cboKhoa = new ComboBox();
            cboKhoa.Location = new Point(250, 290);
            cboKhoa.Width = 220;
            cboKhoa.DropDownStyle = ComboBoxStyle.DropDownList;
            cboKhoa.Font = new Font("Segoe UI", 9F);
            cboKhoa.Items.AddRange(new object[]
            {
                "(Tất cả khoa)",
                "C_TH - Tiêu Hóa",
                "C_TK - Thần Kinh",
                "C_TM - Tim Mạch",
                "C_TH,C_TK - Tiêu Hóa + Thần Kinh"
            });
            cboKhoa.SelectedIndex = 0;
            cboKhoa.SelectedIndexChanged += (s, e) => UpdateOLSLabel();

            // Cơ sở
            Label lblCoso = new Label();
            lblCoso.Text = "Cơ sở:";
            lblCoso.Location = new Point(485, 270);
            lblCoso.AutoSize = true;
            lblCoso.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            cboCoso = new ComboBox();
            cboCoso.Location = new Point(485, 290);
            cboCoso.Width = 230;
            cboCoso.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCoso.Font = new Font("Segoe UI", 9F);
            cboCoso.Items.AddRange(new object[]
            {
                "(Tất cả cơ sở)",
                "G_HCM - Hồ Chí Minh",
                "G_HP - Hải Phòng",
                "G_HN - Hà Nội"
            });
            cboCoso.SelectedIndex = 0;
            cboCoso.SelectedIndexChanged += (s, e) => UpdateOLSLabel();

            // Nhãn OLS
            lblNhanOLS = new Label();
            lblNhanOLS.Text = "Nhãn OLS: NV";
            lblNhanOLS.Location = new Point(15, 330);
            lblNhanOLS.AutoSize = false;
            lblNhanOLS.Width = 700;
            lblNhanOLS.Height = 25;
            lblNhanOLS.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblNhanOLS.ForeColor = Color.Green;
            lblNhanOLS.BackColor = Color.AliceBlue;
            lblNhanOLS.Padding = new Padding(5);
            lblNhanOLS.BorderStyle = BorderStyle.Fixed3D;

            // SQL Preview
            Label lblSQLLabel = new Label();
            lblSQLLabel.Text = "Preview lệnh SQL:";
            lblSQLLabel.Location = new Point(15, 365);
            lblSQLLabel.AutoSize = true;
            lblSQLLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            lblSQLPreview = new Label();
            lblSQLPreview.Text = "";
            lblSQLPreview.Location = new Point(15, 385);
            lblSQLPreview.AutoSize = false;
            lblSQLPreview.Width = 700;
            lblSQLPreview.Height = 60;
            lblSQLPreview.Font = new Font("Courier New", 8F);
            lblSQLPreview.BackColor = Color.FromArgb(240, 240, 240);
            lblSQLPreview.ForeColor = Color.FromArgb(64, 64, 64);
            lblSQLPreview.Padding = new Padding(5);
            lblSQLPreview.BorderStyle = BorderStyle.Fixed3D;
            UpdateSQLPreview();

            // Button Gửi
            btnGui = new Button();
            btnGui.Text = "✉ Gửi thông báo";
            btnGui.Location = new Point(15, 455);
            btnGui.Width = 700;
            btnGui.Height = 40;
            btnGui.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnGui.BackColor = Color.SteelBlue;
            btnGui.ForeColor = Color.White;
            btnGui.FlatStyle = FlatStyle.Flat;
            btnGui.FlatAppearance.BorderSize = 0;
            btnGui.Cursor = Cursors.Hand;
            btnGui.Click += BtnGui_Click;

            // Add to form
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblNoiDung);
            this.Controls.Add(txtNoiDung);
            this.Controls.Add(lblDiaDiem);
            this.Controls.Add(txtDiaDiem);
            this.Controls.Add(lblNgayGio);
            this.Controls.Add(dtpNgayGio);
            this.Controls.Add(lblCap);
            this.Controls.Add(cboCap);
            this.Controls.Add(lblKhoa);
            this.Controls.Add(cboKhoa);
            this.Controls.Add(lblCoso);
            this.Controls.Add(cboCoso);
            this.Controls.Add(lblNhanOLS);
            this.Controls.Add(lblSQLLabel);
            this.Controls.Add(lblSQLPreview);
            this.Controls.Add(btnGui);

            this.ResumeLayout(false);
            this.AutoScroll = true;
            this.BackColor = Color.White;
        }

        private void SetupEventHandlers()
        {
        }

        private void UpdateOLSLabel()
        {
            try
            {
                string level = ExtractLevel(cboCap.SelectedItem?.ToString());
                string comp = ExtractCompartment(cboKhoa.SelectedItem?.ToString());
                string group = ExtractGroup(cboCoso.SelectedItem?.ToString());

                string label = OracleHelper.GetBuildLabel(DBConfig.ConnectionString, level, comp, group);
                lblNhanOLS.Text = $"Nhãn OLS: {label}";
                UpdateSQLPreview();
            }
            catch
            {
                lblNhanOLS.Text = "Nhãn OLS: Lỗi khi tính toán";
            }
        }

        private void UpdateSQLPreview()
        {
            try
            {
                string noidung = txtNoiDung.Text.Replace("'", "''");
                string diadiem = txtDiaDiem.Text.Replace("'", "''");
                string level = ExtractLevel(cboCap.SelectedItem?.ToString());
                string comp = ExtractCompartment(cboKhoa.SelectedItem?.ToString());
                string group = ExtractGroup(cboCoso.SelectedItem?.ToString());

                string sql = $"EXEC ADMIN.SP_INSERT_THONGBAO(\n" +
                    $"  p_noidung => N'{noidung}',\n" +
                    $"  p_diadiem => N'{diadiem}',\n" +
                    $"  p_level => '{level}',\n" +
                    $"  p_comp => '{comp}',\n" +
                    $"  p_group => '{group}'\n" +
                    $")";

                lblSQLPreview.Text = sql;
            }
            catch
            {
                lblSQLPreview.Text = "";
            }
        }

        private string ExtractLevel(string item)
        {
            if (string.IsNullOrEmpty(item)) return "NV";
            if (item.StartsWith("NV")) return "NV";
            if (item.StartsWith("LDK")) return "LDK";
            if (item.StartsWith("BGD")) return "BGD";
            return "NV";
        }

        private string ExtractCompartment(string item)
        {
            if (string.IsNullOrEmpty(item) || item.StartsWith("(Tất cả")) return "";

            int dashIndex = item.IndexOf(" - ");
            if (dashIndex > 0)
                return item.Substring(0, dashIndex);

            return "";
        }

        private string ExtractGroup(string item)
        {
            if (string.IsNullOrEmpty(item) || item.StartsWith("(Tất cả")) return "";

            int dashIndex = item.IndexOf(" - ");
            if (dashIndex > 0)
                return item.Substring(0, dashIndex);

            return "";
        }

        private void BtnGui_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNoiDung.Text))
            {
                MessageBox.Show("Vui lòng nhập nội dung thông báo!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ✅ Thêm: Xác nhận label trước khi gửi
            string labelPreview = lblNhanOLS.Text.Replace("Nhãn OLS: ", "");
            var confirm = MessageBox.Show(
                $"Xác nhận gửi thông báo với nhãn OLS:\n{labelPreview}",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                string level = ExtractLevel(cboCap.SelectedItem?.ToString());
                string comp = ExtractCompartment(cboKhoa.SelectedItem?.ToString());
                string group = ExtractGroup(cboCoso.SelectedItem?.ToString());

                OracleParameter[] parameters = new OracleParameter[]
                {
            new OracleParameter { ParameterName = ":p_noidung", OracleDbType = OracleDbType.NVarchar2,  Value = txtNoiDung.Text,    Direction = ParameterDirection.Input },
            new OracleParameter { ParameterName = ":p_ngaygio", OracleDbType = OracleDbType.TimeStamp,  Value = dtpNgayGio.Value,   Direction = ParameterDirection.Input },
            new OracleParameter { ParameterName = ":p_diadiem", OracleDbType = OracleDbType.NVarchar2,  Value = txtDiaDiem.Text,    Direction = ParameterDirection.Input },
            new OracleParameter { ParameterName = ":p_level",   OracleDbType = OracleDbType.Varchar2,   Value = level,              Direction = ParameterDirection.Input },
            new OracleParameter { ParameterName = ":p_comp",    OracleDbType = OracleDbType.Varchar2,   Value = comp  ?? "",        Direction = ParameterDirection.Input },
            new OracleParameter { ParameterName = ":p_group",   OracleDbType = OracleDbType.Varchar2,   Value = group ?? "",        Direction = ParameterDirection.Input }
                };

                OracleHelper.ExecuteNonQuery(
                    DBConfig.ConnectionString,
                    "ADMIN.SP_INSERT_THONGBAO",
                    parameters);

                MessageBox.Show("✓ Gửi thông báo thành công!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reset form
                txtNoiDung.Clear();
                txtDiaDiem.Clear();
                dtpNgayGio.Value = DateTime.Now;
                cboCap.SelectedIndex = 0;
                cboKhoa.SelectedIndex = 0;
                cboCoso.SelectedIndex = 0;
                UpdateOLSLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi gửi thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}