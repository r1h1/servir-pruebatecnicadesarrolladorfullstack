using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GestionProyectosONGBackEnd.Data;
using GestionProyectosONGBackEnd.Models;

namespace GestionProyectosONGBackEnd.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class ProyectosController : ControllerBase
    {
        private readonly ProyectosData _data;

        public ProyectosController(ProyectosData data)
        {
            _data = data;
        }

        // GET: v1/Proyectos
        [HttpGet]
        public async Task<IActionResult> ListarProyectos([FromQuery] bool Active = true)
        {
            try
            {
                var proyectos = await _data.ListarProyectos(Active);

                if (proyectos == null || !proyectos.Any())
                {
                    return NotFound(new { Success = 0, Message = "No se encontraron proyectos.", Data = new List<ProyectosModel>() });
                }

                return Ok(new { Success = 1, Message = "Proyectos encontrados.", Data = proyectos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = 0, Message = "Error interno del servidor: " + ex.Message });
            }
        }

        // GET: v1/Proyectos/{codigo}
        [HttpGet("{codigo}")]
        public async Task<IActionResult> ListarProyectoPorCodigo(string codigo)
        {
            try
            {
                var proyectos = await _data.ListarProyectoPorCodigo(codigo);

                if (proyectos == null)
                {
                    return NotFound(new { Success = 0, Message = "No se encontró el proyecto con código: " + codigo + ".", Data = new List<ProyectosModel>() });
                }

                return Ok(new { Success = 1, Message = "Proyecto encontrado.", Data = proyectos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = 0, Message = "Error interno del servidor: " + ex.Message });
            }
        }

        // GET: v1/Proyectos
        [HttpGet]
        [Route("UltimoCodigo")]
        public async Task<IActionResult> ListarUltimoCodigoDeProyectoGenerado()
        {
            try
            {
                var proyectos = await _data.ListarUltimoCodigoDeProyectoGenerado();

                if (proyectos == null || !proyectos.Any())
                {
                    return NotFound(new { Success = 0, Message = "No se encontró el último código de proyecto.", 
                        Data = new List<ProyectosModel>() });
                }

                return Ok(new { Success = 1, Message = "Código de proyecto encontrado.", Data = proyectos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = 0, Message = "Error interno del servidor: " + ex.Message });
            }
        }

        // POST: v1/Proyectos
        [HttpPost]
        public async Task<IActionResult> CrearProyecto([FromBody] ProyectosModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Success = 0, Message = "Datos inválidos.", Errrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            try
            {
                model = await _data.CrearProyecto(model);

                if (model.Success == 1)
                {
                    return StatusCode(201, new { Success = 1, Message = "Proyecto creado con éxito.", Data = model });
                }

                return BadRequest(new { Success = 0, Message = model.Message, Data = model });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = 0, Message = "Error interno del servidor: " + ex.Message });
            }
        }

        // PUT: v1/Proyectos/{codigo}
        [HttpPut("{codigo}")]
        public async Task<IActionResult> ActualizarProyecto(string codigo, [FromBody] ProyectosModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Success = 0, Message = "Datos inválidos.", Errrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            try
            {
                model = await _data.ActualizarProyecto(model);

                if (model.Success == 1)
                {
                    return StatusCode(201, new { Success = 1, Message = "Proyecto creado con éxito.", Data = model });
                }

                return BadRequest(new { Success = 0, Message = model.Message, Data = model });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = 0, Message = "Error interno del servidor: " + ex.Message });
            }
        }

        // DELETE: v1/Proyectos/{codigo}
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> EliminarProyecto(string codigo)
        {
            try
            {
                var proyectos = await _data.EliminarProyecto(codigo);

                if(proyectos.Success == 1)
                {
                    return Ok(new { Success = 1, Message = "Proyecto eliminado con éxito.", Data = proyectos });
                }

                return BadRequest(new { Success = 0, Message = proyectos.Message, Data = proyectos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = 0, Message = "Error interno del servidor: " + ex.Message });
            }
        }
    }
}
