using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tools.CSharp.AnalyzerLines.Tests
{
    [TestClass]
    public class SequenceStringUnitTests
    {
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestString1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexemAvailable = false;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence.Add(new StringUnit("lexem").Action(() => isLexemAvailable = true));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(isLexemAvailable);
        }

        [TestMethod]
        public void TestString2Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexemAvailable = false;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence.Add(new StringUnit("lexem").Action(() => isLexemAvailable = true));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("  lexem");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(isLexemAvailable);
        }

        [TestMethod]
        public void TestString3Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexemAvailable = false;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence.Add(new StringUnit("lexem").Action(() => isLexemAvailable = true));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("  lexem   ");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(isLexemAvailable);
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestStringValue1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            StringUnit lexemValue = null;
            //-----------------------------------------------------------------
            sequence.DecodeLineIgnoreCaseSymbols = true;
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence.Add(new StringUnit("lexem").Action(unit => lexemValue = unit));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("     Lexem     ");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(lexemValue.Value == "Lexem");
            Assert.IsTrue(lexemValue.StartPosition == 5);
        }
        //---------------------------------------------------------------------
    }
}