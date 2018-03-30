using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

using USheet;

public class TestUSheet : MonoBehaviour {

    public string dataPath = "Assets/Resource/Student.asset";

    public InputField inputField;
    public Button getRow1Btn;
    public Button getRow2Btn;

    public Button modifyBtn;

    public Text output;

    private SheetData _data = null;

    // Use this for initialization
    void Start () {

        _data = AssetDatabase.LoadAssetAtPath<SheetData>(dataPath);
        if (_data == null)
        {
            Debug.LogError("load sheet data error");
            return;
        }

        if (getRow1Btn != null)
        {
            getRow1Btn.onClick.AddListener(onTestGetRowBtnClick);
        }
        if (getRow2Btn != null)
        {
            getRow2Btn.onClick.AddListener(onGetRow2BtnClick);
        }
        if (modifyBtn != null)
        {
            modifyBtn.onClick.AddListener(onModifyBtnClick);
        }

        output.text = "row:" + _data.rowCount + ", col:" + _data.columnCount;
	}

    private string formatRow(Dictionary<string, IGridData> rowData)
    {
        if (rowData == null) return "null";

        string outputText = "";
        foreach (var item in rowData)
        {
            outputText += string.Format("{0}:{1}\n", item.Key, item.Value.ToString());
        }
        return outputText;
    }

    private void onTestGetRowBtnClick()
    {
        if (_data == null)
        {
            Debug.LogError("Sheet is null");
            return;
        }
        string rowStr = inputField.text;
        if (String.IsNullOrEmpty(rowStr))
        {
            output.text = "Input: row number";
            return;
        }
        Dictionary<string, IGridData> rowData = _data.getRow(int.Parse(rowStr));
        output.text = formatRow(rowData);
    }

    private void onGetRow2BtnClick()
    {
        if (_data == null)
        {
            Debug.LogError("Sheet is null");
            return;
        }
        string rowStr = inputField.text;
        if (String.IsNullOrEmpty(rowStr))
        {
            output.text = "Input: title|value (only string)";
            return;
        }
        string[] searchParams = rowStr.Split('|');
        if (searchParams.Length != 2)
        {
            Debug.LogError("searchParams error");
            return;
        }
        List<Dictionary<string, IGridData>> rows = _data.getRows(searchParams[0], searchParams[1]);
        if (rows == null)
        {
            output.text = "No result";
            return;
        }

        string outputStr = "";
        for (int i = 0; i < rows.Count; i++)
        {
            outputStr += formatRow(rows[i]);
            outputStr += "-----------------------------------\n";
        }

        output.text = outputStr;
    }

    private void onModifyBtnClick()
    {
        string[] searchParams = inputField.text.Split('|');
        if (searchParams.Length != 3)
        {
            output.text = "Input: rowNo|colName|value (only string)";
            return;
        }
        int rowNo = int.Parse(searchParams[0]);
        string columnName = searchParams[1];
        string value = searchParams[2];

        _data.modify(columnName, rowNo, new GridData<string>(value));
        EditorUtility.SetDirty(_data);
    }
}
