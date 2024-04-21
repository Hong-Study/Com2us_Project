public interface ITestRepository
{
    public Task<TestData> GetTestData();
    public Task<bool> InsertTestData(TestData testData);
}