using System.IO;
using NUnit.Framework;

namespace TorifyTests
{
    public class TestBase
    {
        public static string TestData => Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");
    }
}