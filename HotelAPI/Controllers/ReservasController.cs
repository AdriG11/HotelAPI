using AutoMapper;
using ClasesModelos;
using ClasesModelos.Dto;
using HotelAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : Controller
    {
        private readonly IHabitacionesRepository _habRepository;
        private readonly IReservaRepository _rep;
        private readonly ILogger<ReservasController> _logger;
        private readonly IMapper _mapper;

        public ReservasController(IHabitacionesRepository habRepository, IReservaRepository rep,
            ILogger<ReservasController>logger, IMapper mapper)
        {
            _habRepository = habRepository;
            _rep = rep;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ReservasDto>>> ObtenerReservas()
        {
            try
            {
                _logger.LogInformation("Obteniendo las reservas de habitaciones");

                var attendances = await _rep.GetAllAsync();

                return Ok(_mapper.Map<IEnumerable<ReservasDto>>(attendances));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener las reservas: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al obtener las reservas.");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ReservasDto>> ObtenerReserva(int id)
        {
            if (id <= 0)
            {
                _logger.LogError($"Numero de reserva no valido: {id}");
                return BadRequest("Numero de rerva no valido");
            }

            try
            {
                _logger.LogInformation($"Obteniendo reserva con el numero: {id}");

                var reservation = await _rep.GetByIdAsync(id);

                if (reservation == null)
                {
                    _logger.LogWarning($"No se encontró ninguna reserva de numero: {id}");
                    return NotFound("Reserva no fue hallada.");
                }

                return Ok(_mapper.Map<ReservasDto>(reservation));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener la reserva numero {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al obtener la reserva.");
            }
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ReservasDto>> PostRervation(ReservasCreateDto createDto)
        {
            if (createDto == null)
            {
                _logger.LogError("Se recibió una reserva nula en la solicitud.");
                return BadRequest("La reserva no puede ser nula.");
            }

            try
            {
                _logger.LogInformation($"Creando una nueva reserva de la habitacion: {createDto.RoomId} " +
                    $"en la fecha: {createDto.StarDate}");

                // Verificar si el estudiante existe
                var studentExists = await _habRepository.ExistsAsync(s => s.RoomId == createDto.RoomId);
                
                if (!studentExists)
                {
                    _logger.LogWarning($"La habitacion con el numero '{createDto.RoomId}' no existe.");
                    ModelState.AddModelError("Habitacion Existe", "¡La habitacion con existe!");
                    return BadRequest(ModelState);
                }

                // Verificar si la aistencia ya existe para la fecha y el estudiante
                var reservationexisted = await _rep
                    .GetAsync(a => a.RoomId == createDto.RoomId && a.StarDate == createDto.StarDate);

                if(reservationexisted != null)
                {
                    _logger.LogWarning($"La reserva para la habitacion '{createDto.RoomId}' " +
                        $"ya existe para la fecha '{createDto.StarDate}'");
                    ModelState.AddModelError("ReservaExiste", "¡La reserva para esa fecha ya existe!");
                    return BadRequest(ModelState);
                }

                // Verificar la validez del modelo
                if (!ModelState.IsValid)
                {
                    _logger.LogError("El modelo de Reserva no es válido.");
                    return BadRequest(ModelState);
                }

                // Crear la nueva asistencia
                var newReservation = _mapper.Map<Reservas>(createDto);

                await _rep.CreateAsync(newReservation);

                _logger.LogInformation($"Nueva asistencia creada con ID: " +
                    $"{newReservation.ReservationId}");
                return CreatedAtAction(nameof(ObtenerReserva), new { id = newReservation.ReservationId }, newReservation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear una nueva reserva: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al crear una nueva reserva.");
            }
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutAttendance(int id, ReservasUpdateDto updateDto)
        {
            if (updateDto == null || id != updateDto.ReservationId)
            {
                return BadRequest("Los datos de entrada no son válidos " +
                    "o el numero de reserva no coincide.");
            }

            try
            {
                _logger.LogInformation($"Actualizando asistencia con ID: {id}");

                var reservationexisted = await _rep.GetByIdAsync(id);
                if (reservationexisted == null)
                {
                    _logger.LogWarning($"No se encontró ninguna reserva con el numero: {id}");
                    return NotFound("La reserva no existe");
                }

                // Verificar si el estudiante existe
                var roomExisted = await _habRepository.ExistsAsync(s => s.RoomId == updateDto.RoomId);
                if (!roomExisted)
                {
                    _logger.LogWarning($"El estudiante con ID '{updateDto.RoomId}' no existe.");
                    ModelState.AddModelError("EstudianteNoExiste", "¡El estudiante no existe!");
                    return BadRequest(ModelState);
                }


                // Actualizar solo las propiedades necesarias de la asistenca existente
                _mapper.Map(updateDto, reservationexisted);

                await _rep.SaveChangesAsync();

                _logger.LogInformation($"Asistencia con ID {id} actualizada correctamente.");

                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await _rep.ExistsAsync(s => s.ReservationId == id))
                {
                    _logger.LogWarning($"No se encontró ninguna assitenca con ID: {id}");
                    return NotFound("La asistencia no se encontró durante la actualización.");
                }
                else
                {
                    _logger.LogError($"Error de concurrencia al actualizar la asistencia " +
                        $"con ID: {id}. Detalles: {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Error interno del servidor al actualizar la asistencia.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la asistencia con ID {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al actualizar la asistencia.");
            }
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            try
            {
                _logger.LogInformation($"Eliminando reserva N°: {id}");

                var reservation = await _rep.GetByIdAsync(id);
                if (reservation == null)
                {
                    _logger.LogWarning($"No se encontró ninguna asistencia con ID: {id}");
                    return NotFound("Asistencia no encontrada.");
                }

                await _rep.DeleteAsync(reservation);

                _logger.LogInformation($"Asistencia con ID {id} eliminada correctamente.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la asistencia con ID {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Se produjo un error al eliminar la asistencia.");
            }
        }

    }
}
