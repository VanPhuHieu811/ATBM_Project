using System;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Presenters;

namespace ATBM_Project.Views
{
    public class FormUser : Form
    {
        private Label lblUsers;
        private Button btnUserCreate, btnUserUpdate, btnUserDelete, btnUserView;
        private DataGridView dgvUsers;

        public FormUser()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                UserPresenter presenter = new UserPresenter();
                dgvUsers.DataSource = presenter.GetUsers();
                if (dgvUsers.Columns.Count > 0)
                {
                    dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách User: " + ex.Message);
            }
        }

        private void InitializeComponent()
        {
            this.ClientSize = new Size(800, 600);
            this.BackColor = Color.WhiteSmoke;
            Font headerFont = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblUsers = new Label() { Text = "Users Management", Location = new Point(20, 20), AutoSize = true, Font = headerFont, ForeColor = Color.FromArgb(41, 53, 65) };

            int btnY = 65;
            this.btnUserCreate = CreateActionButton("Create", 20, btnY);
            this.btnUserUpdate = CreateActionButton("Update", 120, btnY);
            this.btnUserDelete = CreateActionButton("Delete", 220, btnY);
            this.btnUserView = CreateActionButton("View", 320, btnY);

            this.dgvUsers = new DataGridView();
            this.dgvUsers.Location = new Point(20, 115);
            this.dgvUsers.Size = new Size(740, 450);
            this.dgvUsers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvUsers.BackgroundColor = Color.White;
            this.dgvUsers.BorderStyle = BorderStyle.None;
            this.dgvUsers.RowHeadersVisible = false;
            this.dgvUsers.AllowUserToAddRows = false;
            this.dgvUsers.AllowUserToResizeColumns = false;
            this.dgvUsers.AllowUserToResizeRows = false;
            this.dgvUsers.ReadOnly = true;
            this.dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvUsers.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            this.dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 53, 65);
            this.dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            this.dgvUsers.EnableHeadersVisualStyles = false;

            this.Controls.Add(lblUsers);
            this.Controls.Add(btnUserCreate); this.Controls.Add(btnUserUpdate);
            this.Controls.Add(btnUserDelete); this.Controls.Add(btnUserView);
            this.Controls.Add(dgvUsers);

            this.Text = "FormUser";
        }

        private Button CreateActionButton(string text, int x, int y)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new Point(x, y);
            btn.Size = new Size(90, 35);
            btn.BackColor = Color.SteelBlue;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            return btn;
        }
    }
}