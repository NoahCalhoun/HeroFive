using System.Collections.Generic;
using UnityEngine;

public partial class H5Table
{
    private static LoadingSceneTable mLoadingScene;
    public static LoadingSceneTable LoadingScene
    {
        get
        {
            if (mLoadingScene == null)
                mLoadingScene = new LoadingSceneTable();
            return mLoadingScene;
        }
    }

    public class LoadingSceneTable
    {
        public class LoadingSceneTableData
        {
            public int ID;
            public string NAME;
            public string EP;
            public string TEXTURE;
        }

        private Dictionary<int, LoadingSceneTableData> mIDDic = new Dictionary<int, LoadingSceneTableData>();
        private Dictionary<string, LoadingSceneTableData> mNameDic = new Dictionary<string, LoadingSceneTableData>();
        private List<int> mIDs;
        private List<string> mNames;

        public LoadingSceneTable()
        {
            LoadingSceneTableData data;

            TextAsset asset = Resources.Load<TextAsset>("Table/LoadingScene");
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
                data = new LoadingSceneTableData();
                data.ID = int.Parse(tableStr[i][0]);
                data.NAME = tableStr[i][1];
                data.EP = tableStr[i][2];
                data.TEXTURE = tableStr[i][3];
                mIDDic.Add(data.ID, data);
                mNameDic.Add(data.NAME, data);
            }
            mIDs = new List<int>(mIDDic.Keys);
            mNames = new List<string>(mNameDic.Keys);
        }

        public LoadingSceneTableData GetDataByID(int id)
        {
            return mIDDic.ContainsKey(id) ? mIDDic[id] : null;
        }
        public LoadingSceneTableData GetDataByNAME(string name)
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
