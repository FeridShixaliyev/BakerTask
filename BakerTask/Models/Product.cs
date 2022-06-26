using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BakerTask.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public int Rating { get; set; }
        public bool IsDelete { get; set; }
        public List<ProductImage> Images { get; set; }
        [NotMapped]
        public List<int> ImagesId { get; set; }
        [NotMapped]
        public IFormFileCollection ImagesFile { get; set; }
        [NotMapped]
        public IFormFile MainImage { get; set; }

    }
}
