using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tools.CSharp.AnalyzerLines.Tests
{
    [TestClass]
    public class SequenceActionUnitTests
    {
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestActionsSuccess()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var action1Raised = false;
            var action2Raised = false;
            var action3Raised = false;
            //-----------------------------------------------------------------
            var action1 = new Action(() =>
            {
                if (!action2Raised && !action3Raised)
                { action1Raised = true; }
            });
            var action2 = new Action(() =>
            {
                if (action1Raised)
                { action2Raised = true; }
            });
            var action3 = new Action(() =>
            {
                if (action2Raised)
                { action3Raised = true; }
            });
            //-----------------------------------------------------------------
            sequence
                .Add(new StringUnit("lexem")
                    .Action(action1)
                    .Action(action2)
                    .Action(action3)
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem");

            Assert.IsTrue(result);
            Assert.IsTrue(action1Raised);
            Assert.IsTrue(action2Raised);
            Assert.IsTrue(action3Raised);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestActionsFailed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var action1Raised = false;
            var action2Raised = false;
            var action3Raised = false;
            //-----------------------------------------------------------------
            var action1 = new Action(() =>
            {
                if (!action2Raised && !action3Raised)
                { action1Raised = true; }
            });
            var action2 = new Action(() =>
            {
                action2Raised = true;
            });
            var action3 = new Action(() =>
            {
                action3Raised = true;
            });
            //-----------------------------------------------------------------
            sequence
                .Add(new StringUnit("lexem")
                    .Action(action1)
                    .Action(action2)
                    .Action(action3)
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem");

            Assert.IsTrue(result);
            Assert.IsTrue(action1Raised);
            Assert.IsTrue(action2Raised);
            Assert.IsTrue(action3Raised);
            Assert.IsTrue(error == null);
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestActionParamSuccess()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var isLexemAvailable = false;
            //-----------------------------------------------------------------
            sequence
                .Add(new StringUnit("lexem").Action(() => isLexemAvailable = true))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem");

            Assert.IsTrue(result);
            Assert.IsTrue(isLexemAvailable);
            Assert.IsTrue(error == null);
        }

       [TestMethod]
        public void TestActionParamFailed()
        {
            var sequence = new Sequence();
            UnitError error = null;
            var isLexemAvailable = false;
            //-----------------------------------------------------------------
            sequence
                .Add(new StringUnit("lexem").Action(() => isLexemAvailable = true))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("ledem");

            Assert.IsFalse(result);
            Assert.IsFalse(isLexemAvailable);
            Assert.IsTrue(error.PositionInLine == 2);
            Assert.IsTrue(string.Equals(((StringUnit)error.Unit).OriginalValue, "lexem", StringComparison.OrdinalIgnoreCase));
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestActionsAndActionsParamsSuccess()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var action1Raised = false;
            var action2Raised = false;
            var isAction2Available = false;
            var action3Raised = false;
            //-----------------------------------------------------------------
            var action1 = new Action(() =>
            {
                if (!action2Raised && !action3Raised)
                { action1Raised = true; }
            });
            var action2 = new Action(() =>
            {
                if (action1Raised)
                {
                    action2Raised = true;
                    isAction2Available = true;
                }
            });
            var action3 = new Action(() =>
            {
                if (action2Raised)
                { action3Raised = true; }
            });
            //-----------------------------------------------------------------
            sequence
                .Add(new StringUnit("lexem")
                    .Action(action1)
                    .Action(action2)
                    .Action(action3)
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem");

            Assert.IsTrue(result);
            Assert.IsTrue(action1Raised);
            Assert.IsTrue(action2Raised);
            Assert.IsTrue(isAction2Available);
            Assert.IsTrue(action3Raised);
            Assert.IsTrue(error == null);
        }

        [TestMethod]
        public void TestActionsAndActionsParamsFailed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            var action1Raised = false;
            var action2Raised = false;
            var action3Raised = false;
            //-----------------------------------------------------------------
            var action1 = new Action(() =>
            {
                if (!action2Raised && !action3Raised)
                {
                    action1Raised = true;
                }
            });
            var action2 = new Action(() =>
            {
                action2Raised = true;
            });
            var action3 = new Action(() =>
            {
                action3Raised = true;
            });
            //-----------------------------------------------------------------
            sequence
                .Add(new StringUnit("lexem")
                    .Action(action1)
                    .Action(action2)
                    .Action(action3)
                )
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("lexem");

            Assert.IsTrue(result);
            Assert.IsTrue(action1Raised);
            Assert.IsTrue(action2Raised);
            Assert.IsTrue(action3Raised);
            Assert.IsTrue(error == null);
        }
        //---------------------------------------------------------------------
    }
}