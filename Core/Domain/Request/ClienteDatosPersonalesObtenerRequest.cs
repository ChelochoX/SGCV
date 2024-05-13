using FluentValidation;

namespace sgcv_backend.Core.Domain.Request;

public class ClienteDatosPersonalesObtenerRequest
{    
    public string? Cedula { get; set; }
    public string? Ruc { get; set; }
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public string? TelefonoMovil { get; set; }
    public string? TelefonoLineaBaja { get; set; }
    public string? DireccionParticular { get; set; }
    public int? NumeroCasa { get; set; }

    public string? TerminoBusqueda { get; set; }
    public int ParametroCodigoCliente { get; set; }
    public int Pagina { get; set; }
    public int CantidadRegistros { get; set; }
}

public class ClienteDatosPersonalesObtenerRequestValidator : AbstractValidator<ClienteDatosPersonalesObtenerRequest>
{
    public ClienteDatosPersonalesObtenerRequestValidator()
    {
        RuleFor(cliente => cliente.Cedula)           
               .Matches("^[0-9]*$").WithMessage("La cédula solo puede contener números.");

        RuleFor(cliente => cliente.Ruc) 
            .Matches("^[0-9-]*$").When(cliente => cliente.Ruc != null)
                .WithMessage("El RUC solo puede contener números y el guión '-'.")
            .Must(ruc => ruc == null || ruc.Contains('-') || ruc.All(char.IsDigit))
                .WithMessage("El RUC debe contener un guión '-' si está presente.")
                .When(cliente => cliente.Ruc != null);

        RuleFor(cliente => cliente.Nombres)            
              .Matches("^[A-Za-z ]+$").WithMessage("Los nombres solo pueden contener letras y espacios.");

        RuleFor(cliente => cliente.Apellidos)            
            .Matches("^[A-Za-z ]+$").WithMessage("Los apellidos solo pueden contener letras y espacios.");


        RuleFor(cliente => cliente.DireccionParticular)            
            .Matches("^[A-Za-z0-9\\s.,/-]*$").WithMessage("La dirección particular solo puede contener letras, números, espacios, puntos, comas, guiones, barras y guiones bajos.");

        RuleFor(cliente => cliente.TelefonoMovil)
            .Matches(@"^\d{4}-\d{6}$").When(x => !string.IsNullOrEmpty(x.TelefonoMovil))
            .WithMessage("El teléfono móvil debe tener el formato correcto, por ejemplo, 0981-123456.")
            .Matches("^[0-9-]*$").When(x => !string.IsNullOrEmpty(x.TelefonoMovil))
            .WithMessage("El teléfono móvil solo puede contener números y el guión '-'.");

        RuleFor(cliente => cliente.TelefonoLineaBaja)          
            .Matches(@"^\d{1,4}-\d{6}$").When(x => !string.IsNullOrEmpty(x.TelefonoLineaBaja))
            .WithMessage("El teléfono de línea baja debe tener el formato correcto, por ejemplo, 0021-123456.")
            .Matches("^[0-9-]*$").When(x => !string.IsNullOrEmpty(x.TelefonoLineaBaja))
            .WithMessage("El teléfono de línea baja solo puede contener números y el guión '-'.");

        RuleFor(cliente => cliente.NumeroCasa)
           .Must(numCasa => numCasa.Value.ToString().All(char.IsDigit))
           .When(cliente => cliente.NumeroCasa.HasValue)
           .WithMessage("El número de casa debe contener solo números.");

        RuleFor(cliente => cliente.TerminoBusqueda)
          .Matches("^[A-Za-z ]+$").WithMessage("Los apellidos solo pueden contener letras y espacios.");

        RuleFor(cliente => cliente.ParametroCodigoCliente)
            .NotEmpty().WithMessage("El parámetro de código de cliente es requerido.")
            .GreaterThan(0).WithMessage("El parámetro de código de cliente debe ser mayor que cero.");

        RuleFor(cliente => cliente.Pagina)
            .NotEmpty().WithMessage("La página es requerida.")
            .GreaterThan(0).WithMessage("La página debe ser mayor que cero.");

        RuleFor(cliente => cliente.CantidadRegistros)
            .NotEmpty().WithMessage("La cantidad de registros es requerida.")
            .GreaterThan(0).WithMessage("La cantidad de registros debe ser mayor que cero.");

    }
}
