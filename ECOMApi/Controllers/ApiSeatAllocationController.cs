using Microsoft.AspNetCore.Mvc;
using EcomCore;
using ECOMApi.Models;

namespace ECOMApi.Controllers
{
    [ApiController]
    [Route("SeatAllocation")]
    public class ApiSeatAllocationController : Controller
    {
        private readonly SeatAllocationMaster? _seatAllocate;

        public ApiSeatAllocationController( )
        {
            _seatAllocate = new SeatAllocationMaster();
        }

        [HttpPost]
        [Route("Register")]
        public ActionResult AddSeat(Seat seat)
        {
            int seqNo = _seatAllocate.GetMaxOfId();
            SeatAllocation objSeat = new()
            {
                Id = seqNo,
                MP_state = seat.MP_state,
                Total_seats = seat.Total_seats
            };

            var strResult = _seatAllocate?.Register(objSeat);
            return new JsonResult(strResult);
        }

        [HttpPut]
        [Route("Correction")]
        public ActionResult UpdateSeat(SeatAllocation seat)
        {
            var strResult = _seatAllocate?.Update(seat);
            return new JsonResult(strResult);
        }

        [HttpGet]
        [Route("GetSeatAllocated")]
        public ActionResult GetSeatAllocated(int id)
        {
            var strResult = _seatAllocate?.GetAllocatedSeat(id);
            return new JsonResult(strResult);
        }

        [HttpGet]
        [Route("GetSeatAllocatedList")]
        public ActionResult GetSeatAllocatedList()
        {
            var strResult = _seatAllocate?.GetAllocatedSeats();
            return new JsonResult(strResult);
        }

        [HttpDelete]
        [Route("DeleteSeat")]
        public ActionResult DeleteSeat(int id)
        {
            var strResult = _seatAllocate?.DeleteSeat(id);
            return new JsonResult(strResult);
        }
    }
}
