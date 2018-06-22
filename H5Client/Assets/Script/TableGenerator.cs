using System.Collections.Generic;
using System.Text;
using System.IO;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TableGenerator))]
public class TableGeneratorEditor : Editor
{
    TILE_TYPE CurrentTileType = TILE_TYPE.TILE_TYPE_NONE;

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        if (GUILayout.Button("Generate Tables"))
            TableGenerator.GenerateTable();
    }
}

[ExecuteInEditMode]
public class TableGenerator : MonoBehaviour
{
    public enum IndexerType
    {
        None,
        Ten,
        Hundred
    }

    public static void GenerateTable()
    {
        var deleteNames = Directory.GetFiles("Assets/Script/H5Table", "*.cs");
        for (int i = 0; i < deleteNames.Length; ++i)
        {
            File.Delete(deleteNames[i]);
        }

        var tableNames = Directory.GetFiles("Assets/Resources/Table", "*.csv");

        for (int i = 0; i < tableNames.Length; ++i)
        {
            var table = tableNames[i];
            table = table.Replace("Assets/Resources/Table\\", "");
            table = table.Replace(".csv", "");
            CreateTable(tableNames[i], "Assets/Script/H5Table/" + table + "Table.cs", table);
        }
    }

    private static Dictionary<string, KeyValuePair<IndexerType, string>> FieldSet = new Dictionary<string, KeyValuePair<IndexerType, string>>();    //자료명, 인덱서타입, 자료형
    private static Dictionary<string, int> FieldAmount = new Dictionary<string, int>();                                                            //자료명, 총인덱스수
    private static List<KeyValuePair<string, int>> FieldGroup = new List<KeyValuePair<string, int>>();                                              //자료명, 인덱스

    private static bool ArrangeTableData(List<string[]> inStr)
    {
        FieldSet.Clear();
        FieldAmount.Clear();
        FieldGroup.Clear();
        
        if (inStr.Count < 2 || inStr[0].Length < 2)
            return false;

        for (int i = 0; i < inStr[1].Length; ++i)
        {
            var testStr = inStr[1][i];
            var type = IndexerType.None;
            if (testStr.EndsWith("_0"))
            {
                type = IndexerType.Ten;
                testStr = testStr.Substring(0, testStr.Length - 1);
                FieldSet.Add(testStr, new KeyValuePair<IndexerType, string>(type, inStr[0][i]));
                FieldAmount.Add(testStr, 1);
                FieldGroup.Add(new KeyValuePair<string, int>(testStr, FieldAmount[testStr] - 1));
                continue;
            }
            else if (testStr.EndsWith("_00"))
            {
                type = IndexerType.Hundred;
                testStr = testStr.Substring(0, testStr.Length - 2);
                FieldSet.Add(testStr, new KeyValuePair<IndexerType, string>(type, inStr[0][i]));
                FieldAmount.Add(testStr, 1);
                FieldGroup.Add(new KeyValuePair<string, int>(testStr, FieldAmount[testStr] - 1));
                continue;
            }

            bool isIndexer = false;
            var e = FieldSet.GetEnumerator();
            while (e.MoveNext())
            {
                if (testStr.StartsWith(e.Current.Key))
                {
                    switch (e.Current.Value.Key)
                    {
                        case IndexerType.Ten:
                            {
                                int num = 0;
                                if (int.TryParse(testStr.Substring(testStr.Length - 1), out num))
                                {
                                    var idxTestStr = testStr.Substring(0, testStr.Length - 1);
                                    if (FieldAmount.ContainsKey(idxTestStr) && num == FieldAmount[idxTestStr])
                                    {
                                        FieldAmount[testStr.Substring(0, testStr.Length - 1)] += 1;
                                        FieldGroup.Add(new KeyValuePair<string, int>(idxTestStr, FieldAmount[idxTestStr] - 1));
                                        isIndexer = true;
                                        break;
                                    }
                                }
                                break;
                            }

                        case IndexerType.Hundred:
                            {
                                int num = 0;
                                if (int.TryParse(testStr.Substring(testStr.Length - 2), out num))
                                {
                                    var idxTestStr = testStr.Substring(0, testStr.Length - 2);
                                    if (FieldAmount.ContainsKey(idxTestStr) && num == FieldAmount[idxTestStr])
                                    {
                                        FieldAmount[testStr.Substring(0, testStr.Length - 2)] += 1;
                                        FieldGroup.Add(new KeyValuePair<string, int>(idxTestStr, FieldAmount[idxTestStr] - 1));
                                        isIndexer = true;
                                        break;
                                    }
                                }
                                break;
                            }

                        default:
                            continue;
                    }

                    if (isIndexer == true)
                        break;
                }
            }

            if (isIndexer == false)
            {
                FieldSet.Add(testStr, new KeyValuePair<IndexerType, string>(type, inStr[0][i]));
                FieldAmount.Add(testStr, 1);
                FieldGroup.Add(new KeyValuePair<string, int>(testStr, FieldAmount[testStr] - 1));
            }
        }

        return true;
    }



