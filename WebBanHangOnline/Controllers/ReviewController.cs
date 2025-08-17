using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;
using PagedList;
using WebBanHangOnline.Models.ViewModels;

namespace WebBanHangOnline.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        // GET: Review
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult _Review(int productId)
        {
            ViewBag.ProductId = productId;
            var item = new ReviewProduct();
            if (User.Identity.IsAuthenticated)
            {
                var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
                var userManager = new UserManager<ApplicationUser>(userStore);
                var user = userManager.FindByName(User.Identity.Name);
                if (user != null)
                {
                    item.Email = user.Email;
                    item.FullName = user.FullName;
                    item.UserName = user.UserName;
                }
                return PartialView(item);
            }
            return PartialView();
        }

        public ActionResult LichSuDonHang(int? page)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userStore = new UserStore<ApplicationUser>(_db);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var user = userManager.FindByName(User.Identity.Name);

                // Lấy danh sách đơn hàng của người dùng hiện tại
                var orders = _db.Orders
                    .Where(x => x.CustomerId == user.Id)
                    .OrderByDescending(x => x.CreatedDate) // Sắp xếp theo ngày tạo giảm dần
                    .ToList();

                // Chuyển đổi sang OrderHistoryViewModel
                var orderHistoryViewModels = orders.Select(order => new OrderHistoryViewModel
                {
                    OrderId = order.Id,
                    OrderCode = order.Code,
                    CreatedDate = order.CreatedDate,
                    TotalAmount = order.TotalAmount,
                    
                    DiscountAmount = order.DiscountAmount,
                    
                    Status = order.Status,
                    OrderDetails = _db.OrderDetails
                        .Where(detail => detail.OrderId == order.Id)
                        .Select(detail => new OrderDetailViewModel
                        {
                            ProductName = detail.Product.Title,
                            Price = detail.Price,
                            Quantity = detail.Quantity,
                            
                        }).ToList()
                }).ToList();

                // Phân trang
                int pageSize = 10; // Kích thước trang
                int pageNumber = page ?? 1; // Nếu không có tham số page thì mặc định là trang 1
                var pagedOrderHistory = orderHistoryViewModels.ToPagedList(pageNumber, pageSize);

                ViewBag.PageSize = pageSize;
                ViewBag.Page = pageNumber;

                return View(pagedOrderHistory); // Trả về view với IPagedList
            }

            return RedirectToAction("Index", "Account"); // Nếu chưa đăng nhập, chuyển hướng tới trang đăng nhập
        }




        [AllowAnonymous]
        public ActionResult _Load_Review(int productId)
        {
            var item = _db.Reviews.Where(x => x.ProductId == productId).OrderByDescending(x => x.Id).ToList();
            ViewBag.Count = item.Count;
            return PartialView(item);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult PostReview(ReviewProduct req)
        {
            if (ModelState.IsValid)
            {
                req.CreatedDate = DateTime.Now;
                _db.Reviews.Add(req);
                _db.SaveChanges();
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}