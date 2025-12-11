using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace QuanLyKhachSan.Models
{
    public class PhongDB : DbContext
    {
    
        public PhongDB() : base("MyConnectionString")
        {
            // Dòng này bảo EF: "Database đã có sẵn rồi, đừng cố tạo lại hay kiểm tra version nữa"
            Database.SetInitializer<PhongDB>(null);
        }

        public DbSet<Account> Account { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Loai> Loai { get; set; }
        public DbSet<Phong> Phong { get; set; }
        public DbSet<HoaDon> HoaDon { get; set; }
        public DbSet<CTHD> CTHD { get; set; }
        public virtual DbSet<DichVu> DichVu { get; set; }
        public virtual DbSet<CT_DichVu> CT_DichVu { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}