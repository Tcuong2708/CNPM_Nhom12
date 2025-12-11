using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyKhachSan.Models;

namespace QuanLyKhachSan.Controllers
{
    public class AdminProductController : Controller
    {
        private PhongDB db = new PhongDB();

        public bool CheckAdmin()
        {
            if (Session["User"] == null) return false;
            var user = (Account)Session["User"];
            if (user.RoleID == 1) return true;
            return false;
        }

        // 1. DANH SÁCH PHÒNG
        public ActionResult Index()
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            // Sắp xếp phòng mới nhất lên đầu
            var listPhong = db.Phong.OrderByDescending(x => x.ID).ToList();
            return View(listPhong);
        }

        // 2. THÊM PHÒNG (CREATE) - GET
        public ActionResult Create()
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            ViewBag.ListLoai = new SelectList(db.Loai, "MaLoai", "Name");
            // ----------------------------------------

            return View();
        }

        // 2. THÊM PHÒNG (CREATE) - POST
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Phong model, HttpPostedFileBase imageFile)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            // Xử lý ảnh
            if (imageFile != null && imageFile.ContentLength > 0)
            {
                string fileName = Path.GetFileName(imageFile.FileName);
                string path = Server.MapPath("~/Content/Images/" + fileName);
                imageFile.SaveAs(path);
                model.ImageUrl = "~/Content/Images/" + fileName;
            }
            else
            {
                model.ImageUrl = "~/Content/Images/default.png";
            }

            if (ModelState.IsValid)
            {
                db.Phong.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // --- SỬA TẠI ĐÂY: Nạp lại danh sách nếu lỗi ---
            ViewBag.ListLoai = new SelectList(db.Loai, "MaLoai", "Name", model.MaLoai);
            // ----------------------------------------------

            return View(model);
        }

        // 3. SỬA PHÒNG (EDIT) - GET
        public ActionResult Edit(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");
            var phong = db.Phong.Find(id);
            if (phong == null) return HttpNotFound();

            // --- SỬA TẠI ĐÂY ---
            ViewBag.ListLoai = new SelectList(db.Loai, "MaLoai", "Name", phong.MaLoai);

            return View(phong);
        }

        // 3. SỬA PHÒNG (EDIT) - POST
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Phong model, HttpPostedFileBase imageFile)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                var phongInDb = db.Phong.Find(model.ID);
                if (phongInDb != null)
                {
                    phongInDb.Name = model.Name;
                    phongInDb.Price = model.Price;
                    phongInDb.Detail = model.Detail;

                    // --- SỬA TẠI ĐÂY ---
                    phongInDb.MaLoai = model.MaLoai; // Cập nhật mã loại mới
                    // ------------------

                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(imageFile.FileName);
                        string path = Server.MapPath("~/Content/Images/" + fileName);
                        imageFile.SaveAs(path);
                        phongInDb.ImageUrl = "~/Content/Images/" + fileName;
                    }

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            // --- SỬA TẠI ĐÂY ---
            ViewBag.ListLoai = new SelectList(db.Loai, "MaLoai", "Name", model.MaLoai);

            return View(model);
        }

        // 4. XÓA PHÒNG
        public ActionResult Delete(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");
            var phong = db.Phong.Find(id);
            if (phong == null) return HttpNotFound();
            return View(phong);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");
            var phong = db.Phong.Find(id);
            if (phong != null)
            {
                db.Phong.Remove(phong);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // 5. CHI TIẾT
        public ActionResult Details(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Index", "Home");
            var phong = db.Phong.Find(id);
            if (phong == null) return HttpNotFound();
            return View(phong);
        }
    }
}