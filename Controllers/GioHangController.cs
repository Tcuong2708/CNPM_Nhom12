using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyKhachSan.Models;

namespace QuanLyKhachSan.Controllers
{
    public class GioHangController : Controller
    {
        private PhongDB db = new PhongDB();

        // --- 1. LẤY GIỎ HÀNG TỪ SESSION ---
        public List<CTHD> LayGioHang()
        {
            List<CTHD> lstGioHang = Session["GioHang"] as List<CTHD>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<CTHD>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }

        // --- 2. THÊM VÀO GIỎ ---
        [HttpPost]
        public JsonResult ThemVaoGio(int MaSP)
        {
            List<CTHD> gioHang = LayGioHang();

            // Kiểm tra xem phòng đã có trong giỏ chưa
            CTHD item = gioHang.FirstOrDefault(x => x.MaSP == MaSP);

            if (item == null)
            {
                var phong = db.Phong.Find(MaSP);
                if (phong == null)
                {
                    return Json(new { status = false });
                }

                item = new CTHD
                {
                    MaSP = phong.ID,
                    TenSP = phong.Name,
                    HinhAnh = phong.ImageUrl,
                    DonGia = (decimal)phong.Price,
                    SoLuong = 1 // Mặc định đặt 1 phòng
                };

                gioHang.Add(item);
            }
            else
            {
                // Nếu phòng đã có thì tăng số lượng
                item.SoLuong++;
            }

            Session["GioHang"] = gioHang;

            return Json(new
            {
                status = true,
                soLuong = gioHang.Sum(x => x.SoLuong)
            });
        }

