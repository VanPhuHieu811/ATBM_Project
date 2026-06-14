using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using ATBM_Project.Utilities;

namespace ATBM_Project.Views
{
    /// <summary>
    /// Form xem thông báo OLS cho các user (U1_BGD đến U8_NV).
    /// OLS tự động lọc dữ liệu theo session label của user.
    /// </summary>
    public class FormThongBao : Form, ILogoutSupport
    {
        // ── Fields ────────────────────────────────────────────────────────────
        private readonly string connectionString;

        private Panel pnlHeader;
        private Panel pnlAvatar;
        private Label lblAvatarText;
        private Label lblWelcome;
        private Button btnLamMoi;
        private Button btnDangXuat;
        private DataGridView dgvThongBao;
        private Panel pnlEmpty;
        public event EventHandler LogoutRequested;
        // ── Constructor ───────────────────────────────────────────────────────
        public FormThongBao(string connStr)
        {
            connectionString = connStr;
            InitializeComponent();
            LoadData();
        }

        // ── UI Setup ──────────────────────────────────────────────────────────
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // ── Header panel ──────────────────────────────────────────────
            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 54;
            pnlHeader.BackColor = Color.FromArgb(41, 53, 65);   // header tối

            // Avatar (hình tròn vẽ qua Paint)
            pnlAvatar = new Panel();
            pnlAvatar.Size = new Size(36, 36);
            pnlAvatar.Location = new Point(14, 9);
            pnlAvatar.BackColor = Color.Transparent;
            pnlAvatar.Paint += PnlAvatar_Paint;

            lblAvatarText = new Label();
            lblAvatarText.Text = "??";
            lblAvatarText.AutoSize = false;
            lblAvatarText.Size = new Size(36, 36);
            lblAvatarText.Location = new Point(0, 0);
            lblAvatarText.TextAlign = ContentAlignment.MiddleCenter;
            lblAvatarText.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblAvatarText.ForeColor = Color.FromArgb(41, 53, 65);
            lblAvatarText.BackColor = Color.Transparent;
            pnlAvatar.Controls.Add(lblAvatarText);

            // Welcome label
            lblWelcome = new Label();
            lblWelcome.Text = "Đang tải...";
            lblWelcome.Location = new Point(60, 0);
            lblWelcome.AutoSize = false;
            lblWelcome.Size = new Size(580, 54);
            lblWelcome.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblWelcome.ForeColor = Color.White;
            lblWelcome.TextAlign = ContentAlignment.MiddleLeft;

            // Nút Làm mới
            btnLamMoi = new Button();
            btnLamMoi.Text = "🔄  Làm mới";
            btnLamMoi.Size = new Size(100, 32);
            btnLamMoi.Location = new Point(648, 11);
            btnLamMoi.Font = new Font("Segoe UI", 9F);
            btnLamMoi.BackColor = Color.SteelBlue;
            btnLamMoi.ForeColor = Color.White;
            btnLamMoi.FlatStyle = FlatStyle.Flat;
            btnLamMoi.FlatAppearance.BorderSize = 0;
            btnLamMoi.Cursor = Cursors.Hand;
            btnLamMoi.Click += BtnLamMoi_Click;

            // Nút Đăng xuất
            btnDangXuat = new Button();
            btnDangXuat.Text = "⏻  Đăng xuất";
            btnDangXuat.Size = new Size(110, 32);
            btnDangXuat.Location = new Point(756, 11);
            btnDangXuat.Font = new Font("Segoe UI", 9F);
            btnDangXuat.BackColor = Color.FromArgb(220, 53, 69);
            btnDangXuat.ForeColor = Color.White;
            btnDangXuat.FlatStyle = FlatStyle.Flat;
            btnDangXuat.FlatAppearance.BorderSize = 0;
            btnDangXuat.Cursor = Cursors.Hand;
            btnDangXuat.Click += BtnDangXuat_Click;

            pnlHeader.Controls.AddRange(new Control[]
            {
                pnlAvatar, lblWelcome, btnLamMoi, btnDangXuat
            });

            // ── DataGridView ──────────────────────────────────────────────
            dgvThongBao = new DataGridView();
            dgvThongBao.Location = new Point(15, 68);
            dgvThongBao.Size = new Size(850, 420);
            dgvThongBao.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvThongBao.ReadOnly = true;
            dgvThongBao.RowHeadersVisible = false;
            dgvThongBao.AllowUserToAddRows = false;
            dgvThongBao.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvThongBao.BackgroundColor = Color.White;
            dgvThongBao.BorderStyle = BorderStyle.Fixed3D;
            dgvThongBao.Font = new Font("Segoe UI", 9F);
            dgvThongBao.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;
            dgvThongBao.GridColor = Color.FromArgb(220, 230, 240);
            // Header
            dgvThongBao.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvThongBao.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 53, 65);
            dgvThongBao.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvThongBao.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvThongBao.EnableHeadersVisualStyles = false;

