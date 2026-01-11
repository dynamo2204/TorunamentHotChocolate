using System.Text.RegularExpressions;

namespace TorunamentHotChocolate.Models
{
    public class Bracket
    {
        public int Id { get; set; }
        public ICollection<TorunamentHotChocolate.Models.Match> Matches { get; set; } = new List<TorunamentHotChocolate.Models.Match>();
    }
}
