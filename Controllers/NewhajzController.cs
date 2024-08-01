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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hajz.Controllers
{

    [Authorize(Roles = "بحث,مدير_النظام,مستخدم")]
    public class NewhajzController : Controller
    {
        private readonly HajzDbContext _context;
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;
        private readonly UserManager<User> _userManager;


        public NewhajzController(HajzDbContext context, IMapper mapper, IToastNotification toastNotification, UserManager<User> userManager)
        {
            _context = context;
            _mapper = mapper;
            _toastNotification = toastNotification;
            _userManager = userManager;

        }

        public async Task<IActionResult> Newhajz(string term, int RequiredPage = 1, int rowNum = 10)
        {

            ViewBag.Term = term;
            ViewBag.num = rowNum;
            int pageSize = ViewBag.num;

            if (term != null)
            {
                decimal rowsCount = await _context.Newhajz.Include(a=>a.Typerev).Include(a=>a.Itemrev).Include(a=>a.Typemor).Where(a => a.Name.Contains(term) || a.Number.ToString().Contains(term)).CountAsync();
                RequiredPage = RequiredPage <= 0 ? 1 : RequiredPage;
                var pageCount = Math.Ceiling(rowsCount / pageSize);
                if (RequiredPage > pageCount)
                {
                    RequiredPage = 1;
                }
                int skipcount = (RequiredPage - 1) * pageSize;
                var searchruselt = await _context.Newhajz.Include(a => a.Typerev).Include(a => a.Itemrev).Include(a => a.Typemor).Where(a => a.Name.Contains(term) || a.Number.ToString().Contains(term))
                    .Skip(skipcount)
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.pagesCount = pageCount;
                ViewBag.CurrentPage = RequiredPage;
                return View("Newhajz", searchruselt);
            }
            else
            {
                decimal rowsCount = await _context.Newhajz.CountAsync();
                RequiredPage = RequiredPage <= 0 ? 1 : RequiredPage;
                var pageCount = Math.Ceiling(rowsCount / pageSize);
                if (RequiredPage > pageCount)
                {
                    RequiredPage = 1;
                }
                int skipcount = (RequiredPage - 1) * pageSize;
                var ruselt = await _context.Newhajz.Include(a => a.Typerev).Include(a => a.Itemrev).Include(a => a.Typemor)
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
          
            var numberOrder = await _context.Newhajz.OrderBy(n => n.Number).LastOrDefaultAsync();
          
            var addOne = 0;
            if (numberOrder is not null)
            {
                addOne = ++numberOrder.Number;
            }
            else
            {
                addOne = 1;
            }
            var searchitem = new List<int> { 1, 3 };
            searchitem.Add(4);
            var newhajzVM = new NewhajzFormVM
            {
                UserId = user.Id,
                Number = addOne,
                Typerev = await _context.Typerev.OrderBy(n => n.Name).ToListAsync(),
                Typemor = await _context.Typemor.OrderBy(n => n.Name).ToListAsync(),
                Itemrev = await _context.Itemrev.OrderBy(a => a.Name).ToListAsync(),
                

            };
            var d = await _context.Typerev.Where(a => searchitem.Contains(a.Id)).ToListAsync();
            return View("NewhajzForm", newhajzVM);

            //var addOne=0;
            //if (numberOrder is not null)
            //{
            //    addOne = ++numberOrder.Number;
            //}else
            //{
            //    addOne=1;
            //}
            //var newhajz = new Newhajz
            //{
            //    Number = addOne

            //};
            //Typerev = new SelectList(typerev, "Id", "Name");
            //return View("NewhajzForm", newhajz);
        }
        public JsonResult GetItemrev(int id)
        {
            //var item = _context.Newhajz.Where(a => a.Daydate == DateTime.Now && a.TyperevId == id).ToList();
            //var item = _context.Newhajz.ToList();
            //ViewBag.Typerev = _context.Itemrev.Where(b => b.TyperevId == id).ToList();
            //ViewBag.Typerev = _context.Itemrev.Where(b => b.TyperevId == id && !(item.Any(a=>a.Daydate == DateTime.Now)) ).ToList();
            ViewBag.Typerev = _context.Itemrev.Where(b => b.TyperevId == id).ToList();
            var s = ViewBag.Typerev;

            return Json(s);
        }
        // POST: Aistinafs/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewhajzFormVM newhajzVM)
        {
            if (_context.Newhajz.Any(a => a.Number == newhajzVM.Number))
            {
                ModelState.AddModelError("Number", "رقم  موجود مسبقا");
                return View("NewhajzForm", newhajzVM);
            }
            if (_context.Newhajz.Any(a => a.PhoneNumber == newhajzVM.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "رقم  موجود مسبقا");
                return View("NewhajzForm", newhajzVM);
            }
            DateTime selectedBookingDate = newhajzVM.Daydate;
            bool isDuplicateBooking = CheckDuplicateBooking(selectedBookingDate, newhajzVM.ItemrevId);

            if (isDuplicateBooking)
            {
                // إضافة خطأ تحقق النموذج يشير إلى أن العنصر "Itimerev" غير متاح في اليوم المحدد
                ModelState.AddModelError("ItemrevId", "العنصر  المحدد غير متاح في اليوم المحدد.");
                return View("NewhajzForm", newhajzVM);
            }
            newhajzVM.Hajzdate = DateTime.Now;


            var newhajz = _mapper.Map<Newhajz>(newhajzVM);
            

            if (ModelState.IsValid)
            {
                newhajzVM.Itemrev = await _context.Itemrev.Where(a => a.TyperevId == newhajzVM.TyperevId).ToListAsync();

                var user = await _userManager.GetUserAsync(User);
                newhajzVM.UserId = user.Id;
                _context.Add(newhajz);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("تم الاضافة بنجاح");
                return RedirectToAction(nameof(Newhajz));
            }


            return View("NewhajzForm", newhajzVM);
        }
        private bool CheckDuplicateBooking(DateTime bookingDate, int ItemrevId)
        {
            bool isDuplicate = _context.Newhajz.Any(b => b.Daydate.Date == bookingDate.Date && b.ItemrevId == ItemrevId);

            return isDuplicate;
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();


            var newhajz = await _context.Newhajz.FindAsync(id);
            if (newhajz == null)
                return NotFound();

            var newhajzform = _mapper.Map<NewhajzFormVM>(newhajz);
            newhajzform.Typerev = await _context.Typerev.ToListAsync();
            newhajzform.Itemrev = await _context.Itemrev.Where(a => a.TyperevId == newhajzform.TyperevId).ToListAsync();
            newhajzform.Typemor = await _context.Typemor.ToListAsync();

            return View("NewhajzForm", newhajzform);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NewhajzFormVM newhajz)
        {

            if (ModelState.IsValid)
            {
                if (_context.Newhajz.Any(a => a.Number == newhajz.Number && a.Id != newhajz.Id))
                {

                    ModelState.AddModelError("Number", "رقم  موجود مسبقا");
                    newhajz.Typerev = await _context.Typerev.ToListAsync();
                    newhajz.Itemrev = await _context.Itemrev.Where(a => a.TyperevId == newhajz.TyperevId).ToListAsync();
                    newhajz.Typemor = await _context.Typemor.ToListAsync();
                    return View("NewhajzForm", newhajz);


                }
                if (_context.Newhajz.Any(a => a.PhoneNumber == newhajz.PhoneNumber && a.Id != newhajz.Id))
                {

                    ModelState.AddModelError("PhoneNumber", "رقم  موجود مسبقا");
                    newhajz.Typerev = await _context.Typerev.ToListAsync();
                    newhajz.Itemrev = await _context.Itemrev.Where(a => a.TyperevId == newhajz.TyperevId).ToListAsync();
                    newhajz.Typemor = await _context.Typemor.ToListAsync();
                    return View("NewhajzForm", newhajz);



                }
                try
                {
                    _context.Update(_mapper.Map<Newhajz>(newhajz));

                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("تم التعديل بنجاح");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewhajzExists(newhajz.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Newhajz));
            }
            return View("NewhajzForm", newhajz);
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newhajz = await _context.Newhajz.FindAsync(id);

            if (newhajz == null)
            {
                return NotFound();
            }
            _context.Newhajz.Remove(newhajz);
            await _context.SaveChangesAsync();
            return Ok();

        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();
            var user = await _userManager.GetUserAsync(User);


            var newhajz = await _context.Newhajz
            //.Include(a => a.Name)
            //.Include(a => a.Address)
            //.Include(a => a.PhoneNumber)
            //    .Include(a => a.Daydate)
                .Include(a => a.Typerev)
                .Include(a => a.Itemrev)
                .Include(a => a.Typemor)
                 .Include(a => a.User)
            //.Include(a => a.Tall)
            //.Include(a => a.Com)
            //.Include(a => a.Ktf)
            //.Include(a => a.Qsr)
            //.Include(a => a.Notice)
            ////.Include(a => a.Price)
            ////.Include(a => a.PriceD)
            //.Include(a => a.PriceR)
            .FirstOrDefaultAsync(m => m.Id == id);

            if (newhajz == null)
            {
                return NotFound();
            }

            return View("NewhajzDetails", newhajz);

        }



        private bool NewhajzExists(int id)
        {
            return _context.Newhajz.Any(e => e.Id == id);
        }
    }


}

