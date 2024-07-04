using ClasesModelos;
using HotelAPI.Data;
using HotelAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Repository
{
    public class ReservaRepository : Repository<Reservas>, IReservaRepository
    {
        private readonly RoomContext _context;
        public ReservaRepository(RoomContext context) : base(context) 
        {
            _context = context;
        }
        public async Task<Reservas> UpdateAsAsync(Reservas entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
