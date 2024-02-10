using EcomCore.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomCore
{
    public class VotingSystem
    {
        public VotingSystem() { }

        public string? Voting(Votings vote)
        {
            string? strResult = null;
            try
            {
                string insertQuery = "insert into Voting(id,Voting_Date,Party_Id, Party_Name, Symbol_Id, Voter_id) " +
                    "values ("+ vote.Id +",'" + vote.Voting_Date + "'," + vote.Party_Id + ", '" + vote.Party_Name + 
                    "', '" + vote.Symbol_Id + "', '" + vote.Voter_id + "')";

                strResult = ECADB.Insert(insertQuery);
                return strResult;
            }
            catch
            {
                strResult = AppMessages.MSG_SAVE_FAILED;
                return strResult;
            }
        }

        public int GetMaxOfId()
        {
            int strResult = 0;
            try
            {
                string getQuery = "select COALESCE(max(id),0)+1 from Voting";
                var objVoter = ECADB.GetRecords(getQuery);
                if (objVoter != null)
                {
                    var dt = (DataTable)objVoter;
                    strResult = (int)dt.Rows[0][0];
                }
                return strResult;
            }
            catch
            {
                return strResult;
            }
        }
        public List<MachineVoteList> MachineVoteList(int VoterStateId)
        {
            List<MachineVoteList> objMachineList  = new List<MachineVoteList>();
            try
            {
                string getQuery = "select p.id as party_id, p.party_name, s.id as symbol_Id, s.symbol,sa.id as state_id, " + " sa.mp_state as state, sa.total_seats, e.mp_seat as party_total_seat from partymaster p, symbol_master s, " + " seatallocation sa, election_candidates e where p.symbol_id = s.id and p.id = e.party_id and e.seat_state_id = " + " sa.id and sa.id = " + VoterStateId + " order by Party_name asc";

                var machineVoteList = ECADB.GetRecords(getQuery);

                if (machineVoteList != null)
                {
                    DataTable? dtSymbl = (DataTable)machineVoteList;
                    objMachineList = (from DataRow dr in dtSymbl.Rows
                              select new MachineVoteList
                              {
                                  Party_Id = (int)dr["Party_Id"],
                                  Party_Name = dr["Party_Name"].ToString(),
                                  Symbol_Id = (int)dr["Symbol_Id"],
                                  symbol =(byte[])dr["symbol"],
                                  state_id = (int)dr["state_id"],
                                  state = dr["state"].ToString(),
                                  total_seats = (int)(dr["total_seats"]),
                                  party_total_seat = (int)dr["party_total_seat"]
                              }).ToList();
                }

                return objMachineList;
            }
            catch
            {
                return objMachineList;
            }
        }
                
        public List<VotedList> GetVotedList()
        {
            List<VotedList> voteings = new();
            try
            {
                string getQuery = "select party_name, mp_state, total_vote," +
                    " (case when total_vote >= (select max(total_vote) from " +
                    " v_vote_majarity where mp_state =v.mp_state) then " +
                    " 'Winner' else 'Loser' end) as EResult from v_vote_majarity v " +
                    " order by mp_state,total_vote desc";

                var objVotedList = ECADB.GetRecords(getQuery);

                if (objVotedList != null)
                {
                    DataTable? dtSymbl = (DataTable)objVotedList;
                    voteings = (from DataRow dr in dtSymbl.Rows
                                      select new VotedList
                                      {
                                          party_name = dr["party_name"].ToString(),
                                          mp_state = dr["mp_state"].ToString(),
                                          Total_Vote = Convert.ToInt32(dr["Total_Vote"]),
                                          EResult = dr["EResult"].ToString()
                                      }).ToList();
                }
                return voteings;
            }
            catch
            {
                return voteings;
            }
        }

    }
}
