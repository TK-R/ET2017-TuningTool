using Microsoft.VisualStudio.TestTools.UnitTesting;
using ET2017_TuningTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ET2017_TuningTool.Model.Tests
{
    [TestClass()]
    public class PlaceTests
    {
        [TestMethod()]
        public void GetDistanceTest()
        {
            var p = new Place { No = 0 };
            var dst = new Point { X = 4, Y = 13 };

            Assert.AreEqual(5, p.GetDistance(dst));
        }
    }
}