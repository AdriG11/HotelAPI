using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClasesModelos.Dto
{
    public class ReservasUpdateDto
    {
        [Required]
        public int ReservationId { get; set; }
        [Required]
        public int RoomId { get; set; }
        [Required]
        public DateOnly StarDate { get; set; }
        [Required]
        public DateOnly EndDate { get; set; }
        [Required]
        public string ReservatioStatus { get; set; }
    }
}
