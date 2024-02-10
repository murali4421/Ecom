using Microsoft.AspNetCore.Mvc;
using EcomCore;
using ECOMApi.Models;
using System.ComponentModel.DataAnnotations;

namespace ECOMApi.Controllers
{
    [ApiController]
    [Route("Symbol")]
    public class ApiSymbolController : Controller
    {
        private SymbolMaster? _symbols;

        public ApiSymbolController()
        {
            _symbols = new SymbolMaster();
        }

        [HttpPost]
        [Route("Registation")]
        public string AddSymbol(Symb symbols)
        {
            int seqNo = _symbols.GetMaxOfId();
            Symbols objSymbol = new()
            {
                Id = seqNo,
                Symbol = symbols.Symbol,
                Symbol_Name = symbols.Symbol_Name
            };

            var strResult = _symbols?.Register(objSymbol);
           return strResult;
        }

        [HttpPost]
        [Route("Correction")]
        public string UpdateSymbol([FromBody] Symbols symbols)
        {
            var strResult = _symbols?.Update(symbols);
            return strResult;
        }

        [HttpGet]
        [Route("GetSymbol")]
        public Symbols GetSymbol([FromBody] string symbolName)
        {
            var strResult = _symbols?.GetSymbol(symbolName);
            return strResult;
        }

        [HttpGet]
        [Route("GetAllSymbols")]
        public List<Symbols> GetAllSymbols()
        {
            var strResult = _symbols?.GetSymbols();
            return  strResult;
        }

        [HttpDelete]
        [Route("DeleteSymbol")]
        public ActionResult DeleteSymbol(int id)
        {
            if (id <= 0)
                return BadRequest("Not a valid Symbol delete");

            var strResult = (_symbols?.DeleteSymbol(id));
            return Ok(strResult);
        }
    }
}
