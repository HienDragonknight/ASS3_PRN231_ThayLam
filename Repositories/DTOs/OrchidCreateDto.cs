using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.DTOs
{
    public class OrchidCreateDto
    {
        [Required]
        public bool IsNatural { get; set; }

        [Required]
        public string OrchidDescription { get; set; }

        [Required]
        public string OrchidName { get; set; }

        [Required]
        public string OrchidUrl { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }

}
