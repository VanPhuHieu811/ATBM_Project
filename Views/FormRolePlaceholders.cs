using System.Drawing;
using System.Windows.Forms;
using ATBM_Project.Data;

namespace ATBM_Project.Views
{
    public class FormCoordinatorMain : FormRolePlaceholder
    {
        public FormCoordinatorMain(string displayName)
            : base("Điều phối viên", displayName, "Demo điều phối: Quản lý bệnh nhân, tạo HSBA mới, phân công bác sĩ và kỹ thuật viên.")
        {
        }
    }

    public class FormTechnicianMain : FormRolePlaceholder
    {
        public FormTechnicianMain(string displayName)
            : base("Kỹ thuật viên", displayName, "Demo kỹ thuật viên: Xem dịch vụ được phân công và cập nhật kết quả.")
        {
        }
    }

    public class FormPatientMain : FormRolePlaceholder
    {
        public FormPatientMain(string displayName)
            : base("Bệnh nhân", displayName, "Demo bệnh nhân: Xem thông tin cá nhân và cập nhật thông tin liên hệ/tiền sử được phép.")
        {
        }
    }

    public class FormRolePlaceholder : Form
    {
        public FormRolePlaceholder(string roleName, string displayName, string description)
        {
            this.Text = roleName;
            this.ClientSize = new Size(760, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            Label title = new Label
            {
                Text = roleName,
                Location = new Point(0, 45),
                Size = new Size(760, 45),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 53, 65)
            };

            Label user = new Label
            {
                Text = $"{displayName} ({DBConfig.User?.ToUpperInvariant()})",
                Location = new Point(0, 95),
                Size = new Size(760, 32),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.DimGray
            };

            Label note = new Label
            {
                Text = description,
                Location = new Point(75, 145),
                Size = new Size(610, 55),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(70, 70, 70)
            };

            DataGridView grid = new DataGridView
            {
                Location = new Point(75, 220),
                Size = new Size(610, 95),
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            grid.Columns.Add("Feature", "Chức năng");
            grid.Columns.Add("Status", "Trạng thái");
            grid.Rows.Add("Điều hướng theo vai trò", "Đã có");
            grid.Rows.Add("Dữ liệu mẫu trên UI", "Đã có");
            grid.Rows.Add("Logic Oracle thật", "Tích hợp sau");

            Button logout = new Button
            {
                Text = "Đăng xuất",
                Location = new Point(310, 340),
                Size = new Size(140, 40),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            logout.FlatAppearance.BorderSize = 0;
            logout.Click += (s, e) => this.Close();

            this.Controls.Add(title);
            this.Controls.Add(user);
            this.Controls.Add(note);
            this.Controls.Add(grid);
            this.Controls.Add(logout);
        }
    }
}
