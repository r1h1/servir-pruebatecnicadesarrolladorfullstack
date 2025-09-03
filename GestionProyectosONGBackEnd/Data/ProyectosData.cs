using GestionProyectosONGBackEnd.Models;
using System.Data;
using System.Data.SqlClient;

namespace GestionProyectosONGBackEnd.Data
{
    public class ProyectosData
    {
        private readonly string connection;
        public ProyectosData(IConfiguration configuration)
        {
            connection = configuration.GetConnectionString("ConexionRemota");
        }

        // Listar todos los proyectos
        public async Task<List<ProyectosModel>> ListarProyectos(bool Active = true)
        {
            List<ProyectosModel> lista = new();

            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_listarProyectos", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Active", Active);

            using var reader = await cmd.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                lista.Add(new ProyectosModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Codigo = reader["Codigo"].ToString() ?? "",
                    Nombre = reader["Nombre"].ToString() ?? "",
                    Municipio = reader["Municipio"].ToString() ?? "",
                    Departamento = reader["Departamento"].ToString() ?? "",
                    FechaInicio = Convert.ToDateTime(reader["FechaInicio"]),
                    FechaFin = Convert.ToDateTime(reader["FechaFin"])
                });
            }
            return lista;
        }

        // Listar proyecto por codigo
        public async Task<ProyectosModel> ListarProyectoPorCodigo(string Codigo)
        {
            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_listarProyectosPorCodigo", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Codigo", Codigo); 

            using var reader = await cmd.ExecuteReaderAsync();
            if(await reader.ReadAsync())
            {
                return new ProyectosModel
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Codigo = reader["Codigo"].ToString() ?? "",
                    Nombre = reader["Nombre"].ToString() ?? "",
                    Municipio = reader["Municipio"].ToString() ?? "",
                    Departamento = reader["Departamento"].ToString() ?? "",
                    FechaInicio = Convert.ToDateTime(reader["FechaInicio"]),
                    FechaFin = Convert.ToDateTime(reader["FechaFin"])
                };
            }
            return null;
        }

        // Crear nuevo proyecto
        // Crear nuevo proyecto
        public async Task<ProyectosModel> CrearProyecto(ProyectosModel model)
        {
            using SqlConnection con = new(connection);
            await con.OpenAsync();

            SqlCommand cmd = new("sp_crearProyecto", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
            cmd.Parameters.AddWithValue("@Municipio", model.Municipio);
            cmd.Parameters.AddWithValue("@Departamento", model.Departamento);
            cmd.Parameters.AddWithValue("@FechaInicio", model.FechaInicio);
            cmd.Parameters.AddWithValue("@FechaFin", model.FechaFin);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                model.Success = reader.GetBoolean(0) ? 1 : 0;
                model.Message = reader.GetString(1);

                // Si fue exitoso, cargar todos los datos del proyecto
                if (model.Success == 1)
                {
                    model.Id = Convert.ToInt32(reader["Id"]);
                    model.Codigo = reader["Codigo"].ToString();
                    model.Nombre = reader["Nombre"].ToString() ?? "";
                    model.Municipio = reader["Municipio"].ToString() ?? "";
                    model.Departamento = reader["Departamento"].ToString() ?? "";
                    model.FechaInicio = Convert.ToDateTime(reader["FechaInicio"]);
                    model.FechaFin = Convert.ToDateTime(reader["FechaFin"]);
                }
            }
            return model;
        }

        // Actualizar proyecto
        public async Task<ProyectosModel> ActualizarProyecto(ProyectosModel model)
        {
            using SqlConnection con = new(connection);
            await con.OpenAsync();
            SqlCommand cmd = new("sp_editarProyecto", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Codigo", model.Codigo);
            cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
            cmd.Parameters.AddWithValue("@Municipio", model.Municipio);
            cmd.Parameters.AddWithValue("@Departamento", model.Departamento);
            cmd.Parameters.AddWithValue("@FechaInicio", model.FechaInicio);
            cmd.Parameters.AddWithValue("@FechaFin", model.FechaFin);

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                model.Success = reader.GetBoolean(0) ? 1 : 0;
                model.Message = reader.GetString(1);

                // Si fue exitoso, cargar todos los datos actualizados
                if (model.Success == 1)
                {
                    model.Id = Convert.ToInt32(reader["Id"]);
                    model.Codigo = reader["Codigo"].ToString();
                    model.Nombre = reader["Nombre"].ToString() ?? "";
                    model.Municipio = reader["Municipio"].ToString() ?? "";
                    model.Departamento = reader["Departamento"].ToString() ?? "";
                    model.FechaInicio = Convert.ToDateTime(reader["FechaInicio"]);
                    model.FechaFin = Convert.ToDateTime(reader["FechaFin"]);
                }
            }
            return model;
        }

        // Eliminar proyecto
        public async Task<ProyectosModel> EliminarProyecto(string Codigo)
        {
            ProyectosModel model = new();
            using SqlConnection con = new(connection);
            await con.OpenAsync();
            SqlCommand cmd = new("sp_eliminarProyecto", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Codigo", Codigo);
            using var reader = await cmd.ExecuteReaderAsync();
            if(await reader.ReadAsync())
            {
                model.Success = reader.GetBoolean(0) ? 1 : 0;
                model.Message = reader.GetString(1);
            }
            return model;
        }
    }
}
