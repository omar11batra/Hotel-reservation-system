using Hajz.Models;
using Hajz.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hajz.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IToastNotification _toastNotification;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager
            , RoleManager<IdentityRole> roleManager, IToastNotification toastNotification)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _toastNotification = toastNotification;
        }



        [Authorize(Roles = "مدير_النظام")]
        public async Task<IActionResult> Users(string term, int RequiredPage = 1, int rowNum = 10)
        {
            ViewBag.Term = term;
            ViewBag.num = rowNum;
            int pageSize = ViewBag.num;

            if (term != null)
            {
                decimal rowsCount = await _userManager.Users.Where(a => a.UserName.Contains(term)).CountAsync();
                RequiredPage = RequiredPage <= 0 ? 1 : RequiredPage;
                var pageCount = Math.Ceiling(rowsCount / pageSize);
                if (RequiredPage > pageCount)
                {
                    RequiredPage = 1;
                }
                int skipcount = (RequiredPage - 1) * pageSize;
                var searchruselt = await _userManager.Users.Select(user => new UsersFormVM
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    IsActive = user.IsActive,
                    NameRoles = _userManager.GetRolesAsync(user).Result
                })
                    .Where(a => a.UserName.Contains(term))
                   .Skip(skipcount)
                   .Take(pageSize)
                    .ToListAsync();

                ViewBag.pagesCount = pageCount;
                ViewBag.CurrentPage = RequiredPage;
                return View(searchruselt);
            }
            else
            {
                decimal rowsCount = await _userManager.Users.CountAsync();
                RequiredPage = RequiredPage <= 0 ? 1 : RequiredPage;
                var pageCount = Math.Ceiling(rowsCount / pageSize);
                if (RequiredPage > pageCount)
                {
                    RequiredPage = 1;
                }
                int skipcount = (RequiredPage - 1) * pageSize;
                var users = await _userManager.Users.Select(user => new UsersFormVM
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    IsActive = user.IsActive,
                    NameRoles = _userManager.GetRolesAsync(user).Result
                }).Skip(skipcount)
                  .Take(pageSize).ToListAsync();
                ViewBag.pagesCount = pageCount;
                ViewBag.CurrentPage = RequiredPage;
                return View(users);
            }
        }

        [Authorize(Roles = "مدير_النظام")]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.Select(r => new RoleViewModel
            { RoleId = r.Id, RoleName = r.Name }).ToListAsync();
            var viewModel = new UsersFormVM
            {
                Roles = roles,
            };

            return View("UsersForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "مدير_النظام")]
        public async Task<IActionResult> Create(UsersFormVM model)
        {
            if (!model.Roles.Any(s => s.IsSelected))
            {
                ModelState.AddModelError("Roles", "يرجى اختيار صلاحية واحدة على الاقل");
                return View("UsersForm", model);
            }

            if (!ModelState.IsValid)
            {
                return View("UsersForm", model);
            }

            if (await _userManager.FindByNameAsync(model.UserName) != null)
            {
                ModelState.AddModelError("UserName", "اسم المستخدم موجود مسبقا");
                return View("UsersForm", model);
            }
            var user = new User
            {
                UserName = model.UserName,
                IsActive = model.IsActive,
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Roles", error.Description);
                    return View("UsersForm", model);
                }
            }
            await _userManager.AddToRolesAsync(user, model.Roles.Where(s => s.IsSelected).Select(s => s.RoleName));
            _toastNotification.AddSuccessToastMessage("تم الاضافة بنجاح");
            return RedirectToAction(nameof(Users));
        }
        [Authorize(Roles = "مدير_النظام")]
        public async Task<IActionResult> Edit(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            var roles = await _roleManager.Roles.ToListAsync();

            var viewModel = new UsersFormVM
            {
                Id = user.Id,
                UserName = user.UserName,
                IsActive = user.IsActive,
                Roles = roles.Select(role => new RoleViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    IsSelected = _userManager.IsInRoleAsync(user, role.Name).Result
                }).ToList()
            };

            return View("UsersForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "مدير_النظام")]
        public async Task<IActionResult> Edit(UsersFormVM model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
          
            if (user == null)
                return NotFound();


            var userWithSameUserName = await _userManager.FindByNameAsync(model.UserName);
            if (userWithSameUserName != null && userWithSameUserName.Id != model.Id)
            {
                ModelState.AddModelError("UserName", "اسم المستخدم موجود مسبقا");
                return View(model);
            }
            user.UserName = model.UserName;
            user.IsActive = model.IsActive;

            if(model.Password != null && model.CurrentPassword !=null)
            {
                await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.Password);
            }
           
           
            await _userManager.UpdateAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in model.Roles)
            {
                if (userRoles.Any(r => r == role.RoleName) && !role.IsSelected)
                    await _userManager.RemoveFromRoleAsync(user, role.RoleName);

                if (!userRoles.Any(r => r == role.RoleName) && role.IsSelected)
                    await _userManager.AddToRoleAsync(user, role.RoleName);
            }
            _toastNotification.AddSuccessToastMessage("تم التعديل بنجاح");
            return RedirectToAction(nameof(Users));
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginFormVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);
                    if (!user.IsActive)
                    {
                        ModelState.AddModelError(string.Empty, "المستخدم غير مفعل");
                        return View(model);
                    }
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "يرجى التأكد من الاسم وكلمة المرور");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
