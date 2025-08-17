# Toàn Trương Water - Hệ thống bán nước uống online

## Mô tả
Hệ thống thương mại điện tử bán nước uống online được phát triển bằng ASP.NET MVC 5. Đây là một website hoàn chỉnh với đầy đủ các chức năng cần thiết cho việc kinh doanh nước uống online.

## Tính năng chính

### 🛒 **Quản lý sản phẩm**
- CRUD sản phẩm với hình ảnh
- Phân loại sản phẩm theo danh mục
- Quản lý giá cả và khuyến mãi
- Tìm kiếm và lọc sản phẩm

### 🛍️ **Giỏ hàng và thanh toán**
- Giỏ hàng session-based
- Thanh toán online qua VNPay
- Quản lý đơn hàng và trạng thái
- Lịch sử đơn hàng

### 👥 **Quản lý người dùng**
- Đăng ký/Đăng nhập
- Phân quyền admin/user
- Quản lý thông tin cá nhân
- Wishlist sản phẩm

### ⭐ **Đánh giá và tương tác**
- Hệ thống đánh giá sản phẩm
- Chatbot hỗ trợ khách hàng
- Vòng quay may mắn
- Newsletter subscription

### 📊 **Thống kê và báo cáo**
- Thống kê doanh thu
- Báo cáo đơn hàng
- Analytics truy cập
- Dashboard admin

## Công nghệ sử dụng

### Backend
- **ASP.NET MVC 5**
- **Entity Framework 6** (Code First)
- **ASP.NET Identity** (Authentication/Authorization)
- **C# .NET Framework 4.8**

### Frontend
- **Bootstrap 4** (Responsive Design)
- **jQuery**
- **CKEditor** (Rich Text Editor)
- **Font Awesome** (Icons)

### Database
- **SQL Server**
- **Entity Framework Migrations**

### Payment Integration
- **VNPay** (Thanh toán online)

### Email
- **SMTP** (Gmail)

## Cài đặt

### Yêu cầu hệ thống
- Visual Studio 2019/2022
- SQL Server 2016+
- .NET Framework 4.8

### Bước 1: Clone repository
```bash
git clone https://github.com/YOUR_USERNAME/toan-truong-water.git
cd toan-truong-water
```

### Bước 2: Cấu hình database
1. Tạo database mới trong SQL Server
2. Copy `WebBanHangOnline/Web.config.template` thành `WebBanHangOnline/Web.config`
3. Cập nhật connection string trong `Web.config`

### Bước 3: Cấu hình email và VNPay
Trong `Web.config`, cập nhật các thông tin:
- Email settings (SMTP)
- VNPay settings (TMN Code, Hash Secret)

### Bước 4: Restore NuGet packages
```bash
nuget restore
```

### Bước 5: Build và chạy
```bash
# Build project
msbuild WebBanHangOnline.sln

# Hoặc mở trong Visual Studio và nhấn F5
```

## Cấu trúc project

```
WebBanHangOnline/
├── Areas/
│   └── Admin/          # Admin panel
├── Controllers/        # MVC Controllers
├── Models/            # Data models
│   ├── EF/           # Entity Framework models
│   └── ViewModels/   # View models
├── Views/             # Razor views
├── Content/           # CSS, JS, Images
├── Scripts/           # JavaScript files
└── Uploads/           # User uploaded files
```

## Tính năng bảo mật

- **Authentication**: ASP.NET Identity
- **Authorization**: Role-based access control
- **Data Validation**: Model validation attributes
- **SQL Injection Protection**: Entity Framework
- **XSS Protection**: HTML encoding

## Deployment

### Local Development
- Visual Studio IIS Express
- SQL Server LocalDB

### Production
- IIS Server
- SQL Server
- SSL Certificate (HTTPS)


