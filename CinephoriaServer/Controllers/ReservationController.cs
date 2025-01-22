using CinephoriaServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinephoriaServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

       
    }
}
