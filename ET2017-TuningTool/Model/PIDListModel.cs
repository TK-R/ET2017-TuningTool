using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET2017_TuningTool.Model
{
    public class PIDListModel
    {
        public enum PIDStateNo
        {
            LineTraceStraight = 1,
            pR_B,
            pR_C,
            pR_D,
            pR_E,
            pL_B,
            pL_C,
            pL_D,
            pL_E,
            pL_F,
            pL_G,
            BlockMovePIDState = 30,
            BlockMoveHighPIDState,
            ETSumoPIDState,
            ETSumoHighPIDState,
            ETTrainSlow,
            ETTrainHigh,
            ForwardPID = 99,
        }
        
        private PIDModel[] PIDPrametorArray { get; set; }

        public PIDListModel()
        {
            PIDPrametorArray = Enum.GetValues(typeof(PIDStateNo)).Cast<PIDStateNo>()
                                   .Select(p => new PIDModel { StateNo = (int)p }).ToArray();
        }


        public bool SaveAsFile(string fileName)
        {
            //保存先のファイル名
            fileName = Environment.CurrentDirectory + "\\hoge.xml";

            //XmlSerializerオブジェクトを作成
            //オブジェクトの型を指定する
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(PIDListModel));
            //書き込むファイルを開く（UTF-8 BOM無し）
            System.IO.StreamWriter sw = new System.IO.StreamWriter(
                fileName, false, new System.Text.UTF8Encoding(false));
            //シリアル化し、XMLファイルに保存する
            serializer.Serialize(sw, this);
            //ファイルを閉じる
            sw.Close();


            return true;
        }


    }
}
