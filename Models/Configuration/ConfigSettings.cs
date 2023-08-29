namespace api.gestaopessoal.Models.Configuration
{
    public class ConfigSettings : IConfigSettings
    {
        public string urlPictureDefault { get; set; } = string.Empty;
        public string smtpHost { get ; set ; } = string.Empty;
        public int smtpPort { get ; set ; }
        public string smtpUsername { get ; set ; } = string.Empty;
        public string smtpSecret { get ; set ; } = string.Empty;
        public string toEmail { get ; set ; } = string.Empty;
    }
}
