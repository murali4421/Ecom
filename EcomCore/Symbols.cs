using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EcomCore
{
    public class Symbols
    {
        public int Id { get; set; }
        
        public byte[]? Symbol { get; set; }
        [Required(AllowEmptyStrings =false, ErrorMessage ="Enter the Symbol Name")]
        public string? Symbol_Name { get; set; }
    }

}
