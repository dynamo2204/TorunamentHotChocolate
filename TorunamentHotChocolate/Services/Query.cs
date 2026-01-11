using System.Security.Claims;
using TorunamentHotChocolate.Data;
using TorunamentHotChocolate.Models;
using Microsoft.EntityFrameworkCore;
using HotChocolate.Authorization;

namespace TorunamentHotChocolate.Services
{
    public class Query
    {
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Tournament> GetTournaments(AppDbContext context) => context.Tournaments;

        [Authorize]
        public async Task<IEnumerable<Match>> GetMyMatches(
            AppDbContext context,
            ClaimsPrincipal claimsPrincipal)
        {
            var userIdString = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                throw new Exception("Błąd identyfikacji użytkownika.");

            return await context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .Where(m => m.Player1Id == userId || m.Player2Id == userId)
                .ToListAsync();
        }
    }
}
