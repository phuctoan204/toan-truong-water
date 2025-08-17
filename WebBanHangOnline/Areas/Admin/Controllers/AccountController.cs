using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using PagedList;
using WebBanHangOnline.Models.ViewModels;
using System.Drawing.Printing;
using System.Drawing;
using System.IO;
using System.Xml.Linq;
using iText.Layout.Properties;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;
using Font = iTextSharp.text.Font;

namespace WebBanHangOnline.Areas.Admin.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Admin/Account
        public ActionResult Index(string searchTerm = "")
        {
            // Lọc danh sách tài khoản dựa trên từ khóa tìm kiếm
            var items = db.Users
                          .Where(u => string.IsNullOrEmpty(searchTerm) ||
                                      u.UserName.Contains(searchTerm) ||
                                      u.FullName.Contains(searchTerm))
                          .ToList();

            ViewBag.SearchTerm = searchTerm; // Để giữ lại từ khóa tìm kiếm trên giao diện
            return View(items);
        }



        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Create()
        {
            ViewBag.Role = new SelectList(db.Roles.ToList(), "Name", "Name");
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                    Phone = model.Phone
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (model.Roles != null)
                    {
                        foreach (var r in model.Roles)
                        {
                            UserManager.AddToRole(user.Id, r);
                        }
                    }



                    return RedirectToAction("Index", "Account");
                }
                AddErrors(result);
            }
            ViewBag.Role = new SelectList(db.Roles.ToList(), "Name", "Name");

            return View(model);
        }


        public ActionResult Edit(string id)
        {
            var item = UserManager.FindById(id);
            var newUser = new EditAccountViewModel();
            if (item != null)
            {
                var rolesForUser = UserManager.GetRoles(id);
                var roles = new List<string>();
                if (rolesForUser != null)
                {
                    foreach (var role in rolesForUser)
                    {
                        roles.Add(role);

                    }

                }
                newUser.FullName = item.FullName;
                newUser.Email = item.Email;
                newUser.Phone = item.Phone;
                newUser.UserName = item.UserName;
                newUser.Roles = roles;
            }
            ViewBag.Role = new SelectList(db.Roles.ToList(), "Name", "Name");
            return View(newUser);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindByName(model.UserName);
                user.FullName = model.FullName;
                user.Phone = model.Phone;
                user.Email = model.Email;
                var result = await UserManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    var rolesForUser = UserManager.GetRoles(user.Id);
                    if (model.Roles != null)
                    {

                        foreach (var r in model.Roles)
                        {
                            var checkRole = rolesForUser.FirstOrDefault(x => x.Equals(r));
                            if (checkRole == null)
                            {
                                UserManager.AddToRole(user.Id, r);
                            }

                        }
                    }


                    return RedirectToAction("Index", "Account");
                }
                AddErrors(result);
            }
            ViewBag.Role = new SelectList(db.Roles.ToList(), "Name", "Name");

            return View(model);
        }
        public ActionResult LichSuMuaHang(string userId, int? page)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                // Lấy thông tin khách hàng
                var user = db.Users.Find(userId);
                if (user == null)
                {
                    return HttpNotFound("Người dùng không tồn tại.");
                }

                // Lấy các đơn hàng của người dùng
                var orders = db.Orders.Where(x => x.CustomerId == user.Id)
                                      .OrderByDescending(x => x.CreatedDate)
                                      .ToList();

                // Chuyển đổi các đơn hàng thành danh sách OrderHistoryViewModel
                var orderHistories = orders.Select(order => new OrderHistoryViewModel
                {
                    OrderId = order.Id,
                    OrderCode = order.Code,
                    CreatedDate = order.CreatedDate,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    OrderDetails = order.OrderDetails.Select(detail => new OrderDetailViewModel
                    {
                        ProductName = detail.Product.Title,
                        Price = detail.Price,
                        Quantity = detail.Quantity
                    }).ToList()
                }).ToList();

                // Phân trang
                int pageSize = 10;
                int pageNumber = page ?? 1;
                var pagedOrderHistories = orderHistories.ToPagedList(pageNumber, pageSize);

                ViewBag.UserId = user.Id;
                ViewBag.UserName = user.UserName;
                ViewBag.FullName = user.FullName;

                return View(pagedOrderHistories);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAccount(string user, string id)
        {
            var code = new { Success = false };
            var item = UserManager.FindByName(user);
            if (item != null)
            {
                var rolesForUser = UserManager.GetRoles(id);
                if (rolesForUser != null)
                {
                    foreach (var role in rolesForUser)
                    {
                        //roles.Add(role);
                        await UserManager.RemoveFromRoleAsync(id, role);
                    }

                }

                var res = await UserManager.DeleteAsync(item);
                code = new { Success = res.Succeeded };
            }
            return Json(code);
        }
        [HttpGet]
        public ActionResult XuatFilePdfLichSuMuaHang(string userId)
        {
            var user = db.Users.Find(userId);
            if (user == null)
            {
                return HttpNotFound("Người dùng không tồn tại.");
            }

            var orders = db.Orders.Where(x => x.CustomerId == userId)
                                  .OrderByDescending(x => x.CreatedDate)
                                  .ToList();

            if (!orders.Any())
            {
                return new HttpStatusCodeResult(400, "Người dùng không có lịch sử mua hàng.");
            }

            using (var ms = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 25, 25, 30, 30);
                var writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                string fontPath = @"C:\Windows\Fonts\Arial.ttf";
                if (!System.IO.File.Exists(fontPath))
                {
                    throw new FileNotFoundException("Font Arial.ttf không tồn tại tại đường dẫn " + fontPath);
                }
                BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                var textFont = new Font(baseFont, 12, Font.NORMAL, BaseColor.BLACK);
                var titleFont = new Font(baseFont, 16, Font.BOLD, BaseColor.BLACK);

                // Tiêu đề "Lịch sử mua hàng"
                var header = new Paragraph("Lịch sử mua hàng", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                };
                document.Add(header);

                // Tiêu đề phần thông tin người dùng
                var title = new Paragraph($"Lịch Sử Mua Hàng - {user.FullName} ({user.UserName})", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                };
                document.Add(title);

                foreach (var order in orders)
                {
                    // Thông tin đơn hàng
                    var orderInfo = new Paragraph($"Mã Đơn Hàng: {order.Code} - Ngày Đặt: {order.CreatedDate:dd/MM/yyyy}", textFont)
                    {
                        SpacingAfter = 10
                    };
                    document.Add(orderInfo);

                    // Tạo bảng chi tiết đơn hàng
                    var table = new PdfPTable(5) { WidthPercentage = 100 };
                    table.SetWidths(new float[] { 3, 6, 3, 2, 4 });

                    // Tiêu đề cột
                    AddCellToTable(table, "STT", textFont);
                    AddCellToTable(table, "Tên Sản Phẩm", textFont);
                    AddCellToTable(table, "Đơn Giá", textFont);
                    AddCellToTable(table, "Số Lượng", textFont);
                    AddCellToTable(table, "Tổng Cộng", textFont);

                    var orderDetails = order.OrderDetails.ToList();
                    for (int i = 0; i < orderDetails.Count; i++)
                    {
                        var detail = orderDetails[i];
                        AddCellToTable(table, (i + 1).ToString(), textFont);
                        AddCellToTable(table, detail.Product.Title, textFont);
                        AddCellToTable(table, $"{detail.Price:N0} VNĐ", textFont, Element.ALIGN_RIGHT);
                        AddCellToTable(table, detail.Quantity.ToString(), textFont, Element.ALIGN_CENTER);

                        if (i == 0)
                        {
                            AddCellToTable(table, $"{order.TotalAmount:N0} VNĐ", textFont, Element.ALIGN_RIGHT, orderDetails.Count);
                        }
                    }

                    document.Add(table);

                    // Thêm trạng thái đơn hàng
                    string status = "Không xác định";
                    if (order.Status == 1)
                        status = "Chưa thanh toán";
                    else if (order.Status == 2)
                        status = "Đã thanh toán";
                    else if (order.Status == 3)
                        status = "Hoàn thành";
                    else if (order.Status == 4)
                        status = "Đã hủy";

                    var orderStatus = new Paragraph($"Trạng Thái: {status}", textFont)
                    {
                        SpacingAfter = 20
                    };
                    document.Add(orderStatus);
                }

                document.Close();

                return File(ms.ToArray(), "application/pdf", $"LichSuMuaHang_{user.UserName}.pdf");
            }
        }

        private void AddCellToTable(PdfPTable table, string text, Font font, int alignment = Element.ALIGN_LEFT, int rowspan = 1)
        {
            var cell = new PdfPCell(new Phrase(text, font))
            {
                HorizontalAlignment = alignment,
                Rowspan = rowspan
            };
            table.AddCell(cell);
        }


        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}