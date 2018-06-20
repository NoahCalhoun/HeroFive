using System.Collections.Generic;

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

        public CharacterTable()
        {
            CharacterTableData data;

                data = new CharacterTableData()
                {
                    ID = 1,
                    NAME = "KARIN",
                    HP = 10,
                    ATK = 10,
                    SPD = 10,
                    LCM = 10,
                    EDR = 10,
                    SPRITE = "KARIN",
                };
                mIDDic.Add(1, data);
                mNameDic.Add("KARIN", data);
        }

        public CharacterTableData GetDataByID(int id)
        {
            return mIDDic.ContainsKey(id) ? mIDDic[id] : null;
        }
        public CharacterTableData GetDataByNAME(string name)
        {
            return mNameDic.ContainsKey(name) ? mNameDic[name] : null;
        }
    }
}
