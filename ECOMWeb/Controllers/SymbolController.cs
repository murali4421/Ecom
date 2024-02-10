using Microsoft.AspNetCore.Mvc;
using EcomCore;
using Npgsql;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;

namespace ECOMWeb.Controllers
{
    public class SymbolController : Controller
    {
        private IConfiguration _configuration;
        private Uri base_uribase;
        private readonly ILogger<SymbolController> _logger;

        public SymbolController(ILogger<SymbolController> logger, IConfiguration iconfiguration)
        {
            _logger = logger;
            _configuration = iconfiguration;
            base_uribase = new Uri(_configuration.GetSection("ECApi").Value + "symbol/");
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Add(Symbols model)
        {
            IFormFile file = Request.Form.Files["SymbolImage"];

            if(file == null)
                ModelState.AddModelError("Symbol", "Symbol image required");

            if (file != null)
            {
                int i = UploadImageToDb(file, model,'I');
                
                if (i == 1)
                {
                    return RedirectToAction("Lists");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            List<Symbols> symbols = null;
            Symbols objSymbols = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = base_uribase;

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

                if (symbols != null && symbols.Count > 0)
                {
                    IEnumerable<Symbols> enumerable()
                    {
                        foreach (Symbols img in symbols)
                        {
                            if (img.Id == id)
                            {
                                yield return img;
                            }
                        }
                    }
                    objSymbols = enumerable().First<Symbols>();
                    if(objSymbols !=null)
                    {
                        ViewBag.sId = objSymbols.Id;
                    }                    
                }
                return View(objSymbols);
            }
            catch
            {
                return View(objSymbols);
            }
        }
        
        [HttpPost]
        public IActionResult Edit(Symbols model)
        {
            IFormFile file = Request.Form.Files["SymbolImage"];

            if (file == null)
                ModelState.AddModelError("Symbol", "Symbol image required");

            int i = UploadImageToDb(file, model, 'U');
            if (i == 1)
            {
                return RedirectToAction("Lists");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            int symbols = 0;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = base_uribase;

                    var responseTask = client.DeleteAsync("DeleteSymbol?id="+id);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readSymbols = result.Content.ReadAsAsync<int>();
                        readSymbols.Wait();

                        symbols = readSymbols.Result;
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

        public ActionResult Lists()
        {
            IEnumerable<Symbols> symbols = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = base_uribase;

                    var responseTask = client.GetAsync("GetAllSymbols");
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readSymbols = result.Content.ReadAsAsync<List<Symbols>>();
                        readSymbols.Wait();

                        symbols = (List<Symbols>)readSymbols.Result;                        
                    }
                    else
                    {
                        symbols = Enumerable.Empty<Symbols>();

                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }
            }
            catch
            {
                symbols = Enumerable.Empty<Symbols>();

                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
            }
            
            return View(symbols);
        }
        public ActionResult RetrieveImage(int id)
        {
            List<Symbols> symbols = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = base_uribase;

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

                if (symbols != null && symbols.Count > 0)
                {
                    var q = from img in symbols where img.Id == id select img.Symbol;
                    byte[] cover = q.First();
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
        public int UploadImageToDb(IFormFile file, Symbols symbols, char flag)
        {
            symbols.Symbol = ConvertToBytes(file);
            var Symbol = new Symbols
            {
               Symbol = symbols.Symbol,
               Symbol_Name = symbols.Symbol_Name
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = base_uribase;
                string strAction="";

                if(flag == 'I')
                {
                    strAction = "Registation";
                }
                else if(flag == 'U')
                {
                    strAction = "Correction";
                }

                var postTask =  client.PostAsJsonAsync<Symbols>(strAction, symbols);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return 1;
                }
            }            
            return 0;
        }
        public byte[] ConvertToBytes(IFormFile image)
        {
            byte[]? imageBytes = null;
            if (image != null)
            {                
                BinaryReader reader = new BinaryReader(new BufferedStream(image.OpenReadStream()));
                imageBytes = reader.ReadBytes((int)image.Length);
            }
            return imageBytes;
        }
    }
}
