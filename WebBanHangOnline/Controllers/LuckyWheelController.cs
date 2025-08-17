using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;
using System.Web.Configuration;

namespace WebBanHangOnline.Controllers
{
    public class LuckyWheelController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Spin(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            {
                return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin." });
            }

            // Kiểm tra nếu email đã quay trước đó
            if (db.LuckyWheelEntries.Any(e => e.Email == email))
            {
                return Json(new { success = false, message = "Bạn đã tham gia vòng quay rồi!" });
            }

            var random = new Random();
            var prizes = new List<string> { "10% Giảm giá", "Chúc may mắn lần sau", "20% Giảm giá", "Miễn phí bình nước", "30% Giảm giá" };
            string prize = prizes[random.Next(prizes.Count)];
            string discountCode = null;
            int discountCodeId = 0;

            if (prize.Contains("Giảm giá"))
            {
                // Tạo mã giảm giá mới
                var discount = new DiscountCode
                {
                    Code = GenerateDiscountCode(),
                    DiscountPercent = int.Parse(prize.Split('%')[0]),
                    MaxDiscountAmount = 100000, // Giảm tối đa 100,000 VND
                    IsUsed = false,
                    ExpiryDate = DateTime.Now.AddDays(7) // Mã giảm giá có hiệu lực trong 7 ngày
                };

                db.DiscountCodes.Add(discount);
                db.SaveChanges();

                discountCode = discount.Code;
                discountCodeId = discount.Id;
            }

            // Lưu lịch sử quay vào database
            var luckyEntry = new LuckyWheelEntry
            {
                Name = name,
                Email = email,
                DiscountCodeId = discountCodeId,
                CreatedAt = DateTime.Now
            };

            db.LuckyWheelEntries.Add(luckyEntry);
            db.SaveChanges();

            // Gửi email sau khi trúng thưởng
            SendWinningEmail(email, name, prize, discountCode);

            return Json(new { success = true, prize, discountCode });
        }

        private string GenerateDiscountCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void SendWinningEmail(string email, string name, string prize, string discountCode)
        {
            try
            {
                string smtpServer = WebConfigurationManager.AppSettings["SmtpServer"];
                int smtpPort = int.Parse(WebConfigurationManager.AppSettings["SmtpPort"]);
                string smtpUsername = WebConfigurationManager.AppSettings["SmtpUsername"];
                string smtpPassword = WebConfigurationManager.AppSettings["SmtpPassword"];
                bool enableSSL = bool.Parse(WebConfigurationManager.AppSettings["SmtpEnableSSL"]);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(smtpUsername, "PHUCTOANWATER");
                    mail.To.Add(email);
                    mail.Subject = "🎉 Chúc mừng! Bạn đã trúng thưởng 🎉";
                    mail.Body = $"Xin chào {name},\n\nBạn đã trúng: {prize} 🎊\n\n";

                    if (!string.IsNullOrEmpty(discountCode))
                    {
                        mail.Body += $"🎁 Mã giảm giá của bạn: **{discountCode}**\n";
                    }

                    mail.Body += "\nCảm ơn bạn đã tham gia chương trình Vòng quay may mắn!\n\nPHUCTOANWATER";
                    mail.IsBodyHtml = false;

                    using (SmtpClient smtp = new SmtpClient(smtpServer, smtpPort))
                    {
                        smtp.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi gửi email: " + ex.Message);
            }
        }
    }
}