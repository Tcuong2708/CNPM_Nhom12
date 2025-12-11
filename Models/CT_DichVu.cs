using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhachSan.Models
{
    [Table("CT_DichVu")]
    public class CT_DichVu
    {
        [Key]
        public int ID { get; set; }
        public int MaHD { get; set; }
        public int MaDV { get; set; }
        public decimal GiaTien { get; set; }

        [ForeignKey("MaHD")]
        public virtual HoaDon HoaDon { get; set; }
        [ForeignKey("MaDV")]
        public virtual DichVu DichVu { get; set; }
    }
}