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
        /// <summary>
        /// 初期位置コードの分解処理テスト
        /// </summary>
        [TestMethod()]
        public void GetPositionFromCodeTest()
        {
            int black = 9, red = 0, yellow = 2, blue = 7;
            int code = 12008;

            var position = BlockFieldModel.GetPositionFromCode(code);

            Assert.AreEqual(black, position.Black);
            Assert.AreEqual(red, position.Red);
            Assert.AreEqual(yellow, position.Yellow);
            Assert.AreEqual(blue, position.Blue);

            code = 146;
            position = BlockFieldModel.GetPositionFromCode(code);
            black = 0; red = 1; yellow = 2; blue = 3;

            Assert.AreEqual(black, position.Black);
            Assert.AreEqual(red, position.Red);
            Assert.AreEqual(yellow, position.Yellow);
            Assert.AreEqual(blue, position.Blue);

        }

        /// <summary>
        /// 取得したコードを全体の位置コードに変換するメソッドのテスト
        /// </summary>
        [TestMethod()]
        public void AdjustBlockPositionFieldTest()
        {
            var codepos = new Position();
            var redtable = new int[] { 1, 2, 3, 4, 5, 8, 9, 10, 11, 12, 14 };

            for (var i = 0; i < 11; i++)
            {
                codepos.Red = i;
                var decodepos = BlockFieldModel.AdjustBlockPositionField(codepos);
                Assert.AreEqual(redtable[i], decodepos.Red);
            }

            var yellowTable = new int[] { 0, 1, 3, 5, 6, 7, 8, 9, 10, 11, 13 };
            for (var i = 0; i < 10; i++)
            {
                codepos.Yellow = i;
                var decodepos = BlockFieldModel.AdjustBlockPositionField(codepos);
                Assert.AreEqual(yellowTable[i], decodepos.Yellow);

            }

            var blueTable = new int[] { 0, 2, 4, 5, 6, 7, 9, 10, 12, 13, 14 };
            for (var i = 0; i < 10; i++)
            {
                codepos.Blue = i;
                var decodepos = BlockFieldModel.AdjustBlockPositionField(codepos);
                Assert.AreEqual(blueTable[i], decodepos.Blue);
            }
        }

        [TestMethod()]
        public void SetBlockPositionTest()
        {
            var BlockField = new BlockFieldModel();

            // 初期配置の整合性を確認
            BlockField.SetBlockPosition(12008, 1);
            
            Assert.AreEqual(BlockColor.Green, BlockField.PlaceArray[0].OnBlockColor);
            Assert.AreEqual(BlockColor.Red, BlockField.PlaceArray[1].OnBlockColor);
            Assert.AreEqual(BlockColor.Yellow, BlockField.PlaceArray[3].OnBlockColor);
            Assert.AreEqual(BlockColor.Black, BlockField.PlaceArray[9].OnBlockColor);
            Assert.AreEqual(BlockColor.Blue, BlockField.PlaceArray[10].OnBlockColor);

            BlockField.ChangeNextPosition(0);
            
        }
    }
}