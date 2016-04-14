using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tools.CSharp.AnalyzerLines.Tests
{
    [TestClass]
    public class SequenceAddUnitTests
    {
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestAdd1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new StringUnit("lexem"))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAdd1Failed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new StringUnit("lexem"))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("ledem");

            Assert.IsFalse(result);
            Assert.IsTrue(error.PositionInLine == 2);
            Assert.IsTrue(string.Equals(((StringUnit)error.Unit).OriginalValue, "lexem", StringComparison.OrdinalIgnoreCase));
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestComplexAdd1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var value = "";
            //-----------------------------------------------------------------
            sequence
                .Add(
                    new StringUnit("lexem"),
                    new SymbolUnit(' '),
                    new StringUnit("x"),
                    new SymbolUnit('('),
                    new ValueUnit().Action(x => value = x.Value),
                    new SymbolUnit(')')
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem x(     222    )");

            Assert.IsTrue(result);
            Assert.IsTrue(string.Equals(value, "222", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestComplexAdd1Failed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            //-----------------------------------------------------------------
            sequence
                .Add(
                    new StringUnit("lexem"),
                    new SymbolUnit(' '),
                    new StringUnit("x"),
                    new SymbolUnit('('),
                    new ValueUnit(),
                    new SymbolUnit(')')
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexemx(222 )");

            Assert.IsFalse(result);
            Assert.IsTrue(error.PositionInLine == 5);
            Assert.IsTrue(error.Unit is SymbolUnit);
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestAddAndActionSuccess()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isAddAvailable = false;
            //-----------------------------------------------------------------
            sequence
                .Add(new StringUnit("lexem").Action(() => isAddAvailable = true))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem");

            Assert.IsTrue(result);
            Assert.IsTrue(isAddAvailable);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAddAndActionFailed()
        {
            var sequence = new Sequence();
            UnitError error = null;
            var isAddAvailable = false;
            //-----------------------------------------------------------------
            sequence
                .Add(new StringUnit("lexem").Action(() => isAddAvailable = true))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexeq");

            Assert.IsFalse(result);
            Assert.IsFalse(isAddAvailable);
            Assert.IsTrue(error.PositionInLine == 4);
            Assert.IsTrue(string.Equals(((StringUnit)error.Unit).OriginalValue, "lexem", StringComparison.OrdinalIgnoreCase));
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestAddMoreSuccess()
        {
            var sequence = new Sequence();
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            var isLexem3Available = false;
            //-----------------------------------------------------------------
            sequence
                .Add(
                    new StringUnit("lexem").Action(() => isLexem1Available = true),
                    new SymbolUnit(' '),
                    new StringUnit("pop").Action(() => isLexem2Available = true),
                    new SymbolUnit(' '),
                    new StringUnit("xxx").Action(() => isLexem3Available = true)
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem pop xxx");

            Assert.IsTrue(result);
            Assert.IsTrue(isLexem1Available);
            Assert.IsTrue(isLexem2Available);
            Assert.IsTrue(isLexem3Available);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAddMoreFailed()
        {
            var sequence = new Sequence();
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            var isLexem3Available = false;
            //-----------------------------------------------------------------
            sequence
                .Add(
                    new StringUnit("lexem").Action(() => isLexem1Available = true),
                    new SymbolUnit(' '),
                    new StringUnit("pop").Action(() => isLexem2Available = true),
                    new SymbolUnit(' '),
                    new StringUnit("xxx").Action(() => isLexem3Available = true)
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem xxx");

            Assert.IsFalse(result);
            Assert.IsFalse(isLexem1Available);
            Assert.IsFalse(isLexem2Available);
            Assert.IsFalse(isLexem3Available);
            Assert.IsTrue(error.PositionInLine == 6);
            Assert.IsTrue(string.Equals(((StringUnit)error.Unit).OriginalValue, "pop", StringComparison.OrdinalIgnoreCase));
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestAddAndIgnoreCaseSymbols1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new StringUnit("lexem").DecodeLineIgnoreCaseSymbols(true))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("leXem");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAddAndIgnoreCaseSymbols2Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ').DecodeLineIgnoreCaseSymbols(true);
            UnitError error = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new StringUnit("lexem"))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("leXeM");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAddAndIgnoreCaseSymbols1Failed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            //-----------------------------------------------------------------
            sequence
                .Add(
                    new StringUnit("lexem").DecodeLineIgnoreCaseSymbols(true),
                    new StringUnit("xxx").DecodeLineIgnoreCaseSymbols(false)
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem XXX");

            Assert.IsFalse(result);
            Assert.IsTrue(error.PositionInLine == 6);
            Assert.IsTrue(string.Equals(((StringUnit)error.Unit).OriginalValue, "xxx", StringComparison.OrdinalIgnoreCase));
        }
        //---------------------------------------------------------------------
    }
}