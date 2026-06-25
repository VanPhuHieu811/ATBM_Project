using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Presenters;

namespace ATBM_Project.Views
{
    public class FormAuditLog : Form
    {
        private GroupBox gbFilter;
        private ComboBox cbAuditType, cbTables;
        private Button btnLoadData;
        private DataGridView dgvLogs;
        private Label lblTable;

        private AuditPresenter _presenter;

        public FormAuditLog()
        {
            _presenter = new AuditPresenter();
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
        }

        private void InitializeComponent()
        {
            this.Text = "Nhật ký kiểm toán hệ thống";
            this.Size = new Size(950, 560);
            this.BackColor = Color.WhiteSmoke;

            Font labelFont = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            Font groupFont = new Font("Segoe UI", 9.5F, FontStyle.Bold);

            this.gbFilter = new GroupBox()
            {
                Text = "Bộ lọc nhật ký kiểm toán",
                Location = new Point(20, 10),
                Size = new Size(910, 85),
                Font = groupFont,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            Label lblType = new Label { Text = "Loại nhật ký:", Location = new Point(15, 35), AutoSize = true, Font = labelFont };
            this.cbAuditType = new ComboBox() { Location = new Point(105, 32), Width = 190, DropDownStyle = ComboBoxStyle.DropDownList, Font = labelFont };
            this.cbAuditType.Items.AddRange(new string[] { "Standard Audit", "Fine-Grained Audit (FGA)" });
            this.cbAuditType.SelectedIndex = 0;

            this.lblTable = new Label { Text = "Chọn bảng:", Location = new Point(315, 35), AutoSize = true, Font = labelFont };
            this.cbTables = new ComboBox() { Location = new Point(395, 32), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList, Font = labelFont };

            this.btnLoadData = new Button()
            {
                Text = "TẢI NHẬT KÝ",
                Location = new Point(570, 29),
                Size = new Size(140, 32),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            this.gbFilter.Controls.AddRange(new Control[] { lblType, cbAuditType, lblTable, cbTables, btnLoadData });

            this.dgvLogs = new DataGridView()
            {
                Location = new Point(20, 110),
                Size = new Size(910, 390),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = labelFont,
                RowTemplate = { Height = 30 },
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 40,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            this.dgvLogs.CellFormatting += DgvLogs_CellFormatting;
            this.dgvLogs.CellDoubleClick += DgvLogs_CellDoubleClick;

            this.Load += (s, e) => {
                cbTables.Items.Add("--- TẤT CẢ ---");
                foreach (DataRow row in _presenter.GetTables().Rows)
                {
                    cbTables.Items.Add(row["TABLE_NAME"].ToString());
                }
                if (cbTables.Items.Count > 0) cbTables.SelectedIndex = 0;
            };

            this.btnLoadData.Click += (s, e) => {
                string tableFilter = cbTables.Text == "--- TẤT CẢ ---" ? "" : cbTables.Text;
                dgvLogs.DataSource = cbAuditType.SelectedIndex == 0 ? _presenter.GetStandardAudit(tableFilter) : _presenter.GetFGAAudit(tableFilter);
                FormatAuditGrid();
            };

            this.Controls.AddRange(new Control[] { gbFilter, dgvLogs });
        }

        private void FormatAuditGrid()
        {
            if (dgvLogs.Columns.Count == 0) return;

            if (cbAuditType.SelectedIndex == 0)
            {
                dgvLogs.Columns["USERNAME"].HeaderText = "Người dùng";
                dgvLogs.Columns["ACTION_NAME"].HeaderText = "Thao tác";
                dgvLogs.Columns["OBJ_NAME"].HeaderText = "Bảng dữ liệu";
                dgvLogs.Columns["TIMESTAMP"].HeaderText = "Thời gian";
                dgvLogs.Columns["TIMESTAMP"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
                dgvLogs.Columns["RETURNCODE"].HeaderText = "Trạng thái";

                dgvLogs.Columns["USERNAME"].FillWeight = 80;
                dgvLogs.Columns["ACTION_NAME"].FillWeight = 80;
                dgvLogs.Columns["OBJ_NAME"].FillWeight = 100;
                dgvLogs.Columns["TIMESTAMP"].FillWeight = 120;
                dgvLogs.Columns["RETURNCODE"].FillWeight = 160;
            }
            else
            {
                dgvLogs.Columns["DB_USER"].HeaderText = "Người dùng";
                dgvLogs.Columns["OBJECT_NAME"].HeaderText = "Bảng dữ liệu";
                dgvLogs.Columns["POLICY_NAME"].HeaderText = "Cảm biến FGA";
                dgvLogs.Columns["SQL_TEXT"].HeaderText = "Câu lệnh vi phạm";
                dgvLogs.Columns["TIMESTAMP"].HeaderText = "Thời gian";
                dgvLogs.Columns["TIMESTAMP"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";

                dgvLogs.Columns["DB_USER"].FillWeight = 80;
                dgvLogs.Columns["OBJECT_NAME"].FillWeight = 100;
                dgvLogs.Columns["POLICY_NAME"].FillWeight = 120;
                dgvLogs.Columns["SQL_TEXT"].FillWeight = 250;
                dgvLogs.Columns["TIMESTAMP"].FillWeight = 120;
            }
        }

        private void DgvLogs_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (cbAuditType.SelectedIndex == 0 && dgvLogs.Columns[e.ColumnIndex].Name == "RETURNCODE" && e.Value != null)
            {
                int code;
                if (int.TryParse(e.Value.ToString(), out code))
                {
                    if (code == 0)
                    {
                        e.Value = "✅ Thành công";
                        e.CellStyle.ForeColor = Color.Green;
                    }
                    else
                    {
                        e.Value = $"❌ Thất bại (Lỗi: {code})";
                        e.CellStyle.ForeColor = Color.Red;
                    }
                    e.FormattingApplied = true;
                }
            }
        }

        private void DgvLogs_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && cbAuditType.SelectedIndex == 1)
            {
                if (dgvLogs.Columns[e.ColumnIndex].Name == "SQL_TEXT")
                {
                    string sqlText = dgvLogs.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
                    if (!string.IsNullOrEmpty(sqlText))
                    {
                        MessageBox.Show(this, sqlText, "Chi tiết câu lệnh SQL", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
    }
}