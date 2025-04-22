using System;

namespace ReadingRoomApp.Common.Exceptions
{
    public class DataAccessException : ApplicationException
    {
        public DataAccessException() : base() { }

        public DataAccessException(string message) : base(message) { }

        public DataAccessException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}