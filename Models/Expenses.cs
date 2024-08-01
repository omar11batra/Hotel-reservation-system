using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Hajz.Models
{
    public class Expenses
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "يرجى ادخال اسم المستفيد "), MaxLength(36)]
        [Display(Name = "الاسم")]
        public string Name { get; set; }


        [Display(Name = "رقم السند ")]
        public int Number { get; set; }

        [Required(ErrorMessage = "يرجى كتابة تاريخ السند ")]
        [DataType(DataType.Date)]
        [Display(Name = "تاريخ السند")]
        public DateTime BondDate { get; set; } = DateTime.Now;


        [Required(ErrorMessage = "يرجى كتابة البيان ")]
        [Display(Name = "البيان")]
        [MaxLength(3000)]
        public string Notice { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "المبلغ")]

        public decimal Price { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }


    }
}
