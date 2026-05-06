namespace WebAPITemplate.Domain.Exceptions
{
    /// <summary>
    /// Exception used to indicate that id in route is not valid to find entity in database
    /// </summary>
    public class InvalidIdException : Exception
    {
        public InvalidIdException(string message) : base(message)
        {

        }
    }
}
