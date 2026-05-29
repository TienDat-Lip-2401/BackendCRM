namespace RedmineApp.Models.DtoModels.User
{
    public class CreateUserResponseDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string GeneratedPassword { get; set; }
    }
}
