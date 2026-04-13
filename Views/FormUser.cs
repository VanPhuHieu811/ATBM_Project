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
            this.lblUsers = new Label() { Text = "Users", Location = new Point(20, 20), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };

            int btnY = 50;
            this.btnUserCreate = CreateActionButton("Create", 20, btnY);
            this.btnUserUpdate = CreateActionButton("Update", 110, btnY);
            this.btnUserDelete = CreateActionButton("Delete", 200, btnY);
            this.btnUserView = CreateActionButton("View", 290, btnY);

            this.dgvUsers = new DataGridView();
            this.dgvUsers.Location = new Point(20, 90);
            this.dgvUsers.Size = new Size(760, 480);
            this.dgvUsers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            this.Controls.Add(lblUsers);
            this.Controls.Add(btnUserCreate); this.Controls.Add(btnUserUpdate);
            this.Controls.Add(btnUserDelete); this.Controls.Add(btnUserView);
            this.Controls.Add(dgvUsers);

            this.Text = "FormUser";
            this.BackColor = Color.White;
        }

        private Button CreateActionButton(string text, int x, int y)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new Point(x, y);
            btn.Size = new Size(80, 30);
            return btn;
        }
    }
}