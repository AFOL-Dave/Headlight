namespace Headlight.Models.Options
{
    public class IdentityProvider
    {
        public const string Section = "HeadLight:IdentityProviders";

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string DisplayName { get; set; }

        public string Type { get; set; }
    }
}