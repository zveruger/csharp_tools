using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tools.CSharp.AnalyzerLines.Tests
{
    [TestClass]
    public class SequenceCommentUnitTest
    {
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestCommentEmptyValue1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            CommentUnit commentValue = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new CommentUnit(';').Action(x => commentValue = x))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine(";");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(commentValue.Available);
            Assert.IsTrue(commentValue.StartPosition == 0);
            Assert.IsTrue(string.Equals(commentValue.Value, "", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestCommentEmptyValue2Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            CommentUnit commentValue = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new CommentUnit(';').Action(x => commentValue = x))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("; ");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(commentValue.StartPosition == 1);
            Assert.IsTrue(string.Equals(commentValue.Value, "", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestCommentEmptyValue3Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            CommentUnit commentValue = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new CommentUnit(';').Action(x => commentValue = x))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine(";  ");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(commentValue.StartPosition == 2);
            Assert.IsTrue(string.Equals(commentValue.Value, "", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestCommentEmptyValue4Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            CommentUnit commentValue = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new CommentUnit(';').Action(x => commentValue = x))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine(";          ");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(commentValue.StartPosition == 10);
            Assert.IsTrue(string.Equals(commentValue.Value, "", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestCommentEmptyValue5Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            CommentUnit commentValue = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new CommentUnit(';').Action(x => commentValue = x))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine(" ;");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(commentValue.StartPosition == 1);
            Assert.IsTrue(string.Equals(commentValue.Value, "", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestCommentEmptyValue6Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            CommentUnit commentValue = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new CommentUnit(';').Action(x => commentValue = x))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("  ;");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(commentValue.StartPosition == 2);
            Assert.IsTrue(string.Equals(commentValue.Value, "", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestCommentEmptyValue7Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            CommentUnit commentValue = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new CommentUnit(';').Action(x => commentValue = x))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("          ;");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(commentValue.StartPosition == 10);
            Assert.IsTrue(string.Equals(commentValue.Value, "", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestCommentEmptyValue8Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            CommentUnit commentValue = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new CommentUnit(';').Action(x => commentValue = x))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("          ;          ");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(commentValue.StartPosition == 20);
            Assert.IsTrue(string.Equals(commentValue.Value, "", StringComparison.OrdinalIgnoreCase));
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestCommentValue1Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            CommentUnit commentValue = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new CommentUnit(';').Action(x => commentValue = x))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine(";comment");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(commentValue.StartPosition == 1);
            Assert.IsTrue(string.Equals(commentValue.Value, "comment", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestCommentValue2Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            CommentUnit commentValue = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new CommentUnit(';').Action(x => commentValue = x))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine("; comment");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(commentValue.StartPosition == 2);
            Assert.IsTrue(string.Equals(commentValue.Value, "comment", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestCommentValue3Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            CommentUnit commentValue = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new CommentUnit(';').Action(x => commentValue = x))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine(";         comment");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(commentValue.StartPosition == 10);
            Assert.IsTrue(string.Equals(commentValue.Value, "comment", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestCommentValue4Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            CommentUnit commentValue = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new CommentUnit(';').Action(x => commentValue = x))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine(";         comment ");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(commentValue.StartPosition == 10);
            Assert.IsTrue(string.Equals(commentValue.Value, "comment", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestCommentValue5Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            CommentUnit commentValue = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new CommentUnit(';').Action(x => commentValue = x))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine(";         comment         ");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(commentValue.StartPosition == 10);
            Assert.IsTrue(string.Equals(commentValue.Value, "comment", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestCommentValue6Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            CommentUnit commentValue = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new CommentUnit(';').Action(x => commentValue = x))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine(";         comment comment    ");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(commentValue.StartPosition == 10);
            Assert.IsTrue(string.Equals(commentValue.Value, "comment comment", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestCommentValue7Success()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            CommentUnit commentValue = null;
            //-----------------------------------------------------------------
            sequence
                .Add(new CommentUnit(';').Action(x => commentValue = x))
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine(";         comment   comment  comment    ");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsTrue(commentValue.StartPosition == 10);
            Assert.IsTrue(string.Equals(commentValue.Value, "comment   comment  comment", StringComparison.OrdinalIgnoreCase));
        }
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestCommentFailed()
        {
            var sequence = new Sequence().AddIgnoreSymbol(' ');
            UnitError error = null;
            CommentUnit commentValue = new CommentUnit(';');
            //-----------------------------------------------------------------
            sequence
                .Add(new StringUnit("comment"))
                .Add(commentValue)
                .Error(x => error = x);
            //-----------------------------------------------------------------
            var result = sequence.DecodeLine(" comment");

            Assert.IsTrue(result);
            Assert.IsTrue(error == null);
            Assert.IsFalse(commentValue.Available);
        }
        //---------------------------------------------------------------------
    }
}