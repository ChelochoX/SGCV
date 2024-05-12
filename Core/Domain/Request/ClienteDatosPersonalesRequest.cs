using FluentValidation;

namespace sgcv_backend.Core.Domain.Request;

public class ClienteDatosPersonalesRequest
{    
    public string Cedula { get; set; }
    public string Ruc { get; set; }
    public string Nombres { get; set; }
    public string Apellidos { get; set; }
    public string TelefonoMovil { get; set; }
    public string TelefonoLineaBaja { get; set; }
    public string DireccionParticular { get; set; }
    public string NumeroCasa { get; set; }

    public int ParametroCodigoCliente { get; set; }
    public int Pagina { get; set; }
    public int CantidadRegistros { get; set; }
}

public class ClienteDatosPersonalesRequestValidator : AbstractValidator<ClienteDatosPersonalesRequest>
{
    public ClienteDatosPersonalesRequestValidator()
    {
        RuleFor(cliente => cliente.Cedula).NotEmpty().WithMessage("La cédula es requerida.")
               .Matches("^[0-9]*$").WithMessage("La cédula solo puede contener números.");

        RuleFor(cliente => cliente.Ruc).NotEmpty().WithMessage("El RUC es requerido.")
            .Matches("^[0-9-]*$").WithMessage("El RUC solo puede contener números y el guión '-'.")
            .Must(ruc => ruc.Contains('-') || ruc.All(char.IsDigit))
            .WithMessage("El RUC debe contener un guión '-'.");

        RuleFor(cliente => cliente.Nombres).NotEmpty().WithMessage("Los nombres son requeridos.")
            .Matches("^[A-Za-z]+$").WithMessage("Los nombres solo pueden contener letras.");

        RuleFor(cliente => cliente.Apellidos).NotEmpty().WithMessage("Los apellidos son requeridos.")
            .Matches("^[A-Za-z]+$").WithMessage("Los apellidos solo pueden contener letras.");


    }
}
