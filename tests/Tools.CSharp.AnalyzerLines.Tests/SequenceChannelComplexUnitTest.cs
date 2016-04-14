using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tools.CSharp.AnalyzerLines.Tests
{
    [TestClass]
    public class SequenceChannelComplexUnitTest
    {
        //---------------------------------------------------------------------
        [TestMethod]
        public void Test1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            //-----------------------------------------------------------------
            var shelfValue = "";
            var slotValue = "";
            var channelValue = "";
            var numberValue = "";
            var raniValue = false;
            var aniValue = "";
            var ani2Value = "";
            var arouteDelayValue = "";
            var arouteNumValue = "";
            var prioValue = "";
            var infoValue = "";
            var attribValue = "";
            UnitError error = null;
            //-----------------------------------------------------------------
            var shelfValueAction = new Action<ValueUnit>(x => shelfValue = x.Value);
            var slotValueAction = new Action<ValueUnit>(x => slotValue = x.Value);
            var channelValueAction = new Action<ValueUnit>(x => channelValue = x.Value);
            var numberValueAction = new Action<ValueUnit>(x => numberValue = x.Value);
            var raniValueAction = new Action(() => raniValue = true);
            var aniValueAction = new Action<ValueUnit>(x => aniValue = x.Value);
            var ani2ValueAction = new Action<ValueUnit>(x => ani2Value = x.Value);
            var arouteDelayValueAction = new Action<ValueUnit>(x => arouteDelayValue = x.Value);
            var arouteNumValueAction = new Action<ValueUnit>(x => arouteNumValue = x.Value);
            var prioValueAction = new Action<ValueUnit>(x => prioValue = x.Value);
            var infoValueAction = new Action<ValueUnit>(x => infoValue = x.Value);
            var attribValueAction = new Action<ValueUnit>(x => attribValue = x.Value);
            //-----------------------------------------------------------------
            sequence
                .If(
                    new StringUnit("loc"),
                    new SymbolUnit('('),
                    new ValueUnit().Action(shelfValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(slotValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(channelValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(numberValueAction),
                    new SymbolUnit(')')
                );
            sequence
                .Else(
                    new StringUnit("loc"),
                    new SymbolUnit('('),
                    new ValueUnit().Action(slotValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(channelValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(numberValueAction),
                    new SymbolUnit(')')
                );
            sequence
                .Any(
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("rani").Action(raniValueAction)),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("ani-"), new ValueUnit().Action(aniValueAction)),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("ani2-"), new ValueUnit().Action(ani2ValueAction)),
                    sequence.CreateListContainer(
                            new SymbolUnit(' '),
                            new StringUnit("aroute"),
                            new SymbolUnit('('),
                            new ValueUnit().Action(arouteDelayValueAction),
                            new SymbolUnit(','),
                            new ValueUnit().Action(arouteNumValueAction),
                            new SymbolUnit(')')
                        ),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("prio-"), new ValueUnit().Action(prioValueAction)),
                    sequence.CreateListContainer(
                            new SymbolUnit(' '),
                            new StringUnit("info"),
                            new SymbolUnit('('),
                            new ValueUnit().Action(infoValueAction),
                            new SymbolUnit(')')
                        ),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("attr-"), new ValueUnit().Action(attribValueAction))
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("loc(1,2,3) prio-test");

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(shelfValue));
            Assert.IsTrue(string.Equals(slotValue, "1", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(string.Equals(channelValue, "2", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(string.Equals(numberValue, "3", StringComparison.OrdinalIgnoreCase));
            Assert.IsFalse(raniValue);
            Assert.IsTrue(string.IsNullOrEmpty(aniValue));
            Assert.IsTrue(string.IsNullOrEmpty(ani2Value));
            Assert.IsTrue(string.IsNullOrEmpty(arouteDelayValue));
            Assert.IsTrue(string.IsNullOrEmpty(arouteNumValue));
            Assert.IsTrue(string.Equals(prioValue, "test", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(string.IsNullOrEmpty(infoValue));
            Assert.IsTrue(string.IsNullOrEmpty(attribValue));
            Assert.IsTrue(error == null);
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void Test2Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            //-----------------------------------------------------------------
            var shelfValue = "";
            var slotValue = "";
            var channelValue = "";
            var numberValue = "";
            var raniValue = false;
            var aniValue = "";
            var ani2Value = "";
            var arouteDelayValue = "";
            var arouteNumValue = "";
            var prioValue = "";
            var infoValue = "";
            var attribValue = "";
            UnitError error = null;
            //-----------------------------------------------------------------
            var shelfValueAction = new Action<ValueUnit>(x => shelfValue = x.Value);
            var slotValueAction = new Action<ValueUnit>(x => slotValue = x.Value);
            var channelValueAction = new Action<ValueUnit>(x => channelValue = x.Value);
            var numberValueAction = new Action<ValueUnit>(x => numberValue = x.Value);
            var raniValueAction = new Action(() => raniValue = true);
            var aniValueAction = new Action<ValueUnit>(x => aniValue = x.Value);
            var ani2ValueAction = new Action<ValueUnit>(x => ani2Value = x.Value);
            var arouteDelayValueAction = new Action<ValueUnit>(x => arouteDelayValue = x.Value);
            var arouteNumValueAction = new Action<ValueUnit>(x => arouteNumValue = x.Value);
            var prioValueAction = new Action<ValueUnit>(x => prioValue = x.Value);
            var infoValueAction = new Action<ValueUnit>(x => infoValue = x.Value);
            var attribValueAction = new Action<ValueUnit>(x => attribValue = x.Value);
            //-----------------------------------------------------------------
            sequence
                .If(
                    new StringUnit("loc"),
                    new SymbolUnit('('),
                    new ValueUnit().Action(shelfValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(slotValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(channelValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(numberValueAction),
                    new SymbolUnit(')')
                );
            sequence
                .Else(
                    new StringUnit("loc"),
                    new SymbolUnit('('),
                    new ValueUnit().Action(slotValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(channelValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(numberValueAction),
                    new SymbolUnit(')')
                );
            sequence
                .Any(
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("rani").Action(raniValueAction)),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("ani-"), new ValueUnit().Action(aniValueAction)),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("ani2-"), new ValueUnit().Action(ani2ValueAction)),
                    sequence.CreateListContainer(
                            new SymbolUnit(' '),
                            new StringUnit("aroute"),
                            new SymbolUnit('('),
                            new ValueUnit().Action(arouteDelayValueAction),
                            new SymbolUnit(','),
                            new ValueUnit().Action(arouteNumValueAction),
                            new SymbolUnit(')')
                        ),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("prio-"), new ValueUnit().Action(prioValueAction)),
                    sequence.CreateListContainer(
                            new SymbolUnit(' '),
                            new StringUnit("info"),
                            new SymbolUnit('('),
                            new ValueUnit().Action(infoValueAction),
                            new SymbolUnit(')')
                        ),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("attr-"), new ValueUnit().Action(attribValueAction))
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("loc(6,1,2,3) rani info(value)");

            Assert.IsTrue(result);
            Assert.IsTrue(string.Equals(shelfValue, "6", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(string.Equals(slotValue, "1", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(string.Equals(channelValue, "2", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(string.Equals(numberValue, "3", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(raniValue);
            Assert.IsTrue(string.IsNullOrEmpty(aniValue));
            Assert.IsTrue(string.IsNullOrEmpty(ani2Value));
            Assert.IsTrue(string.IsNullOrEmpty(arouteDelayValue));
            Assert.IsTrue(string.IsNullOrEmpty(arouteNumValue));
            Assert.IsTrue(string.IsNullOrEmpty(prioValue));
            Assert.IsTrue(string.Equals(infoValue, "value", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(string.IsNullOrEmpty(attribValue));
            Assert.IsTrue(error == null);
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void Test3Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            //-----------------------------------------------------------------
            var shelfValue = "";
            var slotValue = "";
            var channelValue = "";
            var numberValue = "";
            var raniValue = false;
            var aniValue = "";
            var ani2Value = "";
            var arouteDelayValue = "";
            var arouteNumValue = "";
            var prioValue = "";
            var infoValue = "";
            var attribValue = "";
            UnitError error = null;
            //-----------------------------------------------------------------
            var shelfValueAction = new Action<ValueUnit>(x => shelfValue = x.Value);
            var slotValueAction = new Action<ValueUnit>(x => slotValue = x.Value);
            var channelValueAction = new Action<ValueUnit>(x => channelValue = x.Value);
            var numberValueAction = new Action<ValueUnit>(x => numberValue = x.Value);
            var raniValueAction = new Action(() => raniValue = true);
            var aniValueAction = new Action<ValueUnit>(x => aniValue = x.Value);
            var ani2ValueAction = new Action<ValueUnit>(x => ani2Value = x.Value);
            var arouteDelayValueAction = new Action<ValueUnit>(x => arouteDelayValue = x.Value);
            var arouteNumValueAction = new Action<ValueUnit>(x => arouteNumValue = x.Value);
            var prioValueAction = new Action<ValueUnit>(x => prioValue = x.Value);
            var infoValueAction = new Action<ValueUnit>(x => infoValue = x.Value);
            var attribValueAction = new Action<ValueUnit>(x => attribValue = x.Value);
            //-----------------------------------------------------------------
            sequence
                .If(
                    new StringUnit("loc"),
                    new SymbolUnit('('),
                    new ValueUnit().Action(shelfValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(slotValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(channelValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(numberValueAction),
                    new SymbolUnit(')')
                );
            sequence
                .Else(
                    new StringUnit("loc"),
                    new SymbolUnit('('),
                    new ValueUnit().Action(slotValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(channelValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(numberValueAction),
                    new SymbolUnit(')')
                );
            sequence
                .Any(
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("rani").Action(raniValueAction)),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("ani-"), new ValueUnit().Action(aniValueAction)),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("ani2-"), new ValueUnit().Action(ani2ValueAction)),
                    sequence.CreateListContainer(
                            new SymbolUnit(' '),
                            new StringUnit("aroute"),
                            new SymbolUnit('('),
                            new ValueUnit().Action(arouteDelayValueAction),
                            new SymbolUnit(','),
                            new ValueUnit().Action(arouteNumValueAction),
                            new SymbolUnit(')')
                        ),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("prio-"), new ValueUnit().Action(prioValueAction)),
                    sequence.CreateListContainer(
                            new SymbolUnit(' '),
                            new StringUnit("info"),
                            new SymbolUnit('('),
                            new ValueUnit().Action(infoValueAction),
                            new SymbolUnit(')')
                        ),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("attr-"), new ValueUnit().Action(attribValueAction))
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("loc(6,1,2,3) attr-111 rani ani-23 prio-dd ani2-33");

            Assert.IsTrue(result);
            Assert.IsTrue(string.Equals(shelfValue, "6", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(string.Equals(slotValue, "1", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(string.Equals(channelValue, "2", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(string.Equals(numberValue, "3", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(raniValue);
            Assert.IsTrue(string.Equals(aniValue, "23", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(string.Equals(ani2Value, "33", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(string.IsNullOrEmpty(arouteDelayValue));
            Assert.IsTrue(string.IsNullOrEmpty(arouteNumValue));
            Assert.IsTrue(string.Equals(prioValue, "dd", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(string.IsNullOrEmpty(infoValue));
            Assert.IsTrue(string.Equals(attribValue, "111", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(error == null);
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void Test4Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            //-----------------------------------------------------------------
            var shelfValue = "";
            var slotValue = "";
            var channelValue = "";
            var numberValue = "";
            var raniValue = false;
            var aniValue = "";
            var ani2Value = "";
            var arouteDelayValue = "";
            var arouteNumValue = "";
            var prioValue = "";
            var infoValue = "";
            var attribValue = "";
            UnitError error = null;
            //-----------------------------------------------------------------
            var shelfValueAction = new Action<ValueUnit>(x => shelfValue = x.Value);
            var slotValueAction = new Action<ValueUnit>(x => slotValue = x.Value);
            var channelValueAction = new Action<ValueUnit>(x => channelValue = x.Value);
            var numberValueAction = new Action<ValueUnit>(x => numberValue = x.Value);
            var raniValueAction = new Action(() => raniValue = true);
            var aniValueAction = new Action<ValueUnit>(x => aniValue = x.Value);
            var ani2ValueAction = new Action<ValueUnit>(x => ani2Value = x.Value);
            var arouteDelayValueAction = new Action<ValueUnit>(x => arouteDelayValue = x.Value);
            var arouteNumValueAction = new Action<ValueUnit>(x => arouteNumValue = x.Value);
            var prioValueAction = new Action<ValueUnit>(x => prioValue = x.Value);
            var infoValueAction = new Action<ValueUnit>(x => infoValue = x.Value);
            var attribValueAction = new Action<ValueUnit>(x => attribValue = x.Value);
            //-----------------------------------------------------------------
            sequence
                .If(
                    new StringUnit("loc"),
                    new SymbolUnit('('),
                    new ValueUnit().Action(shelfValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(slotValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(channelValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(numberValueAction),
                    new SymbolUnit(')')
                );
            sequence
                .Else(
                    new StringUnit("loc"),
                    new SymbolUnit('('),
                    new ValueUnit().Action(slotValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(channelValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(numberValueAction),
                    new SymbolUnit(')')
                );
            sequence
                .Any(
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("rani").Action(raniValueAction)),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("ani-"), new ValueUnit().Action(aniValueAction)),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("ani2-"), new ValueUnit().Action(ani2ValueAction)),
                    sequence.CreateListContainer(
                            new SymbolUnit(' '),
                            new StringUnit("aroute"),
                            new SymbolUnit('('),
                            new ValueUnit().Action(arouteDelayValueAction),
                            new SymbolUnit(','),
                            new ValueUnit().Action(arouteNumValueAction),
                            new SymbolUnit(')')
                        ),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("prio-"), new ValueUnit().Action(prioValueAction)),
                    sequence.CreateListContainer(
                            new SymbolUnit(' '),
                            new StringUnit("info"),
                            new SymbolUnit('('),
                            new ValueUnit().Action(infoValueAction),
                            new SymbolUnit(')')
                        ),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("attr-"), new ValueUnit().Action(attribValueAction))
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("loc(6,1,2,3)");

            Assert.IsTrue(result);
            Assert.IsTrue(string.Equals(shelfValue, "6", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(string.Equals(slotValue, "1", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(string.Equals(channelValue, "2", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(string.Equals(numberValue, "3", StringComparison.OrdinalIgnoreCase));
            Assert.IsFalse(raniValue);
            Assert.IsTrue(string.IsNullOrEmpty(aniValue));
            Assert.IsTrue(string.IsNullOrEmpty(ani2Value));
            Assert.IsTrue(string.IsNullOrEmpty(arouteDelayValue));
            Assert.IsTrue(string.IsNullOrEmpty(arouteNumValue));
            Assert.IsTrue(string.IsNullOrEmpty(prioValue));
            Assert.IsTrue(string.IsNullOrEmpty(infoValue));
            Assert.IsTrue(string.IsNullOrEmpty(attribValue));
            Assert.IsTrue(error == null);
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void Test1Failed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            //-----------------------------------------------------------------
            var shelfValue = "";
            var slotValue = "";
            var channelValue = "";
            var numberValue = "";
            var raniValue = false;
            var aniValue = "";
            var ani2Value = "";
            var arouteDelayValue = "";
            var arouteNumValue = "";
            var prioValue = "";
            var infoValue = "";
            var attribValue = "";
            UnitError error = null;
            //-----------------------------------------------------------------
            var shelfValueAction = new Action<ValueUnit>(x => shelfValue = x.Value);
            var slotValueAction = new Action<ValueUnit>(x => slotValue = x.Value);
            var channelValueAction = new Action<ValueUnit>(x => channelValue = x.Value);
            var numberValueAction = new Action<ValueUnit>(x => numberValue = x.Value);
            var raniValueAction = new Action(() => raniValue = true);
            var aniValueAction = new Action<ValueUnit>(x => aniValue = x.Value);
            var ani2ValueAction = new Action<ValueUnit>(x => ani2Value = x.Value);
            var arouteDelayValueAction = new Action<ValueUnit>(x => arouteDelayValue = x.Value);
            var arouteNumValueAction = new Action<ValueUnit>(x => arouteNumValue = x.Value);
            var prioValueAction = new Action<ValueUnit>(x => prioValue = x.Value);
            var infoValueAction = new Action<ValueUnit>(x => infoValue = x.Value);
            var attribValueAction = new Action<ValueUnit>(x => attribValue = x.Value);
            //-----------------------------------------------------------------
            sequence
                .If(
                    new StringUnit("loc"),
                    new SymbolUnit('('),
                    new ValueUnit().Action(shelfValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(slotValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(channelValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(numberValueAction),
                    new SymbolUnit(')')
                );
            sequence
                .Else(
                    new StringUnit("loc"),
                    new SymbolUnit('('),
                    new ValueUnit().Action(slotValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(channelValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(numberValueAction),
                    new SymbolUnit(')')
                );
            sequence
                .Any(
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("rani").Action(raniValueAction)),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("ani-"), new ValueUnit().Action(aniValueAction)),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("ani2-"), new ValueUnit().Action(ani2ValueAction)),
                    sequence.CreateListContainer(
                            new SymbolUnit(' '),
                            new StringUnit("aroute"),
                            new SymbolUnit('('),
                            new ValueUnit().Action(arouteDelayValueAction),
                            new SymbolUnit(','),
                            new ValueUnit().Action(arouteNumValueAction),
                            new SymbolUnit(')')
                        ),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("prio-"), new ValueUnit().Action(prioValueAction)),
                    sequence.CreateListContainer(
                            new SymbolUnit(' '),
                            new StringUnit("info"),
                            new SymbolUnit('('),
                            new ValueUnit().Action(infoValueAction),
                            new SymbolUnit(')')
                        ),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("attr-"), new ValueUnit().Action(attribValueAction))
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("loc(6,1,2,3) rani rani");

            Assert.IsFalse(result);
            Assert.IsTrue(string.IsNullOrEmpty(shelfValue));
            Assert.IsTrue(string.IsNullOrEmpty(slotValue));
            Assert.IsTrue(string.IsNullOrEmpty(channelValue));
            Assert.IsTrue(string.IsNullOrEmpty(numberValue));
            Assert.IsFalse(raniValue);
            Assert.IsTrue(string.IsNullOrEmpty(aniValue));
            Assert.IsTrue(string.IsNullOrEmpty(ani2Value));
            Assert.IsTrue(string.IsNullOrEmpty(arouteDelayValue));
            Assert.IsTrue(string.IsNullOrEmpty(arouteNumValue));
            Assert.IsTrue(string.IsNullOrEmpty(prioValue));
            Assert.IsTrue(string.IsNullOrEmpty(infoValue));
            Assert.IsTrue(string.IsNullOrEmpty(attribValue));
            Assert.IsTrue(error.PositionInLine == 17);
            Assert.IsTrue(((BaseContainerUnits)error.Unit).FindFirstUnit() is SymbolUnit);
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void Test2Failed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            //-----------------------------------------------------------------
            var shelfValue = "";
            var slotValue = "";
            var channelValue = "";
            var numberValue = "";
            var raniValue = false;
            var aniValue = "";
            var ani2Value = "";
            var arouteDelayValue = "";
            var arouteNumValue = "";
            var prioValue = "";
            var infoValue = "";
            var attribValue = "";
            UnitError error = null;
            //-----------------------------------------------------------------
            var shelfValueAction = new Action<ValueUnit>(x => shelfValue = x.Value);
            var slotValueAction = new Action<ValueUnit>(x => slotValue = x.Value);
            var channelValueAction = new Action<ValueUnit>(x => channelValue = x.Value);
            var numberValueAction = new Action<ValueUnit>(x => numberValue = x.Value);
            var raniValueAction = new Action(() => raniValue = true);
            var aniValueAction = new Action<ValueUnit>(x => aniValue = x.Value);
            var ani2ValueAction = new Action<ValueUnit>(x => ani2Value = x.Value);
            var arouteDelayValueAction = new Action<ValueUnit>(x => arouteDelayValue = x.Value);
            var arouteNumValueAction = new Action<ValueUnit>(x => arouteNumValue = x.Value);
            var prioValueAction = new Action<ValueUnit>(x => prioValue = x.Value);
            var infoValueAction = new Action<ValueUnit>(x => infoValue = x.Value);
            var attribValueAction = new Action<ValueUnit>(x => attribValue = x.Value);
            //-----------------------------------------------------------------
            sequence
                .If(
                    new StringUnit("loc"),
                    new SymbolUnit('('),
                    new ValueUnit().Action(shelfValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(slotValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(channelValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(numberValueAction),
                    new SymbolUnit(')')
                );
            sequence
                .Else(
                    new StringUnit("loc"),
                    new SymbolUnit('('),
                    new ValueUnit().Action(slotValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(channelValueAction),
                    new SymbolUnit(','),
                    new ValueUnit().Action(numberValueAction),
                    new SymbolUnit(')')
                );
            sequence
                .Any(
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("rani").Action(raniValueAction)),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("ani-"), new ValueUnit().Action(aniValueAction)),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("ani2-"), new ValueUnit().Action(ani2ValueAction)),
                    sequence.CreateListContainer(
                            new SymbolUnit(' '),
                            new StringUnit("aroute"),
                            new SymbolUnit('('),
                            new ValueUnit().Action(arouteDelayValueAction),
                            new SymbolUnit(','),
                            new ValueUnit().Action(arouteNumValueAction),
                            new SymbolUnit(')')
                        ),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("prio-"), new ValueUnit().Action(prioValueAction)),
                    sequence.CreateListContainer(
                            new SymbolUnit(' '),
                            new StringUnit("info"),
                            new SymbolUnit('('),
                            new ValueUnit().Action(infoValueAction),
                            new SymbolUnit(')')
                        ),
                    sequence.CreateListContainer(new SymbolUnit(' '), new StringUnit("attr-"), new ValueUnit().Action(attribValueAction))
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("loc(6,1,2,3) attr-111 rani aki-23 prio-dd ani2-33");

            Assert.IsFalse(result);
            Assert.IsTrue(string.IsNullOrEmpty(shelfValue));
            Assert.IsTrue(string.IsNullOrEmpty(slotValue));
            Assert.IsTrue(string.IsNullOrEmpty(channelValue));
            Assert.IsTrue(string.IsNullOrEmpty(numberValue));
            Assert.IsFalse(raniValue);
            Assert.IsTrue(string.IsNullOrEmpty(aniValue));
            Assert.IsTrue(string.IsNullOrEmpty(ani2Value));
            Assert.IsTrue(string.IsNullOrEmpty(arouteDelayValue));
            Assert.IsTrue(string.IsNullOrEmpty(arouteNumValue));
            Assert.IsTrue(string.IsNullOrEmpty(prioValue));
            Assert.IsTrue(string.IsNullOrEmpty(infoValue));
            Assert.IsTrue(string.IsNullOrEmpty(attribValue));
            Assert.IsTrue(error.PositionInLine == 28);
            Assert.IsTrue(error.Unit == null);
        }
        //---------------------------------------------------------------------
    }
}