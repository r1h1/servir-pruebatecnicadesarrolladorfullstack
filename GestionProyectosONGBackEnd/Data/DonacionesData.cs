using GestionProyectosONGBackEnd.Models;
using System.Data;
using System.Data.SqlClient;

namespace GestionProyectosONGBackEnd.Data
{
    public class DonacionesData
    {
        private readonly string connection;

        public DonacionesData(IConfiguration configuration)
        {
            connection = configuration.GetConnectionString("ConexionRemota");
        }

        // Listar todas las donaciones activas (solo si el rubro está activo)
        public async Task<List<DonacionesModel>> ListarDonaciones(bool Active = true)
        {
            List<DonacionesModel> lista = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_listarDonaciones", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Active", Active);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new DonacionesModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    IdRubro = Convert.ToInt32(reader["IdRubro"]),
                    Monto = Convert.ToDecimal(reader["Monto"]),
                    FechaDonacion = Convert.ToDateTime(reader["FechaDonacion"]),
                    NombreDonante = reader["NombreDonante"].ToString() ?? "",
                    Active = reader["Active"] != DBNull.Value ? Convert.ToInt32(reader["Active"]) : 1
                });
            }
            return lista;
        }

        // Listar donaciones por ID de rubro (solo si el rubro está activo)
        public async Task<List<DonacionesModel>> ListarDonacionesPorRubro(int IdRubro, bool Active = true)
        {
            List<DonacionesModel> lista = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_listarDonacionesPorRubro", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@IdRubro", IdRubro);
            cmd.Parameters.AddWithValue("@Active", Active);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new DonacionesModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    IdRubro = Convert.ToInt32(reader["IdRubro"]),
                    Monto = Convert.ToDecimal(reader["Monto"]),
                    FechaDonacion = Convert.ToDateTime(reader["FechaDonacion"]),
                    NombreDonante = reader["NombreDonante"].ToString() ?? "",
                    Active = reader["Active"] != DBNull.Value ? Convert.ToInt32(reader["Active"]) : 1
                });
            }
            return lista;
        }

        // Listar donación por ID (solo si el rubro está activo)
        public async Task<DonacionesModel> ListarDonacionPorId(int Id, bool Active = true)
        {
            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_listarDonacionPorId", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", Id);
            cmd.Parameters.AddWithValue("@Active", Active);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new DonacionesModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    IdRubro = Convert.ToInt32(reader["IdRubro"]),
                    Monto = Convert.ToDecimal(reader["Monto"]),
                    FechaDonacion = Convert.ToDateTime(reader["FechaDonacion"]),
                    NombreDonante = reader["NombreDonante"].ToString() ?? "",
                    Active = reader["Active"] != DBNull.Value ? Convert.ToInt32(reader["Active"]) : 1
                };
            }
            return null;
        }

        // Listar donaciones con detalles completos (incluye información de rubro y proyecto)
        public async Task<List<DonacionesModel>> ListarDonacionesCompletas(bool Active = true)
        {
            List<DonacionesModel> lista = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_listarDonacionesCompletas", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Active", Active);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new DonacionesModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    IdRubro = Convert.ToInt32(reader["IdRubro"]),
                    Monto = Convert.ToDecimal(reader["Monto"]),
                    FechaDonacion = Convert.ToDateTime(reader["FechaDonacion"]),
                    NombreDonante = reader["NombreDonante"].ToString() ?? "",
                    Active = reader["Active"] != DBNull.Value ? Convert.ToInt32(reader["Active"]) : 1,
                    CodigoRubro = reader["CodigoRubro"].ToString() ?? "",
                    NombreRubro = reader["NombreRubro"].ToString() ?? "",
                    CodigoProyecto = reader["CodigoProyecto"].ToString() ?? "",
                    NombreProyecto = reader["NombreProyecto"].ToString() ?? ""
                });
            }
            return lista;
        }

        // Obtener total de donaciones por rubro (solo si el rubro está activo)
        public async Task<TotalDonacionesResponse> ObtenerTotalDonacionesPorRubro(int IdRubro)
        {
            var response = new TotalDonacionesResponse();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_obtenerTotalDonacionesPorRubro", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@IdRubro", IdRubro);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                response.TotalDonaciones = Convert.ToDecimal(reader["TotalDonaciones"]);
                response.CantidadDonaciones = Convert.ToInt32(reader["CantidadDonaciones"]);
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

        // Listar donaciones por donante (solo si el rubro está activo)
        public async Task<List<DonacionesModel>> ListarDonacionesPorDonante(string NombreDonante, bool Active = true)
        {
            List<DonacionesModel> lista = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_listarDonacionesPorDonante", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@NombreDonante", NombreDonante);
            cmd.Parameters.AddWithValue("@Active", Active);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new DonacionesModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    IdRubro = Convert.ToInt32(reader["IdRubro"]),
                    Monto = Convert.ToDecimal(reader["Monto"]),
                    FechaDonacion = Convert.ToDateTime(reader["FechaDonacion"]),
                    NombreDonante = reader["NombreDonante"].ToString() ?? "",
                    Active = reader["Active"] != DBNull.Value ? Convert.ToInt32(reader["Active"]) : 1
                });
            }
            return lista;
        }

        // Listar donaciones por rango de fechas (solo si el rubro está activo)
        public async Task<List<DonacionesModel>> ListarDonacionesPorRangoFechas(DateTime FechaInicio, DateTime FechaFin, bool Active = true)
        {
            List<DonacionesModel> lista = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_listarDonacionesPorRangoFechas", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@FechaInicio", FechaInicio);
            cmd.Parameters.AddWithValue("@FechaFin", FechaFin);
            cmd.Parameters.AddWithValue("@Active", Active);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new DonacionesModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    IdRubro = Convert.ToInt32(reader["IdRubro"]),
                    Monto = Convert.ToDecimal(reader["Monto"]),
                    FechaDonacion = Convert.ToDateTime(reader["FechaDonacion"]),
                    NombreDonante = reader["NombreDonante"].ToString() ?? "",
                    Active = reader["Active"] != DBNull.Value ? Convert.ToInt32(reader["Active"]) : 1
                });
            }
            return lista;
        }

        // Obtener reporte de donaciones por proyecto (solo si proyecto y rubro están activos)
        public async Task<List<DonacionesModel>> ObtenerReporteDonacionesPorProyecto(int IdProyecto)
        {
            List<DonacionesModel> lista = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_obtenerReporteDonacionesPorProyecto", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@IdProyecto", IdProyecto);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new DonacionesModel
                {
                    NombreProyecto = reader["NombreProyecto"].ToString() ?? "",
                    CodigoRubro = reader["CodigoRubro"].ToString() ?? "",
                    NombreRubro = reader["NombreRubro"].ToString() ?? "",
                    // Estos campos se pueden mapear según necesidad
                    Monto = Convert.ToDecimal(reader["TotalDonado"]),
                    // Se puede agregar CantidadDonaciones como propiedad adicional si es necesario
                });
            }
            return lista;
        }

        // Obtener top donantes (solo si el rubro está activo)
        public async Task<List<DonacionesModel>> ObtenerTopDonantes(int Top = 10)
        {
            List<DonacionesModel> lista = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_obtenerTopDonantes", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Top", Top);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new DonacionesModel
                {
                    NombreDonante = reader["NombreDonante"].ToString() ?? "",
                    Monto = Convert.ToDecimal(reader["TotalDonado"]),
                    // Se puede agregar CantidadDonaciones como propiedad adicional si es necesario
                });
            }
            return lista;
        }

        // Crear nueva donación (solo si el rubro está activo)
        public async Task<DonacionesModel> CrearDonacion(DonacionesModel model)
        {
            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_crearDonacion", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@IdRubro", model.IdRubro);
            cmd.Parameters.AddWithValue("@Monto", model.Monto);
            cmd.Parameters.AddWithValue("@FechaDonacion", model.FechaDonacion);
            cmd.Parameters.AddWithValue("@NombreDonante", model.NombreDonante);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                model.Success = reader.GetBoolean(0) ? 1 : 0;
                model.Message = reader.GetString(1);

                // Si fue exitoso, cargar todos los datos de la donación
                if (model.Success == 1)
                {
                    model.Id = Convert.ToInt32(reader["Id"]);
                    model.IdRubro = Convert.ToInt32(reader["IdRubro"]);
                    model.Monto = Convert.ToDecimal(reader["Monto"]);
                    model.FechaDonacion = Convert.ToDateTime(reader["FechaDonacion"]);
                    model.NombreDonante = reader["NombreDonante"].ToString() ?? "";
                    model.Active = reader["Active"] != DBNull.Value ? Convert.ToInt32(reader["Active"]) : 1;
                }
            }
            return model;
        }

        // Actualizar donación (solo si el rubro original y nuevo están activos)
        public async Task<DonacionesModel> ActualizarDonacion(DonacionesModel model)
        {
            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_editarDonacion", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@IdRubro", model.IdRubro);
            cmd.Parameters.AddWithValue("@Monto", model.Monto);
            cmd.Parameters.AddWithValue("@FechaDonacion", model.FechaDonacion);
            cmd.Parameters.AddWithValue("@NombreDonante", model.NombreDonante);

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
                    model.FechaDonacion = Convert.ToDateTime(reader["FechaDonacion"]);
                    model.NombreDonante = reader["NombreDonante"].ToString() ?? "";
                    model.Active = reader["Active"] != DBNull.Value ? Convert.ToInt32(reader["Active"]) : 1;
                }
            }
            return model;
        }

        // Eliminar donación (solo si el rubro asociado está activo)
        public async Task<DonacionesModel> EliminarDonacion(int Id)
        {
            DonacionesModel model = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            using SqlCommand cmd = new("sp_eliminarDonacion", con)
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

        // Inactivar donaciones por rubro (cuando se inactiva un rubro)
        public async Task<DonacionesModel> InactivarDonacionesPorRubro(int IdRubro)
        {
            DonacionesModel model = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            using SqlCommand cmd = new("sp_inactivarDonacionesPorRubro", con)
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