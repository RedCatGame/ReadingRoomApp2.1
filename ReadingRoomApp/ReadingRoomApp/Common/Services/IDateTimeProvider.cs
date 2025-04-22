using System;

namespace ReadingRoomApp.Common.Services
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
        DateTime Today { get; }
        DateTime UtcNow { get; }
    }
}