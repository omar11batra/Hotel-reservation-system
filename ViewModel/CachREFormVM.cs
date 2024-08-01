using Hajz.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hajz.ViewModel
{
    public class CachREFormVM
    {
        public int Id { get; set; }


        //[Required(ErrorMessage = "يرجى ادخال اسم "), MaxLength(36)]
        //[Display(Name = "ا")]
        //public string Name { get; set; }


        [Display(Name = "رقم السند ")]
        public int Number { get; set; }

        //[Required(ErrorMessage = "يرجى كتابة تاريخ السند ")]
        [DataType(DataType.Date)]
        [Display(Name = "تاريخ السند")]
        public DateTime BondDate { get; set; } = DateTime.Now;


        [Required(ErrorMessage = "يرجى كتابة البيان ")]
        [Display(Name = "البيان")]
        [MaxLength(3000)]
        public string Notice { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "المبلغ ")]


        public decimal Price { get; set; }




        //[Column(TypeName = "decimal(18,2)")]
        [Display(Name = "المبلغ المدفوع")]


        public decimal PriceD { get; set; }


        //[Column(TypeName = "decimal(18,2)")]
        //[Display(Name = "المبلغ")]

        //public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "المبلغ المتبقي")]

        public decimal PriceR { get; set; }


        public string UserId { get; set; }
        public User User { get; set; }

        public List <Newhajz> Newhajz { get; set; }

        public int NewhajzId { get; set; }

        //[Required(ErrorMessage = "يرجى ادخال رقم الهاتف"), MaxLength(13)]
        //[Display(Name = "رقم الهاتف")]
        public string PhoneNumber { get; set; }
        public string Name { get; set; }




    }
}
