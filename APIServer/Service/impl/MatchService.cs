using System.Collections.Concurrent;
using System.Net;

public class MatchService : IMatchService
{
    HttpClient _httpClient = new HttpClient();

    ILogger<MatchService> _logger;
    string _matchServerAddress = "";

    public MatchService(ILogger<MatchService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _matchServerAddress = configuration["MatchServerUrl"]!;

        _httpClient.BaseAddress = new Uri(_matchServerAddress);
    }

    public async Task<MatchingRes> RequestMatching(MatchingReq req)
    {
        var httpResponse = await _httpClient.PostAsJsonAsync("api/RequestMatching", req);
        if(httpResponse.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError("RequestMatching failed");
            return new MatchingRes(){ErrorCode = ErrorCode.NONE};
        }

        var response = await httpResponse.Content.ReadFromJsonAsync<MatchingRes>();
        if(response == null)
        {
            _logger.LogError("RequestMatching failed");
            return new MatchingRes(){ErrorCode = ErrorCode.NONE};
        }
        
        return response;
    }

    public async Task<CheckMatchingRes> CheckMatching(CheckMatchingReq req)
    {
        var httpResponse = await _httpClient.PostAsJsonAsync("api/RequestMatching", req);
        if(httpResponse.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError("RequestMatching failed");
            return new CheckMatchingRes(){ErrorCode = ErrorCode.NONE};
        }

        var response = await httpResponse.Content.ReadFromJsonAsync<CheckMatchingRes>();
        if(response == null)
        {
            _logger.LogError("RequestMatching failed");
            return new CheckMatchingRes(){ErrorCode = ErrorCode.NONE};
        }

        return response;
    }
}