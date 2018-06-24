using System.Collections.Generic;
using UnityEngine;

public partial class H5Table
{
    private static SkillTable mSkill;
    public static SkillTable Skill
    {
        get
        {
            if (mSkill == null)
                mSkill = new SkillTable();
            return mSkill;
        }
    }

    public class SkillTable
    {
        public class SkillTableData
        {
            public int ID;
            public string NAME;
            public string VALUE;
            public int DELIMITER;
        }

        private Dictionary<int, SkillTableData> mIDDic = new Dictionary<int, SkillTableData>();
        private Dictionary<string, SkillTableData> mNameDic = new Dictionary<string, SkillTableData>();
        private List<int> mIDs;
        private List<string> mNames;

        public SkillTable()
        {
            SkillTableData data;

            TextAsset asset = Resources.Load<TextAsset>("Table/Skill");
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
                data = new SkillTableData();
                data.ID = int.Parse(tableStr[i][0]);
                data.NAME = tableStr[i][1];
                data.VALUE = tableStr[i][2];
                data.DELIMITER = int.Parse(tableStr[i][3]);
                mIDDic.Add(data.ID, data);
                mNameDic.Add(data.NAME, data);
            }
            mIDs = new List<int>(mIDDic.Keys);
            mNames = new List<string>(mNameDic.Keys);
        }

        public SkillTableData GetDataByID(int id)
        {
            return mIDDic.ContainsKey(id) ? mIDDic[id] : null;
        }
        public SkillTableData GetDataByNAME(string name)
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
