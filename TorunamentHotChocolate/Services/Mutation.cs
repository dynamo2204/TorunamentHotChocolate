using Microsoft.EntityFrameworkCore;
using TorunamentHotChocolate.Data;
using TorunamentHotChocolate.Models;
using MatchModel = TorunamentHotChocolate.Models.Match;

namespace TorunamentHotChocolate.Services
{
    public class Mutation
    {
        public async Task<string> Register(
            AppDbContext context, string firstName, string lastName, string email, string password)
        {
            var user = new User { FirstName = firstName, LastName = lastName, Email = email, PasswordHash = password };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return "Zarejestrowano pomyślnie";
        }

        public string Login(
            [Service] AppDbContext context, [Service] AuthService authService, string email, string password)
        {
            var user = context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == password);
            if (user == null) throw new Exception("Błędne dane");
            return authService.GenerateToken(user);
        }

        public async Task<Tournament> CreateTournament(AppDbContext context, string name)
        {
            var t = new Tournament { Name = name, StartDate = DateTime.Now, Status = "Created" };
            context.Tournaments.Add(t);
            await context.SaveChangesAsync();
            return t;
        }

        public async Task<bool> AddParticipant(AppDbContext context, int tournamentId, int userId)
        {
            var tournament = await context.Tournaments.Include(t => t.Participants).FirstOrDefaultAsync(t => t.Id == tournamentId);
            var user = await context.Users.FindAsync(userId);

            if (tournament == null || user == null) return false;

            tournament.Participants.Add(user);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> StartTournament(AppDbContext context, int tournamentId)
        {
            var tournament = await context.Tournaments.Include(t => t.Participants).FirstOrDefaultAsync(t => t.Id == tournamentId);
            if (tournament == null) return false;

            tournament.Status = "Started";

            var bracket = new Bracket();
            var participants = tournament.Participants.ToList();

            for (int i = 0; i < participants.Count; i += 2)
            {
                if (i + 1 < participants.Count)
                {
                    bracket.Matches.Add(new MatchModel
                    {
                        Round = 1,
                        Player1 = participants[i],
                        Player2 = participants[i + 1]
                    });
                }
            }

            context.Brackets.Add(bracket);
            tournament.Bracket = bracket;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Match> PlayMatch(AppDbContext context, int matchId, int winnerId)
        {
            var match = await context.Matches
                .Include(m => m.Bracket)
                .ThenInclude(b => b.Matches)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null) throw new Exception("Mecz nie istnieje");
            if (match.WinnerId != null) throw new Exception("Mecz został już rozstrzygnięty");

            if (match.Player1Id != winnerId && match.Player2Id != winnerId)
                throw new Exception("Ten użytkownik nie gra w tym meczu");

            match.WinnerId = winnerId;
            var winner = await context.Users.FindAsync(winnerId);

            var allMatches = match.Bracket.Matches.OrderBy(m => m.Id).ToList();

            int currentMatchIndex = allMatches.IndexOf(match);

            int nextRound = match.Round + 1;

            var nextMatch = allMatches
                .Where(m => m.Round == nextRound)
                .FirstOrDefault(m => m.Player1Id == null || m.Player2Id == null);

            if (nextMatch != null)
            {
                if (nextMatch.Player1Id == null)
                {
                    nextMatch.Player1 = winner;
                }
                else
                {
                    nextMatch.Player2 = winner;
                }
            }
            else
            {
                int matchesInThisRound = allMatches.Count(m => m.Round == match.Round);

                if (matchesInThisRound > 1)
                {
                    var newMatch = new Match
                    {
                        Round = nextRound,
                        Bracket = match.Bracket,
                        Player1 = winner
                    };
                    context.Matches.Add(newMatch);
                }
                else
                {
                    var tournament = await context.Tournaments.FirstOrDefaultAsync(t => t.BracketId == match.BracketId);
                    if (tournament != null) tournament.Status = "Finished";
                }
            }

            await context.SaveChangesAsync();
            return match;
        }
    }
}
