using System;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Presenters;

namespace ATBM_Project.Views
{
    public partial class FormAuditLog : Form
    {
        private Label lblTitle;
        private Button btnStandard;
        private Button btnFGA;
        private DataGridView dgvAudit;
        private Panel pnlHeader;

        private AuditPresenter presenter = new AuditPresenter();

        public FormAuditLog()
        {
            InitializeComponent();
            this.Controls.Clear();
            InitializeCustomComponents();

            dgvAudit.DataSource = presenter.GetStandardAudit();
            FormatGridColumns();

            btnStandard.Click += (s, e) => {
                dgvAudit.DataSource = presenter.GetStandardAudit();
                FormatGridColumns();
            };

            btnFGA.Click += (s, e) => {
                dgvAudit.DataSource = presenter.GetFGAAudit();
                FormatGridColumns();
            };
        }

        private void FormatGridColumns()
        {
            if (dgvAudit.Columns["USERNAME"] != null) dgvAudit.Columns["USERNAME"].HeaderText = "Người dùng";
            if (dgvAudit.Columns["ACTION_NAME"] != null) dgvAudit.Columns["ACTION_NAME"].HeaderText = "Thao tác";
            if (dgvAudit.Columns["OBJ_NAME"] != null) dgvAudit.Columns["OBJ_NAME"].HeaderText = "Bảng dữ liệu";
            if (dgvAudit.Columns["RETURNCODE"] != null) dgvAudit.Columns["RETURNCODE"].HeaderText = "Trạng thái";

            if (dgvAudit.Columns["DB_USER"] != null) dgvAudit.Columns["DB_USER"].HeaderText = "Người dùng";
            if (dgvAudit.Columns["OBJECT_NAME"] != null) dgvAudit.Columns["OBJECT_NAME"].HeaderText = "Bảng dữ liệu";

            if (dgvAudit.Columns["POLICY_NAME"] != null) dgvAudit.Columns["POLICY_NAME"].HeaderText = "Quy tắc vi phạm";

            if (dgvAudit.Columns["SQL_TEXT"] != null)
            {
                dgvAudit.Columns["SQL_TEXT"].HeaderText = "Câu lệnh truy vấn (SQL)";
                dgvAudit.Columns["SQL_TEXT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            if (dgvAudit.Columns["TIMESTAMP"] != null)
            {
                dgvAudit.Columns["TIMESTAMP"].HeaderText = "Thời gian ghi nhận";
                dgvAudit.Columns["TIMESTAMP"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
                dgvAudit.Columns["TIMESTAMP"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            }
        }

        private void InitializeCustomComponents()
        {
            this.SuspendLayout();

            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.FromArgb(245, 246, 248) };
            lblTitle = new Label { Text = "🛡️ NHẬT KÝ KIỂM TOÁN HỆ THỐNG", Font = new Font("Segoe UI", 14F, FontStyle.Bold), ForeColor = Color.FromArgb(41, 53, 65), Location = new Point(20, 20), Size = new Size(350, 30), TextAlign = ContentAlignment.MiddleLeft };
            btnStandard = StyleButton("Xem Standard Audit", 380);
            btnFGA = StyleButton("Xem FGA Audit", 570);
            btnFGA.Width = 180;

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(btnStandard);
            pnlHeader.Controls.Add(btnFGA);

            dgvAudit = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, AllowUserToAddRows = false, RowHeadersVisible = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, EnableHeadersVisualStyles = false };
            dgvAudit.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvAudit.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 53, 65);
            dgvAudit.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAudit.ColumnHeadersHeight = 40;
            dgvAudit.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvAudit.DefaultCellStyle.SelectionBackColor = Color.FromArgb(51, 153, 255);
            dgvAudit.RowTemplate.Height = 30;

            dgvAudit.CellDoubleClick += dgvAudit_CellDoubleClick;
            dgvAudit.CellFormatting += dgvAudit_CellFormatting;

            this.Controls.Add(dgvAudit);
            this.Controls.Add(pnlHeader);
            dgvAudit.BringToFront();

            this.Text = "Audit Logs";
            this.BackColor = Color.White;
            this.ResumeLayout(false);
        }

        private Button StyleButton(string text, int xPos)
        {
            return new Button { Text = text, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), BackColor = Color.FromArgb(41, 53, 65), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(180, 40), Location = new Point(xPos, 15), Cursor = Cursors.Hand };
        }
        private void dgvAudit_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null) return;

            string columnName = dgvAudit.Columns[e.ColumnIndex].Name;
            if (columnName == "RETURNCODE")
            {
                string statusCode = e.Value.ToString();
                if (statusCode == "0")
                {
                    e.Value = "✅ Thành công";
                    e.CellStyle.ForeColor = Color.Green;
                    e.CellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                }
                else
                {
                    if (statusCode == "1017") e.Value = "❌ Đăng nhập sai (ORA-1017)";
                    else e.Value = $"❌ Thất bại (Mã lỗi: {statusCode})";

                    e.CellStyle.ForeColor = Color.Red;
                    e.CellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                }
                e.FormattingApplied = true;
            }

            if (columnName == "POLICY_NAME")
            {
                string policyName = e.Value.ToString();
                switch (policyName.ToUpper())
                {
                    case "FGA_AUDIT_ILLEGAL_UPDATE_HSBA":
                        e.Value = "🚨 Sửa hồ sơ bệnh án sai thẩm quyền";
                        e.CellStyle.ForeColor = Color.DarkOrange;
                        break;
                    case "FGA_AUDIT_UPDATE_DONTHUOC":
                        e.Value = "💊 Sửa đổi đơn thuốc trái phép";
                        e.CellStyle.ForeColor = Color.Purple;
                        break;
                    case "FGA_AUDIT_ILLEGAL_DML_HSBADV":
                        e.Value = "🔬 Thao tác trái phép dịch vụ bệnh án";
                        e.CellStyle.ForeColor = Color.Crimson;
                        break;
                    default:
                        e.Value = "🔍 " + policyName;
                        break;
                }
                e.CellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                e.FormattingApplied = true;
            }
        }

        private void dgvAudit_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string columnName = dgvAudit.Columns[e.ColumnIndex].Name;
                if (columnName == "SQL_TEXT")
                {
                    string sqlCode = dgvAudit.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
                    if (!string.IsNullOrEmpty(sqlCode)) showSqlPopup(sqlCode);
                }
            }
        }

        private void showSqlPopup(string sqlCode)
        {
            Form popup = new Form { Text = "Chi tiết câu lệnh SQL", Size = new Size(600, 300), StartPosition = FormStartPosition.CenterParent, ShowIcon = false, BackColor = Color.White };
            TextBox txtCode = new TextBox { Multiline = true, ReadOnly = true, Dock = DockStyle.Fill, Font = new Font("Consolas", 12F), BackColor = Color.FromArgb(41, 53, 65), ForeColor = Color.White, Text = sqlCode, ScrollBars = ScrollBars.Vertical };
            Panel pnlPadding = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            pnlPadding.Controls.Add(txtCode);
            popup.Controls.Add(pnlPadding);
            popup.ShowDialog();
        }
    }
}