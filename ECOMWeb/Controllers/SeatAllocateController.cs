using EcomCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;

namespace ECOMWeb.Controllers
{
    public class SeatAllocateController : Controller
    {
        private readonly IConfiguration _configuration;
        private Uri base_uribase;
        private readonly ILogger<SeatAllocateController> _logger;

        public SeatAllocateController(ILogger<SeatAllocateController> logger, IConfiguration iconfiguration)
        {
            _logger = logger;
            _configuration = iconfiguration;
            base_uribase = new Uri(_configuration.GetSection("ECApi").Value + "SeatAllocation/");
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(SeatAllocation allocation)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = base_uribase;
                var postTask = client.PostAsJsonAsync<SeatAllocation>("Register", allocation);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Lists");
                }
            }
            return View(allocation);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            SeatAllocation sList = new();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = base_uribase;
                    var responseTask = client.GetAsync("GetSeatAllocated?id=" + id);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readSymbols = result.Content.ReadAsAsync<SeatAllocation>();
                        readSymbols.Wait();
                        sList = (SeatAllocation)readSymbols.Result;
                        return View(sList);
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
        public IActionResult Edit(SeatAllocation allocation)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = base_uribase;                    
                    var responseTask = client.PutAsJsonAsync<SeatAllocation>("Correction", allocation);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Lists");
                    }
                }
                return View(allocation);
            }
            catch
            {
                return View(allocation);
            }
        }

        [HttpGet]
        public IActionResult Lists()
        {
            List<SeatAllocation> sList = new();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = base_uribase;

                    var responseTask = client.GetAsync("GetSeatAllocatedList");
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readSymbols = result.Content.ReadAsAsync<List<SeatAllocation>>();
                        readSymbols.Wait();

                        sList = (List<SeatAllocation>)readSymbols.Result;
                        ViewData["TotalCount"]= sList.Sum(x => x.Total_seats);
                    }
                }
                return View(sList);
            }
            catch
            {
                return View();
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
