using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CancleMatchingController : ControllerBase
{
    ILogger<CancleMatchingController> _logger;
    IMatchService _matchService;
    public CancleMatchingController(ILogger<CancleMatchingController> logger, IMatchService matchService)
    {
        _logger = logger;
        _matchService = matchService;
    }

    [HttpPost]
    public async Task<CancleMatchingRes> Post(CancleMatchingReq req)
    {
        return await _matchService.CancleMatching(req);
    }
}