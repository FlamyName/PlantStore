using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Core.Features.Commands
{
    public class UpdateProductFilesCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }

        // Существующие URL (из базы)
        public string CurrentImage1 { get; set; }
        public string CurrentImage2 { get; set; }
        public string CurrentImage3 { get; set; }
        public string CurrentImage4 { get; set; }
        public string CurrentImage5 { get; set; }

        // Временные URL (после загрузки новых файлов)
        public string TempImage1 { get; set; }
        public string TempImage2 { get; set; }
        public string TempImage3 { get; set; }
        public string TempImage4 { get; set; }
        public string TempImage5 { get; set; }
    }
}
