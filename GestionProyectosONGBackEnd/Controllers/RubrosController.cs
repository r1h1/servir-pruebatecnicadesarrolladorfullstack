using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GestionProyectosONGBackEnd.Data;
using GestionProyectosONGBackEnd.Models;

namespace GestionProyectosONGBackEnd.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class RubrosController : ControllerBase
    {
        private readonly RubrosData _data;

        public RubrosController(RubrosData data)
        {
            _data = data;
        }

        // GET: v1/Rubros
        [HttpGet]
        public async Task<IActionResult> ListarRubros([FromQuery] bool Active = true)
        {
            try
            {
                var rubros = await _data.ListarRubros(Active);

                if (rubros == null || !rubros.Any())
                {
                    return NotFound(new
                    {
                        Success = 0,
                        Message = "No se encontraron rubros.",
                        Data = new List<RubrosModel>()
                    });
                }

                return Ok(new
                {
                    Success = 1,
                    Message = "Rubros encontrados.",
                    Data = rubros
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

        // GET: v1/Rubros/Proyecto/{idProyecto}
        [HttpGet("Proyecto/{idProyecto}")]
        public async Task<IActionResult> ListarRubrosPorProyecto(int idProyecto, [FromQuery] bool Active = true)
        {
            try
            {
                var rubros = await _data.ListarRubrosPorProyecto(idProyecto, Active);

                if (rubros == null || !rubros.Any())
                {
                    return NotFound(new
                    {
                        Success = 0,
                        Message = $"No se encontraron rubros para el proyecto con ID: {idProyecto}.",
                        Data = new List<RubrosModel>()
                    });
                }

                return Ok(new
                {
                    Success = 1,
                    Message = "Rubros encontrados.",
                    Data = rubros
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

        // GET: v1/Rubros/{codigo}
        [HttpGet("{codigo}")]
        public async Task<IActionResult> ListarRubroPorCodigo(string codigo, [FromQuery] bool Active = true)
        {
            try
            {
                var rubro = await _data.ListarRubroPorCodigo(codigo, Active);

                if (rubro == null)
                {
                    return NotFound(new
                    {
                        Success = 0,
                        Message = $"No se encontró el rubro con código: {codigo}.",
                        Data = new RubrosModel()
                    });
                }

                return Ok(new
                {
                    Success = 1,
                    Message = "Rubro encontrado.",
                    Data = rubro
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

        // GET: v1/Rubros/Completos
        [HttpGet("Completos")]
        public async Task<IActionResult> ListarRubrosCompletos([FromQuery] bool Active = true)
        {
            try
            {
                var rubros = await _data.ListarRubrosCompletos(Active);

                if (rubros == null || !rubros.Any())
                {
                    return NotFound(new
                    {
                        Success = 0,
                        Message = "No se encontraron rubros con información completa.",
                        Data = new List<RubroCompletoModel>()
                    });
                }

                return Ok(new
                {
                    Success = 1,
                    Message = "Rubros completos encontrados.",
                    Data = rubros
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

        // POST: v1/Rubros
        [HttpPost]
        public async Task<IActionResult> CrearRubro([FromBody] RubrosModel model)
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
                model = await _data.CrearRubro(model);

                if (model.Success == 1)
                {
                    return StatusCode(201, new
                    {
                        Success = 1,
                        Message = "Rubro creado con éxito.",
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

        // PUT: v1/Rubros/{codigo}
        [HttpPut("{codigo}")]
        public async Task<IActionResult> ActualizarRubro(string codigo, [FromBody] RubrosModel model)
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

            // Asegurar que el código de la URL coincide con el modelo
            if (model.Codigo != codigo)
            {
                return BadRequest(new
                {
                    Success = 0,
                    Message = "El código del rubro en la URL no coincide con el código en el cuerpo de la solicitud."
                });
            }

            try
            {
                model = await _data.ActualizarRubro(model);

                if (model.Success == 1)
                {
                    return Ok(new
                    {
                        Success = 1,
                        Message = "Rubro actualizado con éxito.",
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

        // DELETE: v1/Rubros/{codigo}
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> EliminarRubro(string codigo)
        {
            try
            {
                var resultado = await _data.EliminarRubro(codigo);

                if (resultado.Success == 1)
                {
                    return Ok(new
                    {
                        Success = 1,
                        Message = "Rubro eliminado con éxito.",
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

        // POST: v1/Rubros/InactivarPorProyecto/{idProyecto}
        [HttpPost("InactivarPorProyecto/{idProyecto}")]
        public async Task<IActionResult> InactivarRubrosPorProyecto(int idProyecto)
        {
            try
            {
                var resultado = await _data.InactivarRubrosPorProyecto(idProyecto);

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