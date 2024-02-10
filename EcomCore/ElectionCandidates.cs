namespace EcomCore
{
    public class ElectionCandidates
    {
        public int Id { get; set; }
        public int Party_Id { get; set; }
        public int MP_Seat { get; set; }
        public int Seat_State_Id { get; set; }
    }

    public class CandidateList : ElectionCandidates
    {
        public string? Party_Name { get; set; }
        public string? MP_State { get; set; }
    }
}
