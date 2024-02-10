namespace ECOMApi.Models
{
    public class Vote
    {
        public string? Voter_Id { get; set; }
        public string? Voter_Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Voter_State { get; set; }
        public byte[]? Photo { get; set; }
        public char? Voter_Verified { get; set; }
    }
}
