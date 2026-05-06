namespace WebAPITemplate.Domain.Exceptions
{
    /// <summary>
    /// Exception used to indicate that a resource was not found.
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {

        }
    }
}
