using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CancleMatchingController : ControllerBase
{
    IMatchWoker _matchWorker;
    public CancleMatchingController(IMatchWoker matchWoker)
    {
        _matchWorker = matchWoker;
    }

    [HttpPost]
    public CancleMatchingRes Post(CancleMatchingReq req)
    {
        CancleMatchingRes res = new();

        res.ErrorCode = _matchWorker.RemoveUser(req.UserID);

        return res;
    }
}