using Application.Exceptions;

namespace sgcv_backend.Core.Domain.Exceptions
{
    public class ClientesException : ApiException
    {
        public ClientesException(string message) : base(message)
        {
        }
    }
}
