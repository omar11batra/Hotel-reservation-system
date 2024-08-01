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

    public class TyperevController : Controller
    {
        private readonly HajzDbContext _context;
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;

        public TyperevController(HajzDbContext context, IMapper mapper, IToastNotification toastNotification)
        {
            _context = context;
            _mapper = mapper;
            _toastNotification = toastNotification;
        }
        // GET: TypeRevController
        public async Task<IActionResult> Typerev(string term, int RequiredPage = 1, int rowNum = 10)
        {

            ViewBag.Term = term;
            ViewBag.num = rowNum;
            int pageSize = ViewBag.num;

            if (term != null)
            {
                decimal rowsCount = await _context.Typerev.Where(a => a.Name.Contains(term) || a.Number.ToString().Contains(term)).CountAsync();
                RequiredPage = RequiredPage <= 0 ? 1 : RequiredPage;
                var pageCount = Math.Ceiling(rowsCount / pageSize);
                if (RequiredPage > pageCount)
                {
                    RequiredPage = 1;
                }
                int skipcount = (RequiredPage - 1) * pageSize;
                var searchruselt = await _context.Typerev.Where(a => a.Name.Contains(term) || a.Number.ToString().Contains(term))
                    .Skip(skipcount)
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.pagesCount = pageCount;
                ViewBag.CurrentPage = RequiredPage;
                return View("Typerev", searchruselt);
            }
            else
            {
                decimal rowsCount = await _context.Typerev.CountAsync();
                RequiredPage = RequiredPage <= 0 ? 1 : RequiredPage;
                var pageCount = Math.Ceiling(rowsCount / pageSize);
                if (RequiredPage > pageCount)
                {
                    RequiredPage = 1;
                }
                int skipcount = (RequiredPage - 1) * pageSize;
                var ruselt = await _context.Typerev
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
            var typeRev = new Typerev();
            return View("TyperevForm", typeRev);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Typerev typeRev)
        {
            if (_context.Typerev.Any(a => a.Number == typeRev.Number))
            {
                ModelState.AddModelError("Number", "رقم  موجود مسبقا");
                return View("TyperevForm", typeRev);
            }

            if (ModelState.IsValid)
            {
                _context.Add(typeRev);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("تم الاضافة بنجاح");
                return RedirectToAction(nameof(Typerev));
            }
            return View("TyperevForm", typeRev);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeRev = await _context.Typerev.FindAsync(id);
            if (typeRev == null)
            {
                return NotFound();
            }
            return View("TyperevForm", typeRev);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Typerev typeRev)
        {

            if (ModelState.IsValid)
            {
                if (_context.Typerev.Any(a => a.Number == typeRev.Number && a.Id != typeRev.Id))
                {

                    ModelState.AddModelError("Number", "رقم المنطقة موجود مسبقا");
                    return View("TyperevForm", typeRev);



                }
                try
                {
                    _context.Update(typeRev);
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("تم التعديل بنجاح");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TyperevExists(typeRev.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Typerev));
            }
            return View("TyperevForm", typeRev);
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeRev = await _context.Typerev.FindAsync(id);

            if (typeRev == null)
            {
                return NotFound();
            }
            var iteme=await _context.Itemrev.FirstOrDefaultAsync(a=> a.TyperevId==id);
            var hajz = await _context.Newhajz.FirstOrDefaultAsync(a => a.TyperevId == id);
            if(iteme != null || hajz != null)
            {
                _toastNotification.AddWarningToastMessage("لايمكن لك الحذف");
                return NotFound();
            }
            _context.Typerev.Remove(typeRev);
            await _context.SaveChangesAsync();
            return Ok();

        }



        private bool TyperevExists(int id)
        {
            return _context.Typerev.Any(e => e.Id == id);
        }
    }
}