            // ── Empty panel (khi không có dữ liệu) ───────────────────────
            pnlEmpty = new Panel();
            pnlEmpty.Location = new Point(15, 68);
            pnlEmpty.Size = new Size(850, 420);
            pnlEmpty.BackColor = Color.FromArgb(245, 248, 252);
            pnlEmpty.BorderStyle = BorderStyle.Fixed3D;
            pnlEmpty.Visible = false;

            Label lblNoData = new Label();
            lblNoData.Text = "📭  Không có thông báo nào dành cho bạn";
            lblNoData.AutoSize = false;
            lblNoData.Size = new Size(850, 420);
            lblNoData.TextAlign = ContentAlignment.MiddleCenter;
            lblNoData.Font = new Font("Segoe UI", 13F, FontStyle.Italic);
            lblNoData.ForeColor = Color.Gray;
            pnlEmpty.Controls.Add(lblNoData);

            // ── Form ──────────────────────────────────────────────────────
            this.Controls.AddRange(new Control[]
            {
                pnlHeader, dgvThongBao, pnlEmpty
            });

            this.Text = "Xem Thông Báo";
            this.ClientSize = new Size(880, 510);
            this.MinimumSize = new Size(880, 510);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.AutoScroll = true;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            this.ResumeLayout(false);
        }

        // Vẽ hình tròn cho avatar
        private void PnlAvatar_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(180, 210, 240)))
            {
                e.Graphics.FillEllipse(brush, 0, 0, pnlAvatar.Width - 1, pnlAvatar.Height - 1);
            }
        }

        // ── Logic ─────────────────────────────────────────────────────────────
        private void LoadData()
        {
            try
            {
                // Lấy tên user hiện tại từ Oracle session
                string currentUser = OracleHelper.GetCurrentUser(connectionString);
                int totalRows = 0;

                try
                {
                    OracleParameter[] parameters = new OracleParameter[]
                    {
                        new OracleParameter(":p_cursor",
                            OracleDbType.RefCursor,
                            ParameterDirection.Output)
                    };

                    DataTable dt = OracleHelper.ExecuteReader(
                        connectionString,
                        "ADMIN.SP_GET_THONGBAO",
                        parameters);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        dgvThongBao.DataSource = dt;
                        totalRows = dt.Rows.Count;

                        // Tìm cột không phân biệt hoa/thường — tránh NullReferenceException
                        // khi ODP.NET trả về tên cột khác case so với SQL
                        DataGridViewColumn FindCol(string name) =>
                            dgvThongBao.Columns
                                .Cast<DataGridViewColumn>()
                                .FirstOrDefault(c => string.Equals(
                                    c.Name, name, StringComparison.OrdinalIgnoreCase));

                        var colMaTB = FindCol("MATB");
                        var colNoiDung = FindCol("NOIDUNG");
                        var colNgayGio = FindCol("NGAYGIO");
                        var colDiaDiem = FindCol("DIADIEM");

                        // Phải tắt Fill trước khi set Width thủ công,
                        // nếu không sẽ NullReferenceException bên trong WinForms
                        dgvThongBao.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                        if (colMaTB != null) colMaTB.HeaderText = "Mã TB";
                        if (colNoiDung != null) colNoiDung.HeaderText = "Nội dung";
                        if (colNgayGio != null)
                        {
                            colNgayGio.HeaderText = "Ngày giờ";
                            colNgayGio.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                            colNgayGio.Width = 130;
                        }
                        if (colDiaDiem != null)
                        {
                            colDiaDiem.HeaderText = "Địa điểm";
                            colDiaDiem.Width = 150;
                        }

                        // Bật lại Fill cho cột Nội dung để chiếm phần còn lại
                        if (colNoiDung != null)
                            colNoiDung.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                        pnlEmpty.Visible = false;
                        dgvThongBao.Visible = true;
                    }
                    else
                    {
                        dgvThongBao.Visible = false;
                        pnlEmpty.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    pnlEmpty.Visible = false;
                    dgvThongBao.Visible = true;
                    MessageBox.Show(
                        $"Lỗi tải dữ liệu:\n{ex.Message}",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                // Cập nhật welcome label
                lblWelcome.Text = $"Xin chào  {currentUser}   —   {totalRows} thông báo";

                // Cập nhật avatar initials (2 ký tự đầu)
                lblAvatarText.Text = currentUser.Length >= 2
                    ? currentUser.Substring(0, 2).ToUpper()
                    : currentUser.ToUpper();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi kết nối:\n{ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ── Event Handlers ────────────────────────────────────────────────────
        private void BtnLamMoi_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void BtnDangXuat_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes) return;

            this.Hide();
            LogoutRequested?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
    }
}