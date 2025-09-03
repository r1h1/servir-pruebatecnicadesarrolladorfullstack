using GestionProyectosONGBackEnd.Models;
using System.Data;
using System.Data.SqlClient;

namespace GestionProyectosONGBackEnd.Data
{
    public class RubrosData
    {
        private readonly string connection;

        public RubrosData(IConfiguration configuration)
        {
            connection = configuration.GetConnectionString("ConexionRemota");
        }

        // Listar todos los rubros activos (solo si el proyecto está activo)
        public async Task<List<RubrosModel>> ListarRubros(bool Active = true)
        {
            List<RubrosModel> lista = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_listarRubros", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Active", Active);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new RubrosModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Codigo = reader["Codigo"].ToString() ?? "",
                    Nombre = reader["Nombre"].ToString() ?? "",
                    IdProyecto = Convert.ToInt32(reader["IdProyecto"]),
                    Active = reader["Active"] != DBNull.Value ? Convert.ToInt32(reader["Active"]) : 1
                });
            }
            return lista;
        }

        // Listar rubros por ID de proyecto (solo si el proyecto está activo)
        public async Task<List<RubrosModel>> ListarRubrosPorProyecto(int IdProyecto, bool Active = true)
        {
            List<RubrosModel> lista = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_listarRubrosPorProyecto", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@IdProyecto", IdProyecto);
            cmd.Parameters.AddWithValue("@Active", Active);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new RubrosModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Codigo = reader["Codigo"].ToString() ?? "",
                    Nombre = reader["Nombre"].ToString() ?? "",
                    IdProyecto = Convert.ToInt32(reader["IdProyecto"]),
                    Active = reader["Active"] != DBNull.Value ? Convert.ToInt32(reader["Active"]) : 1
                });
            }
            return lista;
        }

        // Listar rubro por código (solo si el proyecto está activo)
        public async Task<RubrosModel> ListarRubroPorCodigo(string Codigo, bool Active = true)
        {
            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_listarRubroPorCodigo", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Codigo", Codigo);
            cmd.Parameters.AddWithValue("@Active", Active);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new RubrosModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Codigo = reader["Codigo"].ToString() ?? "",
                    Nombre = reader["Nombre"].ToString() ?? "",
                    IdProyecto = Convert.ToInt32(reader["IdProyecto"]),
                    Active = reader["Active"] != DBNull.Value ? Convert.ToInt32(reader["Active"]) : 1
                };
            }
            return null;
        }

        // Listar rubros con detalles completos (incluye información del proyecto)
        public async Task<List<RubroCompletoModel>> ListarRubrosCompletos(bool Active = true)
        {
            List<RubroCompletoModel> lista = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_listarRubrosCompletos", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Active", Active);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new RubroCompletoModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Codigo = reader["Codigo"].ToString() ?? "",
                    Nombre = reader["Nombre"].ToString() ?? "",
                    IdProyecto = Convert.ToInt32(reader["IdProyecto"]),
                    Active = reader["Active"] != DBNull.Value ? Convert.ToInt32(reader["Active"]) : 1,
                    CodigoProyecto = reader["CodigoProyecto"].ToString() ?? "",
                    NombreProyecto = reader["NombreProyecto"].ToString() ?? "",
                    Municipio = reader["Municipio"].ToString() ?? "",
                    Departamento = reader["Departamento"].ToString() ?? "",
                    FechaInicio = reader["FechaInicio"] != DBNull.Value ? Convert.ToDateTime(reader["FechaInicio"]) : DateTime.MinValue,
                    FechaFin = reader["FechaFin"] != DBNull.Value ? Convert.ToDateTime(reader["FechaFin"]) : DateTime.MinValue
                });
            }
            return lista;
        }

        // Crear nuevo rubro (solo si el proyecto está activo)
        public async Task<RubrosModel> CrearRubro(RubrosModel model)
        {
            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_crearRubro", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
            cmd.Parameters.AddWithValue("@IdProyecto", model.IdProyecto);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                model.Success = reader.GetBoolean(0) ? 1 : 0;
                model.Message = reader.GetString(1);

                // Si fue exitoso, cargar todos los datos del rubro
                if (model.Success == 1)
                {
                    model.Id = Convert.ToInt32(reader["Id"]);
                    model.Codigo = reader["Codigo"].ToString() ?? "";
                    model.Nombre = reader["Nombre"].ToString() ?? "";
                    model.IdProyecto = Convert.ToInt32(reader["IdProyecto"]);
                    model.Active = reader["Active"] != DBNull.Value ? Convert.ToInt32(reader["Active"]) : 1;
                }
            }
            return model;
        }

        // Actualizar rubro (solo si el proyecto original y nuevo están activos)
        public async Task<RubrosModel> ActualizarRubro(RubrosModel model)
        {
            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_editarRubro", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Codigo", model.Codigo);
            cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
            cmd.Parameters.AddWithValue("@IdProyecto", model.IdProyecto);

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                model.Success = reader.GetBoolean(0) ? 1 : 0;
                model.Message = reader.GetString(1);

                // Si fue exitoso, cargar todos los datos actualizados
                if (model.Success == 1)
                {
                    model.Id = Convert.ToInt32(reader["Id"]);
                    model.Codigo = reader["Codigo"].ToString() ?? "";
                    model.Nombre = reader["Nombre"].ToString() ?? "";
                    model.IdProyecto = Convert.ToInt32(reader["IdProyecto"]);
                    model.Active = reader["Active"] != DBNull.Value ? Convert.ToInt32(reader["Active"]) : 1;
                }
            }
            return model;
        }

        // Eliminar rubro (solo si el proyecto asociado está activo)
        public async Task<RubrosModel> EliminarRubro(string Codigo)
        {
            RubrosModel model = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            using SqlCommand cmd = new("sp_eliminarRubro", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Codigo", Codigo);

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

        // Inactivar rubros por proyecto (cuando se inactiva un proyecto)
        public async Task<RubrosModel> InactivarRubrosPorProyecto(int IdProyecto)
        {
            RubrosModel model = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            using SqlCommand cmd = new("sp_inactivarRubrosPorProyecto", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@IdProyecto", IdProyecto);

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