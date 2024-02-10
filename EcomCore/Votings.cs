namespace EcomCore
{
    public class Votings
    {
        public int Id { get; set; }
        public DateTime? Voting_Date { get; set; }
        public int Party_Id { get; set; }
        public string? Party_Name { get; set; }
        public int Symbol_Id { get; set; }
        public int Voter_id { get; set; }
    }

    public class MachineVoteList : Votings
    {
        public byte[]? symbol { get; set; } 
        public int state_id { get; set;}
        public string? state { get; set; }
        public int total_seats { get; set; }
        public int party_total_seat { get; set;}
        public bool IsSelected { get; set; }    

    }

    public class VotedList
    {
        public string? party_name { get; set; }
        public string? mp_state { get; set; }
        public int Total_Vote { get; set; }
        public string? EResult { get; set; }
    }    
}
