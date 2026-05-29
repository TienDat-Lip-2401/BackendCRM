namespace RedmineApp.Models.DtoModels.User
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public DateTime JoinDate { get; set; }
        public bool Status { get; set; }
        public string Email { get; set; } = null!;
    }
}
