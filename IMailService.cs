namespace ArtShop.Services
{
    //interfetele sunt folositoare daca vrei implementari diferite pt cazuri diferite
    public interface IMailService
    {
        void SendMessage(string to, string subject, string body);
    }
}