using System.ComponentModel.DataAnnotations;

namespace GestionProyectosONGBackEnd.Models
{
    public class DonacionesModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo IdRubro es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El IdRubro debe ser un número válido.")]
        public int IdRubro { get; set; }

        [Required(ErrorMessage = "El campo Monto es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0.")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "El campo FechaDonacion es obligatorio.")]
        public DateTime FechaDonacion { get; set; }

        [Required(ErrorMessage = "El campo NombreDonante es obligatorio.")]
        [StringLength(255, ErrorMessage = "El nombre del donante no puede exceder los 255 caracteres.")]
        public string NombreDonante { get; set; } = "";

        public int Active { get; set; } = 1;

        // Campos para respuestas extendidas (opcionales)
        public string? CodigoRubro { get; set; }
        public string? NombreRubro { get; set; }
        public string? CodigoProyecto { get; set; }
        public string? NombreProyecto { get; set; }

        // DEVOLUCION DE STATUS
        public int? Success { get; set; }

        // DEVOLUCION DE MENSAJE DE ERROR
        public string? Message { get; set; }
    }

    public class DonacionResponse
    {
        public int Success { get; set; }
        public string Message { get; set; } = "";
        public DonacionesModel? Data { get; set; }
    }

    public class DonacionesListResponse
    {
        public int Success { get; set; }
        public string Message { get; set; } = "";
        public List<DonacionesModel>? Data { get; set; }
    }

    public class TotalDonacionesResponse
    {
        public int Success { get; set; }
        public string Message { get; set; } = "";
        public decimal TotalDonaciones { get; set; }
        public int CantidadDonaciones { get; set; }
    }

    public class ReporteDonacionesResponse
    {
        public int Success { get; set; }
        public string Message { get; set; } = "";
        public List<DonacionesModel>? Data { get; set; }
    }
}