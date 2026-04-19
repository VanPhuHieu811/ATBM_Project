using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ATBM_Project.Presenters;

namespace ATBM_Project.Views
{
    public class FormGrantRoles : Form
    {
        private GroupBox groupBox1, groupBox2;
        private Label lblTarget;
        private RadioButton rdoUser, rdoRole;
        private ComboBox cbGrantee, cbRole;
        private CheckBox chkWithAdminOption;
        private Button btnGrantRole;
        private PrivilegePresenter _presenter;

        public FormGrantRoles()
        {
            InitializeComponent();
            _presenter = new PrivilegePresenter();

            // Cấu hình để nhúng vào FormMain
            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
        }

        private void InitializeComponent()
        {
            this.Size = new Size(650, 520);
            this.BackColor = Color.WhiteSmoke;
            Font fNormal = new Font("Segoe UI", 9F);
            Font fBold = new Font("Segoe UI", 9F, FontStyle.Bold);

            // --- 1. Đối tượng nhận Role ---
            groupBox1 = new GroupBox { Text = "1. Đối tượng nhận Role", Location = new Point(20, 20), Size = new Size(280, 160), Font = fBold };
            rdoUser = new RadioButton { Text = "User", Location = new Point(25, 40), Checked = true, AutoSize = true, Font = fNormal };
            rdoRole = new RadioButton { Text = "Role", Location = new Point(130, 40), AutoSize = true, Font = fNormal };

            lblTarget = new Label { Text = "Danh sách đối tượng:", Location = new Point(20, 80), AutoSize = true, Font = fNormal };
            cbGrantee = new ComboBox { Location = new Point(20, 105), Width = 240, DropDownStyle = ComboBoxStyle.DropDownList, Font = fNormal };
            groupBox1.Controls.AddRange(new Control[] { rdoUser, rdoRole, lblTarget, cbGrantee });

            // --- 2. Chọn Role để cấp ---
            groupBox2 = new GroupBox { Text = "2. Chọn Role cần cấp", Location = new Point(320, 20), Size = new Size(280, 110), Font = fBold };
            Label lblRoleList = new Label { Text = "Danh sách Role hiện có:", Location = new Point(20, 35), AutoSize = true, Font = fNormal };
            cbRole = new ComboBox { Location = new Point(20, 60), Width = 240, DropDownStyle = ComboBoxStyle.DropDownList, Font = fNormal };
            groupBox2.Controls.AddRange(new Control[] { lblRoleList, cbRole });

            // --- Quyền ADMIN OPTION ---
            chkWithAdminOption = new CheckBox
            {
                Text = "WITH ADMIN OPTION",
                Location = new Point(340, 145),
                AutoSize = true,
                Font = fBold,
                ForeColor = Color.DarkBlue
            };

            // --- Nút bấm ---
            btnGrantRole = new Button
            {
                Text = "THỰC HIỆN GÁN ROLE",
                Location = new Point(210, 230),
                Size = new Size(220, 50),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            // Sự kiện
            this.Load += FormGrantRoles_Load;
            rdoUser.CheckedChanged += (s, e) => { if (rdoUser.Checked) LoadGrantee(); };
            rdoRole.CheckedChanged += (s, e) => { if (rdoRole.Checked) LoadGrantee(); };
            btnGrantRole.Click += btnGrantRole_Click;

            this.Controls.AddRange(new Control[] { groupBox1, groupBox2, chkWithAdminOption, btnGrantRole});
        }

        private void FormGrantRoles_Load(object sender, EventArgs e)
        {
            LoadGrantee();
            cbRole.Items.Clear();
            foreach (var role in _presenter.GetRoles()) cbRole.Items.Add(role);
            if (cbRole.Items.Count > 0) cbRole.SelectedIndex = 0;
        }

        private void LoadGrantee()
        {
            cbGrantee.Items.Clear();
            var list = rdoUser.Checked ? _presenter.GetUsers() : _presenter.GetRoles();
            foreach (var item in list) cbGrantee.Items.Add(item);
            if (cbGrantee.Items.Count > 0) cbGrantee.SelectedIndex = 0;
        }

        private void btnGrantRole_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(cbGrantee.Text) || string.IsNullOrEmpty(cbRole.Text))
                {
                    MessageBox.Show("Vui lòng chọn đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // CHẶN LỖI ORA-01934: Circular role grant
                if (rdoRole.Checked && cbGrantee.Text == cbRole.Text)
                {
                    MessageBox.Show($"Lỗi logic: Không thể gán Role '{cbRole.Text}' cho chính nó!",
                                    "Cảnh báo vòng lặp", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _presenter.ExecuteGrantRole(cbGrantee.Text, cbRole.Text, chkWithAdminOption.Checked);
                MessageBox.Show($"Đã gán Role '{cbRole.Text}' cho '{cbGrantee.Text}' thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}