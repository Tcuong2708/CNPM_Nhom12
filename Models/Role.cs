using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace QuanLyKhachSan.Models
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }
        [Required]
        public string ChucVu { get; set; }
        public virtual ICollection<Account> Account { get; set; }
    }
}