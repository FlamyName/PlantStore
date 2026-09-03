using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.ValidationServices.IValidationServices
{
    public interface IImageValidationService
    {
        string? Validate(IFormFile file);
        bool IsAllowedType(string contentType);
        bool IsValidSignature(Stream stream);
    }
}
