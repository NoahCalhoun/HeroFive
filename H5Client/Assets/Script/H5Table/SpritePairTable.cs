using System.Collections.Generic;
using UnityEngine;

public partial class H5Table
{
    private static SpritePairTable mSpritePair;
    public static SpritePairTable SpritePair
    {
        get
        {
            if (mSpritePair == null)
                mSpritePair = new SpritePairTable();
            return mSpritePair;
        }
    }

    public class SpritePairTable
    {
        public class SpritePairTableData
        {
            public int ID;
            public string NAME;
            public string SPRITE1;
            public string SPRITE2;
        }

        private Dictionary<int, SpritePairTableData> mIDDic = new Dictionary<int, SpritePairTableData>();
        private Dictionary<string, SpritePairTableData> mNameDic = new Dictionary<string, SpritePairTableData>();

        public SpritePairTable()
        {
            SpritePairTableData data;

            TextAsset asset = Resources.Load<TextAsset>("Table/SpritePair");
            var strs = asset.text;
            strs = strs.Replace("\r", "");
            var lines = strs.Split('\n');
            List<string[]> tableStr = new List<string[]>();
            for (int i = 0; i < lines.Length; ++i)
            {
                if (lines[i].Length <= 0)
                    continue;
                tableStr.Add(lines[i].Split(','));
            }
            for (int i = 2; i < tableStr.Count; ++i)
            {
                data = new SpritePairTableData();
                data.ID = int.Parse(tableStr[i][0]);
                data.NAME = tableStr[i][1];
                data.SPRITE1 = tableStr[i][2];
                data.SPRITE2 = tableStr[i][3];
                mIDDic.Add(data.ID, data);
                mNameDic.Add(data.NAME, data);
            }
        }

        public SpritePairTableData GetDataByID(int id)
        {
            return mIDDic.ContainsKey(id) ? mIDDic[id] : null;
        }
        public SpritePairTableData GetDataByNAME(string name)
        {
            return mNameDic.ContainsKey(name) ? mNameDic[name] : null;
        }
    }
}
