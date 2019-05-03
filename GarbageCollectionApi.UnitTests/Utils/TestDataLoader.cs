namespace GarbageCollectionApi.UnitTests.Utils
{
    using System.IO;
    using System.Text;
    using NUnit.Framework;

    public static class TestDataLoader
    {
        public static string GetTestDataPath(string filename)
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", filename);
        }

        public static string LoadTestData(string filename)
        {
            var path = GetTestDataPath(filename);
            return File.ReadAllText(path);
        }

        public static string LoadTestData(string filename, Encoding encoding)
        {
            var path = GetTestDataPath(filename);
            return File.ReadAllText(path, encoding);
        }
    }
}