using EcomCore;

namespace ECOMWeb.Common
{
    public class Commons
    {
        private IConfiguration _configuration;
        public Commons(IConfiguration iconfiguration)
        {
             _configuration = iconfiguration;
    }
        public List<Parties> PartyList()
        {
            List<Parties> party = new();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_configuration.GetSection("ECApi").Value + "Party/");
                    var responseTask = client.GetAsync("GetPartyList");
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readParty = result.Content.ReadAsAsync<List<Parties>>();
                        readParty.Wait();
                        party = (List<Parties>)readParty.Result;
                    }
                }
                return party;
            }
            catch
            {
                return party;
            }
        }

        public List<SeatAllocation> StateList()
        {
            List<SeatAllocation> state = new();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_configuration.GetSection("ECApi").Value + "SeatAllocation/");
                    var responseTask = client.GetAsync("GetSeatAllocatedList");
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readSeat = result.Content.ReadAsAsync<List<SeatAllocation>>();
                        readSeat.Wait();
                        state = (List<SeatAllocation>)readSeat.Result;
                    }
                }
                return state;
            }
            catch
            {
                return state;
            }
        }

    }
}
