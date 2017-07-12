using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotController.ColorUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.ColorUtil.Tests
{
    [TestClass()]
    public class HSLColorTests
    {
        [TestMethod()]
        public void FromRGBTest()
        {
            var trueWhite = HSLColor.FromRGB(255, 255, 255);

            Assert.AreEqual(1, trueWhite.Luminosity);
            
        }
        
    }


}