﻿using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinephoriaServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowtimeController : ControllerBase
    {
        private readonly IShowtimeService _showtimeService;

        public ShowtimeController(IShowtimeService showtimeService)
        {
            _showtimeService = showtimeService;
        }

        /// <summary>
        /// Crée une nouvelle séance (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="createShowtimeDto">Les données de la séance à créer.</param>
        /// <returns>Une réponse indiquant si la séance a été créée avec succès.</returns>
        [Authorize(Roles = "Admin,Employee")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateShowtime([FromBody] CreateShowtimeDto createShowtimeDto)
        {
            try
            {
                await _showtimeService.CreateShowtimeAsync(createShowtimeDto);
                return StatusCode(StatusCodes.Status201Created, new { Message = "Séance créée avec succès." });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }


    }
}
