using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ATBM_Project.Presenters;

namespace ATBM_Project.Views
{
    public class FormBackupRestore : Form
    {
        private GroupBox gbConfig, gbPreview;
        private ComboBox cbMethods, cbTables;
        private DateTimePicker dtpFlashback;
        private Button btnBackup, btnRestore;
        private DataGridView dgvPreview;
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
                Text = "Cấu hình giải pháp quản trị",
                Location = new Point(20, 10),
                Size = new Size(910, 160),
                Font = groupFont,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            Label lblMethod = new Label { Text = "Phương pháp:", Location = new Point(20, 35), AutoSize = true, Font = labelFont };
            this.cbMethods = new ComboBox() { Location = new Point(160, 32), Width = 410, DropDownStyle = ComboBoxStyle.DropDownList, Font = labelFont };
            this.cbMethods.Items.AddRange(new string[] {
                "1. Oracle Data Pump (Sao lưu Logic mức Bảng)",
                "2. Oracle RMAN (Sao lưu Vật lý toàn hệ thống)",
                "3. Oracle Flashback (Lùi thời gian theo log Audit)"
            });
            this.cbMethods.SelectedIndex = 0;

            this.lblTable = new Label { Text = "Chọn bảng dữ liệu:", Location = new Point(20, 75), AutoSize = true, Font = labelFont };
            this.cbTables = new ComboBox() { Location = new Point(160, 72), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList, Font = labelFont };

