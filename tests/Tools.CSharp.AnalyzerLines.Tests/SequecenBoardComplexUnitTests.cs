using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tools.CSharp.AnalyzerLines.Tests
{
    [TestClass]
    public class SequecenBoardComplexUnitTests
    {
        #region private
        private const string _SlavingLexemName = "slaving";
        private const string _UserLexemName = "user";
        #endregion
        //---------------------------------------------------------------------
        [TestMethod]
        public void Test1BoardSuccess()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            //-----------------------------------------------------------------
            var shelfValue = "";
            var slotValue = "";
            var isSlavingAvailable = false;
            var isUserAvailable = false;
            UnitError error = null;
            //-----------------------------------------------------------------
            var shelfValueAction = new Action<ValueUnit>(x => shelfValue = x.Value);
            var slotValueAction = new Action<ValueUnit>(x => slotValue = x.Value);
            var slavingAction = new Action(() => isSlavingAvailable = true);
            var userAction = new Action(() => isUserAvailable = true);
            //-----------------------------------------------------------------
            sequence
                .If(
                    new ValueUnit().Action(shelfValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(slotValueAction)
                );
            sequence
                .Else(
                    new ValueUnit().Action(slotValueAction)
                );
            sequence.Any(
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit(_SlavingLexemName).Action(slavingAction)),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit(_UserLexemName).Action(userAction))
                );
            sequence
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("8,12");

            Assert.IsTrue(result);
            Assert.IsTrue(string.Equals(shelfValue, "8", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(string.Equals(slotValue, "12", StringComparison.OrdinalIgnoreCase));
            Assert.IsFalse(isSlavingAvailable);
            Assert.IsFalse(isUserAvailable);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void Tes2BoardSuccess()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            //-----------------------------------------------------------------
            var shelfValue = "";
            var slotValue = "";
            var isSlavingAvailable = false;
            var isUserAvailable = false;
            UnitError error = null;
            //-----------------------------------------------------------------
            var shelfValueAction = new Action<ValueUnit>(x => shelfValue = x.Value);
            var slotValueAction = new Action<ValueUnit>(x => slotValue = x.Value);
            var slavingAction = new Action(() => isSlavingAvailable = true);
            var userAction = new Action(() => isUserAvailable = true);
            //-----------------------------------------------------------------
            sequence
                .If(
                    new ValueUnit().Action(shelfValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(slotValueAction)
                );
            sequence
                .Else(
                    new ValueUnit().Action(slotValueAction)
                );
            sequence.Any(
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit(_SlavingLexemName).Action(slavingAction)),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit(_UserLexemName).Action(userAction))
                );
            sequence
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine($"8,12   {_SlavingLexemName}");

            Assert.IsTrue(result);
            Assert.IsTrue(string.Equals(shelfValue, "8", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(string.Equals(slotValue, "12", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(isSlavingAvailable);
            Assert.IsFalse(isUserAvailable);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void Test3BoardSuccess()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            //-----------------------------------------------------------------
            var shelfValue = "";
            var slotValue = "";
            var isSlavingAvailable = false;
            var isUserAvailable = false;
            UnitError error = null;
            //-----------------------------------------------------------------
            var shelfValueAction = new Action<ValueUnit>(x => shelfValue = x.Value);
            var slotValueAction = new Action<ValueUnit>(x => slotValue = x.Value);
            var slavingAction = new Action(() => isSlavingAvailable = true);
            var userAction = new Action(() => isUserAvailable = true);
            //-----------------------------------------------------------------
            sequence
                .If(
                    new ValueUnit().Action(shelfValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(slotValueAction)
                );
            sequence
                .Else(
                    new ValueUnit().Action(slotValueAction)
                );
            sequence.Any(
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit(_SlavingLexemName).Action(slavingAction)),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit(_UserLexemName).Action(userAction))
                );
            sequence
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine($"8,12 {_UserLexemName} {_SlavingLexemName}");

            Assert.IsTrue(result);
            Assert.IsTrue(string.Equals(shelfValue, "8", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(string.Equals(slotValue, "12", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(isSlavingAvailable);
            Assert.IsTrue(isUserAvailable);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void Test4BoardSuccess()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            //-----------------------------------------------------------------
            var shelfValue = "";
            var slotValue = "";
            var isSlavingAvailable = false;
            var isUserAvailable = false;
            UnitError error = null;
            //-----------------------------------------------------------------
            var shelfValueAction = new Action<ValueUnit>(x => shelfValue = x.Value);
            var slotValueAction = new Action<ValueUnit>(x => slotValue = x.Value);
            var slavingAction = new Action(() => isSlavingAvailable = true);
            var userAction = new Action(() => isUserAvailable = true);
            //-----------------------------------------------------------------
            sequence
                .If(
                    new ValueUnit().Action(shelfValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(slotValueAction)
                );
            sequence
                .Else(
                    new ValueUnit().Action(slotValueAction)
                );
            sequence.Any(
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit(_SlavingLexemName).Action(slavingAction)),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit(_UserLexemName).Action(userAction))
                );
            sequence
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("8");

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(shelfValue));
            Assert.IsTrue(string.Equals(slotValue, "8", StringComparison.OrdinalIgnoreCase));
            Assert.IsFalse(isSlavingAvailable);
            Assert.IsFalse(isUserAvailable);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void Test5BoardSuccess()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            //-----------------------------------------------------------------
            var shelfValue = "";
            var slotValue = "";
            var isSlavingAvailable = false;
            var isUserAvailable = false;
            UnitError error = null;
            //-----------------------------------------------------------------
            var shelfValueAction = new Action<ValueUnit>(x => shelfValue = x.Value);
            var slotValueAction = new Action<ValueUnit>(x => slotValue = x.Value);
            var slavingAction = new Action(() => isSlavingAvailable = true);
            var userAction = new Action(() => isUserAvailable = true);
            //-----------------------------------------------------------------
            sequence
                .If(
                    new ValueUnit().Action(shelfValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(slotValueAction)
                );
            sequence
                .Else(
                    new ValueUnit().Action(slotValueAction)
                );
            sequence.Any(
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit(_SlavingLexemName).Action(slavingAction)),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit(_UserLexemName).Action(userAction))
                );
            sequence
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine($"8 {_UserLexemName}");

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(shelfValue));
            Assert.IsTrue(string.Equals(slotValue, "8", StringComparison.OrdinalIgnoreCase));
            Assert.IsFalse(isSlavingAvailable);
            Assert.IsTrue(isUserAvailable);
            Assert.IsTrue(error == null);
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void Test1BoardFailed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            //-----------------------------------------------------------------
            UnitError error = null;
            //-----------------------------------------------------------------
            sequence
                .If(
                    new ValueUnit(),
                    new SymbolUnit(','),
                    new ValueUnit()
                );
            sequence
                .Else(
                    new ValueUnit()
                );
            sequence.Any(
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit(_SlavingLexemName)),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit(_UserLexemName))
                );
            sequence
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("8, 12");

            Assert.IsFalse(result);
            Assert.IsTrue(error.PositionInLine == 3);
            Assert.IsTrue(error.Unit == null);
        }

        [TestMethod]
        public void Test2BoardFailed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            //-----------------------------------------------------------------
            UnitError error = null;
            //-----------------------------------------------------------------
            sequence
                .If(
                    new ValueUnit(),
                    new SymbolUnit(','),
                    new ValueUnit()
                );
            sequence
                .Else(
                    new ValueUnit()
                );
            sequence.Any(
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit(_SlavingLexemName)),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit(_UserLexemName))
                );
            sequence
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("8,12 d");

            Assert.IsFalse(result);
            Assert.IsTrue(error.PositionInLine == 5);
            Assert.IsTrue(error.Unit == null);
        }
        //---------------------------------------------------------------------
    }
}