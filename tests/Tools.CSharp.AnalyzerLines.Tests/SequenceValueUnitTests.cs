using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tools.CSharp.AnalyzerLines.Tests
{
    [TestClass]
    public class SequenceValueUnitTests
    {
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestValueAndNoIgnoreSymbols1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            ValueUnit value = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new ValueUnit().Action(x => value = x).ClearIgnoreSymbols(true))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("       ");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(value.Available);
            Assert.IsTrue(value.StartPosition == 0);
            Assert.IsTrue(string.Equals(value.Value, "       ", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestValueAndNoIgnoreSymbols2Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            ValueUnit value = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new ValueUnit().Action(x => value = x).ClearIgnoreSymbols(true))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("       value   ");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(value.Available);
            Assert.IsTrue(value.StartPosition == 0);
            Assert.IsTrue(string.Equals(value.Value, "       value   ", StringComparison.OrdinalIgnoreCase));
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestValue1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            ValueUnit value = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new ValueUnit().Action(x => value = x))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(value.Available);
            Assert.IsTrue(value.StartPosition == 0);
            Assert.IsTrue(string.Equals(value.Value, "", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestValue2Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            ValueUnit value = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new ValueUnit().Action(x => value = x))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("        ");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(value.Available);
            Assert.IsTrue(value.StartPosition == 0);
            Assert.IsTrue(string.Equals(value.Value, "", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestValue3Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            ValueUnit value = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new ValueUnit().Action(x => value = x))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("value");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(value.Available);
            Assert.IsTrue(value.StartPosition == 0);
            Assert.IsTrue(string.Equals(value.Value, "value", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestValue4Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            ValueUnit value = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new ValueUnit().Action(x => value = x))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("value       ");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(value.Available);
            Assert.IsTrue(value.StartPosition == 0);
            Assert.IsTrue(string.Equals(value.Value, "value", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestValueFailed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            ValueUnit value = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new ValueUnit().Action(x => value = x))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("value value      ");

            Assert.IsFalse(result);
            Assert.IsTrue(error != null);
            Assert.IsTrue(error.PositionInLine == 6);
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestValueAndNextUnit1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            ValueUnit value = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new ValueUnit().Action(x => value = x), new SymbolUnit(';'))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("       value       ;    ");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(value.StartPosition == 7);
            Assert.IsTrue(string.Equals(value.Value, "value", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestValueAndNextUnit2Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            ValueUnit value = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new ValueUnit().Action(x => value = x), new SymbolUnit(';'))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("value       ;    ");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(value.StartPosition == 0);
            Assert.IsTrue(string.Equals(value.Value, "value", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestValueAndNextUnit3Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            ValueUnit value = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new ValueUnit().Action(x => value = x), new SymbolUnit(';'))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("value;    ");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(value.StartPosition == 0);
            Assert.IsTrue(string.Equals(value.Value, "value", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestValueAndNextUnit4Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            ValueUnit value = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new ValueUnit().Action(x => value = x), new SymbolUnit(';'))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine(";    ");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(value.StartPosition == 0);
            Assert.IsTrue(string.Equals(value.Value, "", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestValueAndNextUnit5Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            ValueUnit value = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new ValueUnit().Action(x => value = x), new SymbolUnit(';'))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("       value value      ;    ");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(value.StartPosition == 7);
            Assert.IsTrue(string.Equals(value.Value, "value value", StringComparison.OrdinalIgnoreCase));
        }
        //---------------------------------------------------------------------
    }
}