            this.lblTime = new Label { Text = "Mốc thời gian lùi:", Location = new Point(330, 75), AutoSize = true, Font = labelFont, Visible = false };
            this.dtpFlashback = new DateTimePicker() { Location = new Point(460, 72), Width = 190, Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm:ss", Font = labelFont, Visible = false };

            this.btnBackup = new Button() { Text = "🚀 THỰC THI SAO LƯU", Location = new Point(160, 115), Size = new Size(180, 35), BackColor = Color.SteelBlue, ForeColor = Color.White, Font = groupFont, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            this.btnRestore = new Button() { Text = "⏪ KHÔI PHỤC DỮ LIỆU", Location = new Point(360, 115), Size = new Size(180, 35), BackColor = Color.DarkRed, ForeColor = Color.White, Font = groupFont, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };

            this.gbConfig.Controls.AddRange(new Control[] { lblMethod, cbMethods, lblTable, cbTables, lblTime, dtpFlashback, btnBackup, btnRestore });

            this.gbPreview = new GroupBox()
            {
                Text = "Dữ liệu bảng",
                Location = new Point(20, 180),
                Size = new Size(910, 340),
                Font = groupFont,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            this.dgvPreview = new DataGridView()
            {
                Location = new Point(15, 25),
                Size = new Size(880, 300),
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
            this.gbPreview.Controls.Add(dgvPreview);

            this.Load += (s, e) => {
                foreach (DataRow row in _presenter.GetTables().Rows)
                {
                    cbTables.Items.Add(row["TABLE_NAME"].ToString());
                }
                if (cbTables.Items.Count > 0) cbTables.SelectedIndex = 0;
            };

            this.cbTables.SelectedIndexChanged += (s, e) => { LoadTablePreview(); };

            this.cbMethods.SelectedIndexChanged += (s, e) => {
                int sel = cbMethods.SelectedIndex;
                lblTable.Visible = cbTables.Visible = (sel == 0 || sel == 2);
                lblTime.Visible = dtpFlashback.Visible = (sel == 2);
                btnBackup.Enabled = !(sel == 2);

                if (sel == 1) dgvPreview.DataSource = null;
                else LoadTablePreview();
            };

            this.btnBackup.Click += BtnBackup_Click;
            this.btnRestore.Click += BtnRestore_Click;

            this.Controls.AddRange(new Control[] { gbConfig, gbPreview });
        }

        private void LoadTablePreview()
        {
            if (!string.IsNullOrEmpty(cbTables.Text) && cbTables.Visible)
            {
                dgvPreview.DataSource = _presenter.GetTablePreview(cbTables.Text);
            }
            else
            {
                dgvPreview.DataSource = null;
            }
        }

        private string GetDbLogonString()
        {
            try
            {
                var builder = new System.Data.Common.DbConnectionStringBuilder();
                builder.ConnectionString = ATBM_Project.Data.DBConfig.ConnectionString;

                if (builder.ContainsKey("User Id") && builder.ContainsKey("Password"))
                {
                    string user = builder["User Id"].ToString();
                    string pass = builder["Password"].ToString();
                    return $"{user}/{pass}@//localhost:1521/XEPDB1";
                }
                return null;
            }
            catch { return null; }
        }

        private string GetCurrentSchema()
        {
            try
            {
                var builder = new System.Data.Common.DbConnectionStringBuilder();
                builder.ConnectionString = ATBM_Project.Data.DBConfig.ConnectionString;

                if (builder.ContainsKey("User Id"))
                {
                    return builder["User Id"].ToString().ToUpper();
                }
                return null;
            }
            catch { return null; }
        }

        private async void BtnBackup_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(@"C:\ATBM_Backup"))
            {
                Directory.CreateDirectory(@"C:\ATBM_Backup");
            }

            string tableName = cbTables.Text;
            string dbLogon = GetDbLogonString();
            string schema = GetCurrentSchema();

            if (string.IsNullOrEmpty(dbLogon) || string.IsNullOrEmpty(schema))
            {
                MessageBox.Show(this, "Không thể trích xuất thông tin đăng nhập từ chuỗi kết nối.", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnBackup.Enabled = false;
            btnRestore.Enabled = false;

            if (cbMethods.SelectedIndex == 0 && !string.IsNullOrEmpty(tableName))
            {
                string args = $"/C expdp {dbLogon} tables={schema}.{tableName} directory=ATBM_DIR dumpfile=DP_{tableName}.dmp logfile=DP_exp_{tableName}.log reuse_dumpfiles=y";
                await ExecuteExternalCommandAsync(args, $"Đã hoàn tất Data Pump Export cho bảng {tableName}.");
            }
            else if (cbMethods.SelectedIndex == 1)
            {
                File.WriteAllText(@"C:\ATBM_Backup\rman_backup.txt", "backup database plus archivelog;");
                string args = $"/C rman target / cmdfile='C:\\ATBM_Backup\\rman_backup.txt' log='C:\\ATBM_Backup\\rman.log'";
                await ExecuteExternalCommandAsync(args, "Đã hoàn tất sao lưu vật lý RMAN toàn hệ thống.");
            }

            btnBackup.Enabled = true;
            btnRestore.Enabled = true;
        }

        private async void BtnRestore_Click(object sender, EventArgs e)
        {
            string tableName = cbTables.Text;

            if (cbMethods.SelectedIndex == 0 || cbMethods.SelectedIndex == 1)
            {
                if (!Directory.Exists(@"C:\ATBM_Backup"))
                {
                    MessageBox.Show(this, @"Không tìm thấy thư mục C:\ATBM_Backup. Hệ thống không có dữ liệu để phục hồi!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            string dbLogon = GetDbLogonString();
            string schema = GetCurrentSchema();

            if (cbMethods.SelectedIndex == 0 && (string.IsNullOrEmpty(dbLogon) || string.IsNullOrEmpty(schema)))
            {
                MessageBox.Show(this, "Không thể trích xuất thông tin đăng nhập từ chuỗi kết nối.", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnBackup.Enabled = false;
            btnRestore.Enabled = false;

            if (cbMethods.SelectedIndex == 0 && !string.IsNullOrEmpty(tableName))
            {
                string dumpFile = $@"C:\ATBM_Backup\DP_{tableName}.dmp";
                if (!File.Exists(dumpFile))
                {
                    MessageBox.Show(this, $"Không tìm thấy file sao lưu logic: {dumpFile}", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnBackup.Enabled = true;
                    btnRestore.Enabled = true;
                    return;
                }

                string args = $"/C impdp {dbLogon} tables={schema}.{tableName} directory=ATBM_DIR dumpfile=DP_{tableName}.dmp logfile=DP_imp_{tableName}.log table_exists_action=replace";
                await ExecuteExternalCommandAsync(args, $"Đã hoàn tất Data Pump Import cho bảng {tableName}.");
                LoadTablePreview();
            }
            else if (cbMethods.SelectedIndex == 1)
            {
                File.WriteAllText(@"C:\ATBM_Backup\rman_restore.txt", "startup force mount;\nrestore database;\nrecover database;");
                File.WriteAllText(@"C:\ATBM_Backup\open_db.sql", "ALTER DATABASE OPEN RESETLOGS;\nALTER PLUGGABLE DATABASE ALL OPEN;\nEXIT;");

                string batPath = @"C:\ATBM_Backup\execute_rman_recovery.bat";
                string batContent = "@echo off\n" +
                                    "rman target / cmdfile=\"C:\\ATBM_Backup\\rman_restore.txt\" log=\"C:\\ATBM_Backup\\rman_restore.log\"\n" +
                                    "sqlplus / as sysdba @\"C:\\ATBM_Backup\\open_db.sql\" > \"C:\\ATBM_Backup\\open_db.log\"";
                File.WriteAllText(batPath, batContent);

                string args = $"/C \"{batPath}\"";
                await ExecuteExternalCommandAsync(args, "Đã hoàn tất toàn bộ tiến trình khôi phục RMAN. Cơ sở dữ liệu đã mở cửa tự động thành công!");
                LoadTablePreview();
            }
            else if (cbMethods.SelectedIndex == 2 && !string.IsNullOrEmpty(tableName))
            {
                string timeStr = dtpFlashback.Value.ToString("yyyy-MM-dd HH:mm:ss");
                string errorMsg = _presenter.ExecuteFlashback(tableName, timeStr);

                this.Invoke((MethodInvoker)delegate {
                    this.Activate();
                    this.BringToFront();
                    if (string.IsNullOrEmpty(errorMsg))
                    {
                        MessageBox.Show(this, $"Khôi phục Flashback bảng {tableName} thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadTablePreview();
                    }
                    else
                    {
                        MessageBox.Show(this, "Lỗi Flashback: " + errorMsg, "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
            }

            btnBackup.Enabled = (cbMethods.SelectedIndex != 2);
            btnRestore.Enabled = true;
        }

        private async Task ExecuteExternalCommandAsync(string arguments, string successMsg)
        {
            try
            {
                ProcessStartInfo procInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                };

                using (Process process = Process.Start(procInfo))
                {
                    string error = await Task.Run(() =>
                    {
                        string errText = process.StandardError.ReadToEnd();
                        process.WaitForExit();
                        return errText;
                    });

                    this.Invoke((MethodInvoker)delegate {
                        this.Activate();
                        this.BringToFront();

                        if (!string.IsNullOrEmpty(error) && error.Contains("ORA-") && !error.Contains("ORA-01109") && !error.Contains("ORA-01081"))
                            MessageBox.Show(this, "Cảnh báo tiến trình: \n" + error, "Lỗi Oracle", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        else
                            MessageBox.Show(this, successMsg, "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    });
                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate {
                    MessageBox.Show(this, "Lỗi hệ điều hành: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
        }
    }
}