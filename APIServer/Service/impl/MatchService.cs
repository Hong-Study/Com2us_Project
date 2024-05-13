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
        try
        {
            var httpResponse = await _httpClient.PostAsJsonAsync("api/requestmatching", req);
            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError("RequestMatching failed");
                return new MatchingRes() { ErrorCode = ErrorCode.MATCHING_SERVER_ERROR };
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<MatchingRes>();
            if (response == null)
            {
                _logger.LogError("RequestMatching failed");
                return new MatchingRes() { ErrorCode = ErrorCode.MATCHING_SERVER_ERROR };
            }

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "RequestMatching failed");
            return new MatchingRes() { ErrorCode = ErrorCode.MATCHING_SERVER_ERROR };
        }
    }

    public async Task<CheckMatchingRes> CheckMatching(CheckMatchingReq req)
    {
        try
        {
            var httpResponse = await _httpClient.PostAsJsonAsync("api/checkmatching", req);
            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError("CheckMaching failed");
                return new CheckMatchingRes() { ErrorCode = ErrorCode.MATCHING_SERVER_ERROR };
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<CheckMatchingRes>();
            if (response == null)
            {
                _logger.LogError("CheckMaching failed");
                return new CheckMatchingRes() { ErrorCode = ErrorCode.MATCHING_SERVER_ERROR };
            }

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "CheckMaching failed");
            return new CheckMatchingRes() { ErrorCode = ErrorCode.MATCHING_SERVER_ERROR };
        }
    }
}