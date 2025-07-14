using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Categories
{
    public class CatInputDTO
    {
        [Required, MaxLength(50)]
        public string Name { get; set; } = default!;
    }
}
