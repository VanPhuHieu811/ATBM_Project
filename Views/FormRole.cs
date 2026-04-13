using System;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Presenters;

namespace ATBM_Project.Views
{
    public class FormRole : Form
    {
        private Label lblRoles;
        private Button btnRoleCreate, btnRoleUpdate, btnRoleDelete, btnRoleView;
        private DataGridView dgvRoles;

        public FormRole()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                AccountPresenter presenter = new AccountPresenter();
                dgvRoles.DataSource = presenter.GetRoles();
                if (dgvRoles.Columns.Count > 0)
                {
                    dgvRoles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách Role: " + ex.Message);
            }
        }

        private void InitializeComponent()
        {
            this.ClientSize = new Size(800, 600);
            this.lblRoles = new Label() { Text = "Roles", Location = new Point(20, 20), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };

            int btnY = 50;
            this.btnRoleCreate = CreateActionButton("Create", 20, btnY);
            this.btnRoleUpdate = CreateActionButton("Update", 110, btnY);
            this.btnRoleDelete = CreateActionButton("Delete", 200, btnY);
            this.btnRoleView = CreateActionButton("View", 290, btnY);

            this.dgvRoles = new DataGridView();
            this.dgvRoles.Location = new Point(20, 90);
            this.dgvRoles.Size = new Size(760, 480);
            this.dgvRoles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            this.Controls.Add(lblRoles);
            this.Controls.Add(btnRoleCreate); this.Controls.Add(btnRoleUpdate);
            this.Controls.Add(btnRoleDelete); this.Controls.Add(btnRoleView);
            this.Controls.Add(dgvRoles);

            this.Text = "FormRole";
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