using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ATBM_Project.Models;
using ATBM_Project.Presenters;

namespace ATBM_Project.Views
{
    public class FormGrantPrivileges : Form
    {
        private GroupBox groupBox1, groupBox2, groupBox3, groupBox4;
        private RadioButton rdoUser, rdoRole;
        private ComboBox cbGrantee, cbObjectType, cbObjectName;
        private CheckedListBox chkListPrivileges, chkListColumns;
        private CheckBox chkWithGrantOption;
        private Button btnGrant;
        private PrivilegePresenter _presenter;

        public FormGrantPrivileges()
        {
            _presenter = new PrivilegePresenter();
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
        }

        private void InitializeComponent()
        {
            this.Text = "Cấp quyền đối tượng";
            this.Size = new Size(650, 520);
            this.BackColor = Color.WhiteSmoke;

            Font labelFont = new Font("Segoe UI", 9F, FontStyle.Regular);
            Font groupFont = new Font("Segoe UI", 9F, FontStyle.Bold);

            // --- 1. Đối tượng nhận quyền ---
            this.groupBox1 = new GroupBox() { Text = "1. Đối tượng nhận quyền", Location = new Point(20, 20), Size = new Size(270, 150), Font = groupFont };
            this.rdoUser = new RadioButton() { Text = "User", Location = new Point(25, 35), Checked = true, AutoSize = true, Font = labelFont };
            this.rdoRole = new RadioButton() { Text = "Role", Location = new Point(130, 35), AutoSize = true, Font = labelFont };

            Label lblDs = new Label { Text = "Danh sách đối tượng:", Location = new Point(20, 70), AutoSize = true, Font = labelFont };
            this.cbGrantee = new ComboBox() { Location = new Point(20, 95), Width = 230, DropDownStyle = ComboBoxStyle.DropDownList, Font = labelFont };
            this.groupBox1.Controls.AddRange(new Control[] { rdoUser, rdoRole, lblDs, cbGrantee });

            // --- 2. Đối tượng CSDL ---
            this.groupBox2 = new GroupBox() { Text = "2. Chọn đối tượng CSDL", Location = new Point(310, 20), Size = new Size(300, 150), Font = groupFont };
            Label lblType = new Label { Text = "Loại đối tượng:", Location = new Point(20, 30), AutoSize = true, Font = labelFont };
            this.cbObjectType = new ComboBox() { Location = new Point(20, 50), Width = 260, DropDownStyle = ComboBoxStyle.DropDownList, Font = labelFont };
            Label lblObjName = new Label { Text = "Tên đối tượng:", Location = new Point(20, 90), AutoSize = true, Font = labelFont };
            this.cbObjectName = new ComboBox() { Location = new Point(20, 115), Width = 260, DropDownStyle = ComboBoxStyle.DropDownList, Font = labelFont };
            this.groupBox2.Controls.AddRange(new Control[] { lblType, cbObjectType, lblObjName, cbObjectName });

            // --- 3. Chọn quyền ---
            this.groupBox3 = new GroupBox() { Text = "3. Chọn quyền", Location = new Point(20, 180), Size = new Size(270, 220), Font = groupFont };
            this.chkListPrivileges = new CheckedListBox() { Location = new Point(20, 30), Size = new Size(230, 130), CheckOnClick = true, BorderStyle = BorderStyle.FixedSingle, Font = labelFont };
            this.chkWithGrantOption = new CheckBox() { Text = "WITH GRANT OPTION", Location = new Point(20, 175), AutoSize = true, Font = groupFont, ForeColor = Color.DarkRed };
            this.groupBox3.Controls.AddRange(new Control[] { chkListPrivileges, chkWithGrantOption });

            // --- 4. Chọn cột ---
            this.groupBox4 = new GroupBox() { Text = "4. Chọn cột (chỉ SELECT/UPDATE)", Location = new Point(310, 180), Size = new Size(300, 220), Enabled = false, Font = groupFont };
            this.chkListColumns = new CheckedListBox() { Location = new Point(20, 30), Size = new Size(260, 160), CheckOnClick = true, BorderStyle = BorderStyle.FixedSingle, Font = labelFont };
            this.groupBox4.Controls.Add(chkListColumns);

            // --- Nút bấm ---
            this.btnGrant = new Button()
            {
                Text = "THỰC HIỆN CẤP QUYỀN",
                Location = new Point(210, 420),
                Size = new Size(220, 50),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            // Sự kiện
            this.Load += FormGrantPrivileges_Load;
            this.rdoUser.CheckedChanged += (s, e) => { if (rdoUser.Checked) LoadGrantee(); };
            this.rdoRole.CheckedChanged += (s, e) => { if (rdoRole.Checked) LoadGrantee(); };
            this.cbObjectType.SelectedIndexChanged += cbObjectType_SelectedIndexChanged;
            this.cbObjectName.SelectedIndexChanged += cbObjectName_SelectedIndexChanged;
            this.chkListPrivileges.ItemCheck += chkListPrivileges_ItemCheck;
            this.btnGrant.Click += btnGrant_Click;

            this.Controls.AddRange(new Control[] { groupBox1, groupBox2, groupBox3, groupBox4, btnGrant });
        }

        private void FormGrantPrivileges_Load(object sender, EventArgs e)
        {
            cbObjectType.Items.AddRange(new string[] { "TABLE", "VIEW", "PROCEDURE", "FUNCTION" });
            cbObjectType.SelectedIndex = 0;
            LoadGrantee();
        }

        private void LoadGrantee()
        {
            cbGrantee.Items.Clear();
            var list = rdoUser.Checked ? _presenter.GetUsers() : _presenter.GetRoles();
            foreach (var item in list) cbGrantee.Items.Add(item);
            if (cbGrantee.Items.Count > 0) cbGrantee.SelectedIndex = 0;
            if (rdoRole.Checked)
            {
                chkWithGrantOption.Checked = false; 
                chkWithGrantOption.Enabled = false;
            }
            else
            {
                chkWithGrantOption.Enabled = true;
            }
        }

        private void cbObjectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbObjectName.Items.Clear();
            var objects = _presenter.GetObjects(cbObjectType.Text);
            foreach (var obj in objects) cbObjectName.Items.Add(obj);
            if (cbObjectName.Items.Count > 0) cbObjectName.SelectedIndex = 0;

            chkListPrivileges.Items.Clear();
            if (cbObjectType.Text == "TABLE" || cbObjectType.Text == "VIEW")
                chkListPrivileges.Items.AddRange(new string[] { "SELECT", "INSERT", "UPDATE", "DELETE" });
            else
                chkListPrivileges.Items.Add("EXECUTE");

            groupBox4.Enabled = false; // Reset trạng thái khung cột
        }

