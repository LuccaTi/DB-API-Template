namespace WebAPITemplate.Domain.Exceptions
{
    /// <summary>
    /// Exception used to indicate that a resource is already registered in database.
    /// </summary>
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message)
        {

        }
    }
}
