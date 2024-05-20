using FluentValidation;
using sgcv_backend.Core.Domain.Request;

namespace sgcv_backend.Core.Domain.Request
{
    public class ProductoDatosRequest
    {        
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int CodigoCategoria { get; set; }
        public int CodigoUnidadMedida { get; set; }        
    }
}
public class ProductoDatosRequestValidator : AbstractValidator<ProductoDatosRequest>
{
    public ProductoDatosRequestValidator()
    {       
        RuleFor(x => x.CodigoCategoria).GreaterThan(0);
        RuleFor(x => x.CodigoUnidadMedida).GreaterThan(0);       

       //RuleFor(x => x.Codigo).Matches("^[A-Za-z0-9'\" ]+$").WithMessage("La numeración solo puede contener letras, números y los caracteres ' y \".");

    //    RuleFor(x => x.Nombre).Matches("^[A-Za-z0-9'\" ]+$").WithMessage("El nombre solo puede contener letras, números y los caracteres ' y \".");

    //    RuleFor(x => x.Descripcion).Matches("^[A-Za-z0-9'\" ]+$").WithMessage("La descripción solo puede contener letras, números y los caracteres ' y \".");
    }
}