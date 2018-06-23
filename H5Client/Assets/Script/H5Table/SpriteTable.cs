using System.Collections.Generic;
using UnityEngine;

public partial class H5Table
{
    private static SpriteTable mSprite;
    public static SpriteTable Sprite
    {
        get
        {
            if (mSprite == null)
                mSprite = new SpriteTable();
            return mSprite;
        }
    }

    public class SpriteTable
    {
        public class SpriteTableData
        {
            public int ID;
            public string NAME;
            public string[] ACTION_ = new string[10];
            public string[] SPRITE_SET_ = new string[10];
        }

        private Dictionary<int, SpriteTableData> mIDDic = new Dictionary<int, SpriteTableData>();
        private Dictionary<string, SpriteTableData> mNameDic = new Dictionary<string, SpriteTableData>();
        private List<int> mIDs;
        private List<string> mNames;

        public SpriteTable()
        {
            SpriteTableData data;

            TextAsset asset = Resources.Load<TextAsset>("Table/Sprite");
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
                data = new SpriteTableData();
                data.ID = int.Parse(tableStr[i][0]);
                data.NAME = tableStr[i][1];
                data.ACTION_[0] = tableStr[i][2];
                data.SPRITE_SET_[0] = tableStr[i][3];
                data.ACTION_[1] = tableStr[i][4];
                data.SPRITE_SET_[1] = tableStr[i][5];
                data.ACTION_[2] = tableStr[i][6];
                data.SPRITE_SET_[2] = tableStr[i][7];
                data.ACTION_[3] = tableStr[i][8];
                data.SPRITE_SET_[3] = tableStr[i][9];
                data.ACTION_[4] = tableStr[i][10];
                data.SPRITE_SET_[4] = tableStr[i][11];
                data.ACTION_[5] = tableStr[i][12];
                data.SPRITE_SET_[5] = tableStr[i][13];
                data.ACTION_[6] = tableStr[i][14];
                data.SPRITE_SET_[6] = tableStr[i][15];
                data.ACTION_[7] = tableStr[i][16];
                data.SPRITE_SET_[7] = tableStr[i][17];
                data.ACTION_[8] = tableStr[i][18];
                data.SPRITE_SET_[8] = tableStr[i][19];
                data.ACTION_[9] = tableStr[i][20];
                data.SPRITE_SET_[9] = tableStr[i][21];
                mIDDic.Add(data.ID, data);
                mNameDic.Add(data.NAME, data);
            }
            mIDs = new List<int>(mIDDic.Keys);
            mNames = new List<string>(mNameDic.Keys);
        }

        public SpriteTableData GetDataByID(int id)
        {
            return mIDDic.ContainsKey(id) ? mIDDic[id] : null;
        }
        public SpriteTableData GetDataByNAME(string name)
        {
            return mNameDic.ContainsKey(name) ? mNameDic[name] : null;
        }
        public List<int> GetIDList()
        {
            return mIDs;
        }
        public List<string> GetNAMEList()
        {
            return mNames;
        }
    }
}
