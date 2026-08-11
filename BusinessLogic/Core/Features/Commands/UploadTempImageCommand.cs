using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Core.Features.Commands
{
    public class UploadTempImageCommand : IRequest<UploadTempImageResult>
    {
        public IFormFile File { get; set; }
    }

    public class UploadTempImageResult
    {
        public bool Success { get; set; }
        public string Url { get; set; }
        public string ErrorMessage { get; set; }
    }
}
