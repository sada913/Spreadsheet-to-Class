using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//　カスタマイズするクラスを設定
[CustomEditor(typeof(SpreadsheetToClass))]
//　Editorクラスを継承してクラスを作成
public class MyDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // ...
        base.OnInspectorGUI();

        SpreadsheetToClass test = target as SpreadsheetToClass;
        if (GUILayout.Button("Create"))
        {
            test.CreateList();
        }

        if(GUILayout.Button("Reload"))
        {
            test.LoadClass();
            test.FileRef();
        }


    }
}
