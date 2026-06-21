using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ATBM_Project.Presenters;

namespace ATBM_Project.Views
{
    public class FormBackupRestore : Form
    {
        private GroupBox gbConfig, gbConsole;
        private ComboBox cbMethods, cbTables;
        private DateTimePicker dtpFlashback;
        private Button btnBackup, btnRestore;
        private RichTextBox rtbConsoleLog;
        private Label lblTable, lblTime;

        private BackupPresenter _presenter;

        public FormBackupRestore()
        {
            _presenter = new BackupPresenter();
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
        }

        private void InitializeComponent()
        {
            this.Text = "Sao lưu & Khôi phục sự cố";
            this.Size = new Size(950, 560);
            this.BackColor = Color.WhiteSmoke;

            Font labelFont = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            Font groupFont = new Font("Segoe UI", 9.5F, FontStyle.Bold);

            this.gbConfig = new GroupBox()
            {
                Text = "1. Cấu hình giải pháp quản trị",
                Location = new Point(20, 10),
                Size = new Size(910, 160),
                Font = groupFont,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            Label lblMethod = new Label { Text = "Phương pháp:", Location = new Point(20, 35), AutoSize = true, Font = labelFont };
            // Đẩy tọa độ X từ 130 lên 160 để thẳng hàng và không bị đè chữ
            this.cbMethods = new ComboBox() { Location = new Point(160, 32), Width = 410, DropDownStyle = ComboBoxStyle.DropDownList, Font = labelFont };
            this.cbMethods.Items.AddRange(new string[] {
                "1. Oracle Data Pump (Sao lưu Logic mức Bảng)",
                "2. Oracle RMAN (Sao lưu Vật lý toàn hệ thống)",
                "3. Oracle Flashback (Lùi thời gian theo log Audit)"
            });
            this.cbMethods.SelectedIndex = 0;

            this.lblTable = new Label { Text = "Chọn bảng dữ liệu:", Location = new Point(20, 75), AutoSize = true, Font = labelFont };
            // Đẩy tọa độ X lên 160
            this.cbTables = new ComboBox() { Location = new Point(160, 72), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList, Font = labelFont };

            this.lblTime = new Label { Text = "Mốc thời gian lùi:", Location = new Point(330, 75), AutoSize = true, Font = labelFont, Visible = false };
            this.dtpFlashback = new DateTimePicker() { Location = new Point(460, 72), Width = 190, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss", Font = labelFont, Visible = false };

            this.btnBackup = new Button() { Text = "🚀 THỰC THI SAO LƯU", Location = new Point(160, 115), Size = new Size(180, 35), BackColor = Color.SteelBlue, ForeColor = Color.White, Font = groupFont, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            this.btnRestore = new Button() { Text = "⏪ KHÔI PHỤC DỮ LIỆU", Location = new Point(360, 115), Size = new Size(180, 35), BackColor = Color.DarkRed, ForeColor = Color.White, Font = groupFont, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };

            this.gbConfig.Controls.AddRange(new Control[] { lblMethod, cbMethods, lblTable, cbTables, lblTime, dtpFlashback, btnBackup, btnRestore });

            this.gbConsole = new GroupBox()
            {
                Text = "2. Nhật ký luồng tiến trình hệ thống (Console)",
                Location = new Point(20, 180),
                Size = new Size(910, 360),
                Font = groupFont,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            this.rtbConsoleLog = new RichTextBox()
            {
                Location = new Point(15, 25),
                Size = new Size(880, 320),
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.LightGreen,
                Font = new Font("Consolas", 9.5F),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            this.gbConsole.Controls.Add(rtbConsoleLog);

            this.Load += (s, e) => {
                foreach (DataRow row in _presenter.GetTables().Rows)
                {
                    cbTables.Items.Add(row["TABLE_NAME"].ToString());
                }
                if (cbTables.Items.Count > 0) cbTables.SelectedIndex = 0;
            };

            this.cbMethods.SelectedIndexChanged += (s, e) => {
                int sel = cbMethods.SelectedIndex;
                lblTable.Visible = cbTables.Visible = (sel == 0 || sel == 2);
                lblTime.Visible = dtpFlashback.Visible = (sel == 2);
                btnBackup.Enabled = !(sel == 2);
            };

            this.btnBackup.Click += BtnBackup_Click;
            this.btnRestore.Click += BtnRestore_Click;

            this.Controls.AddRange(new Control[] { gbConfig, gbConsole });
        }

        private void BtnBackup_Click(object sender, EventArgs e)
        {
            rtbConsoleLog.Clear();
            LogToConsole("=== BẮT ĐẦU TIẾN TRÌNH SAO LƯU ===");
            if (!Directory.Exists(@"C:\ATBM_Backup")) Directory.CreateDirectory(@"C:\ATBM_Backup");

            string tableName = cbTables.Text;
            if (cbMethods.SelectedIndex == 0 && !string.IsNullOrEmpty(tableName))
            {
                LogToConsole($"[*] Đang gọi tiến trình nền Data Pump Export cho bảng: {tableName}...");
                ExecuteExternalCommand("expdp.exe", $"/C expdp admin/password@XEPDB1 tables=admin.{tableName} directory=ATBM_DIR dumpfile=DP_{tableName}.dmp logfile=DP_exp_{tableName}.log reuse_dumpfiles=y");
            }
            else if (cbMethods.SelectedIndex == 1)
            {
                LogToConsole("[*] Đang gọi tiến trình lõi RMAN: Đóng gói ảnh vật lý hệ thống...");
                File.WriteAllText(@"C:\ATBM_Backup\rman_backup.txt", "backup database plus archivelog;");
                ExecuteExternalCommand("cmd.exe", $"/C rman target / cmdfile='C:\\ATBM_Backup\\rman_backup.txt' log='C:\\ATBM_Backup\\rman.log'");
                if (File.Exists(@"C:\ATBM_Backup\rman.log")) rtbConsoleLog.AppendText(File.ReadAllText(@"C:\ATBM_Backup\rman.log", Encoding.UTF8));
            }
        }

        private void BtnRestore_Click(object sender, EventArgs e)
        {
            rtbConsoleLog.Clear();
            LogToConsole("=== BẮT ĐẦU TIẾN TRÌNH KHÔI PHỤC DỮ LIỆU ===");
            string tableName = cbTables.Text;

            if (cbMethods.SelectedIndex == 0 && !string.IsNullOrEmpty(tableName))
            {
                LogToConsole($"[*] Đang gọi tiến trình nền Data Pump Import ghi đè cấu trúc: {tableName}...");
                ExecuteExternalCommand("impdp.exe", $"/C impdp admin/password@XEPDB1 tables=admin.{tableName} directory=ATBM_DIR dumpfile=DP_{tableName}.dmp logfile=DP_imp_{tableName}.log table_exists_action=replace");
            }
            else if (cbMethods.SelectedIndex == 1)
            {
                LogToConsole("[*] Đang nạp giao thức phục hồi khẩn cấp RMAN (Shutdown -> Mount)...");
                File.WriteAllText(@"C:\ATBM_Backup\rman_restore.txt", "shutdown immediate; startup mount; restore database; recover database; alter database open;");
                ExecuteExternalCommand("cmd.exe", $"/C rman target / cmdfile='C:\\ATBM_Backup\\rman_restore.txt' log='C:\\ATBM_Backup\\rman_restore.log'");
                if (File.Exists(@"C:\ATBM_Backup\rman_restore.log")) rtbConsoleLog.AppendText(File.ReadAllText(@"C:\ATBM_Backup\rman_restore.log", Encoding.UTF8));
            }
            else if (cbMethods.SelectedIndex == 2 && !string.IsNullOrEmpty(tableName))
            {
                string timeStr = dtpFlashback.Value.ToString("yyyy-MM-dd HH:mm:ss");
                LogToConsole($"[*] Đang gửi yêu cầu giao thức Flashback lùi bảng [{tableName}] về mốc: {timeStr}...");

                string errorMsg = _presenter.ExecuteFlashback(tableName, timeStr);

                if (string.IsNullOrEmpty(errorMsg))
                {
                    LogToConsole($"✅ THÀNH CÔNG: Đã kích hoạt cỗ máy thời gian lùi dữ liệu thực thể {tableName} thành công.");
                    MessageBox.Show($"Khôi phục Flashback bảng {tableName} thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    LogToConsole("❌ THẤT BẠI: Lỗi phản hồi từ hệ thống Oracle:\n" + errorMsg);
                    MessageBox.Show("Lỗi Flashback: " + errorMsg, "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExecuteExternalCommand(string fileName, string arguments)
        {
            try
            {
                ProcessStartInfo procInfo = new ProcessStartInfo { FileName = "cmd.exe", Arguments = arguments, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false, CreateNoWindow = true, StandardOutputEncoding = Encoding.UTF8 };
                using (Process process = Process.Start(procInfo))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();
                    if (!string.IsNullOrEmpty(output)) LogToConsole(output);
                    if (!string.IsNullOrEmpty(error)) LogToConsole("⚠️ CẢNH BÁO TIẾN TRÌNH:\n" + error);
                }
            }
            catch (Exception ex) { LogToConsole("Lỗi kích hoạt OS: " + ex.Message); }
        }

        private void LogToConsole(string text)
        {
            rtbConsoleLog.AppendText(text + "\n");
            rtbConsoleLog.ScrollToCaret();
        }
    }
}