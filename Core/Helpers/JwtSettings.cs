namespace Core.Helpers
{
    public class JwtSettings
    {
        public string SecritKey { get; set; } = string.Empty;
        public string AudienceIP { get; set; } = string.Empty;
        public string IssuerIP { get; set; } = string.Empty;
    }
}
