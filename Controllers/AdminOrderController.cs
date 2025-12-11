using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using QuanLyKhachSan.Models;

namespace QuanLyKhachSan.Controllers
{
    public class AdminOrderController : Controller
    {
        private PhongDB db = new PhongDB();

        // --- 1. HÀM CHECK QUYỀN TRUY CẬP (Dùng cho Xem/In) ---
        // Cho phép: Admin (1) HOẶC Lễ tân (3)
        public bool CheckQuyen()
        {
            if (Session["User"] == null) return false;
            var user = (Account)Session["User"];

            // Nếu là Admin hoặc Lễ tân thì cho qua
            if (user.RoleID == 1 || user.RoleID == 3) return true;

            return false;
        }

        // --- 2. DANH SÁCH ĐƠN HÀNG ---
        public ActionResult Index()
        {
            // Sử dụng CheckQuyen (Cả 2 đều vào được)
            if (!CheckQuyen()) return RedirectToAction("Index", "Home");

            // Lấy danh sách, sắp xếp đơn mới nhất lên đầu
            var orders = db.HoaDon.OrderByDescending(o => o.NgayDat).ToList();
            return View(orders);
        }

        // --- 3. XEM CHI TIẾT ---
        public ActionResult Details(int id)
        {
            if (!CheckQuyen()) return RedirectToAction("Index", "Home");

            var order = db.HoaDon.Find(id);
            if (order == null) return HttpNotFound();

            // Lấy danh sách chi tiết KÈM TÊN PHÒNG
            var details = db.CTHD
                        .Include(x => x.Phong)
                        .Where(d => d.MaHD == id)
                        .ToList();

            ViewBag.ChiTiet = details;

            return View(order);
        }

        // --- 4. IN HÓA ĐƠN ---
        public ActionResult InHoaDon(int id)
        {
            if (!CheckQuyen()) return RedirectToAction("Index", "Home");

            var order = db.HoaDon
                          .Include(h => h.CTHDs.Select(c => c.Phong))      
                          .Include(h => h.CT_DichVus.Select(d => d.DichVu)) 
                          .FirstOrDefault(x => x.MaHD == id);

            if (order == null) return HttpNotFound();

            return View(order);
        }

        // --- 5. XÓA ĐƠN HÀNG (QUAN TRỌNG: CHỈ ADMIN ĐƯỢC XÓA) ---
        public ActionResult Delete(int id)
        {
            // Kiểm tra thủ công: Phải đăng nhập VÀ phải là Admin (RoleID = 1)
            var user = (Account)Session["User"];
            if (user == null || user.RoleID != 1)
            {
                // Nếu là Lễ tân bấm vào, hiện thông báo lỗi và đá về trang Index
                TempData["Error"] = "Bạn không có quyền xóa đơn hàng này!";
                return RedirectToAction("Index");
            }

            var order = db.HoaDon.Find(id);
            if (order == null) return HttpNotFound();

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            // Kiểm tra bảo mật lại lần nữa khi submit form
            var user = (Account)Session["User"];
            if (user == null || user.RoleID != 1)
            {
                TempData["Error"] = "Bạn không có quyền xóa!";
                return RedirectToAction("Index");
            }

            var order = db.HoaDon.Find(id);
            if (order != null)
            {
                // 1. Xóa chi tiết phòng trước (CTHD)
                var details = db.CTHD.Where(d => d.MaHD == id).ToList();
                db.CTHD.RemoveRange(details);

                // 2. Xóa chi tiết dịch vụ (CT_DichVu)
                var dichvus = db.CT_DichVu.Where(d => d.MaHD == id).ToList();
                db.CT_DichVu.RemoveRange(dichvus);

                // 3. Xóa hóa đơn chính
                db.HoaDon.Remove(order);

                db.SaveChanges();
                TempData["Success"] = "Đã xóa đơn hàng thành công!";
            }
            return RedirectToAction("Index");
        }
    }
}