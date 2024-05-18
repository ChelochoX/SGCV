﻿using Dapper;
using Persistence;
using sgcv_backend.Core.Application.Services.Interfaces.IRepository;
using sgcv_backend.Core.Domain.Entities;
using sgcv_backend.Core.Domain.Exceptions;
using sgcv_backend.Core.Domain.Request;
using sgcv_backend.Core.Domain.Response;

namespace sgcv_backend.Persistence.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly DbConnections _conexion;
        private readonly ILogger<ProductoRepository> _logger;

        public ProductoRepository(DbConnections conexion, ILogger<ProductoRepository> logger)
        {
            _conexion = conexion;
            _logger = logger;
        }

        public async Task<Datos<int>> InsertarDatosProducto(ProductoDatosRequest request)
        {
            _logger.LogInformation("Inicio de Proceso de insertar datos en la tabla de Productos");

            string query_UltimoNroCodigo = "SELECT ISNULL(MAX(codigo_nombre),0) FROM producto_nombre";

            string query_CheckExistenciaProducto = "SELECT COUNT(*) FROM producto_nombre WHERE nombre = @nombre";


            string query = @"
                            INSERT INTO producto_nombre
                               (codigo_producto,numeracion,nombre,descripcion,codigo_categoria,codigo_unidadmedida,codigo_precio)
                            VALUES
                            (@codigoProducto,@codigo,@nombre,@descripcion,@codigoCategoria,@codigoUnidadMedida,@codigoPrecio)";
            try
            {
                using (var connection = this._conexion.CreateSqlConnection())
                {

                    int existingCedulaCount = await connection.ExecuteScalarAsync<int>(query_CheckExistenciaProducto, new { cedula = request.Nombre });
                    if (existingCedulaCount > 0)
                    {
                        return new Datos<int>
                        {
                            Items = -1,
                            TotalRegistros = 0
                        };
                    }

                    var parametros = new DynamicParameters();

                    //Generamos los valores para registrar la tabla Clientes
                    int ultimoValorCodigo = await connection.ExecuteScalarAsync<int>(query_UltimoNroCodigo);
                    int nuevoCodigoSolicitud = ultimoValorCodigo + 1;
                    parametros.Add("@codigoProducto", nuevoCodigoSolicitud);
                    parametros.Add("@codigo", request.Codigo);
                    parametros.Add("@nombre", request.Nombre);
                    parametros.Add("@descripcion", request.Descripcion);
                    parametros.Add("@codigoCategoria", request.CodigoCategoria);
                    parametros.Add("@codigoUnidadMedida", request.CodigoUnidadMedida);
                    parametros.Add("@codigoPrecio", request.CodigoPrecio);

                    var resultado = await connection.ExecuteAsync(query, parametros);

                    var respuesta = new Datos<int>
                    {
                        Items = nuevoCodigoSolicitud,
                        TotalRegistros = 1
                    };

                    _logger.LogInformation("Fin de Proceso de insertar datos en la tabla de Productos");

                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                throw new ClientesException("Ocurrio un error al insertar los datos en la tabla Datos Nombre del Producto" + "||-->" + ex.Message + "<--||");
            }
        }

        public async Task<Datos<IEnumerable<ClienteDatosPersonalesResponse>>> ObtenerDatosdelProducto(ClienteDatosPersonalesObtenerRequest request)
        {
            _logger.LogInformation("Inicio de Proceso de Obtener Datos del Producto");

            int saltarRegistros = (request.Pagina - 1) * request.CantidadRegistros;
            var query = string.Empty;

            try
            {
                query = @"
                            SELECT s.cedula as Cedula
                              ,s.ruc as Ruc
                              ,s.nombres as Nombres
                              ,s.apellidos as Apellidos
                              ,s.telefono_movil as TelefonoMovil 
                              ,s.telefono_lineabaja as TelefonoLineaBaja
                              ,s.direccion_particular as DireccionParticular
                              ,s.numero_casa as NumeroCasa
                          FROM cliente_datospersonales s
                          WHERE codigo_cliente = @codigoCliente ";


                if (
                    !string.IsNullOrEmpty(request.Cedula) ||
                    !string.IsNullOrEmpty(request.Ruc) ||
                    !string.IsNullOrEmpty(request.Nombres) ||
                    !string.IsNullOrEmpty(request.Apellidos) ||
                    !string.IsNullOrEmpty(request.TelefonoMovil) ||
                    !string.IsNullOrEmpty(request.TelefonoLineaBaja) ||
                    !string.IsNullOrEmpty(request.DireccionParticular) ||
                    !string.IsNullOrEmpty(request.NumeroCasa.ToString()) ||
                    !string.IsNullOrEmpty(request.TerminoBusqueda)
                    )
                {
                    // Se proporcionaron parámetros de búsqueda, agregar filtros adicionales
                    if (!string.IsNullOrEmpty(request.TerminoBusqueda))
                    {
                        query += @"
                                    AND (                                        
                                        s.cedula LIKE '%' + @terminoDeBusqueda + '%'                       
                                        OR s.ruc LIKE '%' + @terminoDeBusqueda + '%'                                                            
                                        OR s.nombres LIKE '%' + @terminoDeBusqueda + '%'                                                               
                                        OR s.apellidos LIKE '%' + @terminoDeBusqueda + '%'                                                          
                                        OR s.telefono_movil LIKE '%' + @terminoDeBusqueda + '%'                                                                
                                        OR s.telefono_lineabaja LIKE '%' + @terminoDeBusqueda + '%' 
                                        OR s.direccion_particular LIKE '%' + @terminoDeBusqueda + '%'                                                   
                                        OR (CONVERT(VARCHAR, s.numero_casa) LIKE '%' + @terminoDeBusqueda + '%'
                                    )";
                    }
                    else
                    {
                        query += @"
                                     AND (@cedula IS NULL OR s.cedula LIKE '%' + @cedula + '%')
                                     AND (@ruc IS NULL OR s.ruc LIKE '%' + @ruc + '%')
                                     AND (@nombres IS NULL OR s.nombres LIKE '%' + @nombres + '%')
                                     AND (@apellidos IS NULL OR s.apellidos LIKE '%' + @apellidos + '%')
                                     AND (@movil IS NULL OR s.telefono_movil LIKE '%' + @movil + '%')
                                     AND (@lineabaja IS NULL OR s.telefono_lineabaja LIKE '%' + @lineabaja + '%')
                                     AND (@direccion IS NULL OR s.direccion_particular LIKE '%' + @direccion + '%')
                                     AND (@numerocasa IS NULL OR CONVERT(VARCHAR, s.numero_casa) LIKE '%' + @numerocasa + '%')";
                    }
                }

                query += @" ORDER BY s.cedula";
                query += @" OFFSET @saltarRegistros ROWS";
                query += @" FETCH NEXT @cantidadRegistros ROWS ONLY";


                string queryCantidadTotalRegistros = @"
                                SELECT COUNT(*) AS TotalRegistros 
                                FROM cliente_datospersonales s
                                WHERE codigo_cliente = @codigoCliente ";

                if (
                   !string.IsNullOrEmpty(request.Cedula) ||
                    !string.IsNullOrEmpty(request.Ruc) ||
                    !string.IsNullOrEmpty(request.Nombres) ||
                    !string.IsNullOrEmpty(request.Apellidos) ||
                    !string.IsNullOrEmpty(request.TelefonoMovil) ||
                    !string.IsNullOrEmpty(request.TelefonoLineaBaja) ||
                    !string.IsNullOrEmpty(request.DireccionParticular) ||
                    !string.IsNullOrEmpty(request.NumeroCasa.ToString()) ||
                    !string.IsNullOrEmpty(request.TerminoBusqueda)
                    )
                {
                    if (!string.IsNullOrEmpty(request.TerminoBusqueda))
                    {
                        queryCantidadTotalRegistros += @"
                                    AND (                                        
                                         s.cedula LIKE '%' + @terminoDeBusqueda + '%'                       
                                        OR s.ruc LIKE '%' + @terminoDeBusqueda + '%'                                                            
                                        OR s.nombres LIKE '%' + @terminoDeBusqueda + '%'                                                               
                                        OR s.apellidos LIKE '%' + @terminoDeBusqueda + '%'                                                          
                                        OR s.telefono_movil LIKE '%' + @terminoDeBusqueda + '%'                                                                
                                        OR s.telefono_lineabaja LIKE '%' + @terminoDeBusqueda + '%' 
                                        OR s.direccion_particular LIKE '%' + @terminoDeBusqueda + '%'                                                   
                                        OR (CONVERT(VARCHAR, s.numero_casa) LIKE '%' + @terminoDeBusqueda + '%'
                                    )";
                    }
                    else
                    {
                        queryCantidadTotalRegistros += @"
                                     AND (@cedula IS NULL OR s.cedula LIKE '%' + @cedula + '%')
                                     AND (@ruc IS NULL OR s.ruc LIKE '%' + @ruc + '%')
                                     AND (@nombres IS NULL OR s.nombres LIKE '%' + @nombres + '%')
                                     AND (@apellidos IS NULL OR s.apellidos LIKE '%' + @apellidos + '%')
                                     AND (@movil IS NULL OR s.telefono_movil LIKE '%' + @movil + '%')
                                     AND (@lineabaja IS NULL OR s.telefono_lineabaja LIKE '%' + @lineabaja + '%')
                                     AND (@direccion IS NULL OR s.direccion_particular LIKE '%' + @direccion + '%')
                                     AND (@numerocasa IS NULL OR CONVERT(VARCHAR, s.numero_casa) LIKE '%' + @numerocasa + '%')";
                    }
                }

                // Definición de parámetros
                var parametros = new DynamicParameters();

                parametros.Add("@codigoCliente", request.ParametroCodigoCliente);
                parametros.Add("@cedula", request.Cedula);
                parametros.Add("@ruc", request.Ruc);
                parametros.Add("@nombres", request.Nombres);
                parametros.Add("@apellidos", request.Apellidos);
                parametros.Add("@movil", request.TelefonoMovil);
                parametros.Add("@lineabaja", request.TelefonoLineaBaja);
                parametros.Add("@direccion", request.DireccionParticular);
                parametros.Add("@numerocasa", request.NumeroCasa);
                parametros.Add("@terminoDeBusqueda", $"%{request.TerminoBusqueda}%");
                parametros.Add("@saltarRegistros", saltarRegistros);
                parametros.Add("@cantidadRegistros", request.CantidadRegistros);


                using (var connection = this._conexion.CreateSqlConnection())
                {
                    var totalTegistros = await connection.ExecuteScalarAsync<int>(queryCantidadTotalRegistros, parametros);

                    var resultado = await connection.QueryAsync<ClienteDatosPersonalesResponse>(query, parametros);

                    var response = new Datos<IEnumerable<ClienteDatosPersonalesResponse>>
                    {
                        Items = resultado,
                        TotalRegistros = totalTegistros
                    };

                    _logger.LogInformation("Inicio de Proceso de Obtener Datos del Producto");
                    return response;
                }
            }

            catch (Exception ex)
            {
                throw new ClientesException("Ocurrió un error inesperado al realizar la Consulta de Datos Particulares de Clientes" + "||-->" + ex.Message + "<--||");
            }
        }

        public async Task<IEnumerable<CategoriasDatosResponse>> ObtenerDatosCategoria ()
        {
            _logger.LogInformation("Inicio de Proceso de Obtener Datos de Categorias");     
            
            try
            {
                var query = @"
                           SELECT codigo_categoria
                                ,nombre
                                ,descripcion
                            FROM categoria";         

                using (var connection = this._conexion.CreateSqlConnection())
                {                
                    var resultado = await connection.QueryAsync<CategoriasDatosResponse>(query);                

                    _logger.LogInformation("Fin de Proceso de Obtener Datos de Categorias");
                    return resultado.ToList();
                }
            }

            catch (Exception ex)
            {
                throw new ClientesException("Ocurrió un error inesperado al realizar la Consulta de Datos de Categorias" + "||-->" + ex.Message + "<--||");
            }
        }

        public async Task<IEnumerable<UnidadMedidaResponse>> ObtenerDatosUnidadMedida()
        {
            _logger.LogInformation("Inicio de Proceso de Obtener Datos de Unidades de Medida");

            try
            {
                var query = @"
                           SELECT codigo_unidadmedida
                              ,siglas
                              ,nombre
                              ,descripcion
                          FROM unidad_medida";

                using (var connection = this._conexion.CreateSqlConnection())
                {
                    var resultado = await connection.QueryAsync<UnidadMedidaResponse>(query);

                    _logger.LogInformation("Fin de Proceso de Obtener Datos de Unidades de Medida");
                    return resultado.ToList();
                }
            }

            catch (Exception ex)
            {
                throw new ClientesException("Ocurrió un error inesperado al realizar la Consulta de Datos de Unidades de Medida" + "||-->" + ex.Message + "<--||");
            }
        }

        public async Task<Datos<int>> InsertarPreciosProducto(PrecioProductoRequest request)
        {
            _logger.LogInformation("Inicio de Proceso de insertar datos en la tabla de Precios");

            string query_UltimoNroCodigo = "SELECT ISNULL(MAX(codigo_precio),0) FROM producto_precio";         

            string query = @"
                            INSERT INTO producto_precio
                               (codigo_precio,lista,compra,venta,iva10,iva5,exenta,estado,observacion,codigo_producto)
                            VALUES
                               (@codigoprecio,@lista,@compra,@venta,@iva10,@iva5,@exenta,@estado,@observacion,@codigoProducto)";
            try
            {
                using (var connection = this._conexion.CreateSqlConnection())
                {                 
                    var parametros = new DynamicParameters();

                    //Generamos los valores para registrar la tabla Clientes
                    int ultimoValorCodigo = await connection.ExecuteScalarAsync<int>(query_UltimoNroCodigo);
                    int nuevoCodigoSolicitud = ultimoValorCodigo + 1;
                    parametros.Add("@codigoprecio", nuevoCodigoSolicitud);
                    parametros.Add("@lista", request.Lista);
                    parametros.Add("@compra", request.Compra);
                    parametros.Add("@venta", request.Venta);
                    parametros.Add("@iva10", request.Iva10);
                    parametros.Add("@iva5", request.Iva5);
                    parametros.Add("@exenta", request.Exenta);
                    parametros.Add("@estado", request.Estado);
                    parametros.Add("@observacion", request.Observacion);
                    parametros.Add("@codigoProducto", request.CodigoProducto);

                    var resultado = await connection.ExecuteAsync(query, parametros);

                    var respuesta = new Datos<int>
                    {
                        Items = nuevoCodigoSolicitud,
                        TotalRegistros = 1
                    };

                    _logger.LogInformation("Fin de Proceso de insertar datos en la tabla de Precios");

                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                throw new ClientesException("Ocurrio un error al insertar los datos en la tabla Precio Producto" + "||-->" + ex.Message + "<--||");
            }
        }

    }
}
