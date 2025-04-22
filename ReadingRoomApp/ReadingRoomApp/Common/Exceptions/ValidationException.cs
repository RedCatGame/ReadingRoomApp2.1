using System;
using System.Collections.Generic;

namespace ReadingRoomApp.Common.Exceptions
{
    public class ValidationException : ApplicationException
    {
        public Dictionary<string, List<string>> Errors { get; } = new Dictionary<string, List<string>>();

        public ValidationException() : base() { }

        public ValidationException(string message) : base(message) { }

        public ValidationException(string message, Exception innerException)
            : base(message, innerException) { }

        public ValidationException(Dictionary<string, List<string>> errors)
            : base("Validation failed")
        {
            Errors = errors;
        }

        public void AddError(string propertyName, string errorMessage)
        {
            if (!Errors.ContainsKey(propertyName))
            {
                Errors[propertyName] = new List<string>();
            }

            Errors[propertyName].Add(errorMessage);
        }
    }
}