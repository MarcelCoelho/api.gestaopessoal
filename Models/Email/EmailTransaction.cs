using api.gestaopessoal.Models.Configuration;

namespace api.gestaopessoal.Models.Email
{
    public class EmailTransaction
    {
        public EmailType Title { get; set; }
        public string Owner { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Bill { get; set; } = string.Empty;
        public string Product { get; set; } = string.Empty;
        public int InstallmentsCount { get; set; }
        public decimal Amount { get; set; }
        public string Observation { get; set; } = string.Empty;
    }
}
