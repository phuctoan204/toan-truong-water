using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using PagedList;
using System.Globalization;
using System.Data.Entity;
using WebBanHangOnline.Models.ViewModels;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Order
        public ActionResult Index(string searchString, int? status, string fromDate, string toDate, int? page)
        {
            var orders = db.Orders.AsQueryable();

            // Tìm kiếm theo từ khóa
            if (!string.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(o =>
                    o.Code.Contains(searchString) ||
                    o.CustomerName.Contains(searchString) ||
                    o.Phone.Contains(searchString));
            }

            // Lọc theo trạng thái
            if (status.HasValue)
            {
                orders = orders.Where(o => o.Status == status.Value);
            }

            // Lọc theo ngày tạo
            if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out DateTime startDate))
            {
                orders = orders.Where(o => DbFunctions.TruncateTime(o.CreatedDate) >= startDate);
            }
            if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out DateTime endDate))
            {
                orders = orders.Where(o => DbFunctions.TruncateTime(o.CreatedDate) <= endDate);
            }

            // Phân trang
            int pageSize = 10; // Số lượng đơn hàng mỗi trang
            int pageNumber = page ?? 1; // Gán giá trị mặc định cho page nếu không có

            // Truyền dữ liệu vào ViewBag để giữ giá trị tìm kiếm
            ViewBag.SearchString = searchString;
            ViewBag.Status = status;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.Page = pageNumber; // Cung cấp giá trị mặc định cho Page
            ViewBag.PageSize = pageSize; // Cung cấp giá trị mặc định cho PageSize

            return View(orders.OrderByDescending(o => o.CreatedDate).ToPagedList(pageNumber, pageSize));
        }


        public ActionResult View(int id)
        {
            var item = db.Orders.Find(id);
            return View(item);
        }

        public ActionResult Partial_SanPham(int id)
        {
            var items = db.OrderDetails.Where(x => x.OrderId == id).ToList();
            return PartialView(items);
        }

        [HttpPost]
        public ActionResult UpdateTT(int id, int trangthai)
        {
            var item = db.Orders.Find(id);
            if (item != null)
            {
                // Kiểm tra trạng thái hiện tại
                var oldStatus = item.Status;

                // Điều kiện không cho phép thay đổi từ "Đã thanh toán" sang "Chưa thanh toán"
                if (oldStatus == 2 && trangthai == 1)
                {
                    return Json(new { message = "Không thể thay đổi trạng thái từ 'Đã thanh toán' sang 'Chưa thanh toán'.", Success = false });
                }

                // Điều kiện không cho phép thay đổi từ trạng thái "Hoàn thành" (3) hoặc "Hủy" (4)
                if (oldStatus == 3 || oldStatus == 4)
                {
                    return Json(new { message = "Không thể thay đổi trạng thái của đơn hàng đã hoàn thành hoặc bị hủy.", Success = false });
                }

                db.Orders.Attach(item);
                item.Status = trangthai; // Cập nhật trạng thái
                db.Entry(item).Property(x => x.Status).IsModified = true;
                db.SaveChanges();

                // Cập nhật doanh thu nếu trạng thái thay đổi từ chưa thanh toán sang đã thanh toán
                if (oldStatus != 2 && trangthai == 2)
                {
                    UpdateRevenue(item);
                }

                return Json(new { message = "Cập nhật trạng thái thành công.", Success = true });
            }
            return Json(new { message = "Không tìm thấy đơn hàng.", Success = false });
        }

        // Phương thức cập nhật doanh thu
        private void UpdateRevenue(Order order)
        {
            var revenue = db.Revenues.FirstOrDefault(r => DbFunctions.TruncateTime(r.Date) == DbFunctions.TruncateTime(DateTime.Today));
            if (revenue == null)
            {
                // Nếu chưa có doanh thu cho ngày hôm nay, tạo mới
                revenue = new Revenue
                {
                    Date = DateTime.Today,
                    Total = 0
                };
                db.Revenues.Add(revenue);
            }

            // Cộng thêm doanh thu từ đơn hàng
            revenue.Total += order.TotalAmount;
            db.SaveChanges();
        }

        public void ThongKe(string fromDate, string toDate)
        {
            var query = from o in db.Orders
                        join od in db.OrderDetails on o.Id equals od.OrderId
                        join p in db.Products on od.ProductId equals p.Id
                        where o.Status == 2 // Chỉ tính các đơn hàng đã thanh toán
                        select new
                        {
                            CreatedDate = o.CreatedDate,
                            Quantity = od.Quantity,
                            Price = od.Price,
                            OriginalPrice = p.Price
                        };
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime start = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(x => x.CreatedDate >= start);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(x => x.CreatedDate < endDate);
            }
            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate)).Select(r => new
            {
                Date = r.Key.Value,
                TotalBuy = r.Sum(x => x.OriginalPrice * x.Quantity), // tổng giá bán
                TotalSell = r.Sum(x => x.Price * x.Quantity) // tổng giá mua
            }).Select(x => new RevenueStatisticViewModel
            {
                Date = x.Date,
                Benefit = x.TotalSell - x.TotalBuy,
                Revenues = x.TotalSell
            });
        }
    }
}
