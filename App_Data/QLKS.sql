-- =============================================
-- PHẦN 1: XÓA BẢNG CŨ (DROP TABLES)
-- Xóa theo thứ tự: Bảng con trước -> Bảng cha sau để tránh lỗi khóa ngoại
-- =============================================

-- 1. Xóa chi tiết hóa đơn & Hóa đơn
IF OBJECT_ID('dbo.CTHD', 'U') IS NOT NULL DROP TABLE [dbo].[CTHD];
IF OBJECT_ID('dbo.HoaDon', 'U') IS NOT NULL DROP TABLE [dbo].[HoaDon];

-- 2. Xóa Phong (và bảng Banh cũ nếu còn sót)
IF OBJECT_ID('dbo.Phong', 'U') IS NOT NULL DROP TABLE [dbo].[Phong];
IF OBJECT_ID('dbo.Banhs', 'U') IS NOT NULL DROP TABLE [dbo].[Banhs]; -- Dọn rác cũ

-- 3. Xóa Loại (và bảng ChuDe cũ nếu còn sót)
IF OBJECT_ID('dbo.Loai', 'U') IS NOT NULL DROP TABLE [dbo].[Loai];
IF OBJECT_ID('dbo.ChuDes', 'U') IS NOT NULL DROP TABLE [dbo].[ChuDes]; -- Dọn rác cũ

-- 4. Xóa Tài khoản & Quyền
IF OBJECT_ID('dbo.Account', 'U') IS NOT NULL DROP TABLE [dbo].[Account];
IF OBJECT_ID('dbo.Role', 'U') IS NOT NULL DROP TABLE [dbo].[Role];

GO 
-- (Lệnh GO để ngắt lô, bắt buộc phải có sau khi DROP và trước khi CREATE)

-- =============================================
-- PHẦN 2: TẠO BẢNG MỚI (CREATE TABLES)
-- =============================================

-- 1. Bảng Role (Quyền hạn)
CREATE TABLE [dbo].[Role] (
    [RoleID] INT IDENTITY(1,1) NOT NULL,
    [ChucVu] NVARCHAR(50) NOT NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([RoleID] ASC)
);

-- 2. Bảng Account (Tài khoản)
CREATE TABLE [dbo].[Account] (
    [IDTaiKhoan]  INT IDENTITY(1,1) NOT NULL,
    [TenDangNhap] VARCHAR(50) NOT NULL,
    [MatKhau]     VARCHAR(100) NOT NULL,
    [SoDienThoai] VARCHAR(15) NULL,
    [HoTen]       NVARCHAR(100) NULL,
    [NgaySinh]    DATETIME NOT NULL,
    [GioiTinh]    NVARCHAR(10) NULL,
    [DiaChi]      NVARCHAR(200) NULL,
    [NgayTaoTK]   DATETIME DEFAULT GETDATE(),
    [RoleID]      INT NOT NULL,
    CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED ([IDTaiKhoan] ASC),
    CONSTRAINT [FK_Account_Role] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[Role] ([RoleID]) ON DELETE CASCADE
);

-- 3. Bảng Loai (Loại phòng - Trước đây là ChuDe)
CREATE TABLE [dbo].[Loai] (
    [DepID] INT IDENTITY(1,1) NOT NULL, -- Khớp với Model Loai.cs
    [Name]  NVARCHAR(100) NOT NULL,
    CONSTRAINT [PK_Loai] PRIMARY KEY CLUSTERED ([DepID] ASC)
);

-- 4. Bảng Phong (Phòng - Trước đây là Banh)
CREATE TABLE [dbo].[Phong] (
    [ID]       INT IDENTITY(1,1) NOT NULL,
    [Name]     NVARCHAR(MAX) NOT NULL,
    [Price]    DECIMAL(18, 0) NOT NULL, -- Dùng Decimal cho tiền tệ chính xác
    [Detail]   NVARCHAR(MAX) NULL,
    [ImageUrl] NVARCHAR(MAX) NULL,
    [DepID]    INT NOT NULL,
    CONSTRAINT [PK_Phong] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Phong_Loai] FOREIGN KEY ([DepID]) REFERENCES [dbo].[Loai] ([DepID]) ON DELETE CASCADE
);

-- 5. Bảng HoaDon
CREATE TABLE [dbo].[HoaDon] (
    [MaHD]       INT IDENTITY(1,1) NOT NULL,
    [NgayDat]    DATETIME DEFAULT GETDATE(),
    [HoTen]      NVARCHAR(100) NULL, -- Tên người đặt vãng lai
    [DienThoai]  VARCHAR(15) NULL,
    [DiaChi]     NVARCHAR(200) NULL,
    [TongTien]   DECIMAL(18, 0) NOT NULL,
    [IDTaiKhoan] INT NULL, -- Có thể null nếu khách không đăng nhập
    CONSTRAINT [PK_HoaDon] PRIMARY KEY CLUSTERED ([MaHD] ASC),
    CONSTRAINT [FK_HoaDon_Account] FOREIGN KEY ([IDTaiKhoan]) REFERENCES [dbo].[Account] ([IDTaiKhoan])
);

-- 6. Bảng CTHD (Chi tiết hóa đơn)
CREATE TABLE [dbo].[CTHD] (
    [MaHD]    INT NOT NULL,
    [MaSP]    INT NOT NULL, -- Đây là ID của Phòng
    [SoLuong] INT NOT NULL,
    [DonGia]  DECIMAL(18, 0) NOT NULL,
    
    -- Khóa chính phức hợp (Composite Primary Key)
    CONSTRAINT [PK_CTHD] PRIMARY KEY CLUSTERED ([MaHD] ASC, [MaSP] ASC),
    
    -- Các khóa ngoại
    CONSTRAINT [FK_CTHD_HoaDon] FOREIGN KEY ([MaHD]) REFERENCES [dbo].[HoaDon] ([MaHD]) ON DELETE CASCADE,
    CONSTRAINT [FK_CTHD_Phong] FOREIGN KEY ([MaSP]) REFERENCES [dbo].[Phong] ([ID]) ON DELETE CASCADE
);

GO

-- =============================================
-- PHẦN 3: THÊM DỮ LIỆU MẪU (SEED DATA)
-- =============================================

-- Thêm Quyền
INSERT INTO [dbo].[Role] ([ChucVu]) VALUES (N'Admin'), (N'Khách hàng');

-- Thêm Tài khoản Admin (Mật khẩu: 123456)
INSERT INTO [dbo].[Account] ([TenDangNhap], [MatKhau], [HoTen], [NgaySinh], [RoleID]) 
VALUES ('admin', '123456', N'Quản Trị Viên', '1995-01-01', 1);

-- Thêm Loại phòng
INSERT INTO [dbo].[Loai] ([Name]) VALUES (N'Phòng Standard'), (N'Phòng VIP'), (N'Phòng Suite');

-- Thêm 1 Phòng mẫu
INSERT INTO [dbo].[Phong] ([Name], [Price], [Detail], [ImageUrl], [DepID]) 
VALUES (N'Phòng 101 - View Biển', 500000, N'Đầy đủ tiện nghi, view đẹp', '~/Images/p101.jpg', 1);

-- Kiểm tra kết quả
SELECT * FROM [dbo].[Phong];