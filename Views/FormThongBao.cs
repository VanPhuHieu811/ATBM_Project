using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using ATBM_Project.Utilities;

namespace ATBM_Project.Views
{
    /// <summary>
    /// Form xem thông báo OLS cho các user (U1_BGD đến U8_NV)
    /// OLS tự động lọc dữ liệu theo session label của user
    /// </summary>
    public class FormThongBao : Form
    {
        private string connectionString;
        private Label lblWelcome;
        private DataGridView dgvThongBao;
        private Panel pnlEmpty;
        private Button btnLamMoi;
        private Button btnDangXuat;

        public FormThongBao(string connStr)
        {
            connectionString = connStr;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Welcome label
            lblWelcome = new Label();
            lblWelcome.Text = "Xin chào...";
            lblWelcome.Location = new Point(15, 15);
            lblWelcome.AutoSize = false;
            lblWelcome.Width = 700;
            lblWelcome.Height = 30;
            lblWelcome.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblWelcome.ForeColor = Color.SteelBlue;

            // Refresh button
            btnLamMoi = new Button();
            btnLamMoi.Text = "🔄 Làm mới";
            btnLamMoi.Location = new Point(720, 15);
            btnLamMoi.Width = 100;
            btnLamMoi.Height = 30;
            btnLamMoi.Font = new Font("Segoe UI", 9F);
            btnLamMoi.BackColor = Color.SteelBlue;
            btnLamMoi.ForeColor = Color.White;
            btnLamMoi.FlatStyle = FlatStyle.Flat;
            btnLamMoi.FlatAppearance.BorderSize = 0;
            btnLamMoi.Cursor = Cursors.Hand;
            btnLamMoi.Click += BtnLamMoi_Click;

            // DataGridView
            dgvThongBao = new DataGridView();
            dgvThongBao.Location = new Point(15, 55);
            dgvThongBao.Width = 805;
            dgvThongBao.Height = 400;
            dgvThongBao.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvThongBao.ReadOnly = true;
            dgvThongBao.RowHeadersVisible = false;
            dgvThongBao.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;
            dgvThongBao.BackgroundColor = Color.White;
            dgvThongBao.BorderStyle = BorderStyle.Fixed3D;
            dgvThongBao.Font = new Font("Segoe UI", 9F);
            dgvThongBao.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvThongBao.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 53, 65);
            dgvThongBao.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            // Empty panel
            pnlEmpty = new Panel();
            pnlEmpty.Location = new Point(15, 55);
            pnlEmpty.Width = 805;
            pnlEmpty.Height = 400;
            pnlEmpty.BackColor = Color.AliceBlue;
            pnlEmpty.BorderStyle = BorderStyle.Fixed3D;
            pnlEmpty.Visible = false;

            Label lblNoData = new Label();
            lblNoData.Text = "Không có thông báo nào dành cho bạn";
            lblNoData.AutoSize = false;
            lblNoData.Width = 805;
            lblNoData.Height = 50;
            lblNoData.TextAlign = ContentAlignment.MiddleCenter;
            lblNoData.Font = new Font("Segoe UI", 12F, FontStyle.Italic);
            lblNoData.ForeColor = Color.Gray;
            pnlEmpty.Controls.Add(lblNoData);

            // Add controls
            this.Controls.Add(lblWelcome);
            this.Controls.Add(btnLamMoi);
            this.Controls.Add(dgvThongBao);
            this.Controls.Add(pnlEmpty);

            this.ResumeLayout(false);
            this.BackColor = Color.White;
            this.AutoScroll = true;
        }

        private void LoadData()
        {
            try
            {
                // Lấy tên user hiện tại
                string currentUser = OracleHelper.GetCurrentUser(connectionString);
                int totalRows = 0;

                try
                {
                    // Gọi SP_GET_THONGBAO
                    OracleParameter[] parameters = new OracleParameter[]
                    {
                        new OracleParameter(":p_cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output)
                    };

                    DataTable dt = OracleHelper.ExecuteReader(connectionString, "ADMIN.SP_GET_THONGBAO", parameters);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        // Bind DataGridView
                        dgvThongBao.DataSource = dt;
                        totalRows = dt.Rows.Count;

                        // Rename columns
                        dgvThongBao.Columns["MATB"].HeaderText = "Mã TB";
                        dgvThongBao.Columns["NOIDUNG"].HeaderText = "Nội dung";
                        dgvThongBao.Columns["NGAYGIO"].HeaderText = "Ngày giờ";
                        dgvThongBao.Columns["DIADIEM"].HeaderText = "Địa điểm";

                        // Format datetime
                        dgvThongBao.Columns["NGAYGIO"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

                        // Ẩn panel rỗng
                        pnlEmpty.Visible = false;
                        dgvThongBao.Visible = true;
                    }
                    else
                    {
                        // Hiển thị panel rỗng
                        dgvThongBao.Visible = false;
                        pnlEmpty.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    // Nếu gọi SP bị lỗi, hiển thị lỗi
                    pnlEmpty.Visible = false;
                    dgvThongBao.Visible = true;
                    MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Update welcome label
                lblWelcome.Text = $"Xin chào {currentUser} — Bạn có {totalRows} thông báo";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLamMoi_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        // Override Form_Load
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Nếu cần xử lý thêm khi form load
        }
    }
}
