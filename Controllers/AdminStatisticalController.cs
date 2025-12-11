using System;
using System.Linq;
using System.Web.Mvc;
using QuanLyKhachSan.Models;

namespace QuanLyKhachSan.Controllers
{
    public class AdminStatisticalController : Controller
    {
        private PhongDB db = new PhongDB();

        public ActionResult Index()
        {
            // Lấy doanh thu theo tháng của năm hiện tại
            var currentYear = DateTime.Now.Year;

            // Truy vấn Group By Month
            var data = db.HoaDon
                .Where(h => h.NgayDat.Year == currentYear)
                .GroupBy(h => h.NgayDat.Month)
                .Select(g => new
                {
                    Thang = g.Key,
                    TongTien = g.Sum(x => x.TongTien),
                    SoDon = g.Count()
                })
                .ToList();

            // Tạo mảng dữ liệu cho 12 tháng (để tránh tháng nào ko có doanh thu bị thiếu)
            decimal[] arrDoanhThu = new decimal[12];
            int[] arrSoDon = new int[12];

            foreach (var item in data)
            {
                // Mảng bắt đầu từ 0 nên tháng 1 sẽ là index 0
                arrDoanhThu[item.Thang - 1] = item.TongTien;
                arrSoDon[item.Thang - 1] = item.SoDon;
            }

            ViewBag.Nam = currentYear;
            ViewBag.ArrDoanhThu = arrDoanhThu; // Gửi mảng tiền sang View
            ViewBag.ArrSoDon = arrSoDon;       // Gửi mảng số đơn

            return View();
        }
    }
}