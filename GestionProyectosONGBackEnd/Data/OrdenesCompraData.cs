using GestionProyectosONGBackEnd.Models;
using System.Data;
using System.Data.SqlClient;

namespace GestionProyectosONGBackEnd.Data
{
    public class OrdenesCompraData
    {
        private readonly string connection;

        public OrdenesCompraData(IConfiguration configuration)
        {
            connection = configuration.GetConnectionString("ConexionRemota");
        }

        // Listar todas las órdenes de compra activas (solo si el rubro está activo)
        public async Task<List<OrdenesCompraModel>> ListarOrdenesCompra(bool Active = true)
        {
            List<OrdenesCompraModel> lista = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_listarOrdenesCompra", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Active", Active);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new OrdenesCompraModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    IdRubro = Convert.ToInt32(reader["IdRubro"]),
                    Monto = Convert.ToDecimal(reader["Monto"]),
                    FechaOrden = Convert.ToDateTime(reader["FechaOrden"]),
                    Active = Convert.ToBoolean(reader["Active"])
                });
            }
            return lista;
        }

        // Listar órdenes de compra por ID de rubro (solo si el rubro está activo)
        public async Task<List<OrdenesCompraModel>> ListarOrdenesCompraPorRubro(int IdRubro, bool Active = true)
        {
            List<OrdenesCompraModel> lista = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_listarOrdenesCompraPorRubro", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@IdRubro", IdRubro);
            cmd.Parameters.AddWithValue("@Active", Active);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new OrdenesCompraModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    IdRubro = Convert.ToInt32(reader["IdRubro"]),
                    Monto = Convert.ToDecimal(reader["Monto"]),
                    FechaOrden = Convert.ToDateTime(reader["FechaOrden"]),
                    Active = Convert.ToBoolean(reader["Active"])
                });
            }
            return lista;
        }

        // Listar orden de compra por ID (solo si el rubro está activo)
        public async Task<OrdenesCompraModel> ListarOrdenCompraPorId(int Id, bool Active = true)
        {
            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_listarOrdenCompraPorId", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", Id);
            cmd.Parameters.AddWithValue("@Active", Active);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new OrdenesCompraModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    IdRubro = Convert.ToInt32(reader["IdRubro"]),
                    Monto = Convert.ToDecimal(reader["Monto"]),
                    FechaOrden = Convert.ToDateTime(reader["FechaOrden"]),
                    Active = Convert.ToBoolean(reader["Active"])
                };
            }
            return null;
        }

        // Listar órdenes de compra con detalles completos (incluye información de rubro y proyecto)
        public async Task<List<OrdenesCompraModel>> ListarOrdenesCompraCompletas(bool Active = true)
        {
            List<OrdenesCompraModel> lista = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_listarOrdenesCompraCompletas", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Active", Active);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new OrdenesCompraModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Monto = Convert.ToDecimal(reader["Monto"]),
                    FechaOrden = Convert.ToDateTime(reader["FechaOrden"]),
                    Active = Convert.ToBoolean(reader["Active"]),
                    CodigoRubro = reader["CodigoRubro"].ToString() ?? "",
                    NombreRubro = reader["NombreRubro"].ToString() ?? "",
                    CodigoProyecto = reader["CodigoProyecto"].ToString() ?? "",
                    NombreProyecto = reader["NombreProyecto"].ToString() ?? ""
                });
            }
            return lista;
        }

        // Obtener total de órdenes de compra por rubro (solo si el rubro está activo)
        public async Task<TotalOrdenesCompraResponse> ObtenerTotalOrdenesCompraPorRubro(int IdRubro)
        {
            var response = new TotalOrdenesCompraResponse();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_obtenerTotalOrdenesCompraPorRubro", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@IdRubro", IdRubro);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                response.TotalOrdenesCompra = Convert.ToDecimal(reader["TotalOrdenesCompra"]);
                response.CantidadOrdenesCompra = Convert.ToInt32(reader["CantidadOrdenesCompra"]);
                response.Success = 1;
                response.Message = "Datos obtenidos correctamente";
            }
            else
            {
                response.Success = 0;
                response.Message = "No se encontraron datos para el rubro especificado";
            }
            return response;
        }

        // Listar órdenes de compra por rango de fechas (solo si el rubro está activo)
        public async Task<List<OrdenesCompraModel>> ListarOrdenesCompraPorRangoFechas(DateTime FechaInicio, DateTime FechaFin, bool Active = true)
        {
            List<OrdenesCompraModel> lista = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_listarOrdenesCompraPorRangoFechas", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@FechaInicio", FechaInicio);
            cmd.Parameters.AddWithValue("@FechaFin", FechaFin);
            cmd.Parameters.AddWithValue("@Active", Active);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new OrdenesCompraModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    IdRubro = Convert.ToInt32(reader["IdRubro"]),
                    Monto = Convert.ToDecimal(reader["Monto"]),
                    FechaOrden = Convert.ToDateTime(reader["FechaOrden"]),
                    Active = Convert.ToBoolean(reader["Active"])
                });
            }
            return lista;
        }

        // Obtener reporte de órdenes de compra por proyecto (solo si proyecto y rubro están activos)
        public async Task<List<OrdenesCompraModel>> ObtenerReporteOrdenesCompraPorProyecto(int IdProyecto)
        {
            List<OrdenesCompraModel> lista = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_obtenerReporteOrdenesCompraPorProyecto", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@IdProyecto", IdProyecto);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new OrdenesCompraModel
                {
                    NombreProyecto = reader["NombreProyecto"].ToString() ?? "",
                    CodigoRubro = reader["CodigoRubro"].ToString() ?? "",
                    NombreRubro = reader["NombreRubro"].ToString() ?? "",
                    Monto = Convert.ToDecimal(reader["TotalOrdenesCompra"]),
                    // Se puede agregar CantidadOrdenesCompra como propiedad adicional si es necesario
                });
            }
            return lista;
        }

        // Obtener balance de rubro (donaciones - órdenes de compra)
        public async Task<BalanceRubroResponse> ObtenerBalanceRubro(int IdRubro)
        {
            var response = new BalanceRubroResponse();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_obtenerBalanceRubro", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@IdRubro", IdRubro);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                response.TotalDonaciones = Convert.ToDecimal(reader["TotalDonaciones"]);
                response.TotalOrdenesCompra = Convert.ToDecimal(reader["TotalOrdenesCompra"]);
                response.Balance = Convert.ToDecimal(reader["Balance"]);
                response.Success = 1;
                response.Message = "Balance obtenido correctamente";
            }
            else
            {
                response.Success = 0;
                response.Message = "No se pudo obtener el balance para el rubro especificado";
            }
            return response;
        }

        // Crear nueva orden de compra (solo si el rubro está activo)
        public async Task<OrdenesCompraModel> CrearOrdenCompra(OrdenesCompraModel model)
        {
            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_crearOrdenCompra", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@IdRubro", model.IdRubro);
            cmd.Parameters.AddWithValue("@Monto", model.Monto);
            cmd.Parameters.AddWithValue("@FechaOrden", model.FechaOrden);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                model.Success = reader.GetBoolean(0) ? 1 : 0;
                model.Message = reader.GetString(1);

                // Si fue exitoso, cargar todos los datos de la orden de compra
                if (model.Success == 1)
                {
                    model.Id = Convert.ToInt32(reader["Id"]);
                    model.IdRubro = Convert.ToInt32(reader["IdRubro"]);
                    model.Monto = Convert.ToDecimal(reader["Monto"]);
                    model.FechaOrden = Convert.ToDateTime(reader["FechaOrden"]);
                    model.Active = Convert.ToBoolean(reader["Active"]);
                }
            }
            return model;
        }

        // Actualizar orden de compra (solo si el rubro original y nuevo están activos)
        public async Task<OrdenesCompraModel> ActualizarOrdenCompra(OrdenesCompraModel model)
        {
            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_editarOrdenCompra", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@IdRubro", model.IdRubro);
            cmd.Parameters.AddWithValue("@Monto", model.Monto);
            cmd.Parameters.AddWithValue("@FechaOrden", model.FechaOrden);

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                model.Success = reader.GetBoolean(0) ? 1 : 0;
                model.Message = reader.GetString(1);

                // Si fue exitoso, cargar todos los datos actualizados
                if (model.Success == 1)
                {
                    model.Id = Convert.ToInt32(reader["Id"]);
                    model.IdRubro = Convert.ToInt32(reader["IdRubro"]);
                    model.Monto = Convert.ToDecimal(reader["Monto"]);
                    model.FechaOrden = Convert.ToDateTime(reader["FechaOrden"]);
                    model.Active = Convert.ToBoolean(reader["Active"]);
                }
            }
            return model;
        }

        // Eliminar orden de compra (solo si el rubro asociado está activo)
        public async Task<OrdenesCompraModel> EliminarOrdenCompra(int Id)
        {
            OrdenesCompraModel model = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            using SqlCommand cmd = new("sp_eliminarOrdenCompra", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", Id);

            try
            {
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    model.Success = reader.GetBoolean(0) ? 1 : 0;
                    model.Message = reader.GetString(1);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                model.Success = 0;
                model.Message = $"Error al ejecutar el procedimiento: {ex.Message}";
            }

            return model;
        }

        // Inactivar órdenes de compra por rubro (cuando se inactiva un rubro)
        public async Task<OrdenesCompraModel> InactivarOrdenesCompraPorRubro(int IdRubro)
        {
            OrdenesCompraModel model = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            using SqlCommand cmd = new("sp_inactivarOrdenesCompraPorRubro", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@IdRubro", IdRubro);

            try
            {
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    model.Success = reader.GetBoolean(0) ? 1 : 0;
                    model.Message = reader.GetString(1);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                model.Success = 0;
                model.Message = $"Error al ejecutar el procedimiento: {ex.Message}";
            }

            return model;
        }
    }
}