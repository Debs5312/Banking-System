namespace Models.DTOs
{
    public class UserReadDTO
    {
        public string UserName { get; set; }
        public string Role { get; set; }
        public string RefreshToken { get; set; }
        public List<Account> Accounts { get; set; }

    }
}