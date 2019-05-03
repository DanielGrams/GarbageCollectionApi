namespace GarbageCollectionApi.UnitTests.Utils
{
    using System.IO;
    using NUnit.Framework;

    public static class TestDataLoader
    {
        public static string LoadTestData(string filename)
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", filename);
            return File.ReadAllText(path);
        }
    }
}