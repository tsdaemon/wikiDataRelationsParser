using System;
using System.IO;
using NUnit.Framework;

namespace Core.Tests
{
    [TestFixture]
    public class TextHelperTests
    {
        private string readFile(string name) => File.ReadAllText($"{TestContext.CurrentContext.TestDirectory}/../../{name}.txt");

        [Test]
        public void Uba_Test()
        {
            var text = readFile("Uba");

            int start = 3028, end = 3101, newStart, newEnd;

            var actualText = TextHelper.ExtractTextWithSentenceWindow(text, start, end, out newStart, out newEnd);

            var expectedText = "* [[Юба I]](60—46 до н.е.)\n* римська провінція (46-30 до н.е.)\n* [[Юба II]](30—23 до н.е.)";

            Assert.AreEqual(expectedText, actualText);
        }
    }
}
