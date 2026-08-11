using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.ViewModels
{
    public class UpdateProductWithFilesRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int CategoryId { get; set; }

        public string CurrentImage1 { get; set; }
        public string CurrentImage2 { get; set; }
        public string CurrentImage3 { get; set; }
        public string CurrentImage4 { get; set; }
        public string CurrentImage5 { get; set; }

        public string TempImage1 { get; set; }
        public string TempImage2 { get; set; }
        public string TempImage3 { get; set; }
        public string TempImage4 { get; set; }
        public string TempImage5 { get; set; }

        public IFormFile Image1 { get; set; }
        public IFormFile Image2 { get; set; }
        public IFormFile Image3 { get; set; }
        public IFormFile Image4 { get; set; }
        public IFormFile Image5 { get; set; }

    }
}
