using EcomCore.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomCore
{
    public class PartyMaster
    {
        public PartyMaster() 
        {
        }
        public string? Register(Parties parties)
        {
            string? strResult = null;
            try
            {
                string insertQuery = "insert into PartyMaster(id,Party_Name, Symbol_Id, Register_Date) " +
                    "values ( "+ parties.Id + " ,'"+ parties.Party_Name + "', '"+ parties.Symbol_Id  + "', '" + parties.Register_Date + "')";

                strResult = ECADB.Insert(insertQuery);
                return strResult;
            }
            catch
            {
                strResult = "Fail";
                return strResult;
            }
        }
        public string? Update(Parties Party)
        {
            string? strResult = null;
            try
            {
                string updateQuery = "update PartyMaster set " +
                    " Party_Name = '" + Party.Party_Name +
                    "', Symbol_id = '" + Party.Symbol_Id +
                    "' where Id = " + Party.Id + "";

                strResult = ECADB.Update(updateQuery);
                return strResult;
            }
            catch
            {
                strResult = AppMessages.MSG_UPDATE_FAILED;
                return strResult;
            }
        }
        public List<PartyLists> GetPartyList()
        {
            List<PartyLists> Parties = new List<PartyLists>();
            try
            {
                string getQuery = "select p.*,s.symbol from PartyMaster p, Symbol_Master s" +
                    " where p.symbol_id = s.id order by Party_Name asc";

                var objParties = ECADB.GetRecords(getQuery);

                if (objParties != null)
                {
                    DataTable? dtparties = (DataTable)objParties;
                    Parties = (from DataRow dr in dtparties.Rows
                               select new PartyLists
                               {
                                   Id = Convert.ToInt32(dr["id"].ToString()),
                                   Symbol = (byte[])dr["Symbol"],
                                   Symbol_Id =(int)dr["Symbol_id"],
                                   Party_Name= dr["Party_Name"].ToString(),
                                   Register_Date =(DateTime)dr["Register_Date"]
                               }).ToList();
                }
                return Parties;
            }
            catch
            {
                return Parties;
            }
        }
        public PartyLists? GetParties(int id)
        {
            PartyLists Parties = new();
            try
            {
                string getQuery = "select p.*,s.symbol from PartyMaster p, Symbol_Master s " +
                    " where p.symbol_id = s.id and p.id = " + id;
                var objParties = ECADB.GetRecords(getQuery);
                if (objParties != null)
                {
                    DataTable? dtparties = (DataTable)objParties;
                    Parties = (from DataRow dr in dtparties.Rows
                               select new PartyLists
                               {
                                   Id = Convert.ToInt32(dr["id"].ToString()),
                                   Symbol = (byte[])dr["Symbol"],
                                   Symbol_Id = (int)dr["Symbol_id"],
                                   Party_Name = dr["Party_Name"].ToString(),
                                   Register_Date = (DateTime)dr["Register_Date"]
                               }).First<PartyLists>();
                }
                return Parties;
            }
            catch
            {
                return Parties;
            }
        }
        public int GetMaxOfId()
        {
            int result = 1;
            try
            {
                string getQuery = "select COALESCE(max(id),0)+1 from PartyMaster";

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
        public int DeleteParties(int id)
        {
            int result = 0;
            try
            {
                string getQuery = "delete from PartyMaster where id = " + id;

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
