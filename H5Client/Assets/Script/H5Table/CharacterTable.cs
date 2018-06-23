using System.Collections.Generic;
using UnityEngine;

public partial class H5Table
{
    private static CharacterTable mCharacter;
    public static CharacterTable Character
    {
        get
        {
            if (mCharacter == null)
                mCharacter = new CharacterTable();
            return mCharacter;
        }
    }

    public class CharacterTable
    {
        public class CharacterTableData
        {
            public int ID;
            public string NAME;
            public int HP;
            public int ATK;
            public int SPD;
            public int LCM;
            public int EDR;
            public string SPRITE;
        }

        private Dictionary<int, CharacterTableData> mIDDic = new Dictionary<int, CharacterTableData>();
        private Dictionary<string, CharacterTableData> mNameDic = new Dictionary<string, CharacterTableData>();
        private List<int> mIDs;
        private List<string> mNames;

        public CharacterTable()
        {
            CharacterTableData data;

            TextAsset asset = Resources.Load<TextAsset>("Table/Character");
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
                data = new CharacterTableData();
                data.ID = int.Parse(tableStr[i][0]);
                data.NAME = tableStr[i][1];
                data.HP = int.Parse(tableStr[i][2]);
                data.ATK = int.Parse(tableStr[i][3]);
                data.SPD = int.Parse(tableStr[i][4]);
                data.LCM = int.Parse(tableStr[i][5]);
                data.EDR = int.Parse(tableStr[i][6]);
                data.SPRITE = tableStr[i][7];
                mIDDic.Add(data.ID, data);
                mNameDic.Add(data.NAME, data);
            }
            mIDs = new List<int>(mIDDic.Keys);
            mNames = new List<string>(mNameDic.Keys);
        }

        public CharacterTableData GetDataByID(int id)
        {
            return mIDDic.ContainsKey(id) ? mIDDic[id] : null;
        }
        public CharacterTableData GetDataByNAME(string name)
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
