using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hajz.ViewModel
{
    public class UsersFormVM
    {
      
        public string Id { get; set; }

        [Required(ErrorMessage = "يرجى ادخال اسم المستخدم")]
        [Display(Name = "اسم المستخدم")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "يرجى ادخال كلمة المرور")]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "كلمة المرور ليست مطابقة")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "حالة المستخدم")]
        public bool IsActive { get; set; }
        public string CurrentPassword { get; set; }
        public IEnumerable<string> NameRoles { get; set; }
        [Display(Name = "الصلاحيات")]
        public List<RoleViewModel> Roles { get; set; }
    }
}
