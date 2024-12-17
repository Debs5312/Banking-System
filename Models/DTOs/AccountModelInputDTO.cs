namespace Models.DTOs
{
    public class AccountModelInputDTO
    {
        public Guid PrimaryUserId { get; set; }
        public Guid? SecondaryUserId { get; set; }
        public Guid? NomineeId { get; set; }
    }
}