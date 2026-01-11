using System.ComponentModel.DataAnnotations.Schema;

namespace TorunamentHotChocolate.Models
{
    public class Match
    {
        public int Id { get; set; }
        public int Round { get; set; }
        public int? BracketId { get; set; }
        public Bracket? Bracket { get; set; }
        public int? Player1Id { get; set; }
        [ForeignKey("Player1Id")]
        public User? Player1 { get; set; }
        public int? Player2Id { get; set; }
        [ForeignKey("Player2Id")]
        public User? Player2 { get; set; }
        public int? WinnerId { get; set; }
        [ForeignKey("WinnerId")]
        public User? Winner { get; set; }
    }
}
