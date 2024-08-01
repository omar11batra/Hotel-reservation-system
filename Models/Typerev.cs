using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Hajz;

namespace Hajz.Models
{
    public class Typerev
    {   
        [Key]
       
        public int Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }

        public List<Itemrev> Itemrev { get; set; }
        public List<Newhajz> Newhajz { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }


    }
}
