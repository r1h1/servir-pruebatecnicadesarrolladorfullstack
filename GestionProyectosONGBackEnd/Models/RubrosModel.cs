using System.ComponentModel.DataAnnotations;

namespace GestionProyectosONGBackEnd.Models
{
    public class RubrosModel
    {
        [Required(ErrorMessage = "El campo Id es obligatorio.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo código es obligatorio.")]
        [StringLength(50, ErrorMessage = "El código no puede exceder los 50 caracteres.")]
        public string Codigo { get; set; } = "";

        [Required(ErrorMessage = "El campo nombre es obligatorio.")]
        [StringLength(255, ErrorMessage = "El nombre no puede exceder los 255 caracteres.")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El campo IdProyecto es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El IdProyecto debe ser un número válido.")]
        public int IdProyecto { get; set; }

        [Required(ErrorMessage = "El campo Active es obligatorio.")]
        public int Active { get; set; } = 1;

        // DEVOLUCION DE STATUS
        public int? Success { get; set; }

        // DEVOLUCION DE MENSAJE DE ERROR
        public string? Message { get; set; }
    }

    public class RubroCompletoModel : RubrosModel
    {
        [Required(ErrorMessage = "El campo CodigoProyecto es obligatorio.")]
        [StringLength(10, ErrorMessage = "El código de proyecto no puede exceder los 10 caracteres.")]
        public string CodigoProyecto { get; set; } = "";

        [Required(ErrorMessage = "El campo NombreProyecto es obligatorio.")]
        [StringLength(255, ErrorMessage = "El nombre de proyecto no puede exceder los 255 caracteres.")]
        public string NombreProyecto { get; set; } = "";

        [Required(ErrorMessage = "El campo Municipio es obligatorio.")]
        [StringLength(255, ErrorMessage = "El municipio no puede exceder los 255 caracteres.")]
        public string Municipio { get; set; } = "";

        [Required(ErrorMessage = "El campo Departamento es obligatorio.")]
        [StringLength(255, ErrorMessage = "El departamento no puede exceder los 255 caracteres.")]
        public string Departamento { get; set; } = "";

        [Required(ErrorMessage = "El campo FechaInicio es obligatorio.")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "El campo FechaFin es obligatorio.")]
        public DateTime FechaFin { get; set; }
    }

    public class RubroResponseModel
    {
        public int? Success { get; set; }
        public string? Message { get; set; } = "";

        public RubrosModel? Rubro { get; set; }
    }

    public class RubrosListResponseModel
    {
        public int? Success { get; set; }
        public string? Message { get; set; } = "";

        public List<RubrosModel>? Rubros { get; set; }
    }

    public class RubroCompletoListResponseModel
    {
        public int? Success { get; set; }
        public string? Message { get; set; } = "";

        public List<RubroCompletoModel>? Rubros { get; set; }
    }
}