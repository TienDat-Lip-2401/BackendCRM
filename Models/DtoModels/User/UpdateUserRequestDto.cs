using RedmineApp.Models.DtoModels.Position;

namespace RedmineApp.Models.DtoModels.User
{
    public class UpdateUserRequestDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public int? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime? JoinedDate { get; set; }
        public DateTime? LeavedDate { get; set; }
        public bool IsActive { get; set; }
        public List<PositionDto> Positions { get; set; } = new();
    }
}
