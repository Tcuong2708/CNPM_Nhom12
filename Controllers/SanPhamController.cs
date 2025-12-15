using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyKhachSan.Models;
using System.Data.Entity;

namespace QuanLyKhachSan.Controllers
{
    public class SanPhamController : Controller
    {
        private PhongDB db = new PhongDB();

        // GET: SanPham
        // Đổi tham số DepID -> MaLoai
        public ActionResult Index(string searchString, int? MaLoai, string priceRange, int? page)
        {
            var phong = db.Phong.Include(e => e.Loai).AsQueryable();

            // Tìm kiếm theo TÊN LOẠI PHÒNG
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                string searchLower = searchString.Trim().ToLower();
                phong = phong.Where(p => p.Loai.Name.ToLower().Contains(searchLower));
            }

            // Lọc theo mã loại (dropdown)
            if (MaLoai != null)
            {
                phong = phong.Where(p => p.MaLoai == MaLoai);
            }
            // 3. Lọc theo Giá (Cập nhật mức giá phù hợp với Khách sạn)
            switch (priceRange)
            {
                case "lt500": // Dưới 500k
                    phong = phong.Where(b => b.Price < 500000);
                    break;

                case "500-1000": // 500k - 1 triệu 
                    phong = phong.Where(b => b.Price >= 500000 && b.Price < 1000000);
                    break;

                case "1000-2000": // 1 triệu - 2 triệu
                    phong = phong.Where(b => b.Price >= 1000000 && b.Price <= 2000000);
                    break;

                case "gt2000": // Trên 2 triệu
                    phong = phong.Where(b => b.Price > 2000000);
                    break;
            }

            // Sắp xếp
            phong = phong.OrderBy(b => b.ID);

            // Phân trang
            int pageSize = 6;
            int pageNumber = page ?? 1;

            var pagedPhong = phong
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            int totalPhong = phong.Count();
            int totalPages = (int)Math.Ceiling((double)totalPhong / pageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = pageNumber;

            // Trả về các giá trị lọc để View giữ lại trạng thái
            ViewBag.SearchString = searchString;
            ViewBag.ListLoai = db.Loai.ToList(); // Đổi tên cho dễ hiểu
            ViewBag.CurrentMaLoai = MaLoai; // Đổi tên biến
            ViewBag.CurrentPriceRange = priceRange;

            return View(pagedPhong);
        }

        // Hiển thị chi tiết phòng
        public ActionResult Details(int? ID)
        {
            if (ID == null)
            {
                return RedirectToAction("Index");
            }

            var phong = db.Phong.Include(b => b.Loai).SingleOrDefault(b => b.ID == ID);

            if (phong == null)
            {
                return HttpNotFound();
            }

            // (Optional) Gợi ý phòng cùng loại
            ViewBag.Related = db.Phong
                                .Where(p => p.MaLoai == phong.MaLoai && p.ID != phong.ID)
                                .Take(4)
                                .ToList();

            return View(phong);
        }
    }
}