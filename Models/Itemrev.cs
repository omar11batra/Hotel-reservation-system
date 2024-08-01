using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hajz.Models
{
    public class Itemrev
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "يرجى ادخال اسم الصنف "), MaxLength(36)]
        [Display(Name = "الصنف")]
        public string Name { get; set; }


        [Display(Name = "رقم الصنف")]
        public int Number { get; set; }
        [Required(ErrorMessage = "يرجى اختيار الصنف")]
        [ForeignKey("TyperevId")]
        public int? TyperevId { get; set; }
        [Display(Name = "نوع الحجز")]
        public Typerev Typerev { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
