namespace CinephoriaServer.Models.PostgresqlDb;

public class ContactViewModel
{
    /// <summary>
    /// Nom de l'utilisateur qui fait la demande (optionnel).
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Titre de la demande de contact.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Description de la demande de contact.
    /// </summary>
    public string Description { get; set; }

}
