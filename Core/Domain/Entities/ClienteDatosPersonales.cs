namespace sgcv_backend.Core.Domain.Entities;

public class ClienteDatosPersonales
{
    public int CodigoCliente { get; set; }
    public string Cedula { get; set; }
    public string Ruc { get; set; }
    public string Nombres { get; set; }
    public string Apellidos { get; set; }
    public string TelefonoMovil { get; set; }
    public string TelefonoLineaBaja { get; set; }
    public string DireccionParticular { get; set; }
    public string NumeroCasa { get; set; }
}
