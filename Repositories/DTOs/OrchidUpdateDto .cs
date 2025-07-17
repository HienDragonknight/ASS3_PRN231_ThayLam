using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.DTOs
{
    public class OrchidUpdateDto
    {
        public bool? IsNatural { get; set; }
        public string? OrchidDescription { get; set; }
        public string? OrchidName { get; set; }
        public string? OrchidUrl { get; set; }
        public decimal? Price { get; set; }
        public int? CategoryId { get; set; }
    }

}
