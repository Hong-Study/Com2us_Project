using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

using Common;

[ApiController]
[Route("[controller]")]
public class RequestMatchingController
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