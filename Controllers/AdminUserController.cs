using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyKhachSan.Models;

namespace QuanLyKhachSan.Controllers
{
    public class AdminUserController : Controller
    {
        private PhongDB db = new PhongDB();

        // --- Kiểm tra quyền Admin ---
        public bool CheckAdmin()
        {
            if (Session["User"] == null) return false;
            var user = (Account)Session["User"];
            if (user.RoleID == 1) return true;
            return false;
        }

        // 1. DANH SÁCH USER
        public ActionResult Index()
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");
            return View(db.Account.ToList());
        }

        // 2. THÊM MỚI (GET) - Chạy khi mở trang
        /// 1. Hàm GET: Chạy khi mới mở trang
        public ActionResult Create()
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            // Lấy dữ liệu từ bảng Role, gán vào ViewBag.ListQuyen
            // Tham số: List dữ liệu, Cột giá trị (Value), Cột hiển thị (Text)
            ViewBag.RoleID = new SelectList(db.Role, "RoleID", "ChucVu");

            return View();
        }

        // 2. Hàm POST: Chạy khi bấm nút "Thêm mới"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Account model)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                var check = db.Account.FirstOrDefault(s => s.TenDangNhap == model.TenDangNhap);
                if (check == null)
                {
                    model.NgayTaoTK = DateTime.Now;
                    db.Account.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                }
            }

            // Nạp lại danh sách nếu có lỗi để tránh crash trang
            // Tham số thứ 4 (model.RoleID) giúp giữ lại lựa chọn cũ của người dùng
            ViewBag.RoleID = new SelectList(db.Role, "RoleID", "ChucVu", model.RoleID);

            return View(model);
        }

        // 3. SỬA (GET)
        public ActionResult Edit(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");
            var user = db.Account.Find(id);
            if (user == null) return HttpNotFound();

            // --- NẠP LIST QUYỀN ---
            ViewBag.RoleID = new SelectList(db.Role, "RoleID", "ChucVu", user.RoleID);

            return View(user);
        }

        // 3. SỬA (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Account model)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                var userInDb = db.Account.Find(model.IDTaiKhoan);
                if (userInDb != null)
                {
                    userInDb.HoTen = model.HoTen;
                    userInDb.SoDienThoai = model.SoDienThoai;
                    userInDb.DiaChi = model.DiaChi;
                    userInDb.GioiTinh = model.GioiTinh;
                    userInDb.NgaySinh = model.NgaySinh;
                    userInDb.RoleID = model.RoleID;

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            // --- NẠP LẠI LIST NẾU LỖI ---
            ViewBag.RoleID = new SelectList(db.Role, "RoleID", "ChucVu", model.RoleID);

            return View(model);
        }

        // 4. XÓA
        public ActionResult Delete(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");
            var user = db.Account.Find(id);
            if (user == null) return HttpNotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");
            var user = db.Account.Find(id);
            if (user != null)
            {
                db.Account.Remove(user);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // 5. CHI TIẾT
        public ActionResult Details(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");
            var user = db.Account.Find(id);
            if (user == null) return HttpNotFound();
            return View(user);
        }
    }
}