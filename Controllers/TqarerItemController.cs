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
    public class TqarerItemController : Controller
    {
        private readonly HajzDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IToastNotification _toastNotification;

        public TqarerItemController(HajzDbContext context, IMapper mapper, UserManager<User> userManager, IToastNotification toastNotification)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> TqarerItem(string term, DateTime? fromDate, DateTime? toDate, int RequiredPage = 1, int rowNum = 10)
        {
            ViewBag.Term = term;
            ViewBag.num = rowNum;
            int pageSize = ViewBag.num;

            IQueryable<Newhajz> query = _context.Newhajz.Include(a => a.User).Include(a => a.Typerev).Include(a => a.Typemor).Include(a => a.Itemrev);

            if (!string.IsNullOrEmpty(term))
            {
                query = query.Where(a => a.Name.Contains(term) || a.Itemrev.Name.Contains(term));
            }

            if (fromDate != null && toDate != null)
            {
                query = query.Where(a => a.Hajzdate >= fromDate && a.Hajzdate <= toDate);
            }

            decimal rowsCount = await query.CountAsync();
            RequiredPage = RequiredPage <= 0 ? 1 : RequiredPage;
            var pageCount = Math.Ceiling(rowsCount / pageSize);
            if (RequiredPage > pageCount)
            {
                RequiredPage = 1;
            }
            int skipcount = (RequiredPage - 1) * pageSize;

            var searchResult = await query
                .Skip(skipcount)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.pagesCount = pageCount;
            ViewBag.CurrentPage = RequiredPage;

            return View(searchResult);
        }
    }
}



