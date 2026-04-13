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
        }

        private void InitializeComponent()
        {
            this.ClientSize = new Size(800, 600);
            this.BackColor = Color.White;
            
            this.btnShowUsers = new Button() { Text = "Users", Location = new Point(20, 20), Size = new Size(100, 30) };
            this.btnShowUsers.Click += BtnShowUsers_Click;

            this.btnShowRoles = new Button() { Text = "Roles", Location = new Point(130, 20), Size = new Size(100, 30) };
            this.btnShowRoles.Click += BtnShowRoles_Click;

            this.dgvList = new DataGridView();
            this.dgvList.Location = new Point(20, 70);
            this.dgvList.Size = new Size(760, 500);
            this.dgvList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvList.ReadOnly = true;
            this.dgvList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvList.CellDoubleClick += DgvList_CellDoubleClick;

            this.Controls.Add(btnShowUsers);
            this.Controls.Add(btnShowRoles);
            this.Controls.Add(dgvList);

            this.Text = "FormRevoke";
        }

        private void BtnShowUsers_Click(object sender, EventArgs e)
        {
            currentMode = "USER";
            try
            {
                AccountPresenter presenter = new AccountPresenter();
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
                AccountPresenter presenter = new AccountPresenter();
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