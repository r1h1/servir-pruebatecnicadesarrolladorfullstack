using System.ComponentModel.DataAnnotations;

namespace GestionProyectosONGBackEnd.Models
{
    public class ProyectosModel
    {
        [Required(ErrorMessage = "El campo Id es obligatorio.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo código es obligatorio y auto-generado.")]
        public string? Codigo { get; set; }

        [Required(ErrorMessage = "El campo nombre es obligatorio.")]
        [StringLength(255, ErrorMessage = "El nombre no puede exceder los 255 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo municipio es obligatorio.")]
        [StringLength(255, ErrorMessage = "El municipio no puede exceder los 255 caracteres.")]
        public string Municipio { get; set; }

        [Required(ErrorMessage = "El campo departamento es obligatorio.")]
        [StringLength(255, ErrorMessage = "El departamento no puede exceder los 255 caracteres.")]
        public string Departamento { get; set; }

        [Required(ErrorMessage = "El campo fecha de inicio es obligatorio.")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "El campo fecha fin es obligatorio.")]
        public DateTime FechaFin { get; set; }

        //DEVOLUCION DE STATUS
        public int? Success { get; set; }

        //DEVOLUCION DE MENSAJE DE ERROR
        public string? Message { get; set; }

    }
}
