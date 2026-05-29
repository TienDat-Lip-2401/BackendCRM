namespace RedmineApp.Models.DtoModels.Auth
{
    public class LoginResponseDto
    {
        public int user_id { get; set; }
        public string name { get; set; }
        public string[] positions { get; set; } = Array.Empty<string>();
        public string email { get; set; }
        //public string access_token { get; set; }
        //public string refresh_token { get; set; }
        public bool isFirstLogin { get; set; }
        public DateTime expires_at { get; set; }
    }
}
