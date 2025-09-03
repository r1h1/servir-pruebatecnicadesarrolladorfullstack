using System.ComponentModel.DataAnnotations;

namespace GestionProyectosONGBackEnd.Models
{
    public class OrdenesCompraModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo IdRubro es obligatorio.")]
        public int IdRubro { get; set; }

        [Required(ErrorMessage = "El campo Monto es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0.")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "El campo FechaOrden es obligatorio.")]
        public DateTime FechaOrden { get; set; }

        public bool Active { get; set; }

        // Nuevas propiedades para los detalles del rubro y proyecto
        public string? NombreProyecto { get; set; }
        public string? CodigoRubro { get; set; }
        public string? NombreRubro { get; set; }
        public string? CodigoProyecto { get; set; }  // Nueva propiedad agregada

        // Para devolución de status
        public int? Success { get; set; }

        // Para devolución de mensaje de error
        public string? Message { get; set; }
    }

    // Clases de respuesta para los métodos que devuelven datos adicionales
    public class TotalOrdenesCompraResponse
    {
        public decimal TotalOrdenesCompra { get; set; }
        public int CantidadOrdenesCompra { get; set; }
        public int Success { get; set; }
        public string Message { get; set; }
    }

    public class BalanceRubroResponse
    {
        public decimal TotalDonaciones { get; set; }
        public decimal TotalOrdenesCompra { get; set; }
        public decimal Balance { get; set; }
        public int Success { get; set; }
        public string Message { get; set; }
    }
}