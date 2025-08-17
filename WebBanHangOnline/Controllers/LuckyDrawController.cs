using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Controllers
{
    public class LuckyDrawController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: LuckyDraw
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Spin(string fullName, string email)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ họ tên và email!";
                return RedirectToAction("Index");
            }

            var prizes = new[] { "Chúc bạn may mắn lần sau", "Giảm 5%", "Giảm 10%", "Miễn phí vận chuyển", "Giảm 15%" };
            var random = new Random();
            string prize = prizes[random.Next(prizes.Length)];

            // Lưu vào DB
            var draw = new LuckyDraw
            {
                FullName = fullName,
                Email = email,
                Prize = prize
            };

            db.LuckyDraws.Add(draw);
            db.SaveChanges();

            // Gửi email nếu trúng thưởng (trừ "Chúc bạn may mắn lần sau")
            if (prize != "Chúc bạn may mắn lần sau")
            {
                SendEmail(email, "Chúc mừng bạn trúng thưởng!", $"Bạn đã trúng: {prize} từ vòng quay may mắn của chúng tôi!");
            }

            TempData["Prize"] = prize;
            return RedirectToAction("Result");
        }

        public ActionResult Result()
        {
            ViewBag.Prize = TempData["Prize"];
            return View();
        }

        private void SendEmail(string toEmail, string subject, string body)
        {
            var mail = new MailMessage();
            mail.To.Add(toEmail);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            mail.From = new MailAddress("youremail@gmail.com"); // cấu hình SMTP riêng

            using (var smtp = new SmtpClient())
            {
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential("youremail@gmail.com", "yourpassword");
                smtp.Send(mail);
            }
        }
    }
}