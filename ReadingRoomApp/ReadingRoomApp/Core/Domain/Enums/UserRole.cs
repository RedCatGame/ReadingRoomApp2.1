using System;

namespace ReadingRoomApp.Core.Domain.Enums
{
    public enum UserRole
    {
        Reader = 0,     // Обычный пользователь (читатель)
        Author = 1,     // Писатель
        Admin = 2       // Администратор
    }
}