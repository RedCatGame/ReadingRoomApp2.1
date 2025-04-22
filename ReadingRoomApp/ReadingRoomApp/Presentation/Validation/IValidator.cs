namespace ReadingRoomApp.Presentation.Validation
{
    public interface IValidator<T>
    {
        ValidationResult Validate(T entity);
    }
}