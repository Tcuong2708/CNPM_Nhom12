using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhachSan.Models
{
    [Table("DichVu")]
    public class DichVu
    {
        [Key]
        public int MaDV { get; set; }

        [Required(ErrorMessage = "Tên dịch vụ không được để trống")]
        [StringLength(100)]
        [Display(Name = "Tên dịch vụ")]
        public string TenDV { get; set; }

        [Required(ErrorMessage = "Giá tiền không được để trống")]
        [Display(Name = "Giá tiền")]
        public decimal GiaTien { get; set; }

        // --- BỔ SUNG THUỘC TÍNH NÀY ĐỂ HẾT LỖI ---
        [StringLength(50)]
        [Display(Name = "Đơn vị tính")]
        public string DonVi { get; set; }
        // -----------------------------------------
    }
}