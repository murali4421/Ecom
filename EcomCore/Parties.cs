using System.ComponentModel.DataAnnotations;

namespace EcomCore
{
    public class Parties
    {
        public int Id { get; set; }
        [Required(AllowEmptyStrings =false,ErrorMessage ="Enter the Party Name")]
        public string? Party_Name { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Choose the Symbol")]
        public int Symbol_Id { get; set; }
        public DateTime Register_Date { get; set; }
    }

    public class PartyLists : Parties
    {
        public byte[]? Symbol { get; set; }
    }
}
