using EcomCore.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace EcomCore
{
    public class VoterMaster
    {
        public VoterMaster() { }
        public string? Register(Voters voter)
        {
            string? strResult = null;
            try
            {                
                string insertQuery = "insert into VoterMaster(id,Voter_Id, Voter_Name,Address, City, " +
                    " Voter_State_id, Photo, Voter_Verified ) " + " values ("+ voter.Id +",'" + voter.Voter_Id + "', '" +
                    voter.Voter_Name + "' , '" + voter.Address + "' , '" + voter.City + "', '" + 
                    voter.Voter_State_Id + "' ,@image , 'N' )";

                if(voter.Photo !=null)
                {
                    insertQuery = insertQuery.Replace(",@image", "");
                }
                strResult = ECADB.Insert(insertQuery, voter.Photo);
                return strResult;
            }
            catch
            {
                strResult = AppMessages.MSG_SAVE_FAILED;
                return strResult;
            }
        }
        public string? Update(Voters voter)
        {
            string? strResult = null;
            try
            {
                string updateQuery = "update VoterMaster set Voter_Name = '" + voter.Voter_Name +
                    "', Voter_Id = '" + voter.Voter_Id +
                    "', Address = '" + voter.Address +
                    "', City = '" + voter.City +
                    "', Voter_State_id = '" + voter.Voter_State_Id +
                    "', Photo = @image "+
                    ", Voter_Verified = '"+ voter.Voter_Verified +"' where Id = " + voter.Id + "";
                
                if (voter.Photo != null)
                {
                    updateQuery = updateQuery.Replace(",@image", "");
                }
                strResult = ECADB.Update(updateQuery, voter.Photo);
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
            int strResult=0;
            try
            {
                string getQuery = "select COALESCE(max(id),0)+1 from VoterMaster";
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
        public string? VoterIdVerify(int id)
        {
            string? strResult = null;
            try
            {
                string updateQuery = "update VoterMaster set Voter_Verified = 'Y' where Id = " + id ;

                strResult = ECADB.Update(updateQuery);
                return strResult;
            }
            catch
            {
                strResult = AppMessages.MSG_UPDATE_FAILED;
                return strResult;
            }
        }
        public List<VoterList> GetVoterList()
        {
            List<VoterList> voters = new();
            try
            {
                string getQuery = "select v.*, a.mp_state as voter_state from VoterMaster v, seatallocation a " +
                    " where v.Voter_state_id = a.id order by v.Voter_name asc";
                var objSymbols = ECADB.GetRecords(getQuery);
                if (objSymbols != null)
                {
                    DataTable? dtSymbl = (DataTable)objSymbols;
                    voters = (from DataRow dr in dtSymbl.Rows
                               select new VoterList
                               {
                                   Id = Convert.ToInt32(dr["id"]),
                                   Voter_Id = dr["Voter_id"].ToString(),
                                   Voter_Name = dr["Voter_name"].ToString(),
                                   Address= dr["address"].ToString(),   
                                   City= dr["city"].ToString(),
                                   Voter_State= dr["voter_state"].ToString(),
                                   Voter_State_Id = Convert.ToInt32(dr["voter_state_id"]),
                                   Voter_Verified = Convert.ToChar(dr["voter_verified"]),
                                   Photo = (byte[])dr["photo"]                              
                               }).ToList();
                }
                return voters;
            }
            catch
            {
                return voters;
            }
        }        
        public VoterList? GetVoter(int id)
        {
            VoterList voters = new();
            try
            {
                string getQuery = "select v.*, a.mp_state as voter_state from VoterMaster v, seatallocation a " +
                    " where v.Voter_state_id = a.id and v.id = " + id;
                var objSymbols = ECADB.GetRecords(getQuery);
                if (objSymbols != null)
                {
                    DataTable? dtSymbl = (DataTable)objSymbols;
                    voters = (from DataRow dr in dtSymbl.Rows
                              select new VoterList
                              {
                                  Id = Convert.ToInt32(dr["id"]),
                                  Voter_Id = dr["Voter_id"].ToString(),
                                  Voter_Name = dr["Voter_name"].ToString(),
                                  Address = dr["address"].ToString(),
                                  City = dr["city"].ToString(),
                                  Voter_State = dr["voter_state"].ToString(),
                                  Voter_State_Id = Convert.ToInt32(dr["voter_state_id"]),
                                  Voter_Verified = Convert.ToChar(dr["voter_verified"]),
                                  Photo = (byte[])dr["photo"]
                              }).First<VoterList>();
                }
                return voters;
            }
            catch
            {
                return voters;
            }
        }
    }
}
