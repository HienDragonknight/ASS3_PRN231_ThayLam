using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViroCureBLL.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public string? AccountName { get; set; }

        // Default role ID for new users (e.g., 2 for regular users)
        public int RoleId { get; set; } = 2;
    }

    public class RegisterResponseDto
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public UserDto User { get; set; }
    }
}