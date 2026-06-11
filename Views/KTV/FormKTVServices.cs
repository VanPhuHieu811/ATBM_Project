using ATBM_Project.Models;
using ATBM_Project.Presenters;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace ATBM_Project.Views.KTV
{
    // Đặt tên là FormKTVServices để phân biệt rõ với FormKTVMain
    public class FormKTVServices : Form
    {
        private DataGridView dgvServices;
        private Button btnUpdate, btnRefresh;
        private Label lblTitle;
        private KTVPresenter presenter;

        public FormKTVServices()
        {
            presenter = new KTVPresenter();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            //this.BackColor = Color.White; // Nền trắng sạch sẽ

            //Font headerFont = new Font("Segoe UI", 14F, FontStyle.Bold);
            //lblTitle = new Label() { Text = "DANH SÁCH DỊCH VỤ ĐƯỢC ĐIỀU PHỐI", Location = new Point(20, 20), AutoSize = true, Font = headerFont, ForeColor = Color.SteelBlue };

            this.ClientSize = new Size(800, 600);
            this.BackColor = Color.WhiteSmoke;
            Font headerFont = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblTitle = new Label() { Text = "DANH SÁCH DỊCH VỤ ĐƯỢC ĐIỀU PHỐI", Location = new Point(20, 20), AutoSize = true, Font = headerFont, ForeColor = Color.FromArgb(41, 53, 65) };

            dgvServices = new DataGridView();
            dgvServices.Location = new Point(20, 70);
            dgvServices.Size = new Size(760, 400);
            dgvServices.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            dgvServices.BackgroundColor = Color.White;
            dgvServices.BorderStyle = BorderStyle.None;
            dgvServices.RowHeadersVisible = false;
            dgvServices.AllowUserToAddRows = false;
            dgvServices.AllowUserToResizeColumns = false;
            dgvServices.AllowUserToResizeRows = false;
            dgvServices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvServices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvServices.MultiSelect = false;
            dgvServices.ReadOnly = true;
            dgvServices.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvServices.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvServices.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 53, 65);
            dgvServices.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvServices.EnableHeadersVisualStyles = false;


            // Thêm Anchor Bottom để nút bấm luôn trôi xuống đáy màn hình khi kéo giãn cửa sổ
            btnUpdate = new Button() { Text = "Cập nhật kết quả", Location = new Point(20, 490), Size = new Size(150, 35), BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), Cursor = Cursors.Hand, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            btnUpdate.Click += BtnUpdate_Click;

            btnRefresh = new Button() { Text = "Làm mới", Location = new Point(180, 490), Size = new Size(100, 35), BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Regular), Cursor = Cursors.Hand, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            btnRefresh.Click += (s, e) => LoadData();

            this.Controls.Add(lblTitle);
            this.Controls.Add(dgvServices);
            this.Controls.Add(btnUpdate);
            this.Controls.Add(btnRefresh);
        }

        private void LoadData()
        {
            try
            {
                dgvServices.DataSource = presenter.GetAssignedServices();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvServices.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một dịch vụ để cập nhật!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            KTVServiceModel currentRow = (KTVServiceModel)dgvServices.SelectedRows[0].DataBoundItem;

            FormEditResult editForm = new FormEditResult(currentRow, presenter);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }
    }
}