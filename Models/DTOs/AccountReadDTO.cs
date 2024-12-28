namespace Models.DTOs
{
    public class AccountReadDTO
    {
        public Guid Id { get; set; }
        public int AccountNumber { get; set; }

        public Guid PrimaryUserId { get; set; }
        public int PrimaryAccountHolderAdharNumber { get; set; }

        public Guid? SecondaryUserId { get; set; }
        public int SecondaryAccountHolderAdharNumber { get; set; }

        public Guid? NomineeId { get; set; }
        public int NomineeAdharNumber { get; set; }

    }
}