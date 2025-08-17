using System;
using System.Linq;
using System.Web.Mvc;
using WebBanHangOnline.Models.EF;
using WebBanHangOnline.Models;
namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ChatbotAdminController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var messages = db.ChatMessages.OrderByDescending(x => x.CreatedDate).ToList();
            return View(messages);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChatMessage model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                model.IsActive = true;
                db.ChatMessages.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var message = db.ChatMessages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ChatMessage model)
        {
            if (ModelState.IsValid)
            {
                var message = db.ChatMessages.Find(model.Id);
                if (message != null)
                {
                    message.Question = model.Question;
                    message.Answer = model.Answer;
                    message.IsActive = model.IsActive;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var message = db.ChatMessages.Find(id);
            if (message != null)
            {
                db.ChatMessages.Remove(message);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}