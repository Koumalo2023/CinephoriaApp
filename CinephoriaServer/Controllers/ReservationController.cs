using CinephoriaServer.Services;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Valide un QRCode scanné pour une réservation.
        /// </summary>
        /// <param name="qrCodeData">Les données du QRCode scanné.</param>
        /// <returns>Une réponse indiquant si la validation a réussi.</returns>
        [Authorize(Roles = "Admin,Employee")]
        [HttpPost("validate")]
        public async Task<IActionResult> ValidateSession([FromBody] string qrCodeData)
        {
            try
            {
                bool isValid = await _reservationService.ValidatedSession(qrCodeData);

                if (isValid)
                {
                    return Ok(new { Message = "QRCode validé avec succès." });
                }
                else
                {
                    return BadRequest(new { Message = "Validation du QRCode échouée." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }


    }
}
