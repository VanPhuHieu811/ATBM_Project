namespace ATBM_Project.Models
{
    public class BenhNhanModel
    {
        // Nhóm thông tin cố định (Read-only)
        public string MaBN { get; set; }
        public string TenBN { get; set; }
        public string Phai { get; set; }
        public string NgaySinh { get; set; }
        public string Cccd { get; set; }

        // Nhóm thông tin được phép sửa
        public string SoNha { get; set; }
        public string TenDuong { get; set; }
        public string QuanHuyen { get; set; }
        public string TinhTp { get; set; }
        public string TienSuBenh { get; set; }
        public string TienSuBenhGd { get; set; }
        public string DiUngThuoc { get; set; }
    }
}
