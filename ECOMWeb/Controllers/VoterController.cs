using EcomCore;
using ECOMWeb.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using EcomCore;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace ECOMWeb.Controllers
{
    public class VoterController : Controller
    {
        private IConfiguration _configuration;
        private Uri base_uribase;
        private readonly ILogger<VoterController> _logger;
        private Commons _commons;

        public VoterController(ILogger<VoterController> logger, IConfiguration iconfiguration)
        {
            _logger = logger;
            _configuration = iconfiguration;
            _commons = new Commons(iconfiguration);
            base_uribase = new Uri(_configuration.GetSection("ECApi").Value + "Voter/");
        }
        public IActionResult Index() // List the registered Voter's. 
        {
            return View();
        }

        [HttpGet]
        public IActionResult Add()
        {
            try
            {
                StateListLoad(false,0);
                HttpContextAccessor httpContext = new();
                if (httpContext.HttpContext.Session.GetString("Dashboard") == "VMD")
                {
                    httpContext.HttpContext.Session.SetString("Dashboard", "NA");
                }
            }
            catch
            {
            }
            return View();
        }

        [HttpPost]
        public IActionResult Add(Voters voter)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = base_uribase;
                string strAction = "Register";
                var sid = Request.Form["StateList"];
                IFormFile? file = Request.Form.Files["VoterPhoto"];

                if (file == null)
                {
                    ModelState.AddModelError("Photo", "Upload the Photo");
                    StateListLoad(false, 0);
                    return View(voter);
                }
                if (voter.Voter_State_Id == 0)
                {
                    ModelState.AddModelError("Voter_State_Id", "Choose the State");
                    StateListLoad(false, 0);
                    return View(voter);
                }


                voter.Voter_State_Id = Convert.ToInt32(sid);
                int result = SendToDb(file, voter, 'I');
                if (result==1)
                {
                    HttpContextAccessor context = new();
                    if (context.HttpContext.Session.GetString("Dashboard") != "NA")
                    {
                        return RedirectToAction("Lists");
                    }
                    else
                    {
                        return RedirectToAction("Add");
                    }
                }
            }
            return View(voter);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            VoterList vote = new VoterList();
            using (var client = new HttpClient())
            {
                client.BaseAddress = base_uribase;
                string strAction = "GetVoter?id="+id;
                var getResponse = client.GetAsync(strAction);
                getResponse.Wait();

                var response = getResponse.Result;
                if (response.IsSuccessStatusCode)
                {
                    var ec = response.Content.ReadAsAsync<VoterList>();
                    ec.Wait();
                    vote = (VoterList)ec.Result;
                    StateListLoad(true, vote.Voter_State_Id);
                    VerifiedFlagLoad(true, vote.Voter_Verified.ToString());
                    ViewBag.id = id;                    
                    return View(vote);
                }
                return RedirectToAction("Lists");
            }
        }

        [HttpPost]
        public IActionResult Edit(Voters voter)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = base_uribase;
                string strAction = "Correction";

                var sid = Request.Form["StateList"];
                var flag = Request.Form["vFlag"];
                voter.Voter_Verified = Convert.ToChar(flag);
                voter.Voter_State_Id = Convert.ToInt32(sid);
                IFormFile? file = Request.Form.Files["VoterPhoto"];

                if (file == null)
                {
                    ModelState.AddModelError("Photo", "Upload the Photo");
                    StateListLoad(false, 0);
                    return View(voter);
                }
                if (voter.Voter_State_Id == 0)
                {
                    ModelState.AddModelError("Voter_State_Id", "Choose the State");
                    StateListLoad(false, 0);
                    return View(voter);
                }

                voter.Voter_State_Id = Convert.ToInt32(sid);
                int result = SendToDb(file, voter, 'U');
                if (result == 1)
                {
                    return RedirectToAction("Lists");
                }
            }
            return RedirectToAction("Lists");
        }

        [HttpGet]
        public IActionResult VoterIdVerify(int id)
        {
            Voters vote = new Voters();
            using (var client = new HttpClient())
            {
                client.BaseAddress = base_uribase;
                string strAction = "VoterIdVerify?id=" + id;
                var getResponse = client.GetAsync(strAction);
                getResponse.Wait();

                var response = getResponse.Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Lists");
                }                
            }
            return RedirectToAction("Lists");
        }

        [HttpGet]
        public IActionResult Lists()
        {
            List<VoterList> voterLists = new List<VoterList>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = base_uribase;
                string strAction = "GetVoterList";
                var getResponse = client.GetAsync(strAction);
                getResponse.Wait();
                var response = getResponse.Result;
                if (response.IsSuccessStatusCode)
                {
                    var vote = response.Content.ReadAsAsync<List<VoterList>>();
                    vote.Wait();

                    voterLists = (List<VoterList>)vote.Result;
                    return View(voterLists);
                }
            }
            return View(voterLists);
        }

        public ActionResult RetrieveImage(int id)
        {
            Voters vote = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = base_uribase;

                    var responseTask = client.GetAsync("GetVoter?id=" + id);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readSymbols = result.Content.ReadAsAsync<Voters>();
                        readSymbols.Wait();

                        vote = (Voters)readSymbols.Result;
                    }
                }

                if (vote != null)
                {                    
                    byte[] cover = vote.Photo;
                    if (cover != null)
                    {
                        return File(cover, "image/jpg");
                    }
                    else
                    {
                        return View();
                    }
                }
                return View();
            }
            catch
            {
                return View();
            }
        }
        public int SendToDb(IFormFile file, Voters voter, char flag)
        {
            voter.Photo = ConvertToBytes(file);            
            using (var client = new HttpClient())
            {
                client.BaseAddress = base_uribase;
                string strAction = "";
                if (flag == 'I')
                {
                    strAction = "Register";
                }
                else if (flag == 'U')
                {
                    strAction = "Correction";
                }
                var postResult = client.PostAsJsonAsync<Voters>(strAction, voter);
                postResult.Wait();
                var result = postResult.Result;
                if (result.IsSuccessStatusCode)
                {
                    return 1;
                }
            }
            return 0;
        }
        public byte[] ConvertToBytes(IFormFile image)
        {
            try
            {
                if (image == null)
                    return null;

                byte[] imageBytes = null;
                BinaryReader reader = new BinaryReader(new BufferedStream(image.OpenReadStream()));
                imageBytes = reader.ReadBytes((int)image.Length);
                return imageBytes;
            }
            catch
            {
                return null;
            }            
        }

        public void VerifiedFlagLoad(bool isSelected, string item)
        {
            try
            {
                List<string> vflag = new List<string>() { "N","Y"};
                var vFlag = (from string dr in vflag
                             select new SelectListItem
                             {
                                 Text = dr,
                                 Value = dr,
                                 Selected = isSelected && item == dr ? true : false
                             });
                ViewData["vFlag"] = vFlag;
            }
            catch
            {
            }
        }

        public void StateListLoad(bool isSelected, int itemId)
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
                                  Selected = isSelected && itemId == a.Id ? true :false,
                              }).ToList();
                ViewData["StateList"] = slists;

            }
            catch
            {

            }
        }
    }
}
