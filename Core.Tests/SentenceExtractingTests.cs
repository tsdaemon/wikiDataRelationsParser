using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Core.Tests
{
    [TestFixture]
    public class SentenceExtractingTests
    {
        private string readFile(string name) => File.ReadAllText($"{TestContext.CurrentContext.TestDirectory}/../../../{name}.txt", Encoding.UTF8);

        [Test]
        public void Uba_Test()
        {
            var text = readFile("Uba");

            int start = 3028, end = 3101, newStart, newEnd;

            var actualText = TextHelper.ExtractTextWithSentenceWindow(text, start, end, out newStart, out newEnd);

            var expectedText = "* [[Юба I]](60—46 до н.е.)\n* римська провінція (46-30 до н.е.)\n* [[Юба II]](30—23 до н.е.)";

            Assert.AreEqual(expectedText, actualText);
        }

        [Test]
        public void Name_Abbrvs_Test()
        {
            var text = readFile("Savchenko");

            int start = 2575, end = 2879, newStart, newEnd;

            var actualText = TextHelper.ExtractTextWithSentenceWindow(text, start, end, out newStart, out newEnd);

            var expectedText = "З 1946 року викладав у [[Всеросійський державний інститут кінематографії|ВДІКу]]. Його учнями були відомі нині режисери: [[Алов Олександр Олександрович|О.О. Алов]], [[Наумов Володимир Наумович|В.Н. Наумов]], [[Габай Генріх Саулович|Г.С. Габай]], [[Файзієв Латіф Абідович|Л.А. Файзієв]], [[Миронер Фелікс Юхимович|Ф.Є. Миронер]], [[Озеров Юрій Миколайович|Ю.М. Озеров]], [[Параджанов Сергій Йосипович|С.І. Параджанов]], [[Хуцієв Марлен Мартинович|М.М. Хуцієв]], [[Коренєв Олексій Олександрович|О.О. Коренєв]], Ю.А. Закревський, Лев Іванов, Л.С. Данилов.";

            Assert.AreEqual(expectedText, actualText);
        }

        [Test]
        public void Zarubin_Test()
        {
            var text = readFile("Zarubin");

            int start = 1262, end = 3014, newStart, newEnd;

            var actualText = TextHelper.ExtractTextWithSentenceWindow(text, start, end, out newStart, out newEnd);

            var expectedText = @"В [[Курськ]]ій чоловічій гімназії отримав середню освіту, поступив до [[Харківський національний університет імені В. Н. Каразіна|Харківського університету]]. 1856 року закінчив курс із залишенням при госпітальній хірургічній клініці ординатором.
1861 року захищає дисертацію «Про органічне відновлення нижньої губи», доктор медичних наук.

Того ж року при університеті з подачі професорів [[Грубе Вільгельм Федорович|Вільгельма Федоровича Грубе]], [[Франковський Владислав Андрійович|Владислава Андрійовича Франковського]] та Семена Григоровича Риндовського при [[Харківське медичне товариство|Харківському медичному товаристві]] засновано медичну бібліотеку; першим скарбником товариства і бібліотекарем призначений Зарубін.

У 1862 році затверджений при кафедрі хірургії ад'юнктом та відряджений на два роки для стажування за кордон. Протягом того року Харківське наукове товариство на медичну бібліотеку виділило 40% всього бюджету.

З 1863 року працює на кафедрі теоретичної хірургії як екстраординарний. Очолював кафедру теоретичної хірургії з 1863 по 1884 рік. По ньому кафедру очолив професор [[Суботін Максим Семенович|Максим Семенович Суботін]].

Починаючи 1865 роком&nbsp;— ординарний професор.

Виконував обов'язки декана, на цій посаді намагався засновувати госпітальні клініки при університеті. Отримав згоду Харківського громадського управління на заснування терапевтичної та хірургічної практики в [[Олександрівська лікарня (Харків)|Олександрівській лікарні]].

Ініціював облаштування лікарні прибуваючих бідних хворих, яка отримала широкий розвиток.

1882 року обійняв посаду завідуючого кафедрою госпітальної хірургічної практики.

Затверджений в званні почесного професора у 1887 році.

Заснував у Харкові дві клініки й лікарню.

Батько дерматолога [[Зарубін Валентин Іванович|В.&nbsp;І.&nbsp;Зарубіна]].";

            Assert.AreEqual(expectedText, actualText);
        }

        [Test]
        public void Galisian_Test()
        {
            var text = readFile("Galisian");

            int start = 2918, end = 3009, newStart, newEnd;

            var actualText = TextHelper.ExtractTextWithSentenceWindow(text, start, end, out newStart, out newEnd);

            var expectedText = "У трьох [[Муніципії Іспанії|муніципіях]] [[Касерес (провінція)|Касереса]] в [[долині Халама]] ([[Вальверде дель Фресно]], [[Ельхас]] і [[Сан-Мартін-де-Тревехо]]) розмовляють мовою [[Фала (мова)|фалі]] ({{lang-es|Fala de Xálima}}), з приводу якої серед учених немає згоди в тому, чи є вона самостійною мовою, разом з галісійською і португальською, або ж є давньопортугальською з [[Леонська мова|леонським]] і іспанським [[суперстрат]]ом.";

            Assert.AreEqual(expectedText, actualText);
        }

        [Test]
        public void OutOfIndexError_Test()
        {
            var text =
                "{{disambig}}\n\n\n* [[Мен (штат)|Мен]]&nbsp;— штат на північному сході [[США]]\n* [[Мен (затока)|Мен]]&nbsp;— затока [[Атлантичний океан|Атлантичного океану]] на східному узбережжі [[Північна Америка|Північної Америки]]\n* [[Мен (бог)|Мен]]&nbsp;— [[Фрігія|фрігійське]] божество [[Місяць (супутник)|Місяця]]\n* [[Мен (острів)|Мен]]&nbsp;— острів в [[Ірландське море|Ірландському морі]], приблизно рівновіддалений від [[Англія|Англії,]] [[Шотландія|Шотландії]] та [[Ірландія|Ірландії]]\n* [[Мен (Луар і Шер)|Мен]] ({{lang-fr|Meusnes}})&nbsp;— [[Комуна (Франція)|муніципалітет]] у [[Франція|Франції]], у регіоні [[Центр-Долина Луари]], департамент [[Луар і Шер]]\n* [[Мен (Верхні Піренеї)|Мен]] ({{lang-fr|Mun}})&nbsp;— муніципалітет у Франції, у регіоні [[Південь-Піренеї]], департамент [[Верхні Піренеї]]\n* [[Мен (Нор)|Мен]] ({{lang-fr|Maing}}) - муніципалітет у Франції, у регіоні [[Нор-Па-де-Кале]], департамент [[Нор (департамент)|Нор]]";

            int start = 572, end = 931, newStart, newEnd;

            var actualText = TextHelper.ExtractTextWithSentenceWindow(text, start, end, out newStart, out newEnd);

        }

        [Test]
        public void Chemistry_Test()
        {
            var text = readFile("Himiya");

            int start = 19107, end = 19165, newStart, newEnd;

            var actualText = TextHelper.ExtractTextWithSentenceWindow(text, start, end, out newStart, out newEnd);

            var expectedText = "За всю історію вручення Нобелівської премії, її отримали тільки 4 жінки: [[Марія Склодовська-Кюрі|Марія Кюрі]], [[Ірен Жоліо-Кюрі]], [[Дороті Ходжкін]] та [[Ада Йонат]].<ref>{{cite web|title=Women Nobel Laureates|publisher=Nobelprize.org|url=http://nobelprize.org/nobel_prizes/lists/women.html|accessdate=2012-02-04|archiveurl=http://www.webcitation.org/6GYpjPfvR|archivedate=2013-05-12}}</ref>";

            Assert.AreEqual(expectedText, actualText);
        }

        [Test]
        public void Orlyk_Test()
        {
            var text = readFile("Ukraine");

            int start = 4497, end = 46933, newStart, newEnd;

            var actualText = TextHelper.ExtractTextWithSentenceWindow(text, start, end, out newStart, out newEnd);

            var expectedText = readFile("Orlyk_test");

            Assert.AreEqual(expectedText, actualText);
        }

        [Test]
        public void Ugoslavia_Test()
        {
            var text = readFile("Ukraine");

            int start = 4249, end = 93139, newStart, newEnd;

            var actualText = TextHelper.ExtractTextWithSentenceWindow(text, start, end, out newStart, out newEnd);

            var expectedText = readFile("Ugoslavia_test");

            Assert.AreEqual(expectedText, actualText);
        }
    }
}
