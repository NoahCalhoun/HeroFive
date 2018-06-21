using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class H5Table
{
    private static TestTable mTest;
    public static TestTable Test
    {
        get
        {
            if (mTest == null)
                mTest = new TestTable();

            return mTest;
        }
    }

    public class TestTable
    {
        public class TestTableData
        {
            public int ID;
            public string NAME;
            public int VALUE;
        }

        private Dictionary<int, TestTableData> mIDDic = new Dictionary<int, TestTableData>();
        private Dictionary<string, TestTableData> mNameDic = new Dictionary<string, TestTableData>();

        public TestTable()
        {
            TestTableData data;

            for (int i = 0; i < 10; ++i)
            {
                data = new TestTableData()
                {
                    ID = 1,
                    NAME = "aa",
                    VALUE = 10,
                };
                mIDDic.Add(1, data);
                mNameDic.Add("aa", data);
            }

            TextAsset asset = Resources.Load<TextAsset>("Table/Test");
            var strs = asset.text;
            var lines = strs.Split('\n');
            List<string[]> tableStr = new List<string[]>();
            for (int i = 0; i < lines.Length; ++i)
            {
                tableStr.Add(lines[i].Split(','));
            }
        }

        public TestTableData GetDataByID(int id)
        {
            return mIDDic.ContainsKey(id) ? mIDDic[id] : null;
        }

        public TestTableData GetDataByName(string name)
        {
            return mNameDic.ContainsKey(name) ? mNameDic[name] : null;
        }
    }

}
