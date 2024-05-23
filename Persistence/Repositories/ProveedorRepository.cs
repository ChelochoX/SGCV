using Dapper;
using Persistence;
using sgcv_backend.Core.Application.Services.Interfaces.IRepository;
using sgcv_backend.Core.Domain.Entities;
using sgcv_backend.Core.Domain.Exceptions;
using sgcv_backend.Core.Domain.Request;

namespace sgcv_backend.Persistence.Repositories
{
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly DbConnections _conexion;
        private readonly ILogger<ProveedorRepository> _logger;
        public ProveedorRepository(DbConnections conexion, ILogger<ProveedorRepository> logger)
        {
            _conexion = conexion;
            _logger = logger;
        }

        public async Task<Datos<int>> InsertarDatosProveedor(ProveedorDatosRequest request)
        {
            _logger.LogInformation("Inicio de Proceso de insertar datos en la tabla de Proveedor");

            string query_UltimoNroCodigo = "SELECT ISNULL(MAX(codigo_proveedor),0) FROM proveedor";

            string query_CheckExistenciaProducto = "SELECT COUNT(*) FROM producto_nombre WHERE razon_social = nombre";


            string query = @"
                            INSERT INTO proveedor
                               (codigo_proveedor,ruc,razon_social,direccion,telefono)
                            VALUES
                               (@codigoproveedor,@ruc,@razonsocial,@direccion,@telefono)";
            try
            {
                using (var connection = this._conexion.CreateSqlConnection())
                {

                    int existingCedulaCount = await connection.ExecuteScalarAsync<int>(query_CheckExistenciaProducto, new { ruc = request.Ruc });
                    if (existingCedulaCount > 0)
                    {
                        return new Datos<int>
                        {
                            Items = -1,
                            TotalRegistros = 0
                        };
                    }

                    var parametros = new DynamicParameters();
                   
                    int ultimoValorCodigo = await connection.ExecuteScalarAsync<int>(query_UltimoNroCodigo);
                    int nuevoCodigoSolicitud = ultimoValorCodigo + 1;
                    parametros.Add("@codigoproveedor", nuevoCodigoSolicitud);
                    parametros.Add("@ruc", request.Ruc);
                    parametros.Add("@razonsocial", request.RazonSocial);
                    parametros.Add("@direccion", request.Direccion);
                    parametros.Add("@telefono", request.Telefono);

                    var resultado = await connection.ExecuteAsync(query, parametros);

                    var respuesta = new Datos<int>
                    {
                        Items = nuevoCodigoSolicitud,
                        TotalRegistros = 1
                    };

                    _logger.LogInformation("Fin de Proceso de insertar datos en la tabla de Proveedor");

                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                throw new ClientesException("Ocurrio un error al insertar los datos en la tabla Proveedor" + "||-->" + ex.Message + "<--||");
            }
        }
    }
}
