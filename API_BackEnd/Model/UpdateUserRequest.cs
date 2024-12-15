namespace API_BackEnd.Model
{
    public class UpdateUserRequest
    {
        public string UserId { get; set; } = null!;
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}
