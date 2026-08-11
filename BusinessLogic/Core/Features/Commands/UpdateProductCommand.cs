using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Core.Features.Commands
{
    public class UpdateProductCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }

        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }
        public string Image5 { get; set; }

        //Вспомогательный список для хранения слотов с картинками
        public List<(int Position, string Url)> ImageUrls 
        {
            get
            {
                var result = new List<(int, string)>();
                var images = new[] { Image1, Image2, Image3, Image4, Image5 };

                for (int i = 0; i < images.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(images[i]))
                        result.Add((i + 1, images[i]));
                }
                return result;
            }
        }
    }
}
