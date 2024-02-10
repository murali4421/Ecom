using Microsoft.AspNetCore.Mvc;
using EcomCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECOMWeb.Controllers
{
    public class PartyController : Controller
    {
        private readonly IConfiguration _configuration;
        private Uri base_uribase;
        private readonly ILogger<PartyController> _logger;

        public PartyController(ILogger<PartyController> logger, IConfiguration iconfiguration)
        {
            _logger = logger;
            _configuration = iconfiguration;
            base_uribase = new Uri(_configuration.GetSection("ECApi").Value + "Party/");            
        }

        [HttpGet]
        public IActionResult Add()
        {
            ddlsymbol();
            return View();
        }

        private void ddlsymbol()
        {
            try
            {
                List<Symbols> partyLists = new List<Symbols>();
                partyLists = SymbolList();
                var slists = (from a in partyLists
                              select new SelectListItem
                              {
                                  Text = a.Symbol_Name,
                                  Value = Convert.ToString(a.Id),
                                  Selected = false,
                              }).ToList();
                ViewBag.SymbolList = slists;
                ViewData["SymbolList"] = slists;               
            }
            catch 
            {
                
            }
        }

        [HttpPost]
        public IActionResult Add(PartyLists parties)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = base_uribase;
                var symbolId = Request.Form["SymbolList"];
                var parties1 = new Parties()
                { 
                    Party_Name = parties.Party_Name,
                    Symbol_Id = Convert.ToInt16(symbolId)
                };
                
                var postTask = client.PostAsJsonAsync<Parties>("Register", parties1);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Lists");
                }
            }
            return View(parties);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            PartyLists pList = new();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = base_uribase;

                    var responseTask = client.GetAsync("GetParty?id="+id);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readSymbols = result.Content.ReadAsAsync<PartyLists>();
                        readSymbols.Wait();

                        pList = (PartyLists)readSymbols.Result;
                        
                        ddlsymbol();
                                                                        
                        return View(pList);
                    }
                }
                return RedirectToAction("Lists");
            }
            catch
            {
                return View(NotFound());
            }
        }

        [HttpPost]
        public IActionResult Edit(PartyLists parties)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = base_uribase;
                    var symbolId = Request.Form["SymbolList"];
                    var parties1 = new Parties()
                    {
                        Id = parties.Id,
                        Party_Name = parties.Party_Name,
                        Symbol_Id = Convert.ToInt16(symbolId)
                    };
                    var responseTask = client.PutAsJsonAsync<Parties>("Correction", parties1);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Lists");
                    }
                }
                return View(parties);
            }
            catch
            {
                return View(parties);
            }
        }

        [HttpGet]
        public IActionResult Lists()
        {
            List<PartyLists> pList = new();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = base_uribase;

                    var responseTask = client.GetAsync("GetPartyList");
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readSymbols = result.Content.ReadAsAsync<List<PartyLists>>();
                        readSymbols.Wait();

                        pList = (List<PartyLists>)readSymbols.Result;
                    }
                }
                return View(pList);
            }
            catch
            {
                return View(NotFound());
            }
        }

        public List<Symbols> SymbolList()
        {
            List<Symbols> symbols = new();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_configuration.GetSection("ECApi").Value + "symbol/");

                    var responseTask = client.GetAsync("GetAllSymbols");
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readSymbols = result.Content.ReadAsAsync<List<Symbols>>();
                        readSymbols.Wait();

                        symbols = (List<Symbols>)readSymbols.Result;
                    }
                }
                return symbols;
            }
            catch
            {
                return symbols;
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = base_uribase;
                    var responseTask = client.DeleteAsync("DeleteSeat?id=" + id);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                       return RedirectToAction("Lists");
                    }
                }
                return RedirectToAction("Lists");
            }
            catch
            {
                return RedirectToAction("Lists");
            }
        }
    }
}
