using System;

namespace NPoint.Validators
{
    /// <summary>
    /// This registry disables validations by always returning a negative reply 
    /// when queried for a validator for a given subject.
    /// </summary>
    public class DisabledValidatorRegistry : IValidatorRegistry
    {
        bool IValidatorRegistry.HasValidatorFor<TSubject>()
        {
            return false;
        }

        IValidator<TSubject> IValidatorRegistry.GetValidator<TSubject>()
        {
            throw new NotImplementedException();
        }
    }
}