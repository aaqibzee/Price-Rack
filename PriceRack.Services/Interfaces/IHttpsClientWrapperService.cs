namespace PriceMicroservice.Services
{
    public interface IHttpsClientWrapperService
    {
        Task<string> GetContentFromExternalCource(string endpoint);
    }
}
