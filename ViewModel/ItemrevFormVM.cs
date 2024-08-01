using Hajz.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Hajz.ViewModel
{
    public class ItemrevFormVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "يرجى ادخال اسم الصنف "), StringLength(36)]
        [Display(Name = "الصنف")]
        public string Name { get; set; }
       
        [Display(Name = "رقم الصنف")]
        public int Number { get; set; }
        [Required(ErrorMessage = "يرجى اختيار الصنف")]
        [Display(Name = "نوع الصنف")]
        public int? TyperevId { get; set; }
        public List<Typerev> Typerev { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
