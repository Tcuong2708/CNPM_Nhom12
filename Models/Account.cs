using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace QuanLyKhachSan.Models
{
    public class Account
    {
        [Key]
        public int IDTaiKhoan { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string SoDienThoai { get; set; }
        public string HoTen { get; set; }

        // --- SỬA Ở ĐÂY: Thêm dấu ? để cho phép Null ---
        public DateTime? NgaySinh { get; set; }
        // ----------------------------------------------

        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }

        // NgayTaoTK cũng nên cho phép null để tránh lỗi tương tự nếu SQL quên insert
        public DateTime? NgayTaoTK { get; set; } = DateTime.Now;

        public int RoleID { get; set; }

        [ForeignKey("RoleID")]
        public virtual Role Role { get; set; }
    }
}