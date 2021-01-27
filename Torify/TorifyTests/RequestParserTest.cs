using System.IO;
using NUnit.Framework;
using Torify;

namespace TorifyTests
{
    [TestFixture]
    public class RequestParserTest : TestBase
    {
        private RequestParser parser;

        [SetUp]
        public void SetUp()
        {
            parser = new RequestParser();
        }

        [Test]
        [TestCase("britax.html")]
        [TestCase("moccamaster.html")]
        public void ParseTest(string fileName)
        {
            var inputFile = Path.Combine(TestData, fileName);
            var html = File.ReadAllText(inputFile);

            var items = parser.ParseItems(html);

            foreach (var item in items)
            {
                Assert.IsNotEmpty(item.Description, $"Description empty: ID {item.Id}");
                Assert.IsNotEmpty(item.Area, $"Area empty: ID {item.Id}");
                Assert.IsNotEmpty(item.LinkUri, $"Link empty: ID {item.Id}");
            }
        }
    }
}
