using BusinessLogic.Services.ImageServices.IImageServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.ImageServices
{
    public class ImageResolver : IImageResolver
    {
        public List<string> Resolve(List<string> tempImages, List<string> currentImages, int maxSlots = 5)
        {
            return Enumerable.Range(0, maxSlots)
                .Select(i => tempImages?.ElementAtOrDefault(i) ?? currentImages?.ElementAtOrDefault(i))
                .Where(url => !string.IsNullOrEmpty(url))
                .ToList();
        }
    }
}
