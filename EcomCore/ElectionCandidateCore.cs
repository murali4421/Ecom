using EcomCore.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomCore
{
    public class ElectionCandidateCore
    {
        public ElectionCandidateCore() { }

        public string? Register(ElectionCandidates ec)
        {
            string? strResult = null;
            try
            {
                string insertQuery = "insert into Election_Candidates(id,Party_Id,MP_Seat, Seat_State_Id) " +
                    "values ("+ ec.Id +","+ ec.Party_Id + "," + ec.MP_Seat + ", '" + ec.Seat_State_Id + "')";

                strResult = ECADB.Insert(insertQuery);
                return strResult;
            }
            catch
            {
                strResult = AppMessages.MSG_SAVE_FAILED;
                return strResult;
            }
        }

        public string? Update(ElectionCandidates ec)
        {
            string? strResult = null;
            try
            {
                string updateQuery = "update Election_Candidates set Party_Id = '" + ec.Party_Id +
                    "',MP_Seat = " + ec.MP_Seat + ", Seat_State_Id ='" + ec.Seat_State_Id + "' where Id = " + 
                    ec.Id + "";

                strResult = ECADB.Update(updateQuery);
                return strResult;
            }
            catch
            {
                strResult = AppMessages.MSG_UPDATE_FAILED;
                return strResult;
            }
        }

        public List<CandidateList> GetCandidateList()
        {
            List<CandidateList> ecList = new();
            try
            {
                string getQuery = "select e.id,p.id as party_id, p.Party_Name, e.mp_seat, s.mp_state " +
                ", e.seat_state_id from Election_Candidates e, PartyMaster p, seatallocation s where e.party_id = p.id " +
                " and s.id = e.seat_state_id order by p.Party_Name asc";

                var objECList = ECADB.GetRecords(getQuery);

                if (objECList != null)
                {
                    DataTable? dtparties = (DataTable)objECList;
                    ecList = (from DataRow dr in dtparties.Rows
                               select new CandidateList
                               {
                                   Id = Convert.ToInt32(dr["id"]),
                                   Party_Id = Convert.ToInt32(dr["party_id"]),
                                   Party_Name = dr["Party_Name"].ToString(),
                                   MP_Seat = Convert.ToInt32(dr["mp_seat"]),
                                   MP_State = dr["mp_state"].ToString(),
                                   Seat_State_Id = Convert.ToInt32(dr["seat_state_id"])
                               }).ToList();
                }
                return ecList;
            }
            catch
            {
                return ecList;
            }
        }

        public CandidateList? GetCandidate(int id)
        {
            CandidateList candidate = new();
            try
            {
                string getQuery = "select e.id,p.id as party_id, p.Party_Name, e.mp_seat, s.mp_state " +
                " , e.seat_state_id from Election_Candidates e, PartyMaster p, seatallocation s where e.party_id = p.id " +
                " and s.id = e.seat_state_id and e.id = " + id +" order by p.Party_Name asc";

                var objECList = ECADB.GetRecords(getQuery);

                if (objECList != null)
                {
                    DataTable? dtparties = (DataTable)objECList;
                    candidate = (from DataRow dr in dtparties.Rows
                              select new CandidateList
                              {
                                  Id = Convert.ToInt32(dr["id"]),
                                  Party_Id = Convert.ToInt32(dr["party_id"]),
                                  Party_Name = dr["Party_Name"].ToString(),
                                  MP_Seat = Convert.ToInt32(dr["mp_seat"]),
                                  MP_State = dr["mp_state"].ToString(),
                                  Seat_State_Id = Convert.ToInt32(dr["seat_state_id"])
                              }).First<CandidateList>();
                }

                return candidate;
            }
            catch
            {
                return candidate;
            }
        }

        public int DeleteCandidate(int id)
        {
            try
            {
                string getQuery = "delete from Election_Candidates where id = " + id;

                var objECList = ECADB.Delete(getQuery);

                if (objECList != null && AppMessages.MSG_DELETE_SUCCESS == objECList)
                {
                    return 1;
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        public int GetMaxOfId()
        {
            int result = 1;
            try
            {
                string getQuery = "select COALESCE(max(id),0)+1 from Election_Candidates";

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
    }
}
