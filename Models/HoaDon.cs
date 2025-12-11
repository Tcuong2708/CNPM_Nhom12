using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhachSan.Models
{
    [Table("HoaDon")] // Tên bảng trong Database
    public class HoaDon
    {
        [Key]
        public int MaHD { get; set; }

        public DateTime NgayDat { get; set; } = DateTime.Now;

        // --- BỔ SUNG QUAN TRỌNG: NGÀY NHẬN & NGÀY TRẢ ---
        [Display(Name = "Ngày nhận phòng")]
        [DataType(DataType.Date)]
        public DateTime NgayNhan { get; set; }

        [Display(Name = "Ngày trả phòng")]
        [DataType(DataType.Date)]
        public DateTime NgayTra { get; set; }
        // ------------------------------------------------

        // Thông tin người đặt
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(20)]
        public string DienThoai { get; set; }

        [StringLength(200)]
        public string DiaChi { get; set; }

        // Tổng tiền (Lưu để đỡ phải tính lại nhiều lần)
        public decimal TongTien { get; set; }

        // Liên kết với tài khoản người mua (Nếu có đăng nhập)
        public int? IDTaiKhoan { get; set; }

        [ForeignKey("IDTaiKhoan")]
        public virtual Account Account { get; set; }

        // Một hóa đơn có nhiều chi tiết
        public virtual ICollection<CTHD> CTHDs { get; set; }
        public virtual ICollection<CT_DichVu> CT_DichVus { get; set; }

        [NotMapped]
        public int SoDem
        {
            get
            {
                if (NgayTra <= NgayNhan) return 1; // Tối thiểu 1 đêm
                return (NgayTra - NgayNhan).Days;
            }
        }
    }
}