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
    public class ExpensesController : Controller
    {
        private readonly HajzDbContext _context;
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;
        private readonly UserManager<User> _userManager;

        public ExpensesController(HajzDbContext context, IMapper mapper, IToastNotification toastNotification, UserManager<User> userManager)
        {
            _context = context;
            _mapper = mapper;
            _toastNotification = toastNotification;
            _userManager = userManager;
        }
        // Assuming you have a list of reservations and receipt vouchers
       
        public async Task<IActionResult> Expenses(string term, int RequiredPage = 1, int rowNum = 10)
        {

            ViewBag.Term = term;
            ViewBag.num = rowNum;
            int pageSize = ViewBag.num;

            if (term != null)
            {
                decimal rowsCount = await _context.Expenses
                    .Where(a => a.Notice.Contains(term) || a.Number.ToString().Contains(term)
                    ).CountAsync();
                RequiredPage = RequiredPage <= 0 ? 1 : RequiredPage;
                var pageCount = Math.Ceiling(rowsCount / pageSize);
                if (RequiredPage > pageCount)
                {
                    RequiredPage = 1;
                }
                int skipcount = (RequiredPage - 1) * pageSize;
                var searchruselt = await _context.Expenses.Where(a => a.Notice.Contains(term) || a.Number.ToString().Contains(term))
                    .Skip(skipcount)
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.pagesCount = pageCount;
                ViewBag.CurrentPage = RequiredPage;
                return View("Expenses", searchruselt);
            }
            else
            {
                decimal rowsCount = await _context.Expenses.CountAsync();
                RequiredPage = RequiredPage <= 0 ? 1 : RequiredPage;
                var pageCount = Math.Ceiling(rowsCount / pageSize);
                if (RequiredPage > pageCount)
                {
                    RequiredPage = 1;
                }
                int skipcount = (RequiredPage - 1) * pageSize;
                var ruselt = await _context.Expenses
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
            var expenses = new Expenses();
            return View("ExpensesForm", expenses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expenses expenses)
        {
            

            if (ModelState.IsValid)
            {
                _context.Add(expenses);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("تم الاضافة بنجاح");
                return RedirectToAction(nameof(Expenses));
            }
            return View("ExpensesForm", expenses);
        }

        // GET: Aistinafs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenses = await _context.Expenses.FindAsync(id);
            if (expenses == null)
            {
                return NotFound();
            }
            return View("ExpensesForm", expenses);
        }

        // POST: Aistinafs/Edit/5




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Expenses expenses)
        {

            if (ModelState.IsValid)
            {
                
                try
                {
                    _context.Update(expenses);
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("تم التعديل بنجاح");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpensesExists(expenses.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Expenses));
            }
            return View("ExpensesForm", expenses);
        }


        // GET: Aistinafs/Delete
        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenses = await _context.Expenses.FindAsync(id);

            if (expenses == null)
            {
                return NotFound();
            }
            _context.Expenses.Remove(expenses);
            await _context.SaveChangesAsync();
            return Ok();

        }



        private bool ExpensesExists(int id)
        {
            return _context.Expenses.Any(e => e.Id == id);
        }

    }
}
