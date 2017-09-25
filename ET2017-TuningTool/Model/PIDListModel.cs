using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ET2017_TuningTool;

namespace ET2017_TuningTool.Model
{

    public class PIDListModel
    {
        public  string FileName;

        
        public PIDModel[] PIDPrametorArray { get;  set; }


        public static PIDListModel LoadFromFile(string fileName)
        {

            // 指定されたファイルが存在しない場合には、規定値で新規作成
            if (!File.Exists(fileName))
            {
                var blankPidListModel = new PIDListModel();

                blankPidListModel.PIDPrametorArray = Enum.GetValues(typeof(PIDStateNo)).Cast<PIDStateNo>()
                                                    .Select(p => new PIDModel { StateNo = (int)p, StateName = Enum.GetName(typeof(PIDStateNo), p) })
                                                    .ToArray();
                return blankPidListModel;
            }

            using (var sr = new StreamReader(fileName, new UTF8Encoding(false)))
            {
                // 指定されたファイルが存在する場合には、デシリアライズして返す
                var serializer = new XmlSerializer(typeof(PIDListModel));
                var pidListModel = serializer.Deserialize(sr) as PIDListModel;

                return pidListModel;
            }
        }

        
        public bool SaveAsFile(string fileName)
        {
            //書き込むファイルを開く（UTF-8 BOM無し）
            using (var sw = new StreamWriter(fileName, false, new UTF8Encoding(false)))
            {
                //XmlSerializerオブジェクトを作成
                //オブジェクトの型を指定する
                var serializer = new XmlSerializer(typeof(PIDListModel));

                //シリアル化し、XMLファイルに保存する
                serializer.Serialize(sw, this);

                return true;
            }
        }
    }
}
