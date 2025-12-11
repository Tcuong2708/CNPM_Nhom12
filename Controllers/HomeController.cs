using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyKhachSan.Models;
namespace QuanLyKhachSan.Controllers
{
    public class HomeController : Controller
    {
        PhongDB db = new PhongDB();
        // GET: Home
        public ActionResult Index()
        {
            var phongMoi = db.Phong.OrderByDescending(x => x.ID).Take(8).ToList();
            return View(phongMoi);
        }
        public ActionResult ThongTin()
        {
            return View();
        }
        public ActionResult DanhGia()
        {
            return View();
        }
        public ActionResult XemChiTiet()
        {
            return View();
        }
    }
}