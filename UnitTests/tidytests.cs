using Tidy.Core;

namespace TidyTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            String htmlcorrect = "<h1>Tidy is Tidying</h1>";
            String htmltotest = "<h1>Tidy is Tidying</H1>";
            Tidy.Core.HtmlTidy oTdyManaged = new Tidy.Core.HtmlTidy();
            Tidy.Core.TidyOptions oTdyOptions = new Tidy.Core.TidyOptions();

            oTdyManaged.Options.Word2000 = true;
            oTdyManaged.Options.XmlOut = true;

            oTdyManaged.Options.MakeClean = true;
            oTdyManaged.Options.MakeBare = true;
            oTdyManaged.Options.Xhtml = true;
            oTdyManaged.Options.DropFontTags = true;
            oTdyManaged.Options.BodyOnly = true;
            oTdyManaged.Options.NumEntities = true;

            Tidy.Core.TidyMessageCollection tidyMsg = new Tidy.Core.TidyMessageCollection();

            string sTidyXhtml = oTdyManaged.Parse(htmltotest, tidyMsg);


            //sTidyXhtml = oTdyManaged.ToString();

            foreach (TidyMessage tm in tidyMsg)
            {
                if (tm.Level == MessageLevel.Error)
                {
                    sTidyXhtml = "<div>html import conversion error result=" + tm.Message + " <br/></div>";
                }
            }

            oTdyManaged = null;

            sTidyXhtml = sTidyXhtml.Replace("<body>", "").Replace("</body>", "").Trim();
            TestContext.WriteLine("htmltotest");
            TestContext.WriteLine(htmltotest);
            TestContext.WriteLine("htmltidied");
            TestContext.WriteLine(sTidyXhtml);

            // Assert.That(result, Is.False, $"{value} should not be prime");


            if (sTidyXhtml == htmlcorrect)
            {
                TestContext.WriteLine("Match!");
                Assert.Pass();
            }
            else
            {
                TestContext.WriteLine("FAIL!");
                Assert.Fail();
            }
        }
    }
}