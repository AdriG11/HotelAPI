using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClasesModelos
{
    public class Reservas
    {
        public int ReservationId { get; set; }
        public int RoomId { get; set; }
        public DateOnly StarDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string ReservatioStatus { get; set; }
        public Habitaciones? Habitaciones { get; set; }
    }
}
