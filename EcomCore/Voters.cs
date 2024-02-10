using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace EcomCore
{
    public class Voters
    {        
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter the Voter No")]
        public string? Voter_Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter the Voter Name")]
        public string? Voter_Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter the Address")]
        public string? Address { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter the City")]
        public string? City { get; set; }

        public int Voter_State_Id { get; set; }
                
        public byte[]? Photo { get; set; }
        public char? Voter_Verified { get; set; }

    }

    public class VoterList : Voters
    {
        public string? Voter_State { get; set; }
    }

}
