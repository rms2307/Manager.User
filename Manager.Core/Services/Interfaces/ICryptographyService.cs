namespace Manager.Core.Services.Interfaces
{
    public interface ICryptographyService
    {
        string Encrypt(string text);
        string Decrypt(string text);
    }
}
