using System;
using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Presenters;
using ATBM_Project.Utilities;

namespace ATBM_Project.Views
{
    public class FormUser : Form
    {
        private Label lblUsers;
        private Button btnUserCreate, btnUserUpdate, btnUserDelete, btnUserView;
        private DataGridView dgvUsers;
        private TextBox txtUserSearch;
        private Button btnUserSearch;
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
                MessageBox.Show(ex.Message);
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
            this.btnUserCreate.Click += BtnUserCreate_Click;

            this.btnUserUpdate = CreateActionButton("Update", 120, btnY);
            this.btnUserUpdate.Click += BtnUserUpdate_Click;

            this.btnUserDelete = CreateActionButton("Delete", 220, btnY);
            this.btnUserDelete.Click += BtnUserDelete_Click;

            this.btnUserView = CreateActionButton("View", 320, btnY);
            this.btnUserView.Click += BtnUserView_Click;

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

            this.txtUserSearch = new TextBox() { Location = new Point(450, 70), Width = 200, Font = new Font("Segoe UI", 10F) };
            this.btnUserSearch = CreateActionButton("Search", 660, 65);
            this.btnUserSearch.Width = 100;
            this.btnUserSearch.Click += (s, e) => {
                string kw = txtUserSearch.Text.Trim();
                if (string.IsNullOrEmpty(kw)) { LoadData(); return; }
                try
                {
                    dgvUsers.DataSource = new UserPresenter().SearchUsers(kw);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            };

            this.Controls.Add(lblUsers);
            this.Controls.Add(btnUserCreate); this.Controls.Add(btnUserUpdate);
            this.Controls.Add(btnUserDelete); this.Controls.Add(btnUserView);
            this.Controls.Add(dgvUsers);
            this.Controls.Add(txtUserSearch);
            this.Controls.Add(btnUserSearch);

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

        private void BtnUserCreate_Click(object sender, EventArgs e)
        {
            string username = Prompt.ShowDialog("Username:", "Create User");
            if (string.IsNullOrEmpty(username)) return;
            string password = Prompt.ShowDialog("Password:", "Create User");
            if (string.IsNullOrEmpty(password)) return;

            try
            {
                new UserPresenter().CreateUser(username, password);
                MessageBox.Show("Success");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnUserUpdate_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0) return;
            string username = dgvUsers.SelectedRows[0].Cells["Username"].Value.ToString();
            string newPassword = Prompt.ShowDialog($"New password for {username}:", "Change Password");
            if (string.IsNullOrEmpty(newPassword)) return;

            try
            {
                new UserPresenter().ChangePassword(username, newPassword);
                MessageBox.Show("Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnUserDelete_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0) return;
            string username = dgvUsers.SelectedRows[0].Cells["Username"].Value.ToString();

            if (MessageBox.Show($"Drop user {username}?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    new UserPresenter().DropUser(username);
                    MessageBox.Show("Success");
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void BtnUserView_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}