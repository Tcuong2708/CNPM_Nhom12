USE [master];
GO

-- ======================================================================================
-- 1. XÓA DATABASE CŨ AN TOÀN (CƠ CHẾ KILL KẾT NỐI)
-- ======================================================================================
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'QuanLyKhachSan')
BEGIN
    -- Chuyển về chế độ 1 người dùng để ngắt mọi kết nối khác ngay lập tức
    ALTER DATABASE [QuanLyKhachSan] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    
    -- Xóa Database
    DROP DATABASE [QuanLyKhachSan];
END
GO

-- ======================================================================================
-- 2. TẠO DATABASE MỚI
-- ======================================================================================
CREATE DATABASE [QuanLyKhachSan];
GO

USE [QuanLyKhachSan];
GO

-- ======================================================================================
-- 3. TẠO CẤU TRÚC BẢNG
-- ======================================================================================

-- 3.1. BẢNG ROLE (QUYỀN)
CREATE TABLE [dbo].[Role] (
    [RoleID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ChucVu] NVARCHAR(50) NOT NULL
);
GO

-- 3.2. BẢNG ACCOUNT (TÀI KHOẢN)
CREATE TABLE [dbo].[Account] (
    [IDTaiKhoan] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [TenDangNhap] VARCHAR(50) NOT NULL,
    [MatKhau] VARCHAR(50) NOT NULL,
    [HoTen] NVARCHAR(100) NOT NULL,
    [SoDienThoai] VARCHAR(15),
    [DiaChi] NVARCHAR(200),
    [GioiTinh] NVARCHAR(10),
    [NgaySinh] DATE, 
    [NgayTaoTK] DATETIME DEFAULT GETDATE(),
    [RoleID] INT NOT NULL,
    CONSTRAINT [FK_Account_Role] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[Role] ([RoleID])
);
GO

-- 3.3. BẢNG LOAI (DANH MỤC PHÒNG)
CREATE TABLE [dbo].[Loai] (
    [MaLoai] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL
);
GO

-- 3.4. BẢNG DICHVU (MỚI)
CREATE TABLE [dbo].[DichVu] (
    [MaDV] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [TenDV] NVARCHAR(100) NOT NULL,
    [GiaTien] DECIMAL(18,0) DEFAULT 0,
    [DonVi] NVARCHAR(50) -- Ví dụ: Lần, Người, Suất
);
GO

