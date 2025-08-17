using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models.EF;
using WebBanHangOnline.Models;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class ProductTypeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/ProductType
        public ActionResult Index()
        {
            var items = db.ProductTypes;
            return View(items);
        }

        // GET: Admin/ProductType/Add
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ProductType model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng lặp tên loại sản phẩm
                var existingType = db.ProductTypes
                    .FirstOrDefault(t => t.Title == model.Title);

                if (existingType != null)
                {
                    ModelState.AddModelError("Title", "Tên loại sản phẩm đã tồn tại.");
                }

                if (ModelState.IsValid)
                {
                    model.CreatedDate = DateTime.Now;
                    model.ModifiedDate = DateTime.Now;
                    model.Alias = WebBanHangOnline.Models.Common.Filter.FilterChar(model.Title);
                    db.ProductTypes.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        // GET: Admin/ProductType/Edit/{id}
        public ActionResult Edit(int id)
        {
            var item = db.ProductTypes.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductType model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng lặp tên loại sản phẩm
                var existingType = db.ProductTypes
                    .FirstOrDefault(t => t.Title == model.Title && t.Id != model.Id);

                if (existingType != null)
                {
                    ModelState.AddModelError("Title", "Tên loại sản phẩm đã tồn tại.");
                }

                if (ModelState.IsValid)
                {
                    model.ModifiedDate = DateTime.Now;
                    model.Alias = WebBanHangOnline.Models.Common.Filter.FilterChar(model.Title);
                    db.ProductTypes.Attach(model);
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }
    }
}