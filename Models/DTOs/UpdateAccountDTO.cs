namespace Models.DTOs
{
    public class UpdateAccountDTO
    {
        public Guid? SecondaryUserId { get; set; }
        public Guid? NomineeId { get; set; }
    }
}