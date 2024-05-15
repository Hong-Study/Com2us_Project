using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RequestMatchingController : ControllerBase
{
    ILogger<RequestMatchingController> _logger;
    IMatchService _matchService;
    public RequestMatchingController(ILogger<RequestMatchingController> logger, IMatchService matchService)
    {
        _logger = logger;
        _matchService = matchService;
    }

    [HttpPost]
    public async Task<MatchingRes> Post(MatchingReq req)
    {
        return await _matchService.RequestMatching(req);
    }
}