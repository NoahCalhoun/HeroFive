using System.Collections.Generic;

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
            public string ACTION_0;
            public string SPRITE_SET_0;
            public string ACTION_1;
            public string SPRITE_SET_1;
            public string ACTION_2;
            public string SPRITE_SET_2;
            public string ACTION_3;
            public string SPRITE_SET_3;
            public string ACTION_4;
            public string SPRITE_SET_4;
            public string ACTION_5;
            public string SPRITE_SET_5;
            public string ACTION_6;
            public string SPRITE_SET_6;
            public string ACTION_7;
            public string SPRITE_SET_7;
            public string ACTION_8;
            public string SPRITE_SET_8;
            public string ACTION_9;
            public string SPRITE_SET_9;
        }

        private Dictionary<int, SpriteTableData> mIDDic = new Dictionary<int, SpriteTableData>();
        private Dictionary<string, SpriteTableData> mNameDic = new Dictionary<string, SpriteTableData>();

        public SpriteTable()
        {
            SpriteTableData data;

                data = new SpriteTableData()
                {
                    ID = 1,
                    NAME = "KARIN",
                    ACTION_0 = "Idle",
                    SPRITE_SET_0 = "KARIN_IDLE",
                    ACTION_1 = "0",
                    SPRITE_SET_1 = "0",
                    ACTION_2 = "0",
                    SPRITE_SET_2 = "0",
                    ACTION_3 = "0",
                    SPRITE_SET_3 = "0",
                    ACTION_4 = "0",
                    SPRITE_SET_4 = "0",
                    ACTION_5 = "0",
                    SPRITE_SET_5 = "0",
                    ACTION_6 = "0",
                    SPRITE_SET_6 = "0",
                    ACTION_7 = "0",
                    SPRITE_SET_7 = "0",
                    ACTION_8 = "0",
                    SPRITE_SET_8 = "0",
                    ACTION_9 = "0",
                    SPRITE_SET_9 = "0",
                };
                mIDDic.Add(1, data);
                mNameDic.Add("KARIN", data);
        }

        public SpriteTableData GetDataByID(int id)
        {
            return mIDDic.ContainsKey(id) ? mIDDic[id] : null;
        }
        public SpriteTableData GetDataByNAME(string name)
        {
            return mNameDic.ContainsKey(name) ? mNameDic[name] : null;
        }
    }
}
