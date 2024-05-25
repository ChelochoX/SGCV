using FluentValidation;

namespace sgcv_backend.Core.Domain.Request;

public class ProveedorDatosActualizarRequest
{       
    public string? Ruc { get; set; }
    public string? RazonSocial { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public int CodigoProveedor { get; set; }
}

public class ProveedorActualizarRequestValidator : AbstractValidator<ProveedorDatosActualizarRequest>
{
    public ProveedorActualizarRequestValidator()
    { 

        RuleFor(cliente => cliente.Ruc) 
            .Matches("^[0-9-]*$").When(cliente => cliente.Ruc != null)
                .WithMessage("El RUC solo puede contener números y el guión '-'.")
            .Must(ruc => ruc == null || ruc.Contains('-') || ruc.All(char.IsDigit))
                .WithMessage("El RUC debe contener un guión '-' si está presente.")
                .When(cliente => cliente.Ruc != null);

        RuleFor(cliente => cliente.RazonSocial)            
              .Matches("^[A-Za-z ]+$").WithMessage("Los nombres solo pueden contener letras y espacios.");

        RuleFor(cliente => cliente.Direccion)            
            .Matches("^[A-Za-z0-9\\s.,°/-]*$").WithMessage("La dirección particular solo puede contener letras, números, espacios, puntos, comas, guiones, barras y guiones bajos.");

        RuleFor(cliente => cliente.Telefono)
           .Matches("^[A-Za-z0-9\\s.,/-]*$").WithMessage("El Telefono solo puede contener letras, números, espacios, puntos, comas, guiones, barras y guiones bajos.");

        RuleFor(cliente => cliente.CodigoProveedor)
            .NotEmpty().WithMessage("El parámetro de código de cliente es requerido.")
            .GreaterThan(0).WithMessage("El parámetro de código de cliente debe ser mayor que cero.");

    }
}
