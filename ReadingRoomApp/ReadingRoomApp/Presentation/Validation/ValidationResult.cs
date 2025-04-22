using System.Collections.Generic;
using System.Linq;

namespace ReadingRoomApp.Presentation.Validation
{
    public class ValidationResult
    {
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public bool IsValid => !_errors.Any();

        public IDictionary<string, List<string>> Errors => _errors;

        public void AddError(string propertyName, string errorMessage)
        {
            if (!_errors.ContainsKey(propertyName))
            {
                _errors[propertyName] = new List<string>();
            }

            _errors[propertyName].Add(errorMessage);
        }

        public void Clear()
        {
            _errors.Clear();
        }

        public List<string> GetAllErrors()
        {
            return _errors.SelectMany(e => e.Value).ToList();
        }

        public List<string> GetErrorsForProperty(string propertyName)
        {
            return _errors.TryGetValue(propertyName, out var errors) ? errors : new List<string>();
        }
    }
}