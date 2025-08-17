using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using WebBanHangOnline.Models;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class StatisticalController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Statistical
        public ActionResult Index()
        {
            var customers = db.Users
                              .Select(c => new { c.Id, c.FullName })
                              .ToList();

            customers.Insert(0, new { Id = "Guest", FullName = "Khách vãng lai" });

            ViewBag.Customers = new SelectList(customers, "Id", "FullName");
            ViewBag.Categories = new SelectList(db.ProductCategories, "Id", "Title");

            return View();
        }

        [HttpGet]
        public ActionResult GetStatistical(string categoryId, string customerId, string fromDate, string toDate)
        {
            // Kiểm tra tính hợp lệ của ngày tháng nhập vào
            DateTime? startDate = null;
            DateTime? endDate = null;

            if (!string.IsNullOrEmpty(fromDate))
            {
                if (!DateTime.TryParseExact(fromDate, "dd/MM/yyyy", null, DateTimeStyles.None, out DateTime parsedStartDate))
                {
                    return Json(new { success = false, message = "Ngày bắt đầu không hợp lệ. Vui lòng nhập lại theo định dạng dd/MM/yyyy." }, JsonRequestBehavior.AllowGet);
                }
                startDate = parsedStartDate;
            }

            if (!string.IsNullOrEmpty(toDate))
            {
                if (!DateTime.TryParseExact(toDate, "dd/MM/yyyy", null, DateTimeStyles.None, out DateTime parsedEndDate))
                {
                    return Json(new { success = false, message = "Ngày kết thúc không hợp lệ. Vui lòng nhập lại theo định dạng dd/MM/yyyy." }, JsonRequestBehavior.AllowGet);
                }
                endDate = parsedEndDate;
            }

            // Parse dữ liệu đầu vào
            int? categoryFilter = null;
            if (!string.IsNullOrEmpty(categoryId))
            {
                categoryFilter = int.Parse(categoryId);
            }

            var query = from o in db.Orders
                        join od in db.OrderDetails on o.Id equals od.OrderId
                        join p in db.Products on od.ProductId equals p.Id
                        where o.Status == 2 // Chỉ tính các đơn hàng đã thanh toán
                        select new
                        {
                            CreatedDate = o.CreatedDate,
                            Quantity = od.Quantity,
                            Price = od.Price,
                            OriginalPrice = p.OriginalPrice,
                            CategoryId = p.ProductCategoryId,
                            CustomerId = o.CustomerId
                        };

            // Lọc theo danh mục sản phẩm
            if (categoryFilter.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryFilter.Value);
            }

            // Lọc theo khách hàng
            if (!string.IsNullOrEmpty(customerId))
            {
                if (customerId == "Guest")
                {
                    query = query.Where(x => x.CustomerId == null);
                }
                else
                {
                    query = query.Where(x => x.CustomerId == customerId);
                }
            }

            // Lọc theo ngày bắt đầu
            if (startDate.HasValue)
            {
                query = query.Where(x => x.CreatedDate >= startDate.Value);
            }

            // Lọc theo ngày kết thúc
            if (endDate.HasValue)
            {
                query = query.Where(x => x.CreatedDate <= endDate.Value);
            }

            // Nhóm và tính toán doanh thu
            var result = query.GroupBy(x => new { x.CategoryId, x.CustomerId, Date = DbFunctions.TruncateTime(x.CreatedDate) })
                              .Select(group => new
                              {
                                  CategoryId = group.Key.CategoryId,
                                  CustomerId = group.Key.CustomerId ?? "Khách vãng lai",
                                  Date = group.Key.Date.Value,
                                  TotalBuy = group.Sum(x => x.Quantity * x.OriginalPrice),
                                  TotalSell = group.Sum(x => x.Quantity * x.Price)
                              })
                              .Select(x => new
                              {
                                  CategoryId = x.CategoryId,
                                  CustomerId = x.CustomerId,
                                  Date = x.Date,
                                  DoanhThu = x.TotalSell,
                                  LoiNhuan = x.TotalSell - x.TotalBuy
                              });

            result = result.OrderByDescending(x => x.Date);

            return Json(new { success = true, Data = result }, JsonRequestBehavior.AllowGet);
        }
    }
}
