using System.Collections.Generic;

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
            public int TIME_0;
            public string SPRITE_0;
            public int TIME_1;
            public string SPRITE_1;
            public int TIME_2;
            public string SPRITE_2;
            public int TIME_3;
            public string SPRITE_3;
            public int TIME_4;
            public string SPRITE_4;
            public int TIME_5;
            public string SPRITE_5;
            public int TIME_6;
            public string SPRITE_6;
            public int TIME_7;
            public string SPRITE_7;
            public int TIME_8;
            public string SPRITE_8;
            public int TIME_9;
            public string SPRITE_9;
        }

        private Dictionary<int, SpriteSetTableData> mIDDic = new Dictionary<int, SpriteSetTableData>();
        private Dictionary<string, SpriteSetTableData> mNameDic = new Dictionary<string, SpriteSetTableData>();

        public SpriteSetTable()
        {
            SpriteSetTableData data;

                data = new SpriteSetTableData()
                {
                    ID = 1,
                    NAME = "KARIN_IDLE",
                    LOOP = true,
                    EP = "FT2",
                    TEXTURE = "char_00a",
                    TIME_0 = 200,
                    SPRITE_0 = "IDLE01",
                    TIME_1 = 200,
                    SPRITE_1 = "IDLE02",
                    TIME_2 = 200,
                    SPRITE_2 = "IDLE03",
                    TIME_3 = 200,
                    SPRITE_3 = "IDLE02",
                    TIME_4 = 0,
                    SPRITE_4 = "0",
                    TIME_5 = 0,
                    SPRITE_5 = "0",
                    TIME_6 = 0,
                    SPRITE_6 = "0",
                    TIME_7 = 0,
                    SPRITE_7 = "0",
                    TIME_8 = 0,
                    SPRITE_8 = "0",
                    TIME_9 = 0,
                    SPRITE_9 = "0",
                };
                mIDDic.Add(1, data);
                mNameDic.Add("KARIN_IDLE", data);
        }

        public SpriteSetTableData GetDataByID(int id)
        {
            return mIDDic.ContainsKey(id) ? mIDDic[id] : null;
        }
        public SpriteSetTableData GetDataByNAME(string name)
        {
            return mNameDic.ContainsKey(name) ? mNameDic[name] : null;
        }
    }
}
