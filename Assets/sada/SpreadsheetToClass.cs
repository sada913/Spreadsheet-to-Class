using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SpreadsheetToClass : MonoBehaviour {
    char delim = ',';

    public List<string[]> csv = new List<string[]>();

    public string ClassName = "Enemy";

    public string Key = "12xlZWZCyXAqGC9hh1ijDLNrDFIHrTpi8PdA_jDHUK2g";

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CreateList()
    {
        StartCoroutine(DlSpreadsheet());
    }
    /// <summary>
    /// スプレッドシートから読み込み
    /// </summary>
    /// <returns></returns>
    IEnumerator DlSpreadsheet()
    {
        //urlには　https://docs.google.com/spreadsheets/d/[key]/gviz/tq?tqx=out:csv&sheet=[シート名]を入力する

        string url2 = "https://docs.google.com/spreadsheets/d/"+Key+"/gviz/tq?tqx=out:csv&sheet=" + ClassName;
        WWW www2 = new WWW(url2);
        yield return www2;
        csv = ReadFile(www2.text);
        TextSave(www2.text, Application.dataPath + "/sada/Classes/" + ClassName + ".csv");
        //ここまでcsv読み込み
        //csv[↓][→]の順
        //例csv[1][2]で2行3列の値が取れる

        ClassCreator(csv);
    }


    /// <summary>
    /// csv読み込み
    /// </summary>
    /// <param name="csv"></param>
    /// <returns></returns>
    List<string[]> ReadFile(string csv)
    {
        // StringReaderで一行ずつ読み込んで、区切り文字で分割
        List<string[]> data = new List<string[]>();
        StringReader sr = new StringReader(csv);
        while (sr.Peek() > -1)
        {
            string line = sr.ReadLine();
            data.Add(line.Split(delim));
        }
        return data;
    }

    /// <summary>
    /// クラス作成
    /// </summary>
    /// <param name="csv"></param>
    void ClassCreator(List<string[]> csv)
    {
        Directory.CreateDirectory("Assets/sada/Editor/");
        Directory.CreateDirectory("Assets/sada/Classes/");

        string templateFilePath =Application.dataPath + "/sada/tmpl.txt" ;

        string entittyTemplate = File.ReadAllText(templateFilePath);


        string cs_member_str = "";
        for(int i = 0; i < csv[0].Length;i++)
        {
            if (csv[0][i] == "\"\"")
            {
                break;
            }
            //ここif文かスイッチにする
            cs_member_str += "public " + csv[0][i].Split('\"')[1] + " " + csv[1][i].Split('\"')[1] + ";\n";

        }
        entittyTemplate = entittyTemplate.Replace("$Types$", cs_member_str);
        entittyTemplate = entittyTemplate.Replace("$ExcelData$", ClassName);

        TextSave(entittyTemplate, Application.dataPath + "/sada/Classes/" +ClassName +".cs");

    }

    /// <summary>
    /// 作成したクラスを読み込んでScriptableObjectを作るCSを作る
    /// </summary>
    public void LoadClass()
    {
        string path = "Assets/sada/Classes/" + ClassName + ".csv";
        string csv_str = File.ReadAllText(path);

        csv = ReadFile(csv_str);
        string s = "            "+ClassName+".Param a2 = new "+ClassName+".Param {";


        List<string> ValName = new List<string>();

        for (int i = 0; i < csv[0].Length; i++)
        {
            if (csv[0][i] == "\"\"")
            {
                break;
            }
            ValName.Add(csv[1][i].Split('\"')[1]);
        }

        for (int i = 2; i < csv.Count; i++)
        {
            List<string> Vals = new List<string>();
            for (int j = 0; j < csv[0].Length; j++)
            {
                if (csv[0][j].Split('\"')[1] == "string")
                {
                    Vals.Add(csv[i][j]);
                }
                else if (csv[0][j].Split('\"')[1] == "float")
                {
                    Vals.Add(csv[i][j].Split('\"')[1] + "f");
                }
                else
                    Vals.Add(csv[i][j].Split('\"')[1]);
            }


            for (int l = 0; l < ValName.Count; l++)
            {

                if (l == ValName.Count - 1)
                {
                    s += ValName[l] + " = " + Vals[l];
                }
                else
                    s += ValName[l] + " = " + Vals[l] + ",";

            }
            if (i < csv.Count - 1)
                s += "};\n          data.param.Add(a" + i.ToString()+");\n            "+ClassName+".Param a" + (i + 1).ToString() +" = new "+ClassName+".Param { ";
            else
                s += "};\n          data.param.Add(a" + i.ToString()+ ");\n";

        }

            FileRef();
            string asset_tmpl_path = "Assets/sada/Scriptable.txt";
            string assettmpl = File.ReadAllText(asset_tmpl_path);
            assettmpl = assettmpl.Replace("$ExcelData$", ClassName);
            assettmpl = assettmpl.Replace("$PATH$", "Assets/sada/Classes/" + ClassName);
            assettmpl = assettmpl.Replace("$PARM$", s);
            TextSave(assettmpl, Application.dataPath + "/sada/Editor/" + ClassName + "asset.cs");

       
    }

    /// <summary>
    /// .cs出力
    /// </summary>
    /// <param name="txt"></param>
    public void TextSave(string txt,string path)
    {

            StreamWriter sw = new StreamWriter(path, false); //true=追記 false=上書き

            sw.WriteLine(txt);
            sw.Flush();
            sw.Close();
    }

    /// <summary>
    /// 何らかのファイルの書き込み
    /// </summary>
    public void FileRef()
    {
        StreamWriter sw = new StreamWriter(Application.dataPath + "/sada/ref " , false); //true=追記 false=上書き
        sw.WriteLine(Random.value);
        sw.Flush();
        sw.Close();
    }

}
