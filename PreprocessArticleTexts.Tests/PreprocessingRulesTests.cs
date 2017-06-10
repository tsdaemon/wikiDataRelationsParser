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

                new RemoveDuplicatesRule(),

                new ForbiddenSymbolsRule(),
                new MoreThenOneLineBreakRule(),

                new RemoveTemplateRule(),
                new RemoveListRule(),
                new RemoveTableRule(),
                new RemoveImageRule(),

                new ReplaceLinksRule(),
                new ReplaceSpaceRule(),

                new RemoveMarkupRule(),
                new RemoveMarkupRule(),

                new ReplaceWeirdCharactersRule(),

                //new CheckObjectAndSubjectStillHereRule(),

                new TrimRule(),
                //new SentenceLengthConstraints(30,2000)
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
        [TestCase("| LF3 || Lifan Industry (Group) Co Ltd (мото) || Чунцін || КНР")]
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

        [Test]
        public void WeirdCharactesTest()
        {
            var notProcessedText = "\'\'\'Ню́дзен\'\'\' ({{lang-ja|入善町, にゅうぜんまち}}, {{МФА2|ɲuːd͡zeɴ mat͡ɕi̥}})&nbsp;— [[містечка Японії|містечко]] в [[Японія|Японії]], в повіті [[Повіт Сімо-Ніїкава|Сімо-Ніїкава]] префектури [[префектура Тояма|Тояма]].";

            var actualProcessedText = GoThroughRules(notProcessedText);

            var expectedProcessedText = "Ню́дзен — містечко в Японії, в повіті Сімо-Ніїкава префектури Тояма.";

            Assert.AreEqual(expectedProcessedText, actualProcessedText);
        }

        [Test]
        public void ImagesRemovalTest()
        {
            var notProcessedText = "Єго́р Андрі́йович Бенкендо́рф (14 лютого 1974 р. м. Київ) — режисер, сценарист, продюсер, голова правління телеканалу «Інтер»[http://www.telekritika.ua/news/2013-02-14/79110 «Інтер» очолив Єгор Бенкендорф, інформаційне мовлення — Євгеній Кисельов (ОНОВЛЕНО)], колишній генеральний директор Національної телекомпанії України.";

            var actualProcessedText = GoThroughRules(notProcessedText);

            var expectedProcessedText = "Єго́р Андрі́йович Бенкендо́рф (14 лютого 1974 р. м. Київ) — режисер, сценарист, продюсер, голова правління телеканалу «Інтер», колишній генеральний директор Національної телекомпанії України.";

            Assert.AreEqual(expectedProcessedText, actualProcessedText);
        }
    }
}
