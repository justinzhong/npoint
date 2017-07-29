namespace NPoint.Validators
{
    public interface IValidatorRegistry
    {
        IValidator<TSubject> GetValidator<TSubject>() where TSubject : class;
        bool HasValidatorFor<TSubject>() where TSubject : class;
    }
}