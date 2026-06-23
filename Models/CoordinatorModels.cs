namespace ATBM_Project.Models
{
    public class CoordinatorPatientModel
    {
        public string MaBN { get; set; }
        public string TenBN { get; set; }
        public string Phai { get; set; }
        public string NgaySinh { get; set; }
        public string Cccd { get; set; }
        public string SoNha { get; set; }
        public string TenDuong { get; set; }
        public string QuanHuyen { get; set; }
        public string TinhTp { get; set; }
        public string TienSuBenh { get; set; }
        public string TienSuBenhGd { get; set; }
        public string DiUngThuoc { get; set; }
    }

    public class CoordinatorMedicalRecordModel
    {
        public string MaHSBA { get; set; }
        public string MaBN { get; set; }
        public string Ngay { get; set; }
        public string ChanDoan { get; set; }
        public string DieuTri { get; set; }
        public string MaBS { get; set; }
        public string MaKhoa { get; set; }
        public string KetLuan { get; set; }
    }

    public class CoordinatorServiceModel
    {
        public string MaHSBA { get; set; }
        public string LoaiDV { get; set; }
        public string NgayDV { get; set; }
        public string MaKTV { get; set; }
        public string KetQua { get; set; }
    }
}
