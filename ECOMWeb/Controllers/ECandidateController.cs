using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using EcomCore;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Rendering;
using ECOMWeb.Common;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace ECOMWeb.Controllers
{
    public class ECandidateController : Controller
    {
        private IConfiguration _configuration;
        private Uri base_uribase;
        private readonly ILogger<SymbolController> _logger;
        private Commons _commons;

        public ECandidateController(ILogger<SymbolController> logger, IConfiguration iconfiguration)
        {
            _logger = logger;
            _configuration = iconfiguration;
            _commons = new Commons(iconfiguration);
            base_uribase = new Uri(_configuration.GetSection("ECApi").Value + "ElectionCandidates/");
        }
        
        [HttpGet]
        public IActionResult Add()
        {
            PartyNameLoad();
            StateListLoad();
            return View();
        }       

        [HttpPost]
        public ActionResult Add(CandidateList ec)
        {
            using(var client= new HttpClient())
            {
                client.BaseAddress = base_uribase;
                string strAction = "AddCandidate";
                var pid = Request.Form["PartyList"];
                var sid = Request.Form["StateList"];

                var ec1 = new ElectionCandidates() 
                {
                    MP_Seat = ec.MP_Seat,
                    Seat_State_Id = Convert.ToInt16(sid),
                    Party_Id= Convert.ToInt16(pid)
                };

                var postResponse = client.PostAsJsonAsync<ElectionCandidates>(strAction, ec1);
                postResponse.Wait();

                var response = postResponse.Result;
                if(response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Lists");
                }
            }
            return View(ec);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = base_uribase;
                string strAction = "GetCandidate?id=" + id;
                CandidateList candidate = new CandidateList();
                var getResponse = client.GetAsync(strAction);
                getResponse.Wait();
                
                var response = getResponse.Result;
                if (response.IsSuccessStatusCode)
                {
                    var ec = response.Content.ReadAsAsync<CandidateList>();                    
                    ec.Wait();
                    candidate = (CandidateList)ec.Result;
                    PartyNameLoad();
                    StateListLoad();
                    return View(candidate);
                }
            }
            return RedirectToAction("Lists");
        }
        [HttpPost]
        public IActionResult Edit(CandidateList candidates)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = base_uribase;
                string strAction = "UpdateCandidate";

                var pid = Request.Form["PartyList"];
                var sid = Request.Form["StateList"];

                var ec1 = new ElectionCandidates()
                {
                    MP_Seat = candidates.MP_Seat,
                    Seat_State_Id = Convert.ToInt16(sid),
                    Party_Id = Convert.ToInt16(pid)
                };

                var postResponse = client.PutAsJsonAsync<ElectionCandidates>(strAction, ec1);
                postResponse.Wait();

                var response = postResponse.Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Lists");
                }
            }
            return RedirectToAction("Lists");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = base_uribase;
                string strAction = "DeleteCandidate?id=" + id;

                var getResponse = client.DeleteAsync(strAction);
                getResponse.Wait();

                var response = getResponse.Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Lists");
                }
            }
            return View();
        }
        [HttpGet]
        public IActionResult Lists()
        {
            List<CandidateList> candidateLists = new List<CandidateList>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = base_uribase;
                string strAction = "GetCandidateList";                
                var getResponse = client.GetAsync(strAction);
                getResponse.Wait();
                var response = getResponse.Result;
                if (response.IsSuccessStatusCode)
                {
                    var ec = response.Content.ReadAsAsync<List<CandidateList>>();
                    ec.Wait();

                    candidateLists = (List<CandidateList>)ec.Result;
                    return View(candidateLists);
                }
            }
            return View(candidateLists);
        }
        private void PartyNameLoad()
        {
            try
            {
                List<Parties> partyLists = new List<Parties>();
                partyLists = _commons.PartyList();

                var plists = (from a in partyLists
                              select new SelectListItem
                              {
                                  Text = a.Party_Name,
                                  Value = Convert.ToString(a.Id),
                                  Selected = false,
                              }).ToList();
                ViewData["PartyList"] = plists;
            }
            catch
            {

            }
        }
        public void StateListLoad()
        {
            try
            {
                List<SeatAllocation> seats = new List<SeatAllocation>();
                seats = _commons.StateList();

                var slists = (from a in seats
                              select new SelectListItem
                              {
                                  Text = a.MP_state,
                                  Value = Convert.ToString(a.Id),
                                  Selected = false,
                              }).ToList();
                ViewData["StateList"] = slists;

            }
            catch
            {

            }
        }
    }
}
