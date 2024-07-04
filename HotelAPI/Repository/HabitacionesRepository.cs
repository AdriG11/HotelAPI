using ClasesModelos;
using HotelAPI.Data;
using HotelAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Repository
{
    public class HabitacionesRepository : Repository<Habitaciones> , IHabitacionesRepository
    {
        private readonly RoomContext _context;
        public HabitacionesRepository(RoomContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Habitaciones> UpdateAsync(Habitaciones entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
    }
    }
