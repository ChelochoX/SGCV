using Dapper;
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

            string query_UltimoNroCodigo = "SELECT ISNULL(MAX(codigo_producto),0) FROM producto_nombre";

            string query_CheckExistenciaProducto = "SELECT COUNT(*) FROM producto_nombre WHERE nombre = nombre";


            string query = @"
                            INSERT INTO producto_nombre
                               (codigo_producto,codigo,nombre,descripcion,codigo_categoria,codigo_unidadmedida)
                            VALUES
                            (@codigoProducto,@codigo,@nombre,@descripcion,@codigoCategoria,@codigoUnidadMedida)";
            try
            {
                using (var connection = this._conexion.CreateSqlConnection())
                {

                    int existingCedulaCount = await connection.ExecuteScalarAsync<int>(query_CheckExistenciaProducto, new { nombre = request.Nombre });
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

        public async Task<Datos<IEnumerable<ProductoConPrecioResponse>>> ObtenerDatosdelProducto(ProductoObtenerDatosRequest request)
        {
            _logger.LogInformation("Inicio de Proceso de Obtener Datos del Producto");

            int saltarRegistros = (request.Pagina - 1) * request.CantidadRegistros;
            var query = string.Empty;

            try
            {
                query = @"
                           SELECT pr.codigo_producto as IDProducto
                              ,pr.codigo as CodigoProducto
                              ,pr.nombre as NombreProducto
                              ,pr.descripcion as DescripcionProducto
                              ,pr.codigo_categoria as CodigoCategoria
	                          ,c.nombre as NombreCategoria
                              ,pr.codigo_unidadmedida as CodigoUnidadMedida
	                          ,u.siglas as SiglasUnidadMedida
	                          ,u.nombre as NombreUnidadMedida
                              ,pc.lista as ListaPrecio,
	                         	pc.codigo_precio as CodigoPrecio,
                                pc.compra as PrecioCompra,
                                pc.venta as PrecioVenta,
                                pc.iva10 as Iva10,
                                pc.iva5 as Iva5,
                                pc.exenta as Exenta,
                                pc.estado as EstadoPrecio,
	                            pc.observacion as Observacion
                          FROM producto_nombre pr 
                          JOIN producto_precio pc ON pr.codigo_producto = pc.codigo_producto
                          JOIN categoria c ON PR.codigo_categoria = c.codigo_categoria
                          JOIN unidad_medida u ON pr.codigo_unidadmedida = u.codigo_unidadmedida ";

                if (
                    !string.IsNullOrEmpty(request.CodigoProducto) ||
                    !string.IsNullOrEmpty(request.NombreProducto) ||
                    !string.IsNullOrEmpty(request.DescripcionProducto) ||
                    !string.IsNullOrEmpty(request.NombreCategoria)
                    )
                {
                    // Se proporcionaron parámetros de búsqueda, agregar filtros adicionales
                    if (!string.IsNullOrEmpty(request.TerminoBusqueda))
                    {
                        query += @"
                            WHERE pr.codigo LIKE '%' + @terminoDeBusqueda + '%'                       
                            OR pr.nombre LIKE '%' + @terminoDeBusqueda + '%'                                                            
                            OR pr.descripcion LIKE '%' + @terminoDeBusqueda + '%'                                                               
                            OR c.nombre LIKE '%' + @terminoDeBusqueda + '%'";
                    }
                    else
                    {
                        query += @"
                            WHERE @codigoProducto IS NULL OR pr.codigo LIKE '%' + @codigoProducto + '%'
                            AND (@nombreProducto IS NULL OR pr.nombre LIKE '%' + @nombreProducto + '%')
                            AND (@descripcionProducto IS NULL OR pr.descripcion LIKE '%' + @descripcionProducto + '%')
                            AND (@nombreCategoria IS NULL OR c.nombre LIKE '%' + @nombreCategoria + '%')";
                    }
                }

                query += @" ORDER BY pr.codigo_producto";
                query += @" OFFSET @saltarRegistros ROWS";
                query += @" FETCH NEXT @cantidadRegistros ROWS ONLY";


                string queryCantidadTotalRegistros = @"
                                SELECT COUNT(*) AS TotalRegistros 
                                FROM producto_nombre pr 
                          JOIN producto_precio pc ON pr.codigo_producto = pc.codigo_producto
                          JOIN categoria c ON PR.codigo_categoria = c.codigo_categoria
                          JOIN unidad_medida u ON pr.codigo_unidadmedida = u.codigo_unidadmedida  ";

                if (
                    !string.IsNullOrEmpty(request.CodigoProducto) ||
                    !string.IsNullOrEmpty(request.NombreProducto) ||
                    !string.IsNullOrEmpty(request.DescripcionProducto) ||
                    !string.IsNullOrEmpty(request.NombreCategoria)
                    )
                {
                    if (!string.IsNullOrEmpty(request.TerminoBusqueda))
                    {
                        queryCantidadTotalRegistros += @"
                            WHERE pr.codigo LIKE '%' + @terminoDeBusqueda + '%'                       
                            OR pr.nombre LIKE '%' + @terminoDeBusqueda + '%'                                                            
                            OR pr.descripcion LIKE '%' + @terminoDeBusqueda + '%'                                                               
                            OR c.nombre LIKE '%' + @terminoDeBusqueda + '%'";
                    }
                    else
                    {
                        queryCantidadTotalRegistros += @"
                             WHERE @codigoProducto IS NULL OR pr.codigo LIKE '%' + @codigoProducto + '%'
                            AND (@nombreProducto IS NULL OR pr.nombre LIKE '%' + @nombreProducto + '%')
                            AND (@descripcionProducto IS NULL OR pr.descripcion LIKE '%' + @descripcionProducto + '%')
                            AND (@nombreCategoria IS NULL OR c.nombre LIKE '%' + @nombreCategoria + '%')";
                    }
                }

                // Definición de parámetros
                var parametros = new DynamicParameters();

                parametros.Add("@codigoProducto", request.CodigoProducto);
                parametros.Add("@nombreProducto", request.NombreProducto);
                parametros.Add("@descripcionProducto", request.DescripcionProducto);
                parametros.Add("@nombreCategoria", request.NombreCategoria);

                parametros.Add("@terminoDeBusqueda", $"%{request.TerminoBusqueda}%");
                parametros.Add("@saltarRegistros", saltarRegistros);
                parametros.Add("@cantidadRegistros", request.CantidadRegistros);


                using (var connection = this._conexion.CreateSqlConnection())
                {
                    var totalRegistros = await connection.ExecuteScalarAsync<int>(queryCantidadTotalRegistros, parametros);

                    var lookup = new Dictionary<int, ProductoConPrecioResponse>();

                    await connection.QueryAsync<ProductoConPrecioResponse, Categoria, UnidadMedida, Precio, ProductoConPrecioResponse>(
                        query,
                        (producto, categoria, unidadMedida, precio) =>
                        {
                            if (!lookup.TryGetValue(producto.IDProducto, out var productoEntry))
                            {
                                productoEntry = producto;
                                productoEntry.Categoria = categoria;
                                productoEntry.UnidadMedida = unidadMedida;
                                productoEntry.Precios = new List<Precio>();
                                lookup.Add(producto.IDProducto, productoEntry);
                            }
                            productoEntry.Precios.Add(precio);
                            return productoEntry;
                        },
                        parametros,
                        splitOn: "CodigoCategoria,CodigoUnidadMedida,ListaPrecio"
                    );

                    var response = new Datos<IEnumerable<ProductoConPrecioResponse>>
                    {
                        Items = lookup.Values,
                        TotalRegistros = totalRegistros
                    };

                    _logger.LogInformation("Inicio de Proceso de Obtener Datos del Producto");
                    return response;
                }
            }

            catch (Exception ex)
            {
                throw new ClientesException("Ocurrió un error inesperado al realizar la Consulta de Datos del producto" + "||-->" + ex.Message + "<--||");
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

        public async Task<int> ActualizarDatosdelProducto(ProductoDatosActualizarRequest request)
        {
            _logger.LogInformation("Inicio del Proceso de actualizar datos del Producto");

            string query = @"UPDATE producto_nombre
                               SET codigo = @codigo,
                                   nombre = @nombre,
                                   descripcion = @descripcion, 
                                   codigo_categoria = @codigo_categoria,
                                   codigo_unidadmedida = @codigo_unidadmedida
                             WHERE codigo_producto = @codigo_producto";
            
            var parametros = new DynamicParameters();

            parametros.Add("@codigo", request.Codigo);
            parametros.Add("@nombre", request.Nombre);
            parametros.Add("@descripcion", request.Descripcion);
            parametros.Add("@codigo_categoria", request.CodigoCategoria);
            parametros.Add("@codigo_unidadmedida", request.CodigoUnidadMedida);
            parametros.Add("@codigo_producto", request.CodigoProducto);

            try
            {
                using (var connection = this._conexion.CreateSqlConnection())
                {              
                    var resultado = await connection.ExecuteAsync(query, parametros);

                    _logger.LogInformation("Fin del Proceso de actualizar datos del Producto");
                    return resultado;
                }
            }
            catch (Exception ex)
            {
                throw new ClientesException("Ocurrió un error al modificar los datos del producto" + "||-->" + ex.Message + "<--||");
            }
        }

        public async Task<int> ActualizarDatosdelPrecioProducto(PrecioProductoActualizarRequest request)
        {
            _logger.LogInformation("Inicio del Proceso de actualizar datos del Percio del Producto");

            string query = @"UPDATE producto_precio
                           SET 
                               lista = @lista
                              ,compra = @compra
                              ,venta = @venta
                              ,iva10 = @iva10
                              ,iva5 = @iva5
                              ,exenta = @exenta
                              ,estado = @estado
                              ,observacion = @observacion
     
                         WHERE codigo_precio = @codigoprecio
                         AND  codigo_producto = @codigoproducto";

            var parametros = new DynamicParameters();

            parametros.Add("@lista", request.Lista);
            parametros.Add("@compra", request.Compra);
            parametros.Add("@venta", request.Venta);
            parametros.Add("@iva10", request.Iva10);
            parametros.Add("@iva5", request.Iva5);
            parametros.Add("@exenta", request.Exenta);
            parametros.Add("@estado", request.Estado);
            parametros.Add("@observacion", request.Observacion);
            parametros.Add("@codigoproducto", request.CodigoProducto);
            parametros.Add("@codigoprecio", request.CodigoPrecio);

            try
            {
                using (var connection = this._conexion.CreateSqlConnection())
                {
                    var resultado = await connection.ExecuteAsync(query, parametros);

                    _logger.LogInformation("Inicio del Proceso de actualizar datos del Percio del Producto");
                    return resultado;
                }
            }
            catch (Exception ex)
            {
                throw new ClientesException("Ocurrió un error al modificar los datos del precio del producto" + "||-->" + ex.Message + "<--||");
            }
        }

    }
}
