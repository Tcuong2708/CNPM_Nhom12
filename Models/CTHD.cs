using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace QuanLyKhachSan.Models
{
    public class CTHD
    {

        // --- KHÓA CHÍNH (Bắt buộc phải có) ---

        // 1. Mã Hóa Đơn (Để biết chi tiết này của đơn hàng nào)
        [Key, Column(Order = 0)]
        public int MaHD { get; set; }

        // 2. Mã Sản Phẩm (Để biết khách mua bánh nào)
        [Key, Column(Order = 1)]
        public int MaSP { get; set; }

        // --- THÔNG TIN MUA HÀNG ---
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }

        // --- LIÊN KẾT BẢNG (Foreign Key) ---
        // Dòng này để code hiểu MaHD liên kết sang bảng HoaDon
        [ForeignKey("MaHD")]
        public virtual HoaDon HoaDon { get; set; }

        // Dòng này để code hiểu MaSP liên kết sang bảng Banh
        [ForeignKey("MaSP")]
        public virtual Phong Phong { get; set; }

        // --- CÁC THUỘC TÍNH PHỤ (Chỉ dùng hiển thị, KHÔNG LƯU DB) ---
        // Dùng [NotMapped] để báo SQL bỏ qua mấy cái này
        [NotMapped]
        public string TenSP { get; set; }

        [NotMapped]
        public string HinhAnh { get; set; }

        [NotMapped]
        public decimal ThanhTien
        {
            get { return SoLuong * DonGia; }
        }
    }
}