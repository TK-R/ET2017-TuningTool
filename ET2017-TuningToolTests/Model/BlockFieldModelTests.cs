using Microsoft.VisualStudio.TestTools.UnitTesting;
using ET2017_TuningTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET2017_TuningTool.Model.Tests
{
    [TestClass()]
    public class BlockFieldModelTests
    {
        [TestMethod()]
        public void GetPositionFromCodeTest()
        {
            int black = 10, red = 1, yellow = 3, blue = 8;
            int code = 12008;

            var position = BlockFieldModel.GetPositionFromCode(code);

            Assert.AreEqual(black, position.Black);
            Assert.AreEqual(red, position.Red);
            Assert.AreEqual(yellow, position.Yellow);
            Assert.AreEqual(blue, position.Blue);

            code = 146;
            position = BlockFieldModel.GetPositionFromCode(code);
            black = 1; red = 2; yellow = 3; blue = 4;

            Assert.AreEqual(black, position.Black);
            Assert.AreEqual(red, position.Red);
            Assert.AreEqual(yellow, position.Yellow);
            Assert.AreEqual(blue, position.Blue);

        }
    }
}