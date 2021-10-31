namespace Cko.PaymentGateway.Infrastructure.Interfaces.Services
{
    public interface IAppSettingsService
    {
        public string BankApiUrl { get; }
        bool UseLocalStorage { get; }
    }
}
