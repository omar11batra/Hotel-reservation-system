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
    public class TqarerPriceController : Controller
    {
        private readonly HajzDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IToastNotification _toastNotification;

        public TqarerPriceController(HajzDbContext context, IMapper mapper, UserManager<User> userManager, IToastNotification toastNotification)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> TqarerPrice(string term, int RequiredPage = 1, int rowNum = 10)
        {

            ViewBag.Term = term;
            ViewBag.num = rowNum;
            int pageSize = ViewBag.num;

            if (term != null)
            {
                decimal rowsCount = await _context.CachRE.Include(a => a.User).Include(a => a.Newhajz).Where(a => a.Newhajz.Name.Contains(term) || a.User.UserName.ToString().Contains(term)).CountAsync();
                RequiredPage = RequiredPage <= 0 ? 1 : RequiredPage;
                var pageCount = Math.Ceiling(rowsCount / pageSize);
                if (RequiredPage > pageCount)
                {
                    RequiredPage = 1;
                }
                int skipcount = (RequiredPage - 1) * pageSize;
                var searchruselt = await _context.CachRE.Include(a => a.User).Include(a => a.Newhajz).Where(a => a.Newhajz.Name.Contains(term) || a.User.UserName.ToString().Contains(term))
                    .Skip(skipcount)
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.pagesCount = pageCount;
                ViewBag.CurrentPage = RequiredPage;
                return View(searchruselt);
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
                var ruselt = await _context.CachRE.Include(a => a.User).Include(a => a.Newhajz)
                       .Skip(skipcount)
                       .Take(pageSize)
                       .ToListAsync();

                ViewBag.pagesCount = pageCount;
                ViewBag.CurrentPage = RequiredPage;
                return View(ruselt);
            }
        }
    }
}
