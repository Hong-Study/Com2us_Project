using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CheckMatchingController
{
    ILogger<CheckMatchingController> _logger;
    IMatchService _matchService;
    public CheckMatchingController(ILogger<CheckMatchingController> logger, IMatchService matchService)
    {
        _logger = logger;
        _matchService = matchService;
    }

    [HttpPost]
    public async Task<CheckMatchingRes> CheckMatching(CheckMatchingReq req)
    {
        return await _matchService.CheckMatching(req);
    }
}