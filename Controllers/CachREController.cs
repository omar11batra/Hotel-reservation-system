using Hajz.Data;
using Hajz.Models;
using Hajz.ViewModel;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hajz.Controllers
{
    [Authorize(Roles = "مدير_النظام,مستخدم")]
    public class CachREController : Controller
    {
        private readonly HajzDbContext _context;
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;
        private readonly UserManager<User> _userManager;

        public CachREController(HajzDbContext context, IMapper mapper, IToastNotification toastNotification, UserManager<User> userManager)
        {
            _context = context;
            _mapper = mapper;
            _toastNotification = toastNotification;
            _userManager = userManager;
        }

        public async Task<IActionResult> CachRE(string term, int RequiredPage = 1, int rowNum = 10)
        {

            ViewBag.Term = term;
            ViewBag.num = rowNum;
            int pageSize = ViewBag.num;

            if (term != null)
            {
                decimal rowsCount = await _context.CachRE.Include(a => a.Newhajz)
                    .Where(a => a.Notice.Contains(term) || a.Number.ToString().Contains(term)
                    ).CountAsync();
                RequiredPage = RequiredPage <= 0 ? 1 : RequiredPage;
                var pageCount = Math.Ceiling(rowsCount / pageSize);
                if (RequiredPage > pageCount)
                {
                    RequiredPage = 1;
                }
                int skipcount = (RequiredPage - 1) * pageSize;
                var searchruselt = await _context.CachRE.Include(a=>a.Newhajz).Where(a => a.Notice.Contains(term) || a.Number.ToString().Contains(term))
                    .Skip(skipcount)
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.pagesCount = pageCount;
                ViewBag.CurrentPage = RequiredPage;
                return View("CachRE", searchruselt);
            }
            else
            {
                decimal rowsCount = await _context.CachRE.CountAsync();
                RequiredPage = RequiredPage <= 0 ? 1 : RequiredPage;
                var pageCount = Math.Ceiling(rowsCount / pageSize);
                if (RequiredPage > pageCount)
                {
                    RequiredPage = 1;
                }
                int skipcount = (RequiredPage - 1) * pageSize;
                var ruselt = await _context.CachRE.Include(a=>a.Newhajz)
                       .Skip(skipcount)
                       .Take(pageSize)
                       .ToListAsync();

                ViewBag.pagesCount = pageCount;
                ViewBag.CurrentPage = RequiredPage;
                return View(ruselt);
            }
        }


        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);

            var numberOrder = await _context.CachRE.OrderBy(n => n.Number).LastOrDefaultAsync();

            var addOne = 0;
            if (numberOrder is not null)
            {
                addOne = ++numberOrder.Number;
            }
            else
            {
                addOne = 1;
            }
            var cachreForm = new CachREFormVM
            {
                UserId = user.Id,
                Number = addOne
            };

            //{
            //    Cach = await _context.Cach.ToListAsync()
            //};
            //var rand = new Random();
            //var uid = rand.Next(1000, 10000000);
            //ViewBag.Number = uid.ToString();
            ////ViewBag.User = user;

            return View("CachREForm", cachreForm);
        }
        public JsonResult GetNewhajz(string id)
        {


            ViewBag.NewhajzName = _context.Newhajz.Where(b => b.PhoneNumber == id).FirstOrDefault();
            var s = ViewBag.NewhajzName;

            return Json(s);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "مدير_النظام,مستخدم")]
        public async Task<IActionResult> Create(CachREFormVM model)
        {


            if (!ModelState.IsValid)
            {
                //model.Cach = await _context.Cach.OrderBy(m => m.Name).ToListAsync();
                return View("CachREform", model);
            }
            if (_context.CachRE.Any(a => a.Number == model.Number /*&& a.CachId == model.CachId*/))
            {
                ModelState.AddModelError("Number", "رقم الصندوق موجود مسبقا");
                //model.Cach = await _context.Cach.ToListAsync();
                return View("CachREform", model);
            }
            var cachre = _mapper.Map<CachRE>(model);
            var user = await _userManager.GetUserAsync(User);
            model.UserId = user.Id;

            var newHag = _context.Newhajz.Find(model.NewhajzId);

            newHag.PriceR=model.PriceR;
            _context.CachRE.Add(cachre);
            _context.Newhajz.Update(newHag);
            await _context.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("تم الاضافة بنجاح");

            return RedirectToAction(nameof(CachRE));
        }


        [Authorize(Roles = "مدير_النظام,مستخدم")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var cachre = await _context.CachRE.FindAsync(id);
            if (cachre == null)
            {
                return NotFound();
            }
            var cachreForm = _mapper.Map<CachREFormVM>(cachre);
            //cachreForm.Cach = await _context.Cach.ToListAsync();



            return View("CachREForm", cachreForm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "مدير_النظام,مستخدم")]
        public async Task<IActionResult> Edit(CachREFormVM model)
        {



            if (ModelState.IsValid)
            {
                if (_context.CachRE.Any(a => a.Number == model.Number /*&& a.CachId == model.CachId*/ && a.Id != model.Id))
                {
                    ModelState.AddModelError("Number", "الرقم  موجود مسبقا");
                    //model.Cach = await _context.Cach.ToListAsync();
                    return View("CachREForm", model);
                }
                try
                {
                    _context.Update(_mapper.Map<CachRE>(model));
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("تم التعديل بنجاح");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CachREExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(CachRE));
            }
            return View("CachREForm", model);
        }

        [HttpDelete]
        [Authorize(Roles = "مدير_النظام")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var cachre = await _context.CachRE.FindAsync(id);

            if (cachre == null)
            {
                return NotFound();
            }
            _context.CachRE.Remove(cachre);
            await _context.SaveChangesAsync();
            return Ok();
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();
            var user = await _userManager.GetUserAsync(User);

            var cachre = await _context.CachRE
            //.Include(a => a.Name)
            //.Include(a => a.Address)
            //.Include(a => a.PhoneNumber)
                .Include(a => a.User)
                .Include(a => a.Newhajz)
              
            //.Include(a => a.Tall)
            //.Include(a => a.Com)
            //.Include(a => a.Ktf)
            //.Include(a => a.Qsr)
            //.Include(a => a.Notice)
            ////.Include(a => a.Price)
            ////.Include(a => a.PriceD)
            //.Include(a => a.PriceR)
            .FirstOrDefaultAsync(m => m.Id == id);

            if (cachre == null)
            {
                return NotFound();
            }

            return View("CachREDetails", cachre);

        }

        private bool CachREExists(int id)
        {
            return _context.CachRE.Any(e => e.Id == id);
        }


    }
}
