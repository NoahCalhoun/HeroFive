using System.Collections.Generic;
using System.Text;
using System.IO;

using UnityEngine;

[ExecuteInEditMode]
public class TableGenerator : MonoBehaviour
{
#if false//UNITY_EDITOR
    FileSystemWatcher fsw = new FileSystemWatcher();
#endif

    // Use this for initialization
    void Start()
    {
#if false//UNITY_EDITOR
        fsw.Path = "Assets/Table";
        fsw.NotifyFilter = NotifyFilters.LastWrite;

        fsw.Changed += new FileSystemEventHandler(GenerateTable);
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void GenerateTable(object sender, FileSystemEventArgs e)
    {
        var tableNames = Directory.GetFiles("Assets/Table", "*.csv");
        for (int i = 0; i < tableNames.Length; ++i)
        {
            var table = tableNames[i];
            table = table.Replace("Assets/Table\\", "");
            table = table.Replace(".csv", "");
            CreateTable(tableNames[i], "Assets/Script/H5Table/" + table + "Table.cs", table);
        }
    }

    public static void CreateTable(string database, string output, string function)
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
                    continue;
                }
            }
        }

        var tableName = function + "Table";
        var tableData = tableName + "Data";
        if (Directory.GetFiles("Assets/Script/H5Table/", function + "Table.cs").Length > 0)
            File.Delete(output);
        using (StreamWriter sw = new StreamWriter(output, false))
        {
            sw.WriteLine("using System.Collections.Generic;");
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
            for (int i = 0; i < tableStr[0].Length; ++i)
            {
                sw.WriteLine("            public " + tableStr[0][i] + " " + tableStr[1][i] + ";");
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
            for (int i = 2; i < tableStr.Count; ++i)
            {
                sw.WriteLine("                data = new " + tableData + "()");
                sw.WriteLine("                {");
                for (int j = 0; j < tableStr[0].Length; ++j)
                {
                    switch (tableStr[0][j])
                    {
                        case "string":
                            sw.WriteLine("                    " + tableStr[1][j] + " = " + '\"' + tableStr[i][j] + '\"' + ",");
                            break;

                        case "bool":
                            sw.WriteLine("                    " + tableStr[1][j] + " = " + (tableStr[i][j] == "TRUE" ? "true" : "false") + ",");
                            break;

                        default:
                            sw.WriteLine("                    " + tableStr[1][j] + " = " +  tableStr[i][j] + ",");
                            break;
                    }
                }
                sw.WriteLine("                };");
                sw.WriteLine("                mIDDic.Add(" + tableStr[2][0] + ", data);");
                sw.WriteLine("                mNameDic.Add(" + (tableStr[0][1] == "string" ? ('\"' + tableStr[2][1] + '\"') : tableStr[2][1]) + ", data);");
            }
            sw.WriteLine("        }");
            sw.WriteLine("");
            sw.WriteLine("        public " + tableData + " GetDataBy" + tableStr[1][0] + "(" + tableStr[0][0] + " id)");
            sw.WriteLine("        {");
            sw.WriteLine("            return mIDDic.ContainsKey(id) ? mIDDic[id] : null;");
            sw.WriteLine("        }");
            sw.WriteLine("        public " + tableData + " GetDataBy" + tableStr[1][1] + "(" + tableStr[0][1] + " name)");
            sw.WriteLine("        {");
            sw.WriteLine("            return mNameDic.ContainsKey(name) ? mNameDic[name] : null;");
            sw.WriteLine("        }");
            sw.WriteLine("    }");
            sw.WriteLine("}");
        }
    }
    public static void CreateTableWithoutLine(string database, string output, string function)
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
                    continue;
                }
            }
        }

        var tableName = function + "Table";
        var tableData = tableName + "Data";
        if (Directory.GetFiles("Assets/Script/H5Table/", function + "Table.cs").Length > 0)
            File.Delete(output);
        using (StreamWriter sw = new StreamWriter(output, false))
        {
            sw.Write("using System.Collections.Generic;");
            sw.WriteLine("");
            sw.Write("public partial class H5Table");
            sw.Write("{");
            sw.Write("    private static " + tableName + " m" + function + ";");
            sw.Write("    public static " + tableName + " " + function);
            sw.Write("    {");
            sw.Write("        get");
            sw.Write("        {");
            sw.Write("            if (m" + function + " == null)");
            sw.Write("                m" + function + " = new " + tableName + "();");
            sw.Write("            return m" + function + ";");
            sw.Write("        }");
            sw.Write("    }");
            sw.WriteLine("");
            sw.Write("    public class " + tableName);
            sw.Write("    {");
            sw.Write("        public class " + tableData);
            sw.Write("        {");
            for (int i = 0; i < tableStr[0].Length; ++i)
            {
                sw.Write("            public " + tableStr[0][i] + " " + tableStr[1][i] + ";");
            }
            sw.Write("        }");
            sw.WriteLine("");
            sw.Write("        private Dictionary<" + tableStr[0][0] + ", " + tableData + "> mIDDic = new Dictionary<" + tableStr[0][0] + ", " + tableData + ">();");
            sw.Write("        private Dictionary<" + tableStr[0][1] + ", " + tableData + "> mNameDic = new Dictionary<" + tableStr[0][1] + ", " + tableData + ">();");
            sw.WriteLine("");
            sw.Write("        public " + tableName + "()");
            sw.Write("        {");
            sw.Write("            " + tableData + " data;");
            sw.WriteLine("");
            for (int i = 2; i < tableStr.Count; ++i)
            {
                sw.Write("                data = new " + tableData + "()");
                sw.Write("                {");
                for (int j = 0; j < tableStr[0].Length; ++j)
                {
                    switch (tableStr[0][j])
                    {
                        case "string":
                            sw.Write("                    " + tableStr[1][j] + " = " + '\"' + tableStr[i][j] + '\"' + ",");
                            break;

                        case "bool":
                            sw.Write("                    " + tableStr[1][j] + " = " + (tableStr[i][j] == "TRUE" ? "true" : "false") + ",");
                            break;

                        default:
                            sw.Write("                    " + tableStr[1][j] + " = " + tableStr[i][j] + ",");
                            break;
                    }
                }
                sw.Write("                };");
                sw.Write("                mIDDic.Add(" + tableStr[2][0] + ", data);");
                sw.Write("                mNameDic.Add(" + (tableStr[0][1] == "string" ? ('\"' + tableStr[2][1] + '\"') : tableStr[2][1]) + ", data);");
            }
            sw.Write("        }");
            sw.WriteLine("");
            sw.Write("        public " + tableData + " GetDataBy" + tableStr[1][0] + "(" + tableStr[0][0] + " id)");
            sw.Write("        {");
            sw.Write("            return mIDDic.ContainsKey(id) ? mIDDic[id] : null;");
            sw.Write("        }");
            sw.Write("        public " + tableData + " GetDataBy" + tableStr[1][1] + "(" + tableStr[0][1] + " name)");
            sw.Write("        {");
            sw.Write("            return mNameDic.ContainsKey(name) ? mNameDic[name] : null;");
            sw.Write("        }");
            sw.Write("    }");
            sw.Write("}");
        }
    }
}
