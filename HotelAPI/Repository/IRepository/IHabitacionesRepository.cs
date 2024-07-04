using ClasesModelos;

namespace HotelAPI.Repository.IRepository
{
    public interface IHabitacionesRepository : IRepository<Habitaciones>
    {
        Task<Habitaciones> UpdateAsync(Habitaciones entity);
    }
}
