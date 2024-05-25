using Dapper;
using Persistence;
using sgcv_backend.Core.Application.Services.Interfaces.IRepository;
using sgcv_backend.Core.Domain.Entities;
using sgcv_backend.Core.Domain.Exceptions;
using sgcv_backend.Core.Domain.Request;
using sgcv_backend.Core.Domain.Response;

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

            string query_CheckExistenciaProducto = "SELECT COUNT(*) FROM proveedor WHERE ruc = ruc";


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

        public async Task<Datos<IEnumerable<ProveedorDatosResponse>>> ObtenerDatosdelProveedor(ProveedorObtenerDatosRequest request)
        {
            _logger.LogInformation("Inicio de Proceso de Obtener Datos del Proveedor");

            int saltarRegistros = (request.Pagina - 1) * request.CantidadRegistros;
            var query = string.Empty;

            try
            {
                query = @"
                           SELECT pr.codigo_proveedor as CodigoProveedor
                                ,pr.ruc as Ruc
                                ,pr.razon_social as RazonSocial
                                ,pr.direccion as Direccion
                                ,pr.telefono as Telefono
                            FROM proveedor pr 
                            WHERE 1 = 1 ";

                if (
                    !string.IsNullOrEmpty(request.Ruc) ||
                    !string.IsNullOrEmpty(request.RazonSocial) ||
                    !string.IsNullOrEmpty(request.Direccion) ||
                    !string.IsNullOrEmpty(request.Telefono)
                    )
                {
                    
                    if (!string.IsNullOrEmpty(request.TerminoBusqueda))
                    {
                        query += @"
                           AND (
                                pr.ruc LIKE '%' + @terminoDeBusqueda + '%'                       
                                OR pr.razon_social LIKE '%' + @terminoDeBusqueda + '%'                                                            
                                OR pr.direccion LIKE '%' + @terminoDeBusqueda + '%'                                                               
                                OR pr.telefono LIKE '%' + @terminoDeBusqueda + '%'
                            )";
                    }
                    else
                    {
                        query += @"
                                    AND (@ruc IS NULL OR pr.ruc LIKE '%' + @ruc + '%')
                                    AND (@razonSocial IS NULL OR pr.razon_social LIKE '%' + @razonSocial + '%')
                                    AND (@direccion IS NULL OR pr.direccion LIKE '%' + @direccion + '%')
                                    AND (@telefono IS NULL OR pr.telefono LIKE '%' + @telefono + '%')";

                    }
                }

                query += @" ORDER BY pr.codigo_proveedor";
                query += @" OFFSET @saltarRegistros ROWS";
                query += @" FETCH NEXT @cantidadRegistros ROWS ONLY";


                string queryCantidadTotalRegistros = @"
                                SELECT COUNT(*) AS TotalRegistros 
                                FROM proveedor pr
                                WHERE 1 = 1 ";

                if (
                    !string.IsNullOrEmpty(request.Ruc) ||
                    !string.IsNullOrEmpty(request.RazonSocial) ||
                    !string.IsNullOrEmpty(request.Direccion) ||
                    !string.IsNullOrEmpty(request.Telefono)
                    )
                {
                    if (!string.IsNullOrEmpty(request.TerminoBusqueda))
                    {
                        queryCantidadTotalRegistros += @"
                             AND (
                                pr.ruc LIKE '%' + @terminoDeBusqueda + '%'                       
                                OR pr.razon_social LIKE '%' + @terminoDeBusqueda + '%'                                                            
                                OR pr.direccion LIKE '%' + @terminoDeBusqueda + '%'                                                               
                                OR pr.telefono LIKE '%' + @terminoDeBusqueda + '%'
                            )";
                    }
                    else
                    {
                        queryCantidadTotalRegistros += @"
                              AND (@ruc IS NULL OR pr.ruc LIKE '%' + @ruc + '%')
                                    AND (@razonSocial IS NULL OR pr.razon_social LIKE '%' + @razonSocial + '%')
                                    AND (@direccion IS NULL OR pr.direccion LIKE '%' + @direccion + '%')
                                    AND (@telefono IS NULL OR pr.telefono LIKE '%' + @telefono + '%')";
                    }
                }

                // Definición de parámetros
                var parametros = new DynamicParameters();

                parametros.Add("@ruc", request.Ruc);
                parametros.Add("@razonSocial", request.RazonSocial);
                parametros.Add("@direccion", request.Direccion);
                parametros.Add("@telefono", request.Telefono);

                parametros.Add("@terminoDeBusqueda", $"%{request.TerminoBusqueda}%");
                parametros.Add("@saltarRegistros", saltarRegistros);
                parametros.Add("@cantidadRegistros", request.CantidadRegistros);


                using (var connection = this._conexion.CreateSqlConnection())
                {
                    var totalRegistros = await connection.ExecuteScalarAsync<int>(queryCantidadTotalRegistros, parametros);

                    var resultado = await connection.QueryAsync<ProveedorDatosResponse>(query, parametros);

                    var response = new Datos<IEnumerable<ProveedorDatosResponse>>
                    {
                        Items = resultado,
                        TotalRegistros = totalRegistros
                    };

                    _logger.LogInformation("Fin de Proceso de Obtener Datos del Proveedor");

                    return response;
                }
            }

            catch (Exception ex)
            {
                throw new ClientesException("Ocurrió un error inesperado al realizar la Consulta de Datos del proveedor" + "||-->" + ex.Message + "<--||");
            }
        }

        public async Task<int> ActualizarDatosProveedor(ProveedorDatosActualizarRequest request)
        {
            _logger.LogInformation("Inicio del Proceso de actualizar datos del proveedor");

            string query = @"UPDATE proveedor
                               SET 
                                  ruc = @ruc
                                  ,razon_social = @razonsocial
                                  ,direccion = @direccion
                                  ,telefono = @telefono
                             WHERE codigo_proveedor = @codigoproveedor";
            

            var parametros = new DynamicParameters();

            parametros.Add("@codigoproveedor", request.CodigoProveedor);
            parametros.Add("@ruc", request.Ruc);
            parametros.Add("@razonsocial", request.RazonSocial);
            parametros.Add("@direccion", request.Direccion);
            parametros.Add("@telefono", request.Telefono);

            try
            {
                using (var connection = this._conexion.CreateSqlConnection())
                {               
                    var resultado = await connection.ExecuteAsync(query, parametros);

                    _logger.LogInformation("Fin del Proceso de actualizar datos del proveedor");
                    return resultado;
                }
            }
            catch (Exception ex)
            {
                throw new ClientesException("Ocurrió un error al modificar los datos particulares del cliente" + "||-->" + ex.Message + "<--||");
            }
        }
    }
}
