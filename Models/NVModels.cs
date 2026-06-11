namespace ATBM_Project.Models
{
    public class NhanVienModel
    {
        // Thông tin hệ thống và hành chính (Khóa sửa)
        public string ManV { get; set; }
        public string HoTen { get; set; }
        public string Phai { get; set; }
        public string NgaySinh { get; set; }
        public string Cmnd { get; set; }
        public string VaiTro { get; set; }
        public string ChuyenKhoa { get; set; }

        // Thông tin liên lạc (Cho phép sửa)
        public string QueQuan { get; set; }
        public string SoDt { get; set; }
    }
}
