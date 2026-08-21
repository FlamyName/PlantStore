using PlantStore.DB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DB.Models
{
    public class Units
    {
        [Key]
        public int Id { get; set; }
        public string NameUnit { get; set; }
        public List<Products> Products { get; set; }
    }
}
