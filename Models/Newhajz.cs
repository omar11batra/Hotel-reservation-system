using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using System.Linq;



namespace Hajz.Models
{
    [Index(nameof(Number), IsUnique = true)]
    [Index(nameof(PhoneNumber), IsUnique = true)]

    public class Newhajz
    {
        [Key]
        [Display(Name = " الرقم ")]

        public int Id { get; set; }

        public DateTime Date { get; set; }

        [Display(Name = "رقم الحجز")]
        public int Number { get; set; }

        [Required(ErrorMessage = "يرجى كتابة اسم المريض "), MaxLength(100)]
        [Display(Name = "الاسم")]
        public string Name { get; set; }





        [Required(ErrorMessage = "يرجى كتابة  التاريخ ")]
        [Display(Name = "تاريخ الحجز ")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Hajzdate { get; set; }

        [Required(ErrorMessage = "ة التاريخ ")]
        [Display(Name = "موعد الحضور  ")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Daydate { get; set; }

        [Required(ErrorMessage = "يرجى كتابة العنوان "), MaxLength(40)]
        [Display(Name = "العنوان")]
        public string Address { get; set; }

        [Required(ErrorMessage = "يرجى كتابة البيان ")]
        [Display(Name = "البيان")]
        [MaxLength(3000)]
        public string Notice { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "المبلغ")]

        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "المبلغ المدفوع")]

        public decimal PriceD { get; set; }


        [Required(ErrorMessage = "يرجى ادخال رقم الهاتف"), MaxLength(13)]
        [Display(Name = "رقم الهاتف")]
        public string PhoneNumber { get; set; }
        [MaxLength(13)]
        [Display(Name = "2 رقم الهاتف")]
        public string PhoneNumber2 { get; set; }

        [MaxLength(13)]
        [Display(Name = "3 رقم الهاتف")]
        public string PhoneNumber3 { get; set; }

        [Display(Name = "نوع الحجز ")]
        public string Typehajz { get; set; }

        [Display(Name = " الفتره ")]
        public string Periodhajz { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "المبلغ المتبقي")]

        public decimal PriceR { get; set; }
        [Display(Name = "نوع الحجز ")]
        [ForeignKey("TyperevId")]
        public int TyperevId { get; set; }
        [Display(Name = "نوع الحجز ")]
        public Typerev Typerev { get; set; }
        [Display(Name = "نوع الجهاز ")]
        [ForeignKey("TypemorId")]
        public int TypemorId { get; set; }
        [Display(Name = "نوع الجهاز ")]
        public Typemor Typemor { get; set; }

        [Display(Name = "الصنف ")]
        public int ItemrevId { get; set; }
        [Display(Name = "نوع الصنف ")]
        public Itemrev Itemrev { get; set; }
        public List<CachRE> CachRE { get; set; }
        
        public string UserId { get; set; }
        public User User { get; set; }


    }
}
