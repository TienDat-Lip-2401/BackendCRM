namespace RedmineApp.Models.DtoModels.Position
{
    public class AssignPositionListRequestDto
    {
        public int UserId { get; set; }
        public List<int> PositionIds { get; set; } = new();
    }
}
