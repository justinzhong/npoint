namespace NPoint.Validators
{
    public interface IValidator { }

    public interface IValidator<in TSubject> : IValidator
         where TSubject : class
    {
        void Validate(TSubject subject);
    }
}