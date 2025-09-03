using GestionProyectosONGBackEnd.Data;
using GestionProyectosONGBackEnd.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionProyectosONGBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonacionesController : ControllerBase
    {
        private readonly DonacionesData _donacionesData;

        public DonacionesController(DonacionesData donacionesData)
        {
            _donacionesData = donacionesData;
        }

        // Listar todas las donaciones activas (solo si el rubro está activo)
        [HttpGet]
        [Route("listar")]
        public async Task<ActionResult<DonacionesListResponse>> ListarDonaciones(bool active = true)
        {
            var donaciones = await _donacionesData.ListarDonaciones(active);

            if (donaciones == null || !donaciones.Any())
            {
                return NotFound(new DonacionesListResponse
                {
                    Success = 0,
                    Message = "No se encontraron donaciones."
                });
            }

            return Ok(new DonacionesListResponse
            {
                Success = 1,
                Message = "Donaciones obtenidas correctamente.",
                Data = donaciones
            });
        }

        // Listar donaciones por rubro (solo si el rubro está activo)
        [HttpGet]
        [Route("listar/{idRubro}")]
        public async Task<ActionResult<DonacionesListResponse>> ListarDonacionesPorRubro(int idRubro, bool active = true)
        {
            var donaciones = await _donacionesData.ListarDonacionesPorRubro(idRubro, active);

            if (donaciones == null || !donaciones.Any())
            {
                return NotFound(new DonacionesListResponse
                {
                    Success = 0,
                    Message = "No se encontraron donaciones para este rubro."
                });
            }

            return Ok(new DonacionesListResponse
            {
                Success = 1,
                Message = "Donaciones por rubro obtenidas correctamente.",
                Data = donaciones
            });
        }

        // Listar una donación por ID (solo si el rubro está activo)
        [HttpGet]
        [Route("listar/donacion/{id}")]
        public async Task<ActionResult<DonacionResponse>> ListarDonacionPorId(int id, bool active = true)
        {
            var donacion = await _donacionesData.ListarDonacionPorId(id, active);

            if (donacion == null)
            {
                return NotFound(new DonacionResponse
                {
                    Success = 0,
                    Message = "Donación no encontrada."
                });
            }

            return Ok(new DonacionResponse
            {
                Success = 1,
                Message = "Donación encontrada.",
                Data = donacion
            });
        }

        // Crear una nueva donación
        [HttpPost]
        [Route("crear")]
        public async Task<ActionResult<DonacionResponse>> CrearDonacion(DonacionesModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new DonacionResponse
                {
                    Success = 0,
                    Message = "Datos inválidos."
                });
            }

            var donacionCreada = await _donacionesData.CrearDonacion(model);

            if (donacionCreada.Success == 0)
            {
                return BadRequest(new DonacionResponse
                {
                    Success = 0,
                    Message = donacionCreada.Message
                });
            }

            return CreatedAtAction(nameof(ListarDonacionPorId), new { id = donacionCreada.Id }, new DonacionResponse
            {
                Success = 1,
                Message = "Donación creada correctamente.",
                Data = donacionCreada
            });
        }

        // Actualizar una donación
        [HttpPut]
        [Route("actualizar/{id}")]
        public async Task<ActionResult<DonacionResponse>> ActualizarDonacion(int id, DonacionesModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new DonacionResponse
                {
                    Success = 0,
                    Message = "Datos inválidos."
                });
            }

            var donacionActualizada = await _donacionesData.ActualizarDonacion(model);

            if (donacionActualizada.Success == 0)
            {
                return BadRequest(new DonacionResponse
                {
                    Success = 0,
                    Message = donacionActualizada.Message
                });
            }

            return Ok(new DonacionResponse
            {
                Success = 1,
                Message = "Donación actualizada correctamente.",
                Data = donacionActualizada
            });
        }

        // Eliminar una donación (borrado lógico)
        [HttpDelete]
        [Route("eliminar/{id}")]
        public async Task<ActionResult<DonacionResponse>> EliminarDonacion(int id)
        {
            var donacionEliminada = await _donacionesData.EliminarDonacion(id);

            if (donacionEliminada.Success == 0)
            {
                return NotFound(new DonacionResponse
                {
                    Success = 0,
                    Message = donacionEliminada.Message
                });
            }

            return Ok(new DonacionResponse
            {
                Success = 1,
                Message = "Donación eliminada correctamente."
            });
        }

        // Obtener el total de donaciones por rubro
        [HttpGet]
        [Route("total/{idRubro}")]
        public async Task<ActionResult<TotalDonacionesResponse>> ObtenerTotalDonacionesPorRubro(int idRubro)
        {
            var totalDonaciones = await _donacionesData.ObtenerTotalDonacionesPorRubro(idRubro);

            if (totalDonaciones.Success == 0)
            {
                return NotFound(totalDonaciones);
            }

            return Ok(totalDonaciones);
        }

        // Obtener reporte de donaciones por proyecto
        [HttpGet]
        [Route("reporte/{idProyecto}")]
        public async Task<ActionResult<ReporteDonacionesResponse>> ObtenerReporteDonacionesPorProyecto(int idProyecto)
        {
            var reporte = await _donacionesData.ObtenerReporteDonacionesPorProyecto(idProyecto);

            if (reporte == null || !reporte.Any())
            {
                return NotFound(new ReporteDonacionesResponse
                {
                    Success = 0,
                    Message = "No se encontraron donaciones para el proyecto."
                });
            }

            return Ok(new ReporteDonacionesResponse
            {
                Success = 1,
                Message = "Reporte de donaciones por proyecto obtenido correctamente.",
                Data = reporte
            });
        }

        // Obtener top donantes
        [HttpGet]
        [Route("topdonantes")]
        public async Task<ActionResult<DonacionesListResponse>> ObtenerTopDonantes(int top = 10)
        {
            var topDonantes = await _donacionesData.ObtenerTopDonantes(top);

            if (topDonantes == null || !topDonantes.Any())
            {
                return NotFound(new DonacionesListResponse
                {
                    Success = 0,
                    Message = "No se encontraron donantes."
                });
            }

            return Ok(new DonacionesListResponse
            {
                Success = 1,
                Message = "Top donantes obtenidos correctamente.",
                Data = topDonantes
            });
        }
    }
}