    public static bool ArrangeTableData(List<string[]> inStr, out Dictionary<string, KeyValuePair<IndexerType, string>> set, out Dictionary<string, int> amount, out List<KeyValuePair<string, int>> group)
    {
        bool succeed = ArrangeTableData(inStr);

        set = succeed ? FieldSet : null;
        amount = succeed ? FieldAmount : null;
        group = succeed ? FieldGroup : null;
        return succeed;

    }

    private static void CreateTable(string database, string output, string function)
    {
        List<string[]> tableStr = new List<string[]>();

        using (FileStream fs = new FileStream(database, FileMode.Open))
        {
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8, false))
            {
                string strLineValue = null;

                while ((strLineValue = sr.ReadLine()) != null)
                {
                    tableStr.Add(strLineValue.Split(','));
                }
            }
        }

        if (ArrangeTableData(tableStr) == false)
            return;

        if (Directory.GetFiles("Assets/Script/H5Table/", function + "Table.cs").Length > 0)
            File.Delete(output);

        var tableName = function + "Table";
        var tableData = tableName + "Data";

        using (StreamWriter sw = new StreamWriter(output, false))
        {
            sw.WriteLine("using System.Collections.Generic;");
            sw.WriteLine("using UnityEngine;");
            sw.WriteLine("");
            sw.WriteLine("public partial class H5Table");
            sw.WriteLine("{");
            sw.WriteLine("    private static " + tableName + " m" + function + ";");
            sw.WriteLine("    public static " + tableName + " " + function);
            sw.WriteLine("    {");
            sw.WriteLine("        get");
            sw.WriteLine("        {");
            sw.WriteLine("            if (m" + function + " == null)");
            sw.WriteLine("                m" + function + " = new " + tableName + "();");
            sw.WriteLine("            return m" + function + ";");
            sw.WriteLine("        }");
            sw.WriteLine("    }");
            sw.WriteLine("");
            sw.WriteLine("    public class " + tableName);
            sw.WriteLine("    {");
            sw.WriteLine("        public class " + tableData);
            sw.WriteLine("        {");
            var ea = FieldAmount.GetEnumerator();
            while (ea.MoveNext())
            {
                if (FieldSet[ea.Current.Key].Key == IndexerType.None)
                {
                    sw.WriteLine("            public " + FieldSet[ea.Current.Key].Value + " " + ea.Current.Key + ";");
                }
                else
                {
                    sw.WriteLine("            public " + FieldSet[ea.Current.Key].Value + "[] " + ea.Current.Key + " = new " + FieldSet[ea.Current.Key].Value + "[" + ea.Current.Value + "];");
                }
            }
            sw.WriteLine("        }");
            sw.WriteLine("");
            sw.WriteLine("        private Dictionary<" + tableStr[0][0] + ", " + tableData + "> mIDDic = new Dictionary<" + tableStr[0][0] + ", " + tableData + ">();");
            sw.WriteLine("        private Dictionary<" + tableStr[0][1] + ", " + tableData + "> mNameDic = new Dictionary<" + tableStr[0][1] + ", " + tableData + ">();");
            sw.WriteLine("");
            sw.WriteLine("        public " + tableName + "()");
            sw.WriteLine("        {");
            sw.WriteLine("            " + tableData + " data;");
            sw.WriteLine("");
            sw.WriteLine("            TextAsset asset = Resources.Load<TextAsset>(\"Table/" + function + "\");");
            sw.WriteLine("            var strs = asset.text;");
            sw.WriteLine("            strs = strs.Replace(\"\\r\", \"\");");
            sw.WriteLine("            var lines = strs.Split('\\n');");
            sw.WriteLine("            List<string[]> tableStr = new List<string[]>();");
            sw.WriteLine("            for (int i = 0; i < lines.Length; ++i)");
            sw.WriteLine("            {");
            sw.WriteLine("                if (lines[i].Length <= 0)");
            sw.WriteLine("                    continue;");
            sw.WriteLine("                tableStr.Add(lines[i].Split(','));");
            sw.WriteLine("            }");
            sw.WriteLine("            for (int i = 2; i < tableStr.Count; ++i)");
            sw.WriteLine("            {");
            sw.WriteLine("                data = new " + tableData + "();");
            for (int i = 0; i < FieldGroup.Count; ++i)
            {
                var eg = FieldGroup[i];
                if (FieldSet[eg.Key].Key == IndexerType.None)
                {
                    switch (FieldSet[eg.Key].Value)
                    {
                        case "int":
                            {
                                sw.WriteLine("                data." + eg.Key + " = " + "int.Parse(tableStr[i][" + i + "]);");
                                break;
                            }

                        case "bool":
                            {
                                sw.WriteLine("                data." + eg.Key + " = " + "(tableStr[i][" + i + "] == \"TRUE\" ? true : false);");
                                break;
                            }

                        default:
                            {
                                sw.WriteLine("                data." + eg.Key + " = " + "tableStr[i][" + i + "];");
                                break;
                            }
                    }
                }
                else
                {
                    switch (FieldSet[eg.Key].Value)
                    {
                        case "int":
                            {
                                sw.WriteLine("                data." + eg.Key + "[" + eg.Value + "] = " + "int.Parse(tableStr[i][" + i + "]);");
                                break;
                            }

                        case "bool":
                            {
                                sw.WriteLine("                data." + eg.Key + "[" + eg.Value + "] = " + "(tableStr[i][" + i + "] == \"TRUE\" ? true : false);");
                                break;
                            }

                        default:
                            {
                                sw.WriteLine("                data." + eg.Key + "[" + eg.Value + "] = " + "tableStr[i][" + i + "];");
                                break;
                            }
                    }
                }
            }
            sw.WriteLine("                mIDDic.Add(data." + FieldGroup[0].Key + ", data);");
            sw.WriteLine("                mNameDic.Add(data." + FieldGroup[1].Key + ", data);");
            sw.WriteLine("            }");
            sw.WriteLine("        }");
            sw.WriteLine("");
            sw.WriteLine("        public " + tableData + " GetDataBy" + FieldGroup[0].Key + "(" + FieldSet[FieldGroup[0].Key].Value + " id)");
            sw.WriteLine("        {");
            sw.WriteLine("            return mIDDic.ContainsKey(id) ? mIDDic[id] : null;");
            sw.WriteLine("        }");
            sw.WriteLine("        public " + tableData + " GetDataBy" + FieldGroup[1].Key + "(" + FieldSet[FieldGroup[1].Key].Value + " name)");
            sw.WriteLine("        {");
            sw.WriteLine("            return mNameDic.ContainsKey(name) ? mNameDic[name] : null;");
            sw.WriteLine("        }");
            sw.WriteLine("    }");
            sw.WriteLine("}");
        }
    }
}
