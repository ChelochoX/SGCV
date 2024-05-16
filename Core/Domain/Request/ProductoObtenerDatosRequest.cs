using FluentValidation;
using sgcv_backend.Core.Domain.Request;

namespace sgcv_backend.Core.Domain.Request
{
    public class ProductoObtenerDatosRequest
    {
        public int CodigoProducto { get; set; }
        public string Numeracion { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int CodigoCategoria { get; set; }
        public int CodigoUnidadMedida { get; set; }
        public int CodigoPrecio { get; set; }
    }
}
public class ProductoObtenerDatosRequestValidator : AbstractValidator<ProductoObtenerDatosRequest>
{
    public ProductoObtenerDatosRequestValidator()
    {
        RuleFor(x => x.CodigoProducto).GreaterThan(0);
        RuleFor(x => x.CodigoCategoria).GreaterThan(0);
        RuleFor(x => x.CodigoUnidadMedida).GreaterThan(0);
        RuleFor(x => x.CodigoPrecio).GreaterThan(0);

    //    RuleFor(x => x.Numeracion).Matches("^[A-Za-z0-9'\" ]+$").WithMessage("La numeración solo puede contener letras, números y los caracteres ' y \".");

    //    RuleFor(x => x.Nombre).Matches("^[A-Za-z0-9'\" ]+$").WithMessage("El nombre solo puede contener letras, números y los caracteres ' y \".");

    //    RuleFor(x => x.Descripcion).Matches("^[A-Za-z0-9'\" ]+$").WithMessage("La descripción solo puede contener letras, números y los caracteres ' y \".");
    }
}