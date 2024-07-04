using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClasesModelos.Dto
{
    public class HabitacionesCreateDto
    {
        [StringLength(40)]
        public string RoomType { get; set; }
        [Required]
        public string RoomStatus { get; set; }
        [Required]
        public double PricePerNight { get; set; }
    }
}
