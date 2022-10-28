public class ChangeLanguageMessage : IPublisherMessage
{
    public ELanguage CurrentLanguage;

    public ChangeLanguageMessage(ELanguage currentLanguage)
    {
        CurrentLanguage = currentLanguage;
    }
}
