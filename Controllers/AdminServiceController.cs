using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity; 
using QuanLyKhachSan.Models; 

namespace QuanLyKhachSan.Controllers
{
    public class AdminServiceController : Controller
    {
        private PhongDB db = new PhongDB();

        // --- HÀM CHECK QUYỀN ADMIN (Dùng chung) ---
        public bool CheckAdmin()
        {
            if (Session["User"] == null) return false;
            var user = (Account)Session["User"];
            if (user.RoleID == 1) return true; // 1 là Admin
            return false;
        }

        // 1. DANH SÁCH DỊCH VỤ
        public ActionResult Index()
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            var listDichVu = db.DichVu.OrderBy(d => d.TenDV).ToList();
            return View(listDichVu);
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
        public ActionResult Create(DichVu model)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                db.DichVu.Add(model);
                db.SaveChanges();
                TempData["Success"] = "Thêm dịch vụ thành công!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // 3. SỬA (GET)
        public ActionResult Edit(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            var dichVu = db.DichVu.Find(id);
            if (dichVu == null) return HttpNotFound();

            return View(dichVu);
        }

        // 3. SỬA (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DichVu model)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Cập nhật dịch vụ thành công!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // 4. XÓA (GET - Hỏi trước khi xóa)
        public ActionResult Delete(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            var dichVu = db.DichVu.Find(id);
            if (dichVu == null) return HttpNotFound();

            return View(dichVu);
        }

        // 4. XÓA (POST - Xác nhận xóa)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            var dichVu = db.DichVu.Find(id);
            if (dichVu != null)
            {
                // Kiểm tra xem dịch vụ này đã có trong hóa đơn nào chưa
                // Nếu có thì không được xóa để tránh lỗi dữ liệu cũ
                var checkInUse = db.CT_DichVu.Any(x => x.MaDV == id);
                if (checkInUse)
                {
                    TempData["Error"] = "Không thể xóa dịch vụ này vì đã có khách hàng sử dụng trong quá khứ!";
                    return RedirectToAction("Index");
                }

                db.DichVu.Remove(dichVu);
                db.SaveChanges();
                TempData["Success"] = "Đã xóa dịch vụ!";
            }
            return RedirectToAction("Index");
        }
    }
}