using Microsoft.AspNetCore.Mvc;
using Common;

[ApiController]
[Route("[controller]")]
public class CheckMatchingController : ControllerBase
{
    IMatchWoker _matchWorker;

    public CheckMatchingController(IMatchWoker matchWorker)
    {
        _matchWorker = matchWorker;
    }

    [HttpPost]
    public CheckMatchingRes Post(CheckMatchingReq request)
    {
        CheckMatchingRes response = new();

        (var result, var completeMatchingData) = _matchWorker.GetCompleteMatching(request.UserID);

        //TODO: 결과를 담아서 보낸다
        if (result == false)
        {
            response.ErrorCode = ErrorCode.MATCH_FAILED;
        }
        else
        {
            response.ErrorCode = ErrorCode.NONE;
            response.ServerAddress = completeMatchingData!.ServerAddress;
            response.Port = completeMatchingData.Port;
            response.RoomNumber = completeMatchingData.RoomNumber;
        }

        return response;
    }
}