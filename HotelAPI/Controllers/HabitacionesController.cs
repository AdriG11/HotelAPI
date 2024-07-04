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
    public class HabitacionesController : Controller
    {
        private readonly IHabitacionesRepository _rep;
        private readonly ILogger<HabitacionesController> _logger;
        private readonly IMapper _mapper;
        public HabitacionesController(IHabitacionesRepository rep, ILogger<HabitacionesController> logger, IMapper mapper)
        {
            _rep = rep;
            _logger = logger;
            _mapper = mapper;
            
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<HabitacionesDto>>> TenerHabitaciones()
        {
            try
            {
                _logger.LogInformation("Obteniendo los estudiantes");

                var rooms = await _rep.GetAllAsync();

                return Ok(_mapper.Map<IEnumerable<HabitacionesDto>>(rooms));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener todas las habitaciones: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al obtener las habitaciones.");
            }
        }
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HabitacionesDto>> ObtenerHabitacion(int id)
        {
            if (id <= 0)
            {
                _logger.LogError($"Numero de la habitacion no válida: {id}");
                return BadRequest("Numero de la habitacion no válida");
            }

            try
            {
                _logger.LogInformation($"Obteniendo la habitacion con el numero: {id}");

                var rooms = await _rep.GetByIdAsync(id);

                if (rooms == null)
                {
                    _logger.LogWarning($"No se encontró ninguna habitacion con el numero {id}");
                    return NotFound("Habitacion no encontrada.");
                }

                return Ok(_mapper.Map<HabitacionesDto>(rooms));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener la habitacion con el numero {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al obtener la habitacion.");
            }
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HabitacionesDto>> PostHabitacion(HabitacionesCreateDto createDto)
        {
            if (createDto == null)
            {
                _logger.LogError("Se recibió una habitacion nula en la solicitud.");
                return BadRequest("La habitacion no puede ser nula.");
            }

            try
            {
                _logger.LogInformation($"Creando nueva habitacion de tipo: {createDto.RoomType}");

                // Crear el nuevo estudiante
                var newRoom = _mapper.Map<Habitaciones>(createDto);

                await _rep.CreateAsync(newRoom);
                await _rep.SaveChangesAsync();

                _logger.LogInformation($"Nuevo estudiante '{createDto.RoomType}' creado con ID: " +
                    $"{newRoom.RoomId}");
                return CreatedAtAction(nameof(ObtenerHabitacion), new { id = newRoom.RoomId }, newRoom);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al registrar una nueva habitacion: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al registrar una nueva .");
            }
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutStudent(int id, HabitacionesUpdateDto updateDto)
        {
            if (updateDto == null || id != updateDto.RoomId)
            {
                return BadRequest("Los datos de entrada no son válidos " +
                    "o el numero de habitacion no coincide.");
            }

            try
            {
                _logger.LogInformation($"Actualizando numero de habitacion: {id}");

                var roomexisted = await _rep.GetByIdAsync(id);
                if (roomexisted == null)
                {
                    _logger.LogWarning($"No se encontró ninguna habitacion con el numero: {id}");
                    return NotFound("La habitacion no existe");
                }

                // Actualizar solo las propiedades necesarias del estudiante existente
                _mapper.Map(updateDto, roomexisted);

                await _rep.SaveChangesAsync();

                _logger.LogInformation($"Habitacion de numero {id} actualizada correctamente.");

                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await _rep.ExistsAsync(s => s.RoomId == id))
                {
                    _logger.LogWarning($"No se encontró ninguna habitacion con el numero: {id}");
                    return NotFound("No se encontro a la habitacion durante la actualización.");
                }
                else
                {
                    _logger.LogError($"Error de concurrencia al actualizar la habitacion " +
                        $"con el numero: {id}. Detalles: {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Error interno del servidor al actualizar la habitacion.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la habiacion con referente de numero {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al actualizar la habitacion.");
            }
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                _logger.LogInformation($"Eliminando numero de habitacion: {id}");

                var room = await _rep.GetByIdAsync(id);
                if (room == null)
                {
                    _logger.LogWarning($"No se encontró ninguna habitacion de numero: {id}");
                    return NotFound("Habitacion no encontrada.");
                }

                await _rep.DeleteAsync(room);

                _logger.LogInformation($"Habitacion con el numero {id} eliminada correctamente.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar numero de habitacion {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Se produjo un error al eliminar la habitacion.");
            }
        }

    }
}
