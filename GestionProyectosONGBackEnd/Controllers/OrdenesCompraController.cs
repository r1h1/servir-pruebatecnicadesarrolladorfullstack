using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GestionProyectosONGBackEnd.Data;
using GestionProyectosONGBackEnd.Models;

namespace GestionProyectosONGBackEnd.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class OrdenesCompraController : ControllerBase
    {
        private readonly OrdenesCompraData _data;

        public OrdenesCompraController(OrdenesCompraData data)
        {
            _data = data;
        }

        // GET: v1/OrdenesCompra
        [HttpGet]
        public async Task<IActionResult> ListarOrdenesCompra([FromQuery] bool Active = true)
        {
            try
            {
                var ordenes = await _data.ListarOrdenesCompra(Active);

                if (ordenes == null || !ordenes.Any())
                {
                    return NotFound(new
                    {
                        Success = 0,
                        Message = "No se encontraron órdenes de compra.",
                        Data = new List<OrdenesCompraModel>()
                    });
                }

                return Ok(new
                {
                    Success = 1,
                    Message = "Órdenes de compra encontradas.",
                    Data = ordenes
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = 0,
                    Message = "Error interno del servidor: " + ex.Message
                });
            }
        }

        // GET: v1/OrdenesCompra/Rubro/{idRubro}
        [HttpGet("Rubro/{idRubro}")]
        public async Task<IActionResult> ListarOrdenesCompraPorRubro(int idRubro, [FromQuery] bool Active = true)
        {
            try
            {
                var ordenes = await _data.ListarOrdenesCompraPorRubro(idRubro, Active);

                if (ordenes == null || !ordenes.Any())
                {
                    return NotFound(new
                    {
                        Success = 0,
                        Message = $"No se encontraron órdenes de compra para el rubro con ID: {idRubro}.",
                        Data = new List<OrdenesCompraModel>()
                    });
                }

                return Ok(new
                {
                    Success = 1,
                    Message = "Órdenes de compra encontradas.",
                    Data = ordenes
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = 0,
                    Message = "Error interno del servidor: " + ex.Message
                });
            }
        }

        // GET: v1/OrdenesCompra/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> ListarOrdenCompraPorId(int id, [FromQuery] bool Active = true)
        {
            try
            {
                var orden = await _data.ListarOrdenCompraPorId(id, Active);

                if (orden == null)
                {
                    return NotFound(new
                    {
                        Success = 0,
                        Message = $"No se encontró la orden de compra con ID: {id}.",
                        Data = new OrdenesCompraModel()
                    });
                }

                return Ok(new
                {
                    Success = 1,
                    Message = "Orden de compra encontrada.",
                    Data = orden
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = 0,
                    Message = "Error interno del servidor: " + ex.Message
                });
            }
        }

        // GET: v1/OrdenesCompra/Completas
        [HttpGet("Completas")]
        public async Task<IActionResult> ListarOrdenesCompraCompletas([FromQuery] bool Active = true)
        {
            try
            {
                var ordenes = await _data.ListarOrdenesCompraCompletas(Active);

                if (ordenes == null || !ordenes.Any())
                {
                    return NotFound(new
                    {
                        Success = 0,
                        Message = "No se encontraron órdenes de compra con información completa.",
                        Data = new List<OrdenesCompraModel>()
                    });
                }

                return Ok(new
                {
                    Success = 1,
                    Message = "Órdenes de compra completas encontradas.",
                    Data = ordenes
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = 0,
                    Message = "Error interno del servidor: " + ex.Message
                });
            }
        }

        // GET: v1/OrdenesCompra/Rubro/{idRubro}/Total
        [HttpGet("Rubro/{idRubro}/Total")]
        public async Task<IActionResult> ObtenerTotalOrdenesCompraPorRubro(int idRubro)
        {
            try
            {
                var total = await _data.ObtenerTotalOrdenesCompraPorRubro(idRubro);

                if (total.Success == 0)
                {
                    return NotFound(new
                    {
                        Success = 0,
                        Message = total.Message,
                        Data = new { TotalOrdenesCompra = 0, CantidadOrdenesCompra = 0 }
                    });
                }

                return Ok(new
                {
                    Success = 1,
                    Message = "Total de órdenes de compra obtenido.",
                    Data = new { total.TotalOrdenesCompra, total.CantidadOrdenesCompra }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = 0,
                    Message = "Error interno del servidor: " + ex.Message
                });
            }
        }

        // GET: v1/OrdenesCompra/RangoFechas
        [HttpGet("RangoFechas")]
        public async Task<IActionResult> ListarOrdenesCompraPorRangoFechas(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin,
            [FromQuery] bool Active = true)
        {
            try
            {
                var ordenes = await _data.ListarOrdenesCompraPorRangoFechas(fechaInicio, fechaFin, Active);

                if (ordenes == null || !ordenes.Any())
                {
                    return NotFound(new
                    {
                        Success = 0,
                        Message = $"No se encontraron órdenes de compra en el rango de fechas especificado.",
                        Data = new List<OrdenesCompraModel>()
                    });
                }

                return Ok(new
                {
                    Success = 1,
                    Message = "Órdenes de compra encontradas.",
                    Data = ordenes
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = 0,
                    Message = "Error interno del servidor: " + ex.Message
                });
            }
        }

        // GET: v1/OrdenesCompra/Proyecto/{idProyecto}/Reporte
        [HttpGet("Proyecto/{idProyecto}/Reporte")]
        public async Task<IActionResult> ObtenerReporteOrdenesCompraPorProyecto(int idProyecto)
        {
            try
            {
                var reporte = await _data.ObtenerReporteOrdenesCompraPorProyecto(idProyecto);

                if (reporte == null || !reporte.Any())
                {
                    return NotFound(new
                    {
                        Success = 0,
                        Message = $"No se encontraron órdenes de compra para el proyecto con ID: {idProyecto}.",
                        Data = new List<OrdenesCompraModel>()
                    });
                }

                return Ok(new
                {
                    Success = 1,
                    Message = "Reporte de órdenes de compra obtenido.",
                    Data = reporte
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = 0,
                    Message = "Error interno del servidor: " + ex.Message
                });
            }
        }

        // GET: v1/OrdenesCompra/Rubro/{idRubro}/Balance
        [HttpGet("Rubro/{idRubro}/Balance")]
        public async Task<IActionResult> ObtenerBalanceRubro(int idRubro)
        {
            try
            {
                var balance = await _data.ObtenerBalanceRubro(idRubro);

                if (balance.Success == 0)
                {
                    return NotFound(new
                    {
                        Success = 0,
                        Message = balance.Message,
                        Data = new { TotalDonaciones = 0, TotalOrdenesCompra = 0, Balance = 0 }
                    });
                }

                return Ok(new
                {
                    Success = 1,
                    Message = "Balance del rubro obtenido.",
                    Data = new { balance.TotalDonaciones, balance.TotalOrdenesCompra, balance.Balance }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = 0,
                    Message = "Error interno del servidor: " + ex.Message
                });
            }
        }

        // POST: v1/OrdenesCompra
        [HttpPost]
        public async Task<IActionResult> CrearOrdenCompra([FromBody] OrdenesCompraModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Success = 0,
                    Message = "Datos inválidos.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            try
            {
                model = await _data.CrearOrdenCompra(model);

                if (model.Success == 1)
                {
                    return StatusCode(201, new
                    {
                        Success = 1,
                        Message = "Orden de compra creada con éxito.",
                        Data = model
                    });
                }

                return BadRequest(new
                {
                    Success = 0,
                    Message = model.Message,
                    Data = model
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = 0,
                    Message = "Error interno del servidor: " + ex.Message
                });
            }
        }

        // PUT: v1/OrdenesCompra/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarOrdenCompra(int id, [FromBody] OrdenesCompraModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Success = 0,
                    Message = "Datos inválidos.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            // Asegurar que el ID de la URL coincide con el modelo
            if (model.Id != id)
            {
                return BadRequest(new
                {
                    Success = 0,
                    Message = "El ID de la orden de compra en la URL no coincide con el ID en el cuerpo de la solicitud."
                });
            }

            try
            {
                model = await _data.ActualizarOrdenCompra(model);

                if (model.Success == 1)
                {
                    return Ok(new
                    {
                        Success = 1,
                        Message = "Orden de compra actualizada con éxito.",
                        Data = model
                    });
                }

                return BadRequest(new
                {
                    Success = 0,
                    Message = model.Message,
                    Data = model
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = 0,
                    Message = "Error interno del servidor: " + ex.Message
                });
            }
        }

        // DELETE: v1/OrdenesCompra/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarOrdenCompra(int id)
        {
            try
            {
                var resultado = await _data.EliminarOrdenCompra(id);

                if (resultado.Success == 1)
                {
                    return Ok(new
                    {
                        Success = 1,
                        Message = "Orden de compra eliminada con éxito.",
                        Data = resultado
                    });
                }

                return BadRequest(new
                {
                    Success = 0,
                    Message = resultado.Message,
                    Data = resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = 0,
                    Message = "Error interno del servidor: " + ex.Message
                });
            }
        }

        // POST: v1/OrdenesCompra/Rubro/{idRubro}/Inactivar
        [HttpPost("Rubro/{idRubro}/Inactivar")]
        public async Task<IActionResult> InactivarOrdenesCompraPorRubro(int idRubro)
        {
            try
            {
                var resultado = await _data.InactivarOrdenesCompraPorRubro(idRubro);

                if (resultado.Success == 1)
                {
                    return Ok(new
                    {
                        Success = 1,
                        Message = resultado.Message,
                        Data = resultado
                    });
                }

                return BadRequest(new
                {
                    Success = 0,
                    Message = resultado.Message,
                    Data = resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = 0,
                    Message = "Error interno del servidor: " + ex.Message
                });
            }
        }
    }
}