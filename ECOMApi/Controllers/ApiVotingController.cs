using Microsoft.AspNetCore.Mvc;
using EcomCore;

namespace ECOMApi.Controllers
{
    [ApiController]
    [Route("VotingSystem")]
    public class ApiVotingController : Controller
    {
        private readonly VotingSystem _votingSystem;
        public ApiVotingController( )
        {
            _votingSystem = new VotingSystem();
        }

        [HttpPost]
        [Route("Voting")]
        public ActionResult Voting([FromBody]Votings vote)
        {
            int id = _votingSystem.GetMaxOfId();

            vote.Id = id;
            vote.Voting_Date = DateTime.Now;
            var strResult = _votingSystem?.Voting(vote);
            return new JsonResult(strResult);
        }

        [HttpGet]
        [Route("GetVotedList")]
        public ActionResult GetVotedList()
        {
            var strResult = _votingSystem?.GetVotedList();
            return new JsonResult(strResult);
        }

        [HttpGet]
        [Route("GetMachineVoteList")]
        public ActionResult GetMachineVoteList(int voterStateId)
        {
            var strResult = _votingSystem?.MachineVoteList(voterStateId);
            return new JsonResult(strResult);
        }

        
    }
}
