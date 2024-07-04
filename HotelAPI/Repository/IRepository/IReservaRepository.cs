using ClasesModelos;

namespace HotelAPI.Repository.IRepository
{
    public interface IReservaRepository : IRepository<Reservas>
    {
        Task<Reservas> UpdateAsAsync(Reservas entity);
    }
}
