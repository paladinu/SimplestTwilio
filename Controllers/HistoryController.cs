using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SimplestTwilio.Controllers
{
    public class HistoryController : Controller
    {
        // GET: History
        public ActionResult Index()
        {
            return View();
        }

        // GET: History/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: History/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: History/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: History/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: History/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: History/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: History/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}