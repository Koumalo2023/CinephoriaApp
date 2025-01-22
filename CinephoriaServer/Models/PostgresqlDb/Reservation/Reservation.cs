using static CinephoriaServer.Configurations.EnumConfig;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CinephoriaServer.Configurations.Extensions;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class Reservation : BaseEntity
    {
        private int _numberOfSeats;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReservationId { get; set; }

        [Required]
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }

        [Required]
        [ForeignKey("Showtime")]
        public int ShowtimeId { get; set; }

        public ICollection<Seat> Seats { get; set; } = new List<Seat>();

        [Required]
        [Range(0, float.MaxValue, ErrorMessage = "Le prix total doit être positif.")]
        public float TotalPrice { get; set; }

        [Required]
        public string QrCode { get; set; } = string.Empty;

        public bool IsValidated { get; set; } = false;

        [Required]
        public ReservationStatus Status { get; set; } = ReservationStatus.Confirmed;

        public int NumberOfSeats
        {
            get => _numberOfSeats;
            set => _numberOfSeats = value;
        }

        public AppUser AppUser { get; set; }
        public Showtime Showtime { get; set; }
        public Movie Movie => Showtime?.Movie;
    }
}
