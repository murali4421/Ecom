using EcomCore.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomCore
{
    public class SeatAllocationMaster
    {
        public SeatAllocationMaster() { }

        public string? Register(SeatAllocation seatAllocate)
        {
            string? strResult = null;
            try
            {
                string insertQuery = "insert into SeatAllocation(id,MP_state,Total_seats) " +
                    "values ("+ seatAllocate.Id + ",'" + seatAllocate.MP_state + "', " + seatAllocate.Total_seats + ")";

                strResult = ECADB.Insert(insertQuery);
                return strResult;
            }
            catch
            {
                strResult = AppMessages.MSG_SAVE_FAILED;
                return strResult;
            }
        }

        public string? Update(SeatAllocation seatAllocate)
        {
            string? strResult = null;
            try
            {
                string updateQuery = "update SeatAllocation set MP_state = '" + seatAllocate.MP_state + 
                    "',Total_seats = " + seatAllocate.Total_seats + " where Id = " + seatAllocate .Id + "";

                strResult = ECADB.Update(updateQuery);
                return strResult;
            }
            catch
            {
                strResult = AppMessages.MSG_UPDATE_FAILED;
                return strResult;
            }
        }

        public List<SeatAllocation> GetAllocatedSeats()
        {
            List<SeatAllocation> seats = new();
            try
            {
                string getQuery = "select * from SeatAllocation order by MP_state asc";
                var objseats = ECADB.GetRecords(getQuery);
                if (objseats != null)
                {
                    DataTable? dtSeats = (DataTable)objseats;
                    seats = (from DataRow dr in dtSeats.Rows
                               select new SeatAllocation
                               {
                                   Id = Convert.ToInt32(dr["id"].ToString()),
                                   MP_state = dr["MP_state"].ToString(),
                                   Total_seats = (int)dr["Total_seats"]
                               }).ToList();
                }
                return seats;
            }
            catch
            {
                return seats;
            }
        }

        public SeatAllocation? GetAllocatedSeat(int id)
        {
            SeatAllocation seat = new();
            try
            {
                string getQuery = "select * from SeatAllocation where id =" + id;
                var objseats = ECADB.GetRecords(getQuery);
                if (objseats != null)
                {
                    DataTable? dtSeats = (DataTable)objseats;
                    seat = (from DataRow dr in dtSeats.Rows
                               select new SeatAllocation
                               {
                                   Id = Convert.ToInt32(dr["id"].ToString()),
                                   MP_state = dr["MP_state"].ToString(),
                                   Total_seats = (int)dr["Total_seats"]
                               }).First<SeatAllocation>();
                }
                return seat;
            }
            catch
            {
                return seat;
            }
        }

        public int GetMaxOfId()
        {
            int result = 1;
            try
            {
                string getQuery = "select COALESCE(max(id),0)+1 from SeatAllocation";

                var objSeat = ECADB.GetRecords(getQuery);

                if (objSeat != null)
                {
                    var dt = (DataTable)objSeat;
                    result = (int)dt.Rows[0][0];
                }

                return result;
            }
            catch
            {
                return result;
            }
        }
        public int DeleteSeat(int id)
        {
            int result = 0;
            try
            {
                string getQuery = "delete from SeatAllocation where id = " + id;

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
