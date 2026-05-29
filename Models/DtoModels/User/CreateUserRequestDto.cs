namespace RedmineApp.Models.DtoModels.User
{
    public class CreateUserRequestDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public int? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime? JoinedDate { get; set; }
        public DateTime? LeavedDate { get; set; }
        public bool IsActive { get; set; }

        // Danh sách các ID của Position mà Admin đã chọn (Assign Position)
        public List<int> PositionIds { get; set; } = new List<int>();
    }
}
