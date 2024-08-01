using System.ComponentModel.DataAnnotations;

namespace Hajz.ViewModel
{
    public class LoginFormVM
    {
        [Required(ErrorMessage = "يرجى ادخال اسم المستخدم")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "يرجى ادخال كلمة المرور")]
        public string Password { get; set; }

        public bool IsActive { get; set; }
    }
}
