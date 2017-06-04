using System.Collections.Generic;
using Core.Model;
using NUnit.Framework;
using PreprocessArticleTexts.Rules;

namespace PreprocessArticleTexts.Tests
{
    [TestFixture]
    public class PreprocessingRulesTests
    {
        private List<IPreprocessRule> rules;

        [SetUp]
        public void Init()
        {
            rules = new List<IPreprocessRule>
            {
                new GenerateIdRule(),

                new ForbiddenSymbolsRule(),
                new MoreThenOneLineBreakRule(),

                new RemoveTemplateRule(),
                new RemoveListRule(),

                new ReplaceLinksRule(),
                new ReplaceSpaceRule(),

                new RemoveMarkupRule(),
                new RemoveMarkupRule(),

                new TrimRule()
            };
        }

        private string GoThroughRules(string text)
        {
            var triplet = new TripletTrain {Text = text};
            foreach (var rule in rules)
            {
                rule.Preprocess(triplet);
                if (string.IsNullOrEmpty(triplet.Text)) break;
            }
            return triplet.Text;
        }

        [Test]
        [TestCase("[[Норчія]]\n* [[Пречі]]")]
        [TestCase("* [[Юба I|Юб I]](60—46 до н.е.)\n* римська провінція (46-30 до н.е.)\n* [[Юба II]](30—23 до н.е.)")]
        public void NotASentenceTest(string notProcessedText)
        {
            var actualProcessedText = GoThroughRules(notProcessedText);

            Assert.IsNull(actualProcessedText);
        }

        [Test]
        public void BreznikTest()
        {
            var notProcessedText = "{{Прапорець|USA}} [[Національне управління з аеронавтики і дослідження космічного простору|(НАСА)]] [[Рендолф Джеймс Брезник|Рендолф Брезник]]&nbsp;— бортінженер";

            var actualProcessedText = GoThroughRules(notProcessedText);

            var expectedProcessedText = "(НАСА) Рендолф Брезник — бортінженер";

            Assert.AreEqual(expectedProcessedText, actualProcessedText);
        }
    }
}
