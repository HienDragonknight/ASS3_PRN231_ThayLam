using BusinessObjects.Entities;
using DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ViroCureBLL.DTOs;

namespace Repositories
{
    public class LoginRepository
    {
        private readonly AccountDAO _repo;
        private readonly IConfiguration _configuration;


        public LoginRepository(AccountDAO repo, IConfiguration configuration)
        {
            _repo = repo;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> LoginFunction(string email, string password)
        {
            var user = await _repo.LoginAsync(email, password);

            if (user == null) throw new Exception("Invalid email or password");

            var token = GenerateJwtToken(user);

            return new LoginResponseDto
            {
                Message = "Login successful",
                Token = token,
                User = new UserDto
                {
                    Id = user.AccountId,
                    Email = user.Email,
                    Role = user.Role?.RoleName ?? "unknown" // Fix: Access RoleName property of Role object  
                }
            };
        }

        public async Task<RegisterResponseDto> RegisterFunction(RegisterDto registerDto)
        {
            // Check if email already exists
            var existingUser = await _repo.GetByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                throw new Exception("Email already registered");
            }

            // Create new account
            var newAccount = new Account
            {
                Email = registerDto.Email,
                Password = registerDto.Password, // In a real app, hash this password
                AcountName = registerDto.AccountName,
                RoleId = registerDto.RoleId
            };

            // Save to database
            await _repo.AddAsync(newAccount);
           

            // Get the created user with role
            var createdUser = await _repo.GetByIdWithRoleAsync(newAccount.AccountId);

            return new RegisterResponseDto
            {
                Message = "Registration successful",
                Success = true,
                User = new UserDto
                {
                    Id = createdUser.AccountId,
                    Email = createdUser.Email,
                    Role = createdUser.Role?.RoleName ?? "unknown"
                }
            };
        }

        private string GenerateJwtToken(Account user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.AccountId.ToString()),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "unknown"),
                new Claim(ClaimTypes.Email, user.Email)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(int.Parse(_configuration["Jwt:ExpiryInDays"])),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
