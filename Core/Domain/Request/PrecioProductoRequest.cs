using FluentValidation;
using sgcv_backend.Core.Domain.Request;

namespace sgcv_backend.Core.Domain.Request
{
    public class PrecioProductoRequest
    {       
        public int Lista { get; set; }
        public decimal Compra { get; set; }
        public decimal Venta { get; set; }
        public decimal Iva10 { get; set; }
        public decimal Iva5 { get; set; }
        public decimal Exenta { get; set; }
        public bool Estado { get; set; }
        public string Observacion { get; set; }
        public int CodigoProducto { get; set; }
    }
}
public class PrecioProductoRequestValidator : AbstractValidator<PrecioProductoRequest>
{
    public PrecioProductoRequestValidator()
    {
        RuleFor(x => x.Lista)
            .GreaterThan(0).WithMessage("La lista debe ser mayor a 0.");

        RuleFor(x => x.Compra)
            .GreaterThanOrEqualTo(0).WithMessage("El precio de compra no puede ser negativo.");

        RuleFor(x => x.Venta)
            .GreaterThanOrEqualTo(0).WithMessage("El precio de venta no puede ser negativo.");

        RuleFor(x => x.Iva10)
            .GreaterThanOrEqualTo(0).WithMessage("El IVA 10% no puede ser negativo.");

        RuleFor(x => x.Iva5)
            .GreaterThanOrEqualTo(0).WithMessage("El IVA 5% no puede ser negativo.");

        RuleFor(x => x.Exenta)
            .GreaterThanOrEqualTo(0).WithMessage("El monto exento no puede ser negativo.");

        RuleFor(x => x.Estado)
            .NotNull().WithMessage("El estado es obligatorio.");       

        RuleFor(x => x.CodigoProducto)
            .GreaterThan(0).WithMessage("El código del producto debe ser mayor a 0.");
    }
}