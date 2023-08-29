namespace api.gestaopessoal.Models.Configuration
{
    public interface IConfigSettings
    {
        string urlPictureDefault { get; set; }
        string smtpHost { get; set; }
        int smtpPort { get; set; }
        string smtpUsername { get; set; }
        string smtpSecret { get; set; }
        string toEmail { get; set; }
    }
}
