using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyKhachSan.Models;
namespace QuanLyKhachSan.Controllers
{
    public class AccountController : Controller
    {
        private PhongDB db = new PhongDB();

        // GET: /Account/Information
        public ActionResult Information()
        {
            // 1. Kiểm tra đăng nhập chưa
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. Lấy ID từ session
            int userId = (int)Session["UserID"];

            // 3. Tìm user trong database
            var user = db.Account.Find(userId);

            if (user == null) return RedirectToAction("Login", "Account");

            return View(user);
        }

        // POST: /Account/Information
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Information(Account model)
        {
            if (Session["UserID"] == null) return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                // Lấy ID user đang đăng nhập để đảm bảo an toàn (tránh hack ID)
                int userId = (int)Session["UserID"];
                var userInDb = db.Account.Find(userId);

                if (userInDb != null)
                {
                    // Cập nhật các thông tin cho phép
                    userInDb.HoTen = model.HoTen;
                    userInDb.SoDienThoai = model.SoDienThoai;
                    userInDb.NgaySinh = model.NgaySinh;
                    userInDb.GioiTinh = model.GioiTinh;
                    userInDb.DiaChi = model.DiaChi;

                    // Lưu xuống DB
                    db.SaveChanges();

                    // Cập nhật lại Session "User" để hiển thị tên mới ngay lập tức trên menu
                    Session["User"] = userInDb;

                    ViewBag.Success = "Cập nhật thông tin thành công!";
                    return View(userInDb);
                }
            }

            ViewBag.Error = "Cập nhật thất bại. Vui lòng kiểm tra lại thông tin.";
            return View(model);
        }
        // GET: /Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public ActionResult Login(Account model)
        {
            if (ModelState.IsValid)
            {
                // --- SỬA Ở ĐÂY ---
                // Thêm điều kiện: u.Status == 1 (Chỉ lấy tài khoản đang hoạt động)
                var user = db.Account.FirstOrDefault(u =>
                    u.TenDangNhap == model.TenDangNhap &&
                    u.MatKhau == model.MatKhau &&
                    u.Status == 1);

                if (user != null)
                {
                    // Đăng nhập thành công
                    Session["UserID"] = user.IDTaiKhoan;
                    Session["User"] = user;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Kiểm tra thêm để báo lỗi chính xác hơn (Optional)
                    var lockedUser = db.Account.FirstOrDefault(u =>
                        u.TenDangNhap == model.TenDangNhap &&
                        u.MatKhau == model.MatKhau &&
                        u.Status == 0);

                    if (lockedUser != null)
                    {
                        ViewBag.Error = "Tài khoản này đã bị khóa hoặc ngừng hoạt động.";
                    }
                    else
                    {
                        ViewBag.Error = "Sai tài khoản hoặc mật khẩu.";
                    }
                }
            }
            return View();
        }

        // GET: /Account/Logout
        public ActionResult Logout()
        {
            // Xóa hết tất cả session khi đăng xuất
            Session.Clear();
            // Hoặc xóa từng cái:
            // Session["UserID"] = null;
            // Session["User"] = null;

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public ActionResult Register(Account model)
        {
            if (ModelState.IsValid)
            {
                // Gán mặc định role cho User (Khách hàng)
                model.RoleID = 2;

                // --- SỬA Ở ĐÂY ---
                // Gán mặc định trạng thái là Hoạt động
                model.Status = 1;

                db.Account.Add(model);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            return View();
        }

    }
}