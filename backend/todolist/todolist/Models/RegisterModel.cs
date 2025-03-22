using System.ComponentModel.DataAnnotations;

namespace todolist.Models
{
    /// <summary>
    /// Modello per la registrazione di un nuovo utente
    /// </summary>
    public class RegisterModel
    {
        /// <summary>
        /// Nome utente
        /// </summary>
        [Required]
        public string Username { get; set; }
        
        /// <summary>
        /// Email dell'utente
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        /// <summary>
        /// Password dell'utente
        /// </summary>
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
        
        /// <summary>
        /// Nome dell'utente
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// Cognome dell'utente
        /// </summary>
        public string LastName { get; set; }
    }
}