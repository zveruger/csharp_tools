using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tools.CSharp.AnalyzerLines.Tests
{
    [TestClass]
    public class SequenceAnyUnitTests
    {
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestAny1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            //-----------------------------------------------------------------
            sequence
                .Any(
                    new StringUnit("lexem").Action(() => isLexem1Available = true),
                    new StringUnit("pop").Action(() => isLexem2Available = true)
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem");

            Assert.IsTrue(result);
            Assert.IsTrue(isLexem1Available);
            Assert.IsFalse(isLexem2Available);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAny2Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            //-----------------------------------------------------------------
            sequence
                .Any(
                    new StringUnit("lexem").Action(() => isLexem1Available = true),
                    new StringUnit("pop").Action(() => isLexem2Available = true)
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("pop");

            Assert.IsTrue(result);
            Assert.IsFalse(isLexem1Available);
            Assert.IsTrue(isLexem2Available);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAny3Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            //-----------------------------------------------------------------
            sequence
                .Any(
                    new StringUnit("lexem").Action(() => isLexem1Available = true),
                    new StringUnit("pop").Action(() => isLexem2Available = true)
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem pop");

            Assert.IsTrue(result);
            Assert.IsTrue(isLexem1Available);
            Assert.IsTrue(isLexem2Available);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAny4Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            //-----------------------------------------------------------------
            sequence
                .Any(
                    new StringUnit("lexem").Action(() => isLexem1Available = true),
                    new StringUnit("pop").Action(() => isLexem2Available = true)
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("pop lexem");

            Assert.IsTrue(result);
            Assert.IsTrue(isLexem1Available);
            Assert.IsTrue(isLexem2Available);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAny5Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            //-----------------------------------------------------------------
            sequence
                .Any(
                    new StringUnit("lexem").Action(() => isLexem1Available = true),
                    new StringUnit("pop").Action(() => isLexem2Available = true)
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("     ");

            Assert.IsTrue(result);
            Assert.IsFalse(isLexem1Available);
            Assert.IsFalse(isLexem2Available);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAny1Failed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            //-----------------------------------------------------------------
            sequence
                .Any(
                    new StringUnit("lexem").Action(() => isLexem1Available = true),
                    new StringUnit("pop").Action(() => isLexem2Available = true)
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("xxx");

            Assert.IsFalse(result);
            Assert.IsFalse(isLexem1Available);
            Assert.IsFalse(isLexem2Available);
            Assert.IsTrue(error.PositionInLine == 0);
            Assert.IsTrue(error.Unit == null);
        }

        [TestMethod]
        public void TestAny2Failed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            //-----------------------------------------------------------------
            sequence
                .Any(
                    new StringUnit("lexem").Action(() => isLexem1Available = true),
                    new StringUnit("pop").Action(() => isLexem2Available = true)
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("pa");

            Assert.IsFalse(result);
            Assert.IsFalse(isLexem1Available);
            Assert.IsFalse(isLexem2Available);
            Assert.IsTrue(error.PositionInLine == 1);
            Assert.IsTrue(string.Equals(((StringUnit)error.Unit).OriginalValue, "pop", StringComparison.OrdinalIgnoreCase));
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestAnyAndAdd1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            var isAddLexemAvailable = false;
            //-----------------------------------------------------------------
            sequence
                .Any(
                    new StringUnit("lexem").Action(() => isLexem1Available = true),
                    new StringUnit("pop").Action(() => isLexem2Available = true)
                )
                .Add(new StringUnit("xxx").Action(() => isAddLexemAvailable = true))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("pop xxx");

            Assert.IsTrue(result);
            Assert.IsFalse(isLexem1Available);
            Assert.IsTrue(isLexem2Available);
            Assert.IsTrue(isAddLexemAvailable);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAnyAndAdd2Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            var isAddLexemAvailable = false;
            //-----------------------------------------------------------------
            sequence
                .Any(
                    new StringUnit("lexem").Action(() => isLexem1Available = true),
                    new StringUnit("pop").Action(() => isLexem2Available = true)
                )
                .Add(new StringUnit("xxx").Action(() => isAddLexemAvailable = true))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("xxx");

            Assert.IsTrue(result);
            Assert.IsFalse(isLexem1Available);
            Assert.IsFalse(isLexem2Available);
            Assert.IsTrue(isAddLexemAvailable);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAnyAndAdd1Failed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            var isAddLexemAvailable = false;
            //-----------------------------------------------------------------
            sequence
                .Any(
                    new StringUnit("lexem").Action(() => isLexem1Available = true),
                    new StringUnit("pop").Action(() => isLexem2Available = true)
                )
                .Add(new StringUnit("xxx").Action(() => isAddLexemAvailable = true))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine(" lexem d");

            Assert.IsFalse(result);
            Assert.IsFalse(isLexem1Available);
            Assert.IsFalse(isLexem2Available);
            Assert.IsFalse(isAddLexemAvailable);
            Assert.IsTrue(error.PositionInLine == 7);
            Assert.IsTrue(string.Equals(((StringUnit)error.Unit).OriginalValue, "xxx", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestAnyAndAdd2Failed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            var isAddLexemAvailable = false;
            //-----------------------------------------------------------------
            sequence
                .Any(
                    new StringUnit("lexem").Action(() => isLexem1Available = true),
                    new StringUnit("pop").Action(() => isLexem2Available = true)
                )
                .Add(new StringUnit("xxx").Action(() => isAddLexemAvailable = true))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine(" lex d");

            Assert.IsFalse(result);
            Assert.IsFalse(isLexem1Available);
            Assert.IsFalse(isLexem2Available);
            Assert.IsFalse(isAddLexemAvailable);
            Assert.IsTrue(error.PositionInLine == 1);
            Assert.IsTrue(string.Equals(((StringUnit)error.Unit).OriginalValue, "xxx", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestAnyAndAdd3Failed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            var isAddLexemAvailable = false;
            //-----------------------------------------------------------------
            sequence
                .Any(
                    new StringUnit("lexem").Action(() => isLexem1Available = true),
                    new StringUnit("pop").Action(() => isLexem2Available = true)
                )
                .Add(new StringUnit("xxx").Action(() => isAddLexemAvailable = true))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine(" a");

            Assert.IsFalse(result);
            Assert.IsFalse(isLexem1Available);
            Assert.IsFalse(isLexem2Available);
            Assert.IsFalse(isAddLexemAvailable);
            Assert.IsTrue(error.PositionInLine == 1);
            Assert.IsTrue(string.Equals(((StringUnit)error.Unit).OriginalValue, "xxx", StringComparison.OrdinalIgnoreCase));
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestAnyAndAdds1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            var isAddLexem1Available = false;
            var isAddLexem2Available = false;
            //-----------------------------------------------------------------
            sequence
                .Any(
                    new StringUnit("lexem").Action(() => isLexem1Available = true),
                    new StringUnit("pop").Action(() => isLexem2Available = true)
                )
                .Add(
                    new StringUnit("xxx").Action(() => isAddLexem1Available = true),
                    new SymbolUnit(' '),
                    new StringUnit("ddd").Action(() => isAddLexem2Available = true)
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("pop xxx ddd");

            Assert.IsTrue(result);
            Assert.IsFalse(isLexem1Available);
            Assert.IsTrue(isLexem2Available);
            Assert.IsTrue(isAddLexem1Available);
            Assert.IsTrue(isAddLexem2Available);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAnyAndAdds2Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            var isAddLexem1Available = false;
            var isAddLexem2Available = false;
            //-----------------------------------------------------------------
            sequence
                .Any(
                    new StringUnit("lexem").Action(() => isLexem1Available = true),
                    new StringUnit("pop").Action(() => isLexem2Available = true)
                )
                .Add(
                    new StringUnit("xxx").Action(() => isAddLexem1Available = true),
                    new SymbolUnit(' '),
                    new StringUnit("ddd").Action(() => isAddLexem2Available = true)
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem xxx ddd");

            Assert.IsTrue(result);
            Assert.IsTrue(isLexem1Available);
            Assert.IsFalse(isLexem2Available);
            Assert.IsTrue(isAddLexem1Available);
            Assert.IsTrue(isAddLexem2Available);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAnyAndAdds3Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            var isAddLexem1Available = false;
            var isAddLexem2Available = false;
            //-----------------------------------------------------------------
            sequence
                .Any(
                    new StringUnit("lexem").Action(() => isLexem1Available = true),
                    new StringUnit("pop").Action(() => isLexem2Available = true)
                )
                .Add(
                    new StringUnit("xxx").Action(() => isAddLexem1Available = true),
                    new SymbolUnit(' '),
                    new StringUnit("ddd").Action(() => isAddLexem2Available = true)
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine(" xxx ddd");

            Assert.IsTrue(result);
            Assert.IsFalse(isLexem1Available);
            Assert.IsFalse(isLexem2Available);
            Assert.IsTrue(isAddLexem1Available);
            Assert.IsTrue(isAddLexem2Available);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAnyAndAdds1Failed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            var isAddLexem1Available = false;
            var isAddLexem2Available = false;
            //-----------------------------------------------------------------
            sequence
                .Any(
                    new StringUnit("lexem").Action(() => isLexem1Available = true),
                    new StringUnit("pop").Action(() => isLexem2Available = true)
                )
                .Add(
                    new StringUnit("xxx").Action(() => isAddLexem1Available = true),
                    new SymbolUnit(' '),
                    new StringUnit("ddd").Action(() => isAddLexem2Available = true)
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("pop xxx ddd d");

            Assert.IsFalse(result);
            Assert.IsFalse(isLexem1Available);
            Assert.IsFalse(isLexem2Available);
            Assert.IsFalse(isAddLexem1Available);
            Assert.IsFalse(isAddLexem2Available);
            Assert.IsTrue(error.PositionInLine == 12);
            Assert.IsTrue(error.Unit == null);
        }

        [TestMethod]
        public void TestAnyAndAdds2Failed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            var isAddLexem1Available = false;
            var isAddLexem2Available = false;
            //-----------------------------------------------------------------
            sequence
                .Any(
                    new StringUnit("lexem").Action(() => isLexem1Available = true),
                    new StringUnit("pop").Action(() => isLexem2Available = true)
                )
                .Add(
                    new StringUnit("xxx").Action(() => isAddLexem1Available = true),
                    new SymbolUnit(' '),
                    new StringUnit("ddd").Action(() => isAddLexem2Available = true)
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine(" xxx y");

            Assert.IsFalse(result);
            Assert.IsFalse(isLexem1Available);
            Assert.IsFalse(isLexem2Available);
            Assert.IsFalse(isAddLexem1Available);
            Assert.IsFalse(isAddLexem2Available);
            Assert.IsTrue(error.PositionInLine == 5);
            Assert.IsTrue(string.Equals(((StringUnit)error.Unit).OriginalValue, "ddd", StringComparison.OrdinalIgnoreCase));
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestAnyAndAny1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            var isLexem3Available = false;
            //-----------------------------------------------------------------
            sequence
                .Any(new StringUnit("lexem")
                        .Action(() => isLexem1Available = true),
                    new StringUnit("pop")
                        .Action(() => isLexem2Available = true)
                )
                .Any(new StringUnit("xxx").Action(() => isLexem3Available = true))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("pop xxx");

            Assert.IsTrue(result);
            Assert.IsFalse(isLexem1Available);
            Assert.IsTrue(isLexem2Available);
            Assert.IsTrue(isLexem3Available);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAnyAndAny2Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            var isLexem3Available = false;
            //-----------------------------------------------------------------
            sequence
                .Any(new StringUnit("lexem")
                        .Action(() => isLexem1Available = true),
                    new StringUnit("pop")
                        .Action(() => isLexem2Available = true)
                )
                .Any(new StringUnit("xxx").Action(() => isLexem3Available = true))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem xxx");

            Assert.IsTrue(result);
            Assert.IsTrue(isLexem1Available);
            Assert.IsFalse(isLexem2Available);
            Assert.IsTrue(isLexem3Available);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAnyAndAny3Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            var isLexem3Available = false;
            //-----------------------------------------------------------------
            sequence
                .Any(new StringUnit("lexem")
                        .Action(() => isLexem1Available = true),
                    new StringUnit("pop")
                        .Action(() => isLexem2Available = true)
                )
                .Any(new StringUnit("xxx").Action(() => isLexem3Available = true))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem");

            Assert.IsTrue(result);
            Assert.IsTrue(isLexem1Available);
            Assert.IsFalse(isLexem2Available);
            Assert.IsFalse(isLexem3Available);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAnyAndAny4Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            var isLexem3Available = false;
            //-----------------------------------------------------------------
            sequence
                .Any(new StringUnit("lexem")
                        .Action(() => isLexem1Available = true),
                    new StringUnit("pop")
                        .Action(() => isLexem2Available = true)
                )
                .Any(new StringUnit("xxx").Action(() => isLexem3Available = true))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("xxx");

            Assert.IsTrue(result);
            Assert.IsFalse(isLexem1Available);
            Assert.IsFalse(isLexem2Available);
            Assert.IsTrue(isLexem3Available);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAnyAndAny1Failed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            var isLexem3Available = false;
            //-----------------------------------------------------------------
            sequence
                .Any(new StringUnit("lexem")
                        .Action(() => isLexem1Available = true),
                    new StringUnit("pop")
                        .Action(() => isLexem2Available = true)
                )
                .Any(new StringUnit("xxx").Action(() => isLexem3Available = true))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("xxx pop");

            Assert.IsFalse(result);
            Assert.IsFalse(isLexem1Available);
            Assert.IsFalse(isLexem2Available);
            Assert.IsFalse(isLexem3Available);
            Assert.IsTrue(error.PositionInLine == 4);
            Assert.IsTrue(error.Unit == null);
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestAnys1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var lexemCountAvailable = 0;
            //-----------------------------------------------------------------
            sequence
                .Any(2, new StringUnit("lexem").Action(() => ++lexemCountAvailable))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem lexem");

            Assert.IsTrue(result);
            Assert.IsTrue(lexemCountAvailable == 2);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAnys2Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var lexemCountAvailable = 0;
            //-----------------------------------------------------------------
            sequence
                .Any(2, new StringUnit("lexem").Action(() => ++lexemCountAvailable))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem");

            Assert.IsTrue(result);
            Assert.IsTrue(lexemCountAvailable == 1);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAnys3Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var lexemCountAvailable = 0;
            //-----------------------------------------------------------------
            sequence
                .Any(2, new StringUnit("lexem").Action(() => ++lexemCountAvailable))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("");

            Assert.IsTrue(result);
            Assert.IsTrue(lexemCountAvailable == 0);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAnys1Failed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var lexemCountAvailable = 0;
            //-----------------------------------------------------------------
            sequence
                .Any(2, new StringUnit("lexem").Action(() => ++lexemCountAvailable))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem lexem lexem");

            Assert.IsFalse(result);
            Assert.IsTrue(lexemCountAvailable == 0);
            Assert.IsTrue(error != null);
            Assert.IsTrue((error.Unit).IsFirstSymbol(error.Line[error.PositionInLine]));
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestAnyContainer1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var lexemCount = 0;
            var xxxCount = 0;
            //-----------------------------------------------------------------
            var anyContainerUnit = sequence.CreateAnyContainer();
            anyContainerUnit
                .Add(2, new StringUnit("lexem").Action(() => ++lexemCount))
                .Add(2, new StringUnit("xxx").Action(() => ++xxxCount));

            sequence
                .Add(anyContainerUnit)
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("xxx lexem xxx lexem");

            Assert.IsTrue(result);
            Assert.IsTrue(lexemCount == 2);
            Assert.IsTrue(xxxCount == 2);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAnyContainer2Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var lexemCount = 0;
            var xxxCount = 0;
            //-----------------------------------------------------------------
            var anyContainerUnit = sequence.CreateAnyContainer();
            anyContainerUnit
                .Add(2, new StringUnit("lexem").Action(() => ++lexemCount))
                .Add(2, new StringUnit("xxx").Action(() => ++xxxCount));

            sequence
                .Add(anyContainerUnit)
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("xxx ");

            Assert.IsTrue(result);
            Assert.IsTrue(lexemCount == 0);
            Assert.IsTrue(xxxCount == 1);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAnyContainer1Failed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var lexemCount = 0;
            var xxxCount = 0;
            //-----------------------------------------------------------------
            var anyContainerUnit = sequence.CreateAnyContainer();
            anyContainerUnit
                .Add(2, new StringUnit("lexem").Action(() => ++lexemCount))
                .Add(2, new StringUnit("xxx").Action(() => ++xxxCount));

            sequence
                .Add(anyContainerUnit)
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("xxx lexem xxx xxx");

            Assert.IsFalse(result);
            Assert.IsTrue(lexemCount == 0);
            Assert.IsTrue(xxxCount == 0);
            Assert.IsTrue(error != null);
            Assert.IsTrue(error.Unit.IsFirstSymbol(error.Line[error.PositionInLine]));
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestAnyAndIfElse1Success()
        {
            var ifElseContainer = new Sequence();

            ifElseContainer
                .If(new StringUnit("lexem"));
            ifElseContainer
                .Else(new StringUnit("xxx"));

            var anyContainer = new ContainerAnyUnits();
            anyContainer.Add(ifElseContainer);
            anyContainer.Add(new StringUnit("aaa"));

            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            sequence
                .Add(anyContainer)
                .Error(x => error = x);

            var result = sequence.DecodeLine("lexem aaa");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAnyAndIfElse2Success()
        {
            //-----------------------------------------------------------------
            var ifElseContainer1 = new Sequence();

            ifElseContainer1
                .If(new StringUnit("lexem"));
            ifElseContainer1
                .Else(new StringUnit("xxx"));

            var anyContainer = new ContainerAnyUnits();
            anyContainer.Add(ifElseContainer1);
            //-----------------------------------------------------------------
            var ifElseContainer2 = new Sequence();

            ifElseContainer2
                .If(new StringUnit("aaa"));
            ifElseContainer2
                .Else(new StringUnit("bbb"));
            //-----------------------------------------------------------------
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            sequence
                .Add(anyContainer, ifElseContainer2)
                .Error(x => error = x);

            var result = sequence.DecodeLine("xxx bbb");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestAnyAndIfElseFailed()
        {
            var ifElseContainer = new Sequence();

            ifElseContainer
                .If(new StringUnit("lexem"));
            ifElseContainer
                .Else(new StringUnit("xxx"));

            var anyContainer = new ContainerAnyUnits();
            anyContainer.Add(ifElseContainer);
            anyContainer.Add(new StringUnit("aaa"));

            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            sequence
                .Add(anyContainer)
                .Error(x => error = x);

            var result = sequence.DecodeLine("lexem xxx");

            Assert.IsFalse(result);
            Assert.IsTrue(error != null);
            Assert.IsTrue(error.PositionInLine == 6);
            Assert.IsTrue(error.Unit.IsFirstSymbol(error.Line[error.PositionInLine]));
        }
        //---------------------------------------------------------------------
    }
}