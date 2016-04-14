using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tools.CSharp.AnalyzerLines.Tests
{
    [TestClass]
    public class SequenceIfAndElseUnitTests
    {
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestIfSuccess()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence
                .If(new StringUnit("lexem"));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestIfFailed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence.If(new StringUnit("lexem"));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexe");

            Assert.IsFalse(result);
            Assert.IsTrue(error.PositionInLine == 3);
            Assert.IsTrue(string.Equals(((StringUnit)error.Unit).OriginalValue, "lexem", StringComparison.OrdinalIgnoreCase));
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestIfAndElseMore1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            var sequenceIf = new Sequence();
            sequenceIf
                .If(new StringUnit("ll"));
            sequenceIf
                .Else(new StringUnit("xxx"));
            //-----------------------------------------------------------------
            sequence
                .If(new StringUnit("lexem"))
                    .Add(sequenceIf);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine(" lexem xxx");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestIfAndElseMore2Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            var sequenceIf = new Sequence();
            sequenceIf
                .If(new StringUnit("ll"));
            sequenceIf
                .Else(new StringUnit("xxx"));
            //-----------------------------------------------------------------
            sequence
                .If(new StringUnit("lexem"))
                    .Add(sequenceIf);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine(" lexem ll     ");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestIfAndActionSuccess()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isIfAvailable = false;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence
                .If(new StringUnit("lexem").Action(() => isIfAvailable = true));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem");

            Assert.IsTrue(result);
            Assert.IsTrue(isIfAvailable);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestIfAndActionFailed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isIfAvailable = false;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence
                .If(new StringUnit("lexem").Action(() => isIfAvailable = true));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("leaem");

            Assert.IsFalse(result);
            Assert.IsFalse(isIfAvailable);
            Assert.IsTrue(error.PositionInLine == 2);
            Assert.IsTrue(error.PositionInUnit == 2);
            Assert.IsTrue(string.Equals(((StringUnit)error.Unit).OriginalValue, "lexem", StringComparison.OrdinalIgnoreCase));
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestIfAndElseSuccess()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isIfAvailable = false;
            var isElseAvailable = false;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence
                .If(new StringUnit("lexem").Action(() => isIfAvailable = true));
            sequence
                .Else(new StringUnit("pop").Action(() => isElseAvailable = true));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("pop");

            Assert.IsTrue(result);
            Assert.IsFalse(isIfAvailable);
            Assert.IsTrue(isElseAvailable);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestIfAndElseFailed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isIfAvailable = false;
            var isElseAvailable = false;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence
                .If(new StringUnit("lexem").Action(() => isIfAvailable = true));
            sequence
                .Else(new StringUnit("pop").Action(() => isElseAvailable = true));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("pxxx");

            Assert.IsFalse(result);
            Assert.IsFalse(isIfAvailable);
            Assert.IsFalse(isElseAvailable);
            Assert.IsTrue(error.PositionInLine == 1);
            Assert.IsTrue(error.PositionInLine == 1);
            Assert.IsTrue(string.Equals(((StringUnit)error.Unit).OriginalValue, "pop", StringComparison.OrdinalIgnoreCase));
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestIfAndElses1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isIfAvailable = false;
            var isElse1Available = false;
            var isElse2Available = false;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence
                .If(new StringUnit("lexem").Action(() => isIfAvailable = true));
            sequence
                .Else(new StringUnit("pop").Action(() => isElse1Available = true));
            sequence
                .Else(new StringUnit("xxx").Action(() => isElse2Available = true));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("xxx");

            Assert.IsTrue(result);
            Assert.IsFalse(isIfAvailable);
            Assert.IsFalse(isElse1Available);
            Assert.IsTrue(isElse2Available);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestIfAndElses1Failed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isIfAvailable = false;
            var isElse1Available = false;
            var isElse2Available = false;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence
                .If(new StringUnit("lexem").Action(() => isIfAvailable = true));
            sequence
                .Else(new StringUnit("pop").Action(() => isElse1Available = true));
            sequence
                .Else(new StringUnit("xxx").Action(() => isElse2Available = true));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("  www");

            Assert.IsFalse(result);
            Assert.IsFalse(isIfAvailable);
            Assert.IsFalse(isElse1Available);
            Assert.IsFalse(isElse2Available);
            Assert.IsTrue(error.PositionInLine == 2);
            Assert.IsTrue(error.Unit == null);
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestIfAndElses2Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isIfAvailable = false;
            var isElse1Available = false;
            var isElse2Available = false;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence
                .If(new StringUnit("lexem").Action(() => isIfAvailable = true));
            sequence
                .Else(new StringUnit("pop").Action(() => isElse1Available = true));
            sequence
                .Else(new StringUnit("xxx").Action(() => isElse2Available = true));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("pop");

            Assert.IsTrue(result);
            Assert.IsFalse(isIfAvailable);
            Assert.IsTrue(isElse1Available);
            Assert.IsFalse(isElse2Available);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestIfAndElses2Failed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isIfAvailable = false;
            var isElse1Available = false;
            var isElse2Available = false;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence
                .If(new StringUnit("lexem").Action(() => isIfAvailable = true));
            sequence
                .Else(new StringUnit("pop").Action(() => isElse1Available = true));
            sequence
                .Else(new StringUnit("xxx").Action(() => isElse2Available = true));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("pxxx");

            Assert.IsFalse(result);
            Assert.IsFalse(isIfAvailable);
            Assert.IsFalse(isElse1Available);
            Assert.IsFalse(isElse2Available);
            Assert.IsTrue(error.PositionInLine == 1);
            Assert.IsTrue(string.Equals(((StringUnit)error.Unit).OriginalValue, "pop", StringComparison.OrdinalIgnoreCase));
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestIfAndElses3Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isIfAvailable = false;
            var isElse1Available = false;
            var isElse2Available = false;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence
                .If(new StringUnit("lexem11").Action(() => isIfAvailable = true));
            sequence
                .Else(new StringUnit("lexem1").Action(() => isElse1Available = true));
            sequence
                .Else(new StringUnit("lexem").Action(() => isElse2Available = true));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem1");

            Assert.IsTrue(result);
            Assert.IsFalse(isIfAvailable);
            Assert.IsTrue(isElse1Available);
            Assert.IsFalse(isElse2Available);
            Assert.IsTrue(error == null);
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestIfAndAddSuccess()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isIfAvailable = false;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence
                .If(new StringUnit("lexem").Action(() => isIfAvailable = true))
                    .Add(new StringUnit("ddd"));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem ddd");

            Assert.IsTrue(result);
            Assert.IsTrue(isIfAvailable);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestIfAndAddFailed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isIfAvailable = false;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence
                .If(new StringUnit("lexem").Action(() => isIfAvailable = true))
                    .Add(new StringUnit("ddd"));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem dd");

            Assert.IsFalse(result);
            Assert.IsFalse(isIfAvailable);
            Assert.IsTrue(error.PositionInLine == 7);
            Assert.IsTrue(error.PositionInUnit == 2);
            Assert.IsTrue(string.Equals(((StringUnit)error.Unit).OriginalValue, "ddd", StringComparison.OrdinalIgnoreCase));
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestIfAndAnySuccess()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexem1Available = false;
            var isLexem2Available = false;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence
                .IfAny(
                    new StringUnit("lexem").Action(() => isLexem1Available = true),
                    new StringUnit("ddd").Action(() => isLexem2Available = true));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine(" ddd");

            Assert.IsTrue(result);
            Assert.IsFalse(isLexem1Available);
            Assert.IsTrue(isLexem2Available);
            Assert.IsTrue(error == null);
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestIfAndAddAndElse1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isIfAvailable = false;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence
                .If(new StringUnit("lexem"))
                    .Add(new StringUnit("ddd").Action(() => isIfAvailable = true));
            sequence
                .Else(new StringUnit("aaa"));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem ddd");

            Assert.IsTrue(result);
            Assert.IsTrue(isIfAvailable);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestIfAndAddAndElse2Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isIfAvailable = false;
            var isElseAvailable = false;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence
                .If(new StringUnit("lexem"))
                    .Add(new StringUnit("ddd").Action(() => isIfAvailable = true));
            sequence
                .Else(new StringUnit("aaa").Action(() => isElseAvailable = true));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("aaa");

            Assert.IsTrue(result);
            Assert.IsFalse(isIfAvailable);
            Assert.IsTrue(isElseAvailable);
            Assert.IsTrue(error == null);
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestIfAndAddAndElseAndAdd1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isIfAvailable = false;
            var isElseAvailable = false;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence
                .If(new StringUnit("lexem"))
                    .Add(new StringUnit("ddd").Action(() => isIfAvailable = true));
            sequence
                .Else(new StringUnit("aaa"))
                    .Add(new StringUnit("yyy").Action(() => isElseAvailable = true));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("aaa yyy");

            Assert.IsTrue(result);
            Assert.IsFalse(isIfAvailable);
            Assert.IsTrue(isElseAvailable);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestIfAndAddAndElseAndAdd2Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isIfAvailable = false;
            var isElseAvailable = false;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence
                .If(new StringUnit("lexem"))
                    .Add(new StringUnit("ddd").Action(() => isIfAvailable = true));
            sequence
                .Else(new StringUnit("aaa"));
            sequence
                 .Add(new StringUnit("yyy").Action(() => isElseAvailable = true));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem ddd yyy");

            Assert.IsTrue(result);
            Assert.IsTrue(isIfAvailable);
            Assert.IsTrue(isElseAvailable);
            Assert.IsTrue(error == null);
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestIfAndAddAndElseAndAddOnlyCurrent1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isIfAvailable = false;
            var isElseAvailable = false;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence
                .If(new StringUnit("lexem"))
                    .Add(new StringUnit("ddd").Action(() => isIfAvailable = true));
            sequence
                .Else(new StringUnit("aaa"))
                    .Add(new StringUnit("yyy").Action(() => isElseAvailable = true));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem ddd");

            Assert.IsTrue(result);
            Assert.IsTrue(isIfAvailable);
            Assert.IsFalse(isElseAvailable);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestIfAndAddAndElseAndAddOnlyCurrent2Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isIfAvailable = false;
            var isElseAvailable = false;
            //-----------------------------------------------------------------
            sequence.Error(x => error = x);
            //-----------------------------------------------------------------
            sequence
                .If(new StringUnit("lexem").Action(() => isIfAvailable = true));
            sequence
                .Else(new StringUnit("aaa"))
                    .Add(new StringUnit("yyy").Action(() => isElseAvailable = true));
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem");

            Assert.IsTrue(result);
            Assert.IsTrue(isIfAvailable);
            Assert.IsFalse(isElseAvailable);
            Assert.IsTrue(error == null);
        }
        //---------------------------------------------------------------------
    }
}