        private void cbObjectName_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkListColumns.Items.Clear();
            if (cbObjectType.Text == "TABLE" || cbObjectType.Text == "VIEW")
            {
                var cols = _presenter.GetColumns(cbObjectName.Text);
                foreach (var col in cols) chkListColumns.Items.Add(col);
            }
        }

        private void chkListPrivileges_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            this.BeginInvoke(new Action(() => {
                var checkedItems = chkListPrivileges.CheckedItems.Cast<string>().ToList();
                bool needsColumns = checkedItems.Any(p => p == "SELECT" || p == "UPDATE");
                groupBox4.Enabled = needsColumns;

                if (!needsColumns)
                {
                    for (int i = 0; i < chkListColumns.Items.Count; i++)
                        chkListColumns.SetItemChecked(i, false);
                }
            }));
        }

        private void btnGrant_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(cbGrantee.Text) || string.IsNullOrEmpty(cbObjectName.Text))
                {
                    MessageBox.Show("Vui lòng chọn đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                PrivilegeModel model = new PrivilegeModel
                {
                    Grantee = cbGrantee.Text,
                    ObjectType = cbObjectType.Text,
                    ObjectName = cbObjectName.Text,
                    SelectedPrivileges = chkListPrivileges.CheckedItems.Cast<string>().ToList(),
                    SelectedColumns = chkListColumns.CheckedItems.Cast<string>().ToList(),
                    WithGrantOption = chkWithGrantOption.Checked
                };

                if (model.SelectedPrivileges.Count == 0)
                {
                    MessageBox.Show("Hãy chọn ít nhất một quyền!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _presenter.ExecuteGrant(model);
                MessageBox.Show("Thực thi cấp quyền thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}