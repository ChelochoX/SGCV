using Persistence;
using sgcv_backend.Core.Application.Services.Interfaces;
using sgcv_backend.Core.Domain.Request;

namespace sgcv_backend.Persistence.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly DbConnections _conexion;
        private readonly ILogger<ClienteRepository> _logger;

        public ClienteRepository(DbConnections conexion, ILogger<ClienteRepository> logger)
        {
            _conexion = conexion;
            _logger = logger;
        }

        #region CLIENTES

        public async Task<Datos<int>> InsertarSolicitudBienesCircunscripcion(ClienteDatosPersonalesRequest request)
        {
            _logger.LogInformation("Inicio de Proceso de insertar datos en la tabla de Clientes");

            string query_UltimoNroCodigo = "SELECT ISNULL(MAX(codigo_cliente),0) FROM [cliente_datospersonales]";    

            string query = @"
                            INSERT INTO cliente_datospersonales
                           (codigo_cliente,cedula,ruc,nombres,apellidos,telefono_movil,telefono_lineabaja,direccion_particular,numero_casa)
                            VALUES
                            ()";

            try
            {
                using (var connection = this._conexion.CreateSqlConnectionCSJ())
                {
                    int ultimoValorCodigo = await connection.ExecuteScalarAsync<int>(queryUltimoValorCodigo);
                    int nuevoCodigoSolicitud = ultimoValorCodigo + 1;

                    int ultimoValorSecuencia = await connection.ExecuteScalarAsync<int>(queryUltimoValorSecuenciaSolicitud);
                    int nuevoCodigoSecuencia = ultimoValorSecuencia + 1;

                    solicitudBienes.CodigoSolicitud = nuevoCodigoSolicitud;
                    solicitudBienes.NumeroSolicitud = nuevoCodigoSecuencia;
                    solicitudBienes.Estado = 1; // Estado Abierto

                    var resultado = await connection.ExecuteAsync(query, solicitudBienes);


                    var listado = new Datos<int>
                    {
                        Items = nuevoCodigoSolicitud,
                        TotalRegistros = 1
                    };

                    _logger.LogInformation("Fin de Proceso de insertar solicitud bienes circunscripcion {@solicitud}", solicitudBienes);

                    return listado;
                }
            }
            catch (Exception ex)
            {
                throw new GeneracionSolicitudesException("Ocurrio un error al insertar los datos en la tabla Solicitud Bienes Circunscripcion" + "||-->" + ex.Message + "<--||");
            }
        }




        #endregion


    }
}
