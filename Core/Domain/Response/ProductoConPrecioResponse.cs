namespace sgcv_backend.Core.Domain.Response;

    public class ProductoConPrecioResponse
    {
        public int IDProducto { get; set; }
        public string CodigoProducto { get; set; }
        public string NombreProducto { get; set; }
        public string DescripcionProducto { get; set; }
        public Categoria Categoria { get; set; }
        public UnidadMedida UnidadMedida { get; set; }
        public List<Precio> Precios { get; set; }
    }

    public class Categoria
    {
        public int CodigoCategoria { get; set; }
        public string NombreCategoria { get; set; }
    }

    public class UnidadMedida
    {
        public int CodigoUnidadMedida { get; set; }
        public string SiglasUnidadMedida { get; set; }
        public string NombreUnidadMedida { get; set; }
    }

    public class Precio
    {
        public int ListaPrecio { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Iva10 { get; set; }
        public decimal Iva5 { get; set; }
        public decimal Exenta { get; set; }
        public bool EstadoPrecio { get; set; }
    }


