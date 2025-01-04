namespace Models.DTOs
{
    public class LoggedInStatus
    {
        public bool LoggedIn { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }
}