namespace sgcv_backend.Core.Domain.Entities;

public class Datos<T>
{
    public T Items { get; set; }
    public int TotalRegistros { get; set; }
}
