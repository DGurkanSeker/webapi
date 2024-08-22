using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webapi.Interfaces;

namespace WebApi.Data
{
    public class User

    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public DateTime CreationTime { get; set; }
        public string Role { get; set; }
    }
}