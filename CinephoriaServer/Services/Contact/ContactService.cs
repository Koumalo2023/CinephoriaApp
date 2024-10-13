using AutoMapper;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository;
using Microsoft.AspNetCore.Identity;

namespace CinephoriaServer.Services
{
    public class ContactService : IContactService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWorkPostgres _unitOfWork;
        private readonly IMapper _mapper;

        public ContactService(UserManager<AppUser> userManager, IUnitOfWorkPostgres unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Récupère les informations du cinéma.
        /// </summary>
        /// <param name="cinemaId">Identifiant du cinéma.</param>
        /// <returns>Les informations du cinéma.</returns>
        public async Task<CinemaDto?> GetCinemaInfoAsync(int cinemaId)
        {
            try
            {
                // Utilisation du UnitOfWork pour accéder au repository des cinémas avec un ID directement
                var cinema = await _unitOfWork.Cinemas.GetByIdAsync(cinemaId);
                if (cinema == null) return null;

                // Utilisation du Mapper pour transformer l'entité Cinema en CinemaDto
                return _mapper.Map<CinemaDto>(cinema);
            }
            catch (Exception ex)
            {
                // Gérer les erreurs et potentiellement loguer
                throw new Exception("Erreur lors de la récupération des informations du cinéma.", ex);
            }
        }

        /// <summary>
        /// Crée une demande de contact à partir d'un utilisateur ou visiteur.
        /// </summary>
        /// <param name="contactDto">Données de la demande de contact.</param>
        /// <returns>La demande de contact créée.</returns>
        public async Task<ContactDto> CreateContactAsync(ContactViewModel contactViewModel)
        {
            try
            {
                // Vérification de l'existence de l'utilisateur via UserName
                var user = await _userManager.FindByNameAsync(contactViewModel.UserName);

                // Créer un nouveau contact à partir du ViewModel
                var contact = _mapper.Map<Contact>(contactViewModel);
                contact.CreatedAt = DateTime.UtcNow;

                if (user != null)
                {
                    // Si l'utilisateur existe, assigner son UserId au contact
                    contact.UserId = user.Id;
                    contact.UserName = user.UserName;
                }

                // Ajouter et sauvegarder le contact dans la base de données
                await _unitOfWork.Contacts.CreateAsync(contact);
                await _unitOfWork.CompleteAsync();

                // Retourner le DTO du contact créé
                return _mapper.Map<ContactDto>(contact);
            }
            catch (Exception ex)
            {
                // Gérer les erreurs
                throw new Exception("Erreur lors de la création du contact.", ex);
            }
        }



    }
}
