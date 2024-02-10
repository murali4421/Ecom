using EcomCore;
using ECOMWeb.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;

namespace ECOMWeb.Controllers
{
    public class VotingController : Controller
    {
        private IConfiguration _configuration;
        private Uri base_uribase;
        private readonly ILogger<VotingController> _logger;
        private Commons _commons;
        HttpContextAccessor httpContext = new();
        public VotingController(ILogger<VotingController> logger, IConfiguration iconfiguration)
        {
            _logger = logger;
            _configuration = iconfiguration;
            base_uribase = new Uri(_configuration.GetSection("ECApi").Value + "VotingSystem/");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Voting()
        {

            httpContext.HttpContext?.Session.SetString("Dashboard", "NA");
            TempData["Message"] = "";            
            return View();
        }

        [HttpPost]
        public IActionResult Voting(Voters voterNo)
        {
            if(voterNo.Voter_Id == null)
            {
                TempData["Message"] = "Enter the Voter no";
                return View();
            }
            List<VoterList> vote = new List<VoterList>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.GetSection("ECApi").Value + "Voter/");
                string strAction = "GetVoterList";
                var getResponse = client.GetAsync(strAction);
                getResponse.Wait();

                var response = getResponse.Result;
                if (response.IsSuccessStatusCode)
                {
                    var ec = response.Content.ReadAsAsync<List<VoterList>>();
                    ec.Wait();
                    vote = (List<VoterList>)ec.Result;

                    var data = vote.Where(x=> x.Voter_Id == voterNo.Voter_Id).FirstOrDefault<VoterList>();

                    string msg = "Your not eligible to Vote,";
                    if (data != null)
                    {
                        if( data.Voter_Verified == 'N')
                        {
                            TempData["Message"] = msg + " Your Voter No not verified.";
                            return View();
                        }
                        else
                        {
                            httpContext.HttpContext.Session.SetString("VoterStateId", data.Voter_State_Id.ToString());  
                            httpContext.HttpContext.Session.SetString("VoterId", data.Id.ToString());
                            TempData["Message"] = "";
                        }
                    }
                    else
                    {
                        TempData["Message"] = msg + " Your Voter No not registered.";
                        return View();
                    }
                    httpContext.HttpContext.Session.SetString("Dashboard", "VMD");
                    return RedirectToAction("VotingMachine");
                }                
            }            
            return View();
        }
                
        [HttpGet]
        public IActionResult VotingMachine()
        {
            string msg = "Somthing wrong in server, please contact to election commission of india";
            try
            {
                int voterStateId = 0;
                int voter_id = 0;

                if (httpContext.HttpContext?.Session.GetString("VoterId") != null)
                {
                    voter_id = Convert.ToInt32(httpContext.HttpContext.Session.GetString("VoterId"));
                }
                
                if (httpContext.HttpContext?.Session.GetString("VoterStateId") != null)
                {
                    voterStateId = Convert.ToInt32(httpContext.HttpContext.Session.GetString("VoterStateId"));
                }
                List<MachineVoteList> vote = new List<MachineVoteList>();
                vote = VoteList(voterStateId);
                if(vote !=null && vote.Count > 0)
                {
                    TempData["ErrorMsg"] = "";
                    httpContext.HttpContext?.Session.SetString("SuccessMsg", "");
                    return View(vote);
                }

                TempData["ErrorMsg"] = msg;
                return View();
            }
            catch
            {
                TempData["ErrorMsg"] = msg;
                return View();
            }
        }

        private List<MachineVoteList> VoteList(int voterStateId)
        {
            List<MachineVoteList> vote = new List<MachineVoteList>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.GetSection("ECApi").Value + "VotingSystem/");
                string strAction = "GetMachineVoteList?voterStateId=" + voterStateId;
                var getResponse = client.GetAsync(strAction);
                getResponse.Wait();

