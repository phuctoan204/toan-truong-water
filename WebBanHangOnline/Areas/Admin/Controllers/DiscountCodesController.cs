using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class DiscountCodesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // Danh sách mã giảm giá
        public ActionResult Index()
        {
            var discountCodes = db.DiscountCodes.ToList();
            return View(discountCodes);
        }

        // Tạo mã giảm giá mới
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DiscountCode discountCode)
        {
            if (ModelState.IsValid)
            {
                discountCode.IsUsed = false; // Đảm bảo mã mới chưa được sử dụng
                db.DiscountCodes.Add(discountCode);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(discountCode);
        }

        // GET: Admin/DiscountCodes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var discountCode = db.DiscountCodes.Find(id);
            if (discountCode == null)
                return HttpNotFound();

            return View(discountCode);
        }

        // POST: Admin/DiscountCodes/DeleteConfirmed/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var discountCode = db.DiscountCodes.Find(id);
            if (discountCode != null)
            {
                db.DiscountCodes.Remove(discountCode);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

    }
}