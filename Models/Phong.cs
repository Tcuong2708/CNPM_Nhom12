using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace QuanLyKhachSan.Models
{
    [Table("Phong")]
    public class Phong
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Tên phòng")]
        [Required(ErrorMessage = "Tên phòng không được để trống")]
        public string Name { get; set; }

        [Display(Name = "Giá phòng")]
        [Required(ErrorMessage = "Giá phòng không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Display(Name = "Mô tả chi tiết")]
        public string Detail { get; set; }

        [Display(Name = "Hình ảnh")]
        public string ImageUrl { get; set; }

        [Display(Name = "Loại phòng")]
        public int MaLoai { get; set; }

        [ForeignKey("MaLoai")]
        public virtual Loai Loai { get; set; }
    }
}