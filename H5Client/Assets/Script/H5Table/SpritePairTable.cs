using System.Collections.Generic;

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

                data = new SpritePairTableData()
                {
                    ID = 1,
                    NAME = "IDLE01",
                    SPRITE1 = "Idle00",
                    SPRITE2 = "Idle01",
                };
                mIDDic.Add(1, data);
                mNameDic.Add("IDLE01", data);
                data = new SpritePairTableData()
                {
                    ID = 2,
                    NAME = "IDLE02",
                    SPRITE1 = "Idle02",
                    SPRITE2 = "Idle03",
                };
                mIDDic.Add(1, data);
                mNameDic.Add("IDLE01", data);
                data = new SpritePairTableData()
                {
                    ID = 3,
                    NAME = "IDLE03",
                    SPRITE1 = "Idle04",
                    SPRITE2 = "Idle05",
                };
                mIDDic.Add(1, data);
                mNameDic.Add("IDLE01", data);
                data = new SpritePairTableData()
                {
                    ID = 4,
                    NAME = "IDLE04",
                    SPRITE1 = "Idle06",
                    SPRITE2 = "Idle07",
                };
                mIDDic.Add(1, data);
                mNameDic.Add("IDLE01", data);
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
