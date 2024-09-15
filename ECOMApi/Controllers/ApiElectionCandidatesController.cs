using Microsoft.AspNetCore.Mvc;
using EcomCore;

// changes

namespace ECOMApi.Controllers
{
    [ApiController]
    [Route("ElectionCandidates")]
    public class ApiElectionCandidatesController : Controller
    {
        private readonly ElectionCandidateCore? _electionCandidate;

        public ApiElectionCandidatesController()
        {
            _electionCandidate = new ElectionCandidateCore();
        }

        [HttpPost]
        [Route("AddCandidate")]
        public ActionResult AddCandidate([FromBody]ElectionCandidates candidates)
        {
            var seqNo = _electionCandidate?.GetMaxOfId();
            candidates.Id =(int)seqNo;

            var strResult = _electionCandidate?.Register(candidates);
            return new JsonResult(strResult);
        }

        [HttpPut]
        [Route("UpdateCandidate")]
        public ActionResult UpdateCandidate([FromBody]ElectionCandidates candidates)
        {
            var strResult = _electionCandidate?.Update(candidates);
            return new JsonResult(strResult);
        }

        [HttpGet]
        [Route("GetCandidate")]
        public ActionResult GetCandidate(int id)
        {
            var strResult = _electionCandidate?.GetCandidate(id);
            return new JsonResult(strResult);
        }

        [HttpDelete]
        [Route("DeleteCandidate")]
        public ActionResult DeleteCandidate(int id)
        {
            var strResult = _electionCandidate?.DeleteCandidate(id);
            return new JsonResult(strResult);
        }

        [HttpGet]
        [Route("GetCandidateList")]
        public ActionResult GetCandidateList()
        {
            var strResult = _electionCandidate?.GetCandidateList();
            return new JsonResult(strResult);
        }
    }
}
