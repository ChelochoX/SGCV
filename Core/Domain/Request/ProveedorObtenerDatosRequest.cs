namespace sgcv_backend.Core.Domain.Request
{
    public class ProveedorObtenerDatosRequest
    {
        public string? Ruc { get; set; }
        public string? RazonSocial { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }

        public string? TerminoBusqueda { get; set; }
        public int Pagina { get; set; }
        public int CantidadRegistros { get; set; }
    }
}
