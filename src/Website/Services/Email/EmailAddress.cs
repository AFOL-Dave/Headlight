namespace Headlight.Services.Email
{
    public class EmailAddress : IEmailAddress
    {
        /// <summary>
        ///  The email address where a message will be sent
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        ///  The display name shown in a mail client
        /// </summary>
        public string Name { get; set; }
    }
}