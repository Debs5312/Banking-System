namespace Models.DTOs
{
    /// <summary>
    /// Represents the possible outcomes of a user registration attempt.
    /// </summary>
    public enum RegistrationResult
    {
        /// <summary>
        /// The registration was successful.
        /// </summary>
        Success,
        /// <summary>
        /// The registration failed because the username is already taken.
        /// </summary>
        UserNameExists,
        /// <summary>
        /// The registration failed because the Adhar ID is already associated with another user.
        /// </summary>
        AdharIdInUse,
        /// <summary>
        /// The registration failed because the provided Adhar ID was in an invalid format.
        /// </summary>
        InvalidAdharId,
        /// <summary>
        /// The registration failed due to a database error when saving changes.
        /// </summary>
        SaveChangesFailure
    }
}
