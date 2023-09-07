namespace Airport.Data.Accessories
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string? message = null, Exception? inner = null)
            : base(message, inner)
        {
        }
    }
}
