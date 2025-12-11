using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyKhachSan.Models; // Gọi Model của bạn

namespace QuanLyKhachSan.Controllers
{
    public class AdminCategoryController : Controller
    {
        private PhongDB db = new PhongDB();

        // --- HÀM KIỂM TRA ADMIN (Dùng chung cho toàn bộ project) ---
        public bool CheckAdmin()
        {
            if (Session["User"] == null) return false;
            var user = (Account)Session["User"];
            if (user.RoleID == 1) return true; // 1 là Admin
            return false;
        }

        // 1. DANH SÁCH CHỦ ĐỀ
        public ActionResult Index()
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            // Lấy danh sách chủ đề từ bảng ChuDe
            var listChuDe = db.Loai.ToList();
            return View(listChuDe);
        }

        // 2. THÊM MỚI (GET)
        public ActionResult Create()
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");
            return View();
        }

        // 2. THÊM MỚI (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Loai model)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                db.Loai.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // 3. SỬA (GET)
        public ActionResult Edit(int id) // Model bạn dùng int, ko cần nullable int?
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            // Tìm theo khóa chính DepID
            var chude = db.Loai.Find(id);
            if (chude == null) return HttpNotFound();

            return View(chude);
        }

        // 3. SỬA (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Loai model)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                // Cập nhật trạng thái Modified
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // 4. XÓA (GET - Hiện trang xác nhận)
        public ActionResult Delete(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            var chude = db.Loai.Find(id);
            if (chude == null) return HttpNotFound();

            return View(chude);
        }

        // 4. XÓA (POST - Xóa thật)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            var chude = db.Loai.Find(id);
            if (chude != null)
            {
                // Lưu ý: Nếu chủ đề này đang có bánh, việc xóa có thể gây lỗi khóa ngoại.
                // Bạn có thể cần kiểm tra: if(chude.Banh.Count > 0) ...

                db.Loai.Remove(chude);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}