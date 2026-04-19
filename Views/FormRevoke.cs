using System;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Presenters;

namespace ATBM_Project.Views
{
    public class FormRevoke : Form
    {
        private Button btnShowUsers, btnShowRoles;
        private DataGridView dgvList;
        private string currentMode = ""; // "USER" or "ROLE"

        public FormRevoke()
        {
            InitializeComponent();
            BtnShowUsers_Click(this, EventArgs.Empty);
        }

        private void InitializeComponent()
        {
            this.ClientSize = new Size(800, 600);
            this.BackColor = Color.WhiteSmoke;

            this.btnShowUsers = CreateActionButton("Users", 20, 20);
            this.btnShowUsers.Click += BtnShowUsers_Click;

            this.btnShowRoles = CreateActionButton("Roles", 130, 20);
            this.btnShowRoles.Click += BtnShowRoles_Click;

            this.dgvList = new DataGridView();
            this.dgvList.Location = new Point(20, 70);
            this.dgvList.Size = new Size(760, 500);
            this.dgvList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvList.BackgroundColor = Color.White;
            this.dgvList.BorderStyle = BorderStyle.None;
            this.dgvList.RowHeadersVisible = false;
            this.dgvList.AllowUserToAddRows = false;
            this.dgvList.AllowUserToResizeColumns = false;
            this.dgvList.AllowUserToResizeRows = false;
            this.dgvList.ReadOnly = true;
            this.dgvList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvList.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            this.dgvList.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.dgvList.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 53, 65);
            this.dgvList.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            this.dgvList.EnableHeadersVisualStyles = false;
            this.dgvList.CellDoubleClick += DgvList_CellDoubleClick;

            this.Controls.Add(btnShowUsers);
            this.Controls.Add(btnShowRoles);
            this.Controls.Add(dgvList);

            this.Text = "FormRevoke";
        }

        private Button CreateActionButton(string text, int x, int y)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new Point(x, y);
            btn.Size = new Size(100, 35);
            btn.BackColor = Color.SteelBlue;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        private void BtnShowUsers_Click(object sender, EventArgs e)
        {
            currentMode = "USER";
            try
            {
                UserPresenter presenter = new UserPresenter();
                dgvList.DataSource = presenter.GetUsers();
                if (dgvList.Columns.Count > 0)
                {
                    dgvList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách User: " + ex.Message);
            }
        }

        private void BtnShowRoles_Click(object sender, EventArgs e)
        {
            currentMode = "ROLE";
            try
            {
                RolePresenter presenter = new RolePresenter();
                dgvList.DataSource = presenter.GetRoles();
                if (dgvList.Columns.Count > 0)
                {
                    dgvList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách Role: " + ex.Message);
            }
        }

        private void DgvList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string selectedName = "";
                if (currentMode == "USER")
                {
                    selectedName = dgvList.Rows[e.RowIndex].Cells["Username"].Value?.ToString();
                }
                else if (currentMode == "ROLE")
                {
                    selectedName = dgvList.Rows[e.RowIndex].Cells["RoleName"].Value?.ToString();
                }

                if (!string.IsNullOrEmpty(selectedName))
                {
                    FormRevokePrivilege f = new FormRevokePrivilege(selectedName);
                    f.ShowDialog();
                }
            }
        }
    }
}