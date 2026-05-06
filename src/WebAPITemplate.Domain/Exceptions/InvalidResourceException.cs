namespace WebAPITemplate.Domain.Exceptions
{
    /// <summary>
    /// Exception used to indicate that a resource used in the request body is not valid
    /// </summary>
    public class InvalidResourceException : Exception
    {
        public InvalidResourceException(string message) : base(message)
        {

        }
    }
}
