using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RequestMatchingController : ControllerBase
{
    IMatchWoker _matchWorker;
    public RequestMatchingController(IMatchWoker matchWorker)
    {
        _matchWorker = matchWorker;
    }

    [HttpPost]
    public MatchResponse Post(MatchingRequest request)
    {
        MatchResponse response = new();

        response.ErrorCode = _matchWorker.AddUser(request.UserID);

        return response;
    }
}