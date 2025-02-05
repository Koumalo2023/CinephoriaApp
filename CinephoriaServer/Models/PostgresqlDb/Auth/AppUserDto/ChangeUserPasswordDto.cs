﻿using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class ChangeUserPasswordDto
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Le mot de passe doit contenir entre 8 et 100 caractères.")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Les mots de passe ne correspondent pas.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Confirmer le mot de passe doit contenir entre 8 et 100 caractères.")]
        public string ConfirmNewPassword { get; set; }
    }
}
