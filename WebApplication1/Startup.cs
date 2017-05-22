using Microsoft.Owin;
using Owin;


//jakis filtr zwiazany z kompilacja - ale co to???
[assembly: OwinStartupAttribute(typeof(WebApplication1.Startup))]
namespace WebApplication1
{
    public partial class Startup
    {
        //tworzy aplikacje, ktora ma cechy zarowno hosta jak i serwera 
        //(zarzadza zarowno procesami w obrebie aplikacji jak i nasluchuje 
        //nadchodzacych zadan i je przekazuje dalej)
        public void Configuration(IAppBuilder app)
        {
            //tutaj dodaje sie middleware, ktory jest
            //jak filtry, ktore cos robia z zadaniem np.
            //sledza je, odrzucaja, modyfikuja itp.

            //odwoluje sie do funkcji z Startup.Auth.cs
            //tam dla kazdej sesji dodaje sie komponenty
            //instancja zawartosci z bazy danych
            //instancja manager'a logowania (podejrzewam, ze tutaj jest autentykacja)
            //instancja manager'a uzytkownika (sledzaca jego stan?)
            //dodatkowo dodaje mozliwe sposoby uwierzytelniania (face, sms, twtter itd.)
            ConfigureAuth(app);


            
        }
    }
}
