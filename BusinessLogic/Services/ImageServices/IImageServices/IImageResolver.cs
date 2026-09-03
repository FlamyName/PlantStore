using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.ImageServices.IImageServices
{
    public interface IImageResolver
    {
        List<string> Resolve(List<string> tempImages, List<string> currentImages, int maxSlots = 5);
    }
}
