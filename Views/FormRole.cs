using System;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Presenters;
using ATBM_Project.Utilities;

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
                RolePresenter presenter = new RolePresenter();
                dgvRoles.DataSource = presenter.GetRoles();
                if (dgvRoles.Columns.Count > 0)
                {
                    dgvRoles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InitializeComponent()
        {
            this.ClientSize = new Size(800, 600);
            this.BackColor = Color.WhiteSmoke;
            Font headerFont = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblRoles = new Label() { Text = "Roles Management", Location = new Point(20, 20), AutoSize = true, Font = headerFont, ForeColor = Color.FromArgb(41, 53, 65) };

            int btnY = 65;
            this.btnRoleCreate = CreateActionButton("Create", 20, btnY);
            this.btnRoleCreate.Click += BtnRoleCreate_Click;

            this.btnRoleUpdate = CreateActionButton("Update", 120, btnY);
            this.btnRoleUpdate.Enabled = false;

            this.btnRoleDelete = CreateActionButton("Delete", 220, btnY);
            this.btnRoleDelete.Click += BtnRoleDelete_Click;

            this.btnRoleView = CreateActionButton("View", 320, btnY);
            this.btnRoleView.Click += BtnRoleView_Click;

            this.dgvRoles = new DataGridView();
            this.dgvRoles.Location = new Point(20, 115);
            this.dgvRoles.Size = new Size(740, 450);
            this.dgvRoles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvRoles.BackgroundColor = Color.White;
            this.dgvRoles.BorderStyle = BorderStyle.None;
            this.dgvRoles.RowHeadersVisible = false;
            this.dgvRoles.AllowUserToAddRows = false;
            this.dgvRoles.AllowUserToResizeColumns = false;
            this.dgvRoles.AllowUserToResizeRows = false;
            this.dgvRoles.ReadOnly = true;
            this.dgvRoles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvRoles.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            this.dgvRoles.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.dgvRoles.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 53, 65);
            this.dgvRoles.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            this.dgvRoles.EnableHeadersVisualStyles = false;

            this.Controls.Add(lblRoles);
            this.Controls.Add(btnRoleCreate); this.Controls.Add(btnRoleUpdate);
            this.Controls.Add(btnRoleDelete); this.Controls.Add(btnRoleView);
            this.Controls.Add(dgvRoles);

            this.Text = "FormRole";
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

        private void BtnRoleCreate_Click(object sender, EventArgs e)
        {
            string roleName = Prompt.ShowDialog("Role Name:", "Create Role");
            if (string.IsNullOrEmpty(roleName)) return;

            try
            {
                new RolePresenter().CreateRole(roleName);
                MessageBox.Show("Success");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnRoleDelete_Click(object sender, EventArgs e)
        {
            if (dgvRoles.SelectedRows.Count == 0) return;
            string roleName = dgvRoles.SelectedRows[0].Cells["RoleName"].Value.ToString();

            if (MessageBox.Show($"Drop role {roleName}?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    new RolePresenter().DropRole(roleName);
                    MessageBox.Show("Success");
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void BtnRoleView_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}