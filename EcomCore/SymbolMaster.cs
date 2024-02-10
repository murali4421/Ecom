using EcomCore.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EcomCore
{
    public class SymbolMaster
    {
        public SymbolMaster() { }
        public string? Register(Symbols symbol)
        {
            string? strResult = null;
            try
            {
                string insertQuery = "insert into Symbol_Master(id, Symbol, Symbol_Name) " +
                    "values ("+ symbol.Id + ",@image, '" + symbol.Symbol_Name + "')";

                if (symbol.Symbol == null)
                {
                    insertQuery = insertQuery.Replace(",@image", "");
                }
                strResult = ECADB.Insert(insertQuery, symbol.Symbol);
                return strResult;
            }
            catch
            {
                strResult = "Fail";
                return strResult;
            }
        }

        public string? Update(Symbols symbol)
        {
            string? strResult = null;
            try
            {                
                string updateQuery = "update Symbol_Master set " +
                    " Symbol_Name = '" + symbol.Symbol_Name +
                    "',Symbol = @image where Id = " + symbol.Id + "";
                if (symbol.Symbol == null)
                {
                    updateQuery = updateQuery.Replace(",Symbol = @image", "");
                }
                strResult = ECADB.Update(updateQuery, symbol.Symbol);
                return strResult;
            }
            catch
            {
                strResult = AppMessages.MSG_UPDATE_FAILED;
                return strResult;
            }
        }

        public int GetMaxOfId()
        {
            int result = 1;
            try
            {
                string getQuery = "select COALESCE(max(id),0)+1 from Symbol_Master";

                var objSymbols = ECADB.GetRecords(getQuery);

                if (objSymbols != null)
                {
                   var dt = (DataTable)objSymbols;
                   result = (int)dt.Rows[0][0];
                }

                return result;
            }
            catch
            {
                return result;
            }
        }

        public List<Symbols> GetSymbols()
        {
            List<Symbols> symbols = new();
            try
            {
                string getQuery = "select * from Symbol_Master order by Symbol_Name asc";

                var objSymbols = ECADB.GetRecords(getQuery);

                if (objSymbols != null)
                {
                    DataTable? dtSymbl = (DataTable)objSymbols;
                    symbols = (from DataRow dr in dtSymbl.Rows
                               select new Symbols
                               {
                                   Id = Convert.ToInt32(dr["id"].ToString()),
                                   Symbol = (byte[])dr["Symbol"],
                                   Symbol_Name = dr["Symbol_Name"].ToString()
                               }).ToList();                                       
                }
                return symbols;
            }
            catch
            {
                return symbols;
            }
        }

        public Symbols? GetSymbol(string SymbolName)
        {
            List<Symbols> symbols = new();
            Symbols? symbol = new();
            try
            {
                symbols = this.GetSymbols();

                if (symbols != null && symbols.Count > 0 && SymbolName != null)
                {
                    symbol = symbols.FirstOrDefault(x => x.Symbol_Name == SymbolName);
                }

                return symbol;
            }
            catch
            {
                return symbol;
            }
        }

        public int DeleteSymbol(int id)
        {
            int result = 0;
            try
            {
                string getQuery = "delete from Symbol_Master where id = " + id;

                var objSymbols = ECADB.Delete(getQuery);

                if (objSymbols != null && AppMessages.MSG_DELETE_SUCCESS == objSymbols)
                {
                    return 1;
                }

                return result;
            }
            catch
            {
                return result;
            }
        }
    }
}
