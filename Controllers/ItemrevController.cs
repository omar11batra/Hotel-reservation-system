using Hajz.Data;
using Hajz.Models;
using Hajz.ViewModel;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hajz.Controllers
{
    [Authorize(Roles = "مدير_النظام")]
    public class ItemrevController : Controller
    {
        private readonly HajzDbContext _context;
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;

        public ItemrevController(HajzDbContext context, IMapper mapper, IToastNotification toastNotification)
        {
            _context = context;
            _mapper = mapper;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Itemrev(string term, int RequiredPage = 1, int rowNum = 10)
        {

            ViewBag.Term = term;
            ViewBag.num = rowNum;
            int pageSize = ViewBag.num;

            if (term != null)
            {
                decimal rowsCount = await _context.Itemrev.Include(a => a.Typerev)
                    .Where(a => a.Name.Contains(term) || a.Number.ToString().Contains(term)
                    || a.Typerev.Name.Contains(term)).CountAsync();
                RequiredPage = RequiredPage <= 0 ? 1 : RequiredPage;
                var pageCount = Math.Ceiling(rowsCount / pageSize);
                if (RequiredPage > pageCount)
                {
                    RequiredPage = 1;
                }
                int skipcount = (RequiredPage - 1) * pageSize;
                var searchruselt = await _context.Itemrev.Include(a => a.Typerev).Where(a => a.Name.Contains(term) || a.Number.ToString().Contains(term) || a.Typerev.Name.Contains(term))
                    .Skip(skipcount)
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.pagesCount = pageCount;
                ViewBag.CurrentPage = RequiredPage;
                return View("Itemrev", searchruselt);
            }
            else
            {
                decimal rowsCount = await _context.Itemrev.CountAsync();
                RequiredPage = RequiredPage <= 0 ? 1 : RequiredPage;
                var pageCount = Math.Ceiling(rowsCount / pageSize);
                if (RequiredPage > pageCount)
                {
                    RequiredPage = 1;
                }
                int skipcount = (RequiredPage - 1) * pageSize;
                var ruselt = await _context.Itemrev.Include(a => a.Typerev)
                       .Skip(skipcount)
                       .Take(pageSize)
                       .ToListAsync();

                ViewBag.pagesCount = pageCount;
                ViewBag.CurrentPage = RequiredPage;
                return View(ruselt);
            }
        }


        [Authorize(Roles = "مدير_النظام,مستخدم")]
        public async Task<IActionResult> Create()
        {
            var itemrevForm = new ItemrevFormVM
            {
                Typerev = await _context.Typerev.ToListAsync()
            };
            return View("ItemrevForm", itemrevForm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "مدير_النظام,مستخدم")]
        public async Task<IActionResult> Create(ItemrevFormVM model)
        {


            if (!ModelState.IsValid)
            {
                model.Typerev = await _context.Typerev.OrderBy(m => m.Name).ToListAsync();
                return View("Itemrevform", model);
            }
            if (_context.Itemrev.Any(a => a.Number == model.Number && a.TyperevId == model.TyperevId))
            {
                ModelState.AddModelError("Number", "رقم المنطقة موجود مسبقا");
                model.Typerev = await _context.Typerev.ToListAsync();
                return View("Itemrevform", model);
            }
            var itemrev = _mapper.Map<Itemrev>(model);

            _context.Itemrev.Add(itemrev);
            await _context.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("تم الاضافة بنجاح");

            return RedirectToAction(nameof(Itemrev));
        }


        [Authorize(Roles = "مدير_النظام,مستخدم")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var itemrev = await _context.Itemrev.FindAsync(id);
            if (itemrev == null)
            {
                return NotFound();
            }
            var itemrevForm = _mapper.Map<ItemrevFormVM>(itemrev);
            itemrevForm.Typerev = await _context.Typerev.ToListAsync();



            return View("ItemrevForm", itemrevForm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "مدير_النظام,مستخدم")]
        public async Task<IActionResult> Edit(ItemrevFormVM model)
        {



            if (ModelState.IsValid)
            {
                if (_context.Itemrev.Any(a => a.Number == model.Number && a.TyperevId == model.TyperevId && a.Id != model.Id))
                {
                    ModelState.AddModelError("Number", "رقم المنطقة موجود مسبقا");
                    model.Typerev = await _context.Typerev.ToListAsync();
                    return View("MudiriaForm", model);
                }
                try
                {
                    _context.Update(_mapper.Map<Itemrev>(model));
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("تم التعديل بنجاح");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemrevExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Itemrev));
            }
            return View("ItemrevForm", model);
        }

        [HttpDelete]
        [Authorize(Roles = "مدير_النظام")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var itemrev = await _context.Itemrev.FindAsync(id);

            if (itemrev == null)
            {
                return NotFound();
            }
            var hajz = await _context.Newhajz.FirstOrDefaultAsync(a => a.ItemrevId == id);
            if (hajz != null)
            {
                _toastNotification.AddWarningToastMessage("لايمكن لك الحذف");
                return NotFound();
            }
            _context.Itemrev.Remove(itemrev);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool ItemrevExists(int id)
        {
            return _context.Itemrev.Any(e => e.Id == id);
        }
    }
}
