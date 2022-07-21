using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using booklistMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace booklistMVC.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDbContext _db;
        [BindProperty]
        public book Book { get; set; }
        public BookController(AppDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Book = new book();
            if (id == null)
            {
                //create
                return View(Book);
            }
            //update
            Book = _db.books.FirstOrDefault(u => u.Id == id);
            if (Book == null)
            {
                return NotFound();
            }
            return View(Book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                if (Book.Id == 0)
                {
                    //create
                    _db.books.Add(Book);
                }
                else
                {
                    _db.books.Update(Book);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Book);
        }


        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _db.books.ToListAsync() });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var bookFromDb = await _db.books.FirstOrDefaultAsync(u => u.Id == id);
            if (bookFromDb == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }
            _db.books.Remove(bookFromDb);
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion
    }
}
