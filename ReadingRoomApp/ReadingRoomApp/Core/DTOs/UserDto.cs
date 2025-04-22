using ReadingRoomApp.Core.Domain.Enums;

namespace ReadingRoomApp.Core.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole Role { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}