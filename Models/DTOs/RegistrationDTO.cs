namespace Models.DTOs
{
    public class RegistrationDTO
    {
        public string UserName { get; set; }
        public string HashedPassword { get; set; }
        public string Address { get; set; }
        public int AdharNumber { get; set; }

    }
}