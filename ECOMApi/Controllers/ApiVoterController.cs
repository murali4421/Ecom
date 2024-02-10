using Microsoft.AspNetCore.Mvc;
using EcomCore;
using ECOMApi.Models;

namespace ECOMApi.Controllers
{
    [ApiController]
    [Route("Voter")]
    public class ApiVoterController : Controller
    {
        private readonly VoterMaster _voterMaster;
        public ApiVoterController()
        {
            _voterMaster = new VoterMaster();
        }              

        [HttpPost]
        [Route("Register")]
        public ActionResult Register([FromBody]Voters voters)
        {
            int id = _voterMaster.GetMaxOfId();
            Voters objVoter = new() 
            { 
                Id = id,
                Voter_Id = voters.Voter_Id,
                Voter_Name= voters.Voter_Name,
                Voter_State_Id= voters.Voter_State_Id,
                Voter_Verified= 'N',
                City = voters.City,
                Address = voters.Address,
                Photo = voters.Photo
            };

            var strResult = _voterMaster?.Register(objVoter);
            return new JsonResult(strResult);
        }

        [HttpPost]
        [Route("Correction")]
        public ActionResult VoterCorrection([FromBody]Voters voters)
        {
            var strResult = _voterMaster?.Update(voters);
            return new JsonResult(strResult);
        }

        [HttpGet]
        [Route("VoterIdVerify")]
        public ActionResult VoterIdVerify(int id)
        {
            var strResult = _voterMaster?.VoterIdVerify(id);
            return new JsonResult(strResult);
        }

        [HttpGet]
        [Route("GetVoter")]
        public ActionResult GetVoter(int id)
        {
            var strResult = _voterMaster?.GetVoter(id);
            return new JsonResult(strResult);
        }

        [HttpGet]
        [Route("GetVoterList")]
        public ActionResult GetVoterList()
        {
            var strResult = _voterMaster?.GetVoterList();
            return new JsonResult(strResult);
        }
               
    }
}
