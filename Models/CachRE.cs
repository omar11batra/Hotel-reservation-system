using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hajz.Models
{

    //[Index(nameof(Number), IsUnique = true)]
    public class CachRE
    {
        [Key]
        public int Id { get; set; }

        //[Required(ErrorMessage = "يرجى ادخال اسم "), MaxLength(36)]
        //[Display(Name = "ا")]
        //public string Name { get; set; }


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
        [Display(Name = "المبلغ المتبقي")]


        public decimal PriceR { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "المبلغ")]

        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "المبلغ المدفوع")]

        public decimal PriceD { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

     

        public Newhajz Newhajz { get; set; }


        public int NewhajzId { get; set; }

    }
}
