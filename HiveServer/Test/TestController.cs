

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    ITestRepository _repository;
    public TestController(ITestRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<string> Get()
    {
        TestData data = await _repository.GetTestData();
        System.Console.WriteLine(data.first_id + " " + data.second_id);

        return "Hello World!";
    }

    [HttpPost]
    public async Task<string> Post()
    {
        await _repository.InsertTestData(new TestData() { first_id = 1, second_id = 1 });

        return "Hello World!";
    }
}