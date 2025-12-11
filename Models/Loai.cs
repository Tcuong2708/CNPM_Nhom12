using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace QuanLyKhachSan.Models
{
    [Table("Loai")] 
    public class Loai
    {
        [Key]

        public int MaLoai { get; set; }

        [Display(Name = "Tên loại phòng")]
        [Required(ErrorMessage = "Vui lòng nhập tên loại phòng")]
        public string Name { get; set; }

        public virtual ICollection<Phong> Phongs { get; set; }
    }
}