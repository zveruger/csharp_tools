using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tools.CSharp.AnalyzerLines.Tests
{
    [TestClass]
    public class SequenceSymbolUnitTests
    {
        [TestMethod]
        public void TestSumbolSuccess()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            SymbolUnit lexemValue = null;
            //-----------------------------------------------------------------
            sequence.DecodeLineIgnoreCaseSymbols = true;
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence.Add(new SymbolUnit('l').Action(unit => lexemValue = unit));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("     L     ");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(lexemValue.Value == 'L');
            Assert.IsTrue(lexemValue.StartPosition == 5);
        }
    }
}