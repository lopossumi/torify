using System.IO;
using NUnit.Framework;

namespace UnitTests
{
    public class TestBase
    {
        public static string TestData => Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");
    }
}