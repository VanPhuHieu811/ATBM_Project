using System;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Presenters;

namespace ATBM_Project.Views
{
    public partial class FormBackupRestore : Form
    {
        private Label lblTitle, lblTable, lblTime, lblPreview;
        private ComboBox cboTableName;
        private DateTimePicker dtpTime;
        private Button btnRestore;
        private GroupBox grpFlashback;
        private DataGridView dgvPreview; 

        private BackupPresenter presenter = new BackupPresenter();

        public FormBackupRestore()
        {
            InitializeComponent();
            this.Controls.Clear();
            InitializeCustomComponents();

            btnRestore.Click += BtnRestore_Click;
            cboTableName.SelectedIndexChanged += CboTableName_SelectedIndexChanged;

            LoadPreviewData();
        }

        private void InitializeCustomComponents()
        {
            this.SuspendLayout();

            lblTitle = new Label { Text = "💾 SAO LƯU VÀ PHỤC HỒI DỮ LIỆU", Font = new Font("Segoe UI", 14F, FontStyle.Bold), ForeColor = Color.FromArgb(41, 53, 65), Location = new Point(20, 15), Size = new Size(500, 30) };

            grpFlashback = new GroupBox { Text = " Tính năng Khôi phục mức dòng (Flashback Query) ", Font = new Font("Segoe UI", 10F, FontStyle.Italic), Location = new Point(25, 55), Size = new Size(730, 180), ForeColor = Color.DimGray };

            lblTable = new Label { Text = "Chọn bảng dữ liệu:", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(41, 53, 65), Location = new Point(20, 35), Size = new Size(200, 25) };
            cboTableName = new ComboBox { Font = new Font("Segoe UI", 11F), Location = new Point(20, 60), Size = new Size(330, 30), DropDownStyle = ComboBoxStyle.DropDownList };
            cboTableName.Items.AddRange(new string[] { "HSBA", "HSBA_DV", "DONTHUOC", "BENHNHAN", "NHANVIEN", "THONGBAO" });

            lblTime = new Label { Text = "Quay ngược về thời điểm:", Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(41, 53, 65), Location = new Point(380, 35), Size = new Size(250, 25) };
            dtpTime = new DateTimePicker { Font = new Font("Segoe UI", 11F), Location = new Point(380, 60), Size = new Size(330, 30), Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm:ss", Value = DateTime.Now.AddMinutes(-5) };

            btnRestore = new Button { Text = "🚀 TIẾN HÀNH KHÔI PHỤC DỮ LIỆU", Font = new Font("Segoe UI", 11F, FontStyle.Bold), BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(20, 110), Size = new Size(690, 45), Cursor = Cursors.Hand };

            grpFlashback.Controls.AddRange(new Control[] { lblTable, cboTableName, lblTime, dtpTime, btnRestore });

            lblPreview = new Label { Text = "👀 Xem trước dữ liệu hiện tại:", Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(41, 53, 65), Location = new Point(20, 250), Size = new Size(300, 25) };

            dgvPreview = new DataGridView
            {
                Location = new Point(25, 280),
                Size = new Size(730, 250),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                EnableHeadersVisualStyles = false
            };
            dgvPreview.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvPreview.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 53, 65);
            dgvPreview.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPreview.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvPreview.DefaultCellStyle.SelectionBackColor = Color.FromArgb(51, 153, 255);

            this.Controls.Add(lblPreview);
            this.Controls.Add(dgvPreview);
            this.Controls.Add(grpFlashback);
            this.Controls.Add(lblTitle);

            this.Text = "Backup & Restore";
            this.BackColor = Color.White;
            this.Size = new Size(800, 600);
            this.ResumeLayout(false);
        }

        private void CboTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPreviewData();
        }

        private void LoadPreviewData()
        {
            if (cboTableName.SelectedItem != null)
            {
                string tableName = cboTableName.SelectedItem.ToString();
                dgvPreview.DataSource = presenter.GetTablePreview(tableName);
            }
        }

        private void BtnRestore_Click(object sender, EventArgs e)
        {
            if (cboTableName.SelectedItem == null) return;
            string tableName = cboTableName.SelectedItem.ToString();
            string formattedTime = dtpTime.Value.ToString("yyyy-MM-dd HH:mm:ss");

            var confirm = MessageBox.Show($"Xác nhận chạy cơ chế Flashback đưa bảng {tableName} về thời điểm {formattedTime}?", "Xác nhận hành động", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            bool success = presenter.ExecuteFlashback(tableName, formattedTime);
            if (success)
            {
                MessageBox.Show($"Khôi phục bảng {tableName} về mốc {formattedTime} thành công! Hãy xem sự thay đổi trên lưới dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadPreviewData();
            }
            else
            {
                MessageBox.Show("Khôi phục thất bại. Hãy kiểm tra lại xem cấu trúc bảng có bị đổi không hoặc thời gian quá xa.", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}