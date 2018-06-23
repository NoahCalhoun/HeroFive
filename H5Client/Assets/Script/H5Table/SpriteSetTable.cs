using System.Collections.Generic;
using UnityEngine;

public partial class H5Table
{
    private static SpriteSetTable mSpriteSet;
    public static SpriteSetTable SpriteSet
    {
        get
        {
            if (mSpriteSet == null)
                mSpriteSet = new SpriteSetTable();
            return mSpriteSet;
        }
    }

    public class SpriteSetTable
    {
        public class SpriteSetTableData
        {
            public int ID;
            public string NAME;
            public bool LOOP;
            public string EP;
            public string TEXTURE;
            public int[] TIME_ = new int[10];
            public string[] SPRITE_L_ = new string[10];
            public string[] SPRITE_R_ = new string[10];
        }

        private Dictionary<int, SpriteSetTableData> mIDDic = new Dictionary<int, SpriteSetTableData>();
        private Dictionary<string, SpriteSetTableData> mNameDic = new Dictionary<string, SpriteSetTableData>();
        private List<int> mIDs;
        private List<string> mNames;

        public SpriteSetTable()
        {
            SpriteSetTableData data;

            TextAsset asset = Resources.Load<TextAsset>("Table/SpriteSet");
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
                data = new SpriteSetTableData();
                data.ID = int.Parse(tableStr[i][0]);
                data.NAME = tableStr[i][1];
                data.LOOP = (tableStr[i][2] == "TRUE" ? true : false);
                data.EP = tableStr[i][3];
                data.TEXTURE = tableStr[i][4];
                data.TIME_[0] = int.Parse(tableStr[i][5]);
                data.SPRITE_L_[0] = tableStr[i][6];
                data.SPRITE_R_[0] = tableStr[i][7];
                data.TIME_[1] = int.Parse(tableStr[i][8]);
                data.SPRITE_L_[1] = tableStr[i][9];
                data.SPRITE_R_[1] = tableStr[i][10];
                data.TIME_[2] = int.Parse(tableStr[i][11]);
                data.SPRITE_L_[2] = tableStr[i][12];
                data.SPRITE_R_[2] = tableStr[i][13];
                data.TIME_[3] = int.Parse(tableStr[i][14]);
                data.SPRITE_L_[3] = tableStr[i][15];
                data.SPRITE_R_[3] = tableStr[i][16];
                data.TIME_[4] = int.Parse(tableStr[i][17]);
                data.SPRITE_L_[4] = tableStr[i][18];
                data.SPRITE_R_[4] = tableStr[i][19];
                data.TIME_[5] = int.Parse(tableStr[i][20]);
                data.SPRITE_L_[5] = tableStr[i][21];
                data.SPRITE_R_[5] = tableStr[i][22];
                data.TIME_[6] = int.Parse(tableStr[i][23]);
                data.SPRITE_L_[6] = tableStr[i][24];
                data.SPRITE_R_[6] = tableStr[i][25];
                data.TIME_[7] = int.Parse(tableStr[i][26]);
                data.SPRITE_L_[7] = tableStr[i][27];
                data.SPRITE_R_[7] = tableStr[i][28];
                data.TIME_[8] = int.Parse(tableStr[i][29]);
                data.SPRITE_L_[8] = tableStr[i][30];
                data.SPRITE_R_[8] = tableStr[i][31];
                data.TIME_[9] = int.Parse(tableStr[i][32]);
                data.SPRITE_L_[9] = tableStr[i][33];
                data.SPRITE_R_[9] = tableStr[i][34];
                mIDDic.Add(data.ID, data);
                mNameDic.Add(data.NAME, data);
            }
            mIDs = new List<int>(mIDDic.Keys);
            mNames = new List<string>(mNameDic.Keys);
        }

        public SpriteSetTableData GetDataByID(int id)
        {
            return mIDDic.ContainsKey(id) ? mIDDic[id] : null;
        }
        public SpriteSetTableData GetDataByNAME(string name)
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
