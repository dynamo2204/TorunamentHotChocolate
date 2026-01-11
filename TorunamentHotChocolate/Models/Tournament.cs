namespace TorunamentHotChocolate.Models
{
    public class Tournament
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public string Status { get; set; }
        public ICollection<User> Participants { get; set; } = new List<User>();
        public int? BracketId { get; set; }
        public Bracket? Bracket { get; set; }
    }
}
