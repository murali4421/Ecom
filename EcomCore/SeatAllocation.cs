using System.ComponentModel.DataAnnotations;

namespace EcomCore
{
    public class SeatAllocation
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage ="Enter the State")]
        public string MP_state { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter the Total Seats")]
        public int Total_seats { get; set; }
    }
}
