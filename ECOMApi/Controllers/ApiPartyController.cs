using Microsoft.AspNetCore.Mvc;
using EcomCore;
using ECOMApi.Models;

namespace ECOMApi.Controllers
{
    [ApiController]
    [Route("Party")]
    public class ApiPartyController : Controller
    {
        private readonly PartyMaster? _parties;

        public ApiPartyController()
        {
            _parties = new PartyMaster();
        }

        [HttpPost]
        [Route("Register")]
        public ActionResult AddParty([FromBody]Party party)
        {
            var seqNo = _parties?.GetMaxOfId();
            Parties objParty = new()
            {
                Id = (int)seqNo,
                Party_Name = party.Party_Name,
                Symbol_Id = party.Symbol_Id,
                Register_Date = DateTime.Now
            };
            var strResult = _parties?.Register(objParty);
            return new JsonResult(strResult);
        }

        [HttpPut]
        [Route("Correction")]
        public ActionResult UpdateParty([FromBody] Parties party)
        {
            var strResult = _parties?.Update(party);
            return new JsonResult(strResult);
        }

        [HttpGet]
        [Route("GetParty")]
        public ActionResult GetParty(int id)
        {
            var strResult = _parties?.GetParties(id);
            return new JsonResult(strResult);
        }

        [HttpGet]
        [Route("GetPartyList")]
        public ActionResult GetPartyList()
        {
            var strResult = _parties?.GetPartyList();
            return new JsonResult(strResult);
        }

        [HttpDelete]
        [Route("DeleteParties")]
        public ActionResult DeleteParties(int id)
        {
            var strResult = _parties?.DeleteParties(id);
            return new JsonResult(strResult);
        }
    }
}
