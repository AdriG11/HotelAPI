namespace ClasesModelos
{
    public class Habitaciones
    {
        public int RoomId {  get; set; }
        public string RoomType { get; set; }
        public string RoomStatus { get; set; }
        public double PricePerNight { get; set; }
        public ICollection<Reservas> Reservas { get; set; }
    }
}