-- 3.5. BẢNG PHONG
CREATE TABLE [dbo].[Phong] (
    [ID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [Price] DECIMAL(18, 0) NOT NULL,
    [Detail] NVARCHAR(MAX),
    [ImageUrl] NVARCHAR(255),
    [MaLoai] INT NOT NULL,
    CONSTRAINT [FK_Phong_Loai] FOREIGN KEY ([MaLoai]) REFERENCES [dbo].[Loai] ([MaLoai])
);
GO

-- 3.6. BẢNG HOADON (CÓ NGAYNHAN, NGAYTRA)
CREATE TABLE [dbo].[HoaDon] (
    [MaHD] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [HoTen] NVARCHAR(100) NOT NULL,
    [DienThoai] VARCHAR(15) NOT NULL,
    [DiaChi] NVARCHAR(200) NOT NULL,
    [NgayDat] DATETIME DEFAULT GETDATE(),
    
    -- Cột bắt buộc cho tính năng đặt phòng --
    [NgayNhan] DATE NOT NULL, 
    [NgayTra] DATE NOT NULL,
    -----------------------------------------
    
    [TongTien] DECIMAL(18, 0) DEFAULT 0,
    [IDTaiKhoan] INT,
    CONSTRAINT [FK_HoaDon_Account] FOREIGN KEY ([IDTaiKhoan]) REFERENCES [dbo].[Account] ([IDTaiKhoan])
);
GO

-- 3.7. BẢNG CTHD (CHI TIẾT PHÒNG)
CREATE TABLE [dbo].[CTHD] (
    [MaCT] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [MaHD] INT NOT NULL,
    [MaSP] INT NOT NULL,
    [SoLuong] INT DEFAULT 1,
    [DonGia] DECIMAL(18, 0) NOT NULL,
    CONSTRAINT [FK_CTHD_HoaDon] FOREIGN KEY ([MaHD]) REFERENCES [dbo].[HoaDon] ([MaHD]),
    CONSTRAINT [FK_CTHD_Phong] FOREIGN KEY ([MaSP]) REFERENCES [dbo].[Phong] ([ID])
);
GO

-- 3.8. BẢNG CT_DICHVU (CHI TIẾT DỊCH VỤ)
CREATE TABLE [dbo].[CT_DichVu] (
    [ID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [MaHD] INT NOT NULL,
    [MaDV] INT NOT NULL,
    [GiaTien] DECIMAL(18,0), 
    CONSTRAINT [FK_CTDV_HoaDon] FOREIGN KEY ([MaHD]) REFERENCES [dbo].[HoaDon] ([MaHD]),
    CONSTRAINT [FK_CTDV_DichVu] FOREIGN KEY ([MaDV]) REFERENCES [dbo].[DichVu] ([MaDV])
);
GO

-- ======================================================================================
-- 4. INSERT DỮ LIỆU MẪU (FULL)
-- ======================================================================================

-- ROLE
SET IDENTITY_INSERT [dbo].[Role] ON;
INSERT INTO [dbo].[Role] ([RoleID], [ChucVu]) VALUES (1, N'Admin');
INSERT INTO [dbo].[Role] ([RoleID], [ChucVu]) VALUES (2, N'Khách hàng');
INSERT INTO [dbo].[Role] ([RoleID], [ChucVu]) VALUES (3, N'Lễ tân');
SET IDENTITY_INSERT [dbo].[Role] OFF;

-- ACCOUNT ADMIN & LỄ TÂN
INSERT INTO [dbo].[Account] ([TenDangNhap], [MatKhau], [HoTen], [SoDienThoai], [DiaChi], [GioiTinh], [NgaySinh], [RoleID]) 
VALUES ('admin', '123456', N'Quản Trị Viên', '0909999999', N'Hà Nội', N'Nam', '1990-01-01', 1);

INSERT INTO [dbo].[Account] ([TenDangNhap], [MatKhau], [HoTen], [SoDienThoai], [DiaChi], [GioiTinh], [NgaySinh], [RoleID]) 
VALUES ('letan', '123456', N'Nhân Viên Lễ Tân', '0908888888', N'Sảnh Chính', N'Nữ', '2000-05-05', 3);

-- DỊCH VỤ
SET IDENTITY_INSERT [dbo].[DichVu] ON;
INSERT INTO [dbo].[DichVu] ([MaDV], [TenDV], [GiaTien], [DonVi]) VALUES (1, N'Ăn sáng Buffet', 150000, N'Người/Ngày');
INSERT INTO [dbo].[DichVu] ([MaDV], [TenDV], [GiaTien], [DonVi]) VALUES (2, N'Giặt ủi cao cấp', 50000, N'Kg');
INSERT INTO [dbo].[DichVu] ([MaDV], [TenDV], [GiaTien], [DonVi]) VALUES (3, N'Thuê xe máy', 120000, N'Ngày');
INSERT INTO [dbo].[DichVu] ([MaDV], [TenDV], [GiaTien], [DonVi]) VALUES (4, N'Spa & Massage', 350000, N'Suất');
INSERT INTO [dbo].[DichVu] ([MaDV], [TenDV], [GiaTien], [DonVi]) VALUES (5, N'Đưa đón sân bay', 200000, N'Lượt');
SET IDENTITY_INSERT [dbo].[DichVu] OFF;

-- LOẠI PHÒNG
SET IDENTITY_INSERT [dbo].[Loai] ON;
INSERT INTO [dbo].[Loai] ([MaLoai], [Name]) VALUES (1, N'Phòng Đơn');
INSERT INTO [dbo].[Loai] ([MaLoai], [Name]) VALUES (2, N'Phòng Đôi');
INSERT INTO [dbo].[Loai] ([MaLoai], [Name]) VALUES (3, N'Phòng Ba');
INSERT INTO [dbo].[Loai] ([MaLoai], [Name]) VALUES (4, N'Phòng Gia Đình');
INSERT INTO [dbo].[Loai] ([MaLoai], [Name]) VALUES (5, N'Phòng Cao Cấp (Deluxe)');
INSERT INTO [dbo].[Loai] ([MaLoai], [Name]) VALUES (6, N'Phòng Suite');
INSERT INTO [dbo].[Loai] ([MaLoai], [Name]) VALUES (7, N'Phòng Executive');
INSERT INTO [dbo].[Loai] ([MaLoai], [Name]) VALUES (8, N'Phòng Studio');
SET IDENTITY_INSERT [dbo].[Loai] OFF;

-- DANH SÁCH PHÒNG (KHÔNG BỎ SÓT PHÒNG NÀO)
SET IDENTITY_INSERT [dbo].[Phong] ON;
INSERT INTO [dbo].[Phong] ([ID], [Name], [Price], [Detail], [ImageUrl], [MaLoai]) VALUES 
-- Loại 1: Phòng Đơn (6 phòng)
(58, N'P101', 500000, N'Phòng đơn tiện nghi, có giường đơn, bàn làm việc và wifi miễn phí.', N'/Content/Images/a1.jpg', 1),
(59, N'P102', 520000, N'Phòng đơn tiện nghi, view thành phố, phòng tắm hiện đại.', N'/Content/Images/a2.jpg', 1),
(60, N'P103', 500000, N'Phòng đơn nhỏ nhưng tiện nghi đầy đủ.', N'/Content/Images/a3.jpg', 1),
(61, N'P104', 550000, N'Phòng đơn sang trọng, nội thất cao cấp.', N'/Content/Images/4.jpg', 1),
(62, N'P105', 530000, N'Phòng đơn có kệ sách và ghế ngồi làm việc.', N'/Content/Images/5.jpg', 1),
(63, N'P106', 510000, N'Phòng đơn với ban công riêng.', N'/Content/Images/6.jpg', 1),

-- Loại 2: Phòng Đôi (6 phòng)
(64, N'P201', 800000, N'Phòng đôi rộng rãi, giường king size, sofa.', N'~/Images/phongdoi201.jpg', 2),
(65, N'P202', 820000, N'Phòng đôi tiện nghi, view thành phố.', N'~/Images/phongdoi202.jpg', 2),
(66, N'P203', 810000, N'Phòng đôi sang trọng, TV màn hình lớn.', N'~/Images/phongdoi203.jpg', 2),
(67, N'P204', 830000, N'Phòng đôi thoải mái, thiết kế hiện đại.', N'~/Images/phongdoi204.jpg', 2),
(68, N'P205', 840000, N'Phòng đôi có khu vực tiếp khách nhỏ.', N'~/Images/phongdoi205.jpg', 2),
(69, N'P206', 820000, N'Phòng đôi với giường vừa và phòng tắm hiện đại.', N'~/Images/phongdoi206.jpg', 2),

-- Loại 3: Phòng Ba (6 phòng)
(70, N'P301', 900000, N'Phòng ba tiện nghi với 3 giường.', N'~/Images/phongba301.jpg', 3),
(71, N'P302', 920000, N'Phòng ba sang trọng, thiết kế hiện đại.', N'~/Images/phongba302.jpg', 3),
(72, N'P303', 910000, N'Phòng ba thoải mái, có ban công riêng.', N'~/Images/phongba303.jpg', 3),
(73, N'P304', 930000, N'Phòng ba tiện nghi với phòng tắm riêng.', N'~/Images/phongba304.jpg', 3),
(74, N'P305', 940000, N'Phòng ba sang trọng, phù hợp cho nhóm bạn.', N'~/Images/phongba305.jpg', 3),
(75, N'P306', 920000, N'Phòng ba thiết kế hiện đại.', N'~/Images/phongba306.jpg', 3),

-- Loại 4: Gia Đình (6 phòng)
(76, N'P401', 1000000, N'Phòng gia đình rộng rãi, giường lớn.', N'~/Images/phonggiadinh401.jpg', 4),
(77, N'P402', 1020000, N'Phòng gia đình tiện nghi, có khu vực cho trẻ em.', N'~/Images/phonggiadinh402.jpg', 4),
(78, N'P403', 1010000, N'Phòng gia đình sang trọng, view thành phố.', N'~/Images/phonggiadinh403.jpg', 4),
(79, N'P404', 1030000, N'Phòng gia đình thiết kế hiện đại.', N'~/Images/phonggiadinh404.jpg', 4),
(80, N'P405', 1040000, N'Phòng gia đình tiện nghi đầy đủ.', N'~/Images/phonggiadinh405.jpg', 4),
(81, N'P406', 1020000, N'Phòng gia đình có phòng tắm riêng.', N'~/Images/phonggiadinh406.jpg', 4),

-- Loại 5: Deluxe (6 phòng)
(82, N'P501', 1200000, N'Phòng Deluxe sang trọng, minibar.', N'~/Images/phongdeluxe501.jpg', 5),
(83, N'P502', 1220000, N'Phòng Deluxe hiện đại, view thành phố.', N'~/Images/phongdeluxe502.jpg', 5),
(84, N'P503', 1210000, N'Phòng Deluxe tiện nghi, ban công rộng.', N'~/Images/phongdeluxe503.jpg', 5),
(85, N'P504', 1230000, N'Phòng Deluxe với phòng tắm hiện đại.', N'~/Images/phongdeluxe504.jpg', 5),
(86, N'P505', 1240000, N'Phòng Deluxe có khu vực tiếp khách nhỏ.', N'~/Images/phongdeluxe505.jpg', 5),
(87, N'P506', 1220000, N'Phòng Deluxe thiết kế hiện đại.', N'~/Images/phongdeluxe506.jpg', 5),

-- Loại 6: Suite (3 phòng)
(88, N'P601', 1500000, N'Phòng Suite có phòng khách riêng.', N'~/Images/phongsuite601.jpg', 6),
(89, N'P602', 1520000, N'Phòng Suite sang trọng với sofa.', N'~/Images/phongsuite602.jpg', 6),
(90, N'P603', 1510000, N'Phòng Suite tiện nghi, phòng thay đồ.', N'~/Images/phongsuite603.jpg', 6),

-- Loại 7: Executive (3 phòng)
(91, N'P701', 1800000, N'Phòng Executive đặc quyền lounge.', N'~/Images/phongexecutive701.jpg', 7),
(92, N'P702', 1820000, N'Phòng Executive sang trọng.', N'~/Images/phongexecutive702.jpg', 7),
(93, N'P703', 1810000, N'Phòng Executive tiện nghi cao cấp.', N'~/Images/phongexecutive703.jpg', 7),

-- Loại 8: Studio (3 phòng)
(94, N'P801', 2000000, N'Phòng Studio kiểu căn hộ mở, bếp nhỏ.', N'~/Images/phongstudio801.jpg', 8),
(95, N'P802', 2020000, N'Phòng Studio tiện nghi, ban công rộng.', N'~/Images/phongstudio802.jpg', 8),
(96, N'P803', 2010000, N'Phòng Studio sang trọng, ở dài hạn.', N'~/Images/phongstudio803.jpg', 8);
SET IDENTITY_INSERT [dbo].[Phong] OFF;
GO