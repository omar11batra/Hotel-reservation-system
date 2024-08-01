using Hajz.Data;
using Hajz.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hajz.Controllers
{
    public class TypemorController : Controller
    {
        private readonly HajzDbContext _context;
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;

        public TypemorController(HajzDbContext context, IMapper mapper, IToastNotification toastNotification)
        {
            _context = context;
            _mapper = mapper;
            _toastNotification = toastNotification;
        }
        // GET: TypeRevController
        public async Task<IActionResult> Typemor(string term, int RequiredPage = 1, int rowNum = 10)
        {

            ViewBag.Term = term;
            ViewBag.num = rowNum;
            int pageSize = ViewBag.num;

            if (term != null)
            {
                decimal rowsCount = await _context.Typemor.Where(a => a.Name.Contains(term) || a.Number.ToString().Contains(term)).CountAsync();
                RequiredPage = RequiredPage <= 0 ? 1 : RequiredPage;
                var pageCount = Math.Ceiling(rowsCount / pageSize);
                if (RequiredPage > pageCount)
                {
                    RequiredPage = 1;
                }
                int skipcount = (RequiredPage - 1) * pageSize;
                var searchruselt = await _context.Typemor.Where(a => a.Name.Contains(term) || a.Number.ToString().Contains(term))
                    .Skip(skipcount)
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.pagesCount = pageCount;
                ViewBag.CurrentPage = RequiredPage;
                return View("Typemor", searchruselt);
            }
            else
            {
                decimal rowsCount = await _context.Typemor.CountAsync();
                RequiredPage = RequiredPage <= 0 ? 1 : RequiredPage;
                var pageCount = Math.Ceiling(rowsCount / pageSize);
                if (RequiredPage > pageCount)
                {
                    RequiredPage = 1;
                }
                int skipcount = (RequiredPage - 1) * pageSize;
                var ruselt = await _context.Typemor
                       .Skip(skipcount)
                       .Take(pageSize)
                       .ToListAsync();

                ViewBag.pagesCount = pageCount;
                ViewBag.CurrentPage = RequiredPage;
                return View(ruselt);
            }
        }


        public IActionResult Create()
        {
            var typemor = new Typemor();
            return View("TypemorForm", typemor);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Typemor typemor)
        {
            if (_context.Typemor.Any(a => a.Number == typemor.Number))
            {
                ModelState.AddModelError("Number", "رقم المنطقة موجود مسبقا");
                return View("TypemorForm", typemor);
            }

            if (ModelState.IsValid)
            {
                _context.Add(typemor);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("تم الاضافة بنجاح");
                return RedirectToAction(nameof(Typemor));
            }
            return View("TypemorForm", typemor);
        }

        // GET: Aistinafs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typemor = await _context.Typemor.FindAsync(id);
            if (typemor == null)
            {
                return NotFound();
            }
            return View("TypemorForm", typemor);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Typemor typemor)
        {

            if (ModelState.IsValid)
            {
                if (_context.Typemor.Any(a => a.Number == typemor.Number && a.Id != typemor.Id))
                {

                    ModelState.AddModelError("Number", "رقم المنطقة موجود مسبقا");
                    return View("TyperevForm", typemor);



                }
                try
                {
                    _context.Update(typemor);
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("تم التعديل بنجاح");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TypemorExists(typemor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Typemor));
            }
            return View("TypemorForm", typemor);
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typemor = await _context.Typemor.FindAsync(id);

            if (typemor == null)
            {
                return NotFound();
            }
          
            var hajz = await _context.Newhajz.FirstOrDefaultAsync(a => a.TypemorId == id);
            if ( hajz != null)
            {
                _toastNotification.AddWarningToastMessage("لايمكن لك الحذف");
                return NotFound();
            }
            _context.Typemor.Remove(typemor);
            await _context.SaveChangesAsync();
            return Ok();

        }



        private bool TypemorExists(int id)
        {
            return _context.Typemor.Any(e => e.Id == id);
        }
    }
}