                var response = getResponse.Result;
                if (response.IsSuccessStatusCode)
                {
                    var ec = response.Content.ReadAsAsync<List<MachineVoteList>>();
                    ec.Wait();
                    vote = (List<MachineVoteList>)ec.Result;
                    TempData["ErrorMsg"] = "";                    
                }
            }
            return vote;
        }

        [HttpPost]
        public IActionResult MachineVote(MachineVoteList machine)
        {
            int voterStateId = 0;
            int voter_id = 0;
            int party_id = 0;
            string party_name = "";

            if(machine==null)
                return View(machine);

            if (machine.Symbol_Id == 0)
            {
                httpContext.HttpContext?.Session.SetString("ErrorMsg", "Select Your Vote");
                return RedirectToAction("VotingMachine");
            }                

            if (httpContext.HttpContext?.Session.GetString("VoterId") != null)
            {
                voter_id = Convert.ToInt32(httpContext.HttpContext.Session.GetString("VoterId"));
            }

            if (httpContext.HttpContext?.Session.GetString("VoterStateId") != null)
            {
                voterStateId = Convert.ToInt32(httpContext.HttpContext.Session.GetString("VoterStateId"));
            }


            List<MachineVoteList> vote = new List<MachineVoteList>();
            vote = VoteList(voterStateId);
            if (vote != null && vote.Count > 0)
            {
                var party = vote.Where(x => x.state_id == voterStateId && x.Symbol_Id == machine.Symbol_Id).FirstOrDefault<MachineVoteList>();
                party_id = party.Party_Id;
                party_name = party.Party_Name;
            }

            var votings = new Votings() 
            { 
                Party_Id = party_id,
                Party_Name = party_name,
                Voter_id = voter_id,
                Voting_Date = DateTime.Now,
                Symbol_Id = machine.Symbol_Id
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = base_uribase;
               
                var postTask = client.PostAsJsonAsync<Votings>("Voting", votings);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    httpContext.HttpContext?.Session.SetString("SuccessMsg", "Your vote Counted");
                    httpContext.HttpContext?.Session.SetString("Dashboard", "VD");
                }
                else
                {
                    httpContext.HttpContext?.Session.SetString("SuccessMsg", "");
                    httpContext.HttpContext?.Session.SetString("ErrorMsg", "Something wrong, contact the Election Commission of India");
                }
            }            
            return View("VotingMachine");
        }

        public IActionResult VotingResult()
        {
            List<VotedList> voted = new List<VotedList>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.GetSection("ECApi").Value + "VotingSystem/");
                string strAction = "GetVotedList";
                var getResponse = client.GetAsync(strAction);
                getResponse.Wait();

                var response = getResponse.Result;
                if (response.IsSuccessStatusCode)
                {
                    var ec = response.Content.ReadAsAsync<List<VotedList>>();
                    ec.Wait();
                    voted = (List<VotedList>)ec.Result;
                    TempData["ErrorMsg"] = "";

                    if(voted != null && voted.Count > 0)
                    {
                        var winnerCandidate = voted
                            .GroupBy(x => x.mp_state)
                            .Select(g => new
                            {
                                majority = g.Max(x => x.Total_Vote),
                                party_name = g.First().party_name
                            })
                            .OrderByDescending(x => x.party_name);

                        var stateCnt = winnerCandidate.GroupBy(x=> x.party_name).Select(
                            g=> new
                            {
                                party_name = g.First().party_name,
                                majority = g.Count()
                            }
                            ).OrderByDescending(x=> x.majority);

                        var max1 = stateCnt.Take(1).FirstOrDefault();
                        var max2 = stateCnt.Skip(1).Take(1).FirstOrDefault();

                        if (max1.majority == max2.majority)
                        {
                            TempData["EWinner"] = "";
                            TempData["EWinnerVote"] = "";
                            return View(voted);
                        }

                        TempData["EWinner"] = max1.party_name;
                        TempData["EWinnerVote"] = max1.majority;
                    }
                    else
                    {
                        TempData["EWinner"] = "";
                        TempData["EWinnerVote"] = "";
                    }                    
                }
            }
            return View(voted);
        }
    }
}
