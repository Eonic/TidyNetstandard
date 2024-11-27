using Tidy.Core;

namespace TidyTests
{
    public class Tests
    {
        private Tidy.Core.TidyOptions oTdyOptions;
        private Tidy.Core.HtmlTidy oTdy;
        [SetUp]
        public void Setup()
        {
            oTdy = new Tidy.Core.HtmlTidy();
            // oTdyOptions = new Tidy.Core.TidyOptions();

            oTdy.Options.Word2000 = true;
            oTdy.Options.XmlOut = true;
            oTdy.Options.MakeClean = true;
            oTdy.Options.MakeBare = true;
            oTdy.Options.Xhtml = true;
            oTdy.Options.DropFontTags = true;
            oTdy.Options.BodyOnly = true;
            oTdy.Options.NumEntities = true;
            oTdy.Options.LogicalEmphasis = true;

        }



        string removeBody(string html)
        {
            return html.Replace("<body>", "").Replace("</body>", "").Trim();
        }

        void runStringStest(string htmltest, string htmlcorrect)
        {
            Setup();
            Tidy.Core.TidyMessageCollection tidyMsg = new Tidy.Core.TidyMessageCollection();
            string sTidyXhtml = oTdy.Parse(htmltest, tidyMsg);
            foreach (TidyMessage tm in tidyMsg)
            {
                TestContext.WriteLine(tm.Level.ToString() + " - " + tm.Message + " line:" + tm.Line);
            }

            sTidyXhtml = removeBody(sTidyXhtml);
            //remove line breaks
            sTidyXhtml = sTidyXhtml.Replace("\r\n", "").Replace("\n", "").Replace("\r", "");

            TestContext.WriteLine("htmltotest");
            TestContext.WriteLine(htmltest);
            TestContext.WriteLine("htmltidied");
            TestContext.WriteLine(sTidyXhtml);

            sTidyXhtml = sTidyXhtml.Replace("\r\n", "").Replace("\n", "").Replace("\r", "");
            htmltest = htmltest.Replace("\r\n", "").Replace("\n", "").Replace("\r", "");

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

        [Test]
        public void h1()
        {
            String htmlcorrect = "<h1>Tidy is Tidying</h1>";
            String htmltotest = "<h1>Tidy is Tidying</H1>";

            runStringStest(htmltotest, htmlcorrect);
        }

        [Test]
        public void em()
        {
            String htmlcorrect = "<div><h1><em class=\"test\">Tidy is Tidying</em></h1><p>some text</p></div><div>some more text</div>";
            String htmltotest = "<div><h1><em class=\"test\">Tidy is Tidying</em></h1><p>some text</p></div><div>some more text</div>";

            runStringStest(htmltotest, htmlcorrect);
        }

        [Test]
        public void table()
        {
            String htmlcorrect = "<div><table><tr><td>Tidy is Tidying</td></tr><tr><td>some text</td></tr></table></div>";
            String htmltotest = "<div><table><tr><td>Tidy is Tidying</td></tr><tr><td>some text</td></tr>";

            runStringStest(htmltotest, htmlcorrect);
        }
        [Test]
        public void em2i()
        {
            String htmlcorrect = "<div><h1><em>Tidy is Tidying</em></h1><p>some text</p></div>";
            String htmltotest = "<div><h1><i>Tidy is Tidying</i></h1><p>some text</p></div>";

            runStringStest(htmltotest, htmlcorrect);
        }
        [Test]
        public void b2strong()
        {
            String htmlcorrect = "<div><strong>Tidy is Tidying</strong><p>some text</p></div>";
            String htmltotest = "<div><b>Tidy is Tidying</b><p>some text</p></div>";

            runStringStest(htmltotest, htmlcorrect);
        }

    }
}