        // --- 3. TRANG THANH TOÁN (HIỂN THỊ) ---
        public ActionResult ThanhToan()
        {
            List<CTHD> gioHang = LayGioHang();

            // Nếu giỏ hàng trống, quay về trang chủ
            if (gioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // A. LẤY DANH SÁCH DỊCH VỤ TỪ DB ĐỂ HIỂN THỊ CHECKBOX
            ViewBag.ListDichVu = db.DichVu.ToList();

            // B. TÍNH TỔNG TIỀN MẶC ĐỊNH (Cho 1 đêm để hiển thị ban đầu)
            ViewBag.TongTien = gioHang.Sum(s => s.ThanhTien);
            ViewBag.TongSoLuong = gioHang.Sum(s => s.SoLuong);

            // C. ĐIỀN SẴN THÔNG TIN USER (Nếu đã đăng nhập)
            if (Session["User"] != null)
            {
                var user = (Account)Session["User"];
                ViewBag.UserInfor = user;
            }

            return View(gioHang);
        }

        // --- 4. XỬ LÝ ĐẶT HÀNG (CÓ KIỂM TRA TRÙNG LỊCH) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DatHang(string HoTen, string DienThoai, string DiaChi, DateTime NgayNhan, DateTime NgayTra, int[] SelectedDichVu)
        {
            List<CTHD> gioHang = LayGioHang();

            if (gioHang == null || gioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // --- KIỂM TRA LOGIC NGÀY THÁNG ---
            if (NgayNhan >= NgayTra)
            {
                TempData["Error"] = "Ngày trả phòng phải lớn hơn ngày nhận phòng!";
                return RedirectToAction("ThanhToan");
            }

            if (NgayNhan < DateTime.Today)
            {
                TempData["Error"] = "Không thể đặt phòng trong quá khứ!";
                return RedirectToAction("ThanhToan");
            }

            // --- QUAN TRỌNG: KIỂM TRA PHÒNG ĐÃ CÓ NGƯỜI ĐẶT CHƯA ---
            foreach (var item in gioHang)
            {
                // Tìm xem có đơn nào trùng lịch với phòng này không
                // Logic trùng: (Khách cũ Nhận < Khách mới Trả) VÀ (Khách cũ Trả > Khách mới Nhận)
                var phongDaDat = db.CTHD.FirstOrDefault(c =>
                    c.MaSP == item.MaSP && // Cùng mã phòng
                    c.HoaDon.NgayNhan < NgayTra && // Điều kiện trùng thời gian
                    c.HoaDon.NgayTra > NgayNhan
                );

                if (phongDaDat != null)
                {
                    // Nếu tìm thấy => Báo lỗi và hủy đặt
                    string tenPhong = item.TenSP; // Lấy tên phòng để báo cho khách
                    string ngayBan = $"{phongDaDat.HoaDon.NgayNhan:dd/MM} - {phongDaDat.HoaDon.NgayTra:dd/MM}";

                    TempData["Error"] = $"Xin lỗi, phòng '{tenPhong}' đã bị đặt trong khoảng thời gian {ngayBan}. Vui lòng chọn ngày khác hoặc phòng khác.";
                    return RedirectToAction("ThanhToan");
                }
            }
            // -------------------------------------------------------


            // --- NẾU KHÔNG TRÙNG THÌ TIẾP TỤC XỬ LÝ NHƯ CŨ ---

            // 1.1 Tính số đêm
            int soDem = (NgayTra - NgayNhan).Days;

            // 1.2 Tính tiền phòng
            decimal tongTienPhong = gioHang.Sum(x => x.DonGia * (x.SoLuong)) * soDem;

            // 1.3 Tính tiền dịch vụ
            decimal tongTienDichVu = 0;
            List<DichVu> dsDichVuChon = new List<DichVu>();

            if (SelectedDichVu != null)
            {
                dsDichVuChon = db.DichVu.Where(d => SelectedDichVu.Contains(d.MaDV)).ToList();
                tongTienDichVu = dsDichVuChon.Sum(d => d.GiaTien);
            }

            decimal tongCongFinal = tongTienPhong + tongTienDichVu;

            // 2. Lưu Hóa Đơn
            HoaDon hd = new HoaDon
            {
                HoTen = HoTen,
                DienThoai = DienThoai,
                DiaChi = DiaChi,
                NgayDat = DateTime.Now,
                NgayNhan = NgayNhan,
                NgayTra = NgayTra,
                TongTien = tongCongFinal,
                IDTaiKhoan = (Session["User"] != null) ? (int?)((Account)Session["User"]).IDTaiKhoan : null
            };

            db.HoaDon.Add(hd);
            db.SaveChanges();

            // 3. Lưu Chi Tiết Phòng
            foreach (var sp in gioHang)
            {
                CTHD ct = new CTHD
                {
                    MaHD = hd.MaHD,
                    MaSP = sp.MaSP,
                    SoLuong = sp.SoLuong,
                    DonGia = sp.DonGia
                };
                db.CTHD.Add(ct);
            }

            // 4. Lưu Chi Tiết Dịch Vụ
            if (dsDichVuChon.Count > 0)
            {
                foreach (var dv in dsDichVuChon)
                {
                    CT_DichVu ctDV = new CT_DichVu
                    {
                        MaHD = hd.MaHD,
                        MaDV = dv.MaDV,
                        GiaTien = dv.GiaTien
                    };
                    db.CT_DichVu.Add(ctDV);
                }
            }

            db.SaveChanges();

            Session["GioHang"] = null;
            TempData["Success"] = "Đặt phòng thành công! Mã đơn: #" + hd.MaHD;

            return RedirectToAction("Index", "Home");
        }

        // --- 5. XÓA SẢN PHẨM KHỎI GIỎ ---
        public ActionResult XoaGioHang(int MaSP)
        {
            List<CTHD> gioHang = Session["GioHang"] as List<CTHD>;
            CTHD itemXoa = gioHang.FirstOrDefault(m => m.MaSP == MaSP);

            if (itemXoa != null)
            {
                gioHang.Remove(itemXoa);
                Session["GioHang"] = gioHang;
            }
            return RedirectToAction("ThanhToan");
        }
        // --- 6. API KIỂM TRA PHÒNG TRỐNG (Dùng cho AJAX) ---
        [HttpPost]
        public JsonResult CheckPhongTrong(DateTime ngayNhan, DateTime ngayTra)
        {
            // 1. Kiểm tra ngày hợp lệ
            if (ngayNhan >= ngayTra)
            {
                return Json(new { status = false, message = "Ngày trả phòng phải lớn hơn ngày nhận phòng!" });
            }
            if (ngayNhan < DateTime.Today)
            {
                return Json(new { status = false, message = "Không thể đặt phòng trong quá khứ!" });
            }

            // 2. Lấy giỏ hàng
            List<CTHD> gioHang = Session["GioHang"] as List<CTHD>;
            if (gioHang == null || gioHang.Count == 0)
            {
                return Json(new { status = true }); // Giỏ trống thì coi như hợp lệ
            }

            // 3. Quét Database xem có trùng lịch không
            foreach (var item in gioHang)
            {
                // Logic trùng: (Khách cũ Nhận < Khách mới Trả) VÀ (Khách cũ Trả > Khách mới Nhận)
                var phongDaDat = db.CTHD.FirstOrDefault(c =>
                    c.MaSP == item.MaSP && // Cùng mã phòng
                    c.HoaDon.NgayNhan < ngayTra &&
                    c.HoaDon.NgayTra > ngayNhan
                );

                if (phongDaDat != null)
                {
                    string tenPhong = item.TenSP;
                    string thoiGian = $"{phongDaDat.HoaDon.NgayNhan:dd/MM} - {phongDaDat.HoaDon.NgayTra:dd/MM}";
                    return Json(new
                    {
                        status = false,
                        message = $"Phòng '{tenPhong}' đã bận trong khoảng {thoiGian}. Vui lòng chọn ngày khác."
                    });
                }
            }

            // Nếu chạy hết vòng lặp mà không trùng ai
            return Json(new { status = true });
        }
    }
}