
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace USheet
{

    [CustomEditor(typeof(SheetData))]
    public class SheetDataEditor : Editor
    {

        SheetData editorData;
        List<List<IGridUI>> _allGridUIs = new List<List<IGridUI>>();

        List<Rect> _titleRects = new List<Rect>();
        List<Rect> _rowNumRects = new List<Rect>();

        private void createSheet()
        {
            editorData.loadData();
            createGridUIs();
        }

        protected void OnEnable()
        {
            editorData = target as SheetData;
            createSheet();
        }

        private void initTitleAndRowNoRects()
        {
            //初始化
            if (_rowNumRects.Count != editorData.rowCount + 1)
            {
                _rowNumRects.Clear();
                for (int i = 0; i <= editorData.rowCount; i++)
                {
                    _rowNumRects.Add(new Rect(0, 0, 0, 0));
                }
            }
            if (_titleRects.Count != editorData.columnCount)
            {
                _titleRects.Clear();
                for (int i = 0; i < editorData.columnCount; i++)
                {
                    _titleRects.Add(new Rect(0, 0, 0, 0));
                }
            }
        }

        public override void OnInspectorGUI()
        {
            //Debug.Log("SheetDataEditor::OnInspectorGUI");

            createSheet();
            initTitleAndRowNoRects();

            GUILayout.Label("row:" + editorData.rowCount + ", col:" + editorData.columnCount);

            drawTitles();
            drawContent();

            base.OnInspectorGUI();
        }


        private void drawTitles()
        {
            EditorGUILayout.BeginHorizontal();
            drawRowNo(-1);

            for (int i = 0; i < editorData.columnCount; i++)
            {
                string titleName = editorData.getTitle(i);
                if (string.IsNullOrEmpty(titleName))
                {
                    continue;
                }

                if (GUILayout.Button(titleName, EditorStyles.toolbarPopup))
                {
                    PopupMenu menu = new PopupMenu();
                    menu.addItem("Add Column", () =>
                    {
                        showAddColumnWindow(i);
                    });
                    menu.addItem("Rename", () =>
                    {
                        showRenameColumnWindow(titleName);
                    });
                    menu.addItem("Delete Column", () =>
                    {
                        showDelColumnConfirmWindow(titleName);
                    });
                    PopupWindow.Show(_titleRects[i], menu);
                }

                if (Event.current.type == EventType.Repaint)
                    _titleRects[i] = GUILayoutUtility.GetLastRect();

            }
            EditorGUILayout.EndHorizontal();
        }

        private void drawContent()
        {
            EditorGUILayout.BeginVertical();

            for (int rowIndex = 0; rowIndex < _allGridUIs.Count; rowIndex++)
            {

                List<IGridUI> row = _allGridUIs[rowIndex];

                EditorGUILayout.BeginHorizontal();
                drawRowNo(rowIndex);
                for (int colIndex = 0; colIndex < row.Count; colIndex++)
                {
                    row[colIndex].initStyle();
                    row[colIndex].draw();
                }
                EditorGUILayout.EndHorizontal();

            }

            EditorGUILayout.EndVertical();
        }

        private void createGridUIs()
        {
            _allGridUIs.Clear();

            for (int rowIndex = 0; rowIndex < editorData.rowCount; rowIndex++)
            {
                Dictionary<string, IGridData> rowData = editorData.getRow(rowIndex);
                if (rowData.Count > 0)
                {
                    List<IGridUI> row = new List<IGridUI>();

                    for (int colIndex = 0; colIndex < editorData.columnCount; colIndex++)
                    {
                        string titleName = editorData.getTitle(colIndex);
                        int mayBeModifiedIndex = rowIndex;
                        IGridUI grid = GridGUIManager.getInstance().createGridUI(rowData[titleName]);
                        grid.dataChangeAction = () =>
                        {
                            editorData.modify(titleName, mayBeModifiedIndex, grid.getData());
                            EditorUtility.SetDirty(target);
                        };
                        grid.title = titleName;
                        row.Add(grid);
                    }

                    _allGridUIs.Add(row);
                }
            }
        }

        private void drawRowNo(int number)
        {
            GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
            style.fixedWidth = 30;
            style.fixedHeight = 20;

            string strNo = "+";
            if (number >= 0) strNo = number.ToString();
            if (GUILayout.Button(strNo, style))
            {
                PopupMenu menu = new PopupMenu();

                //+action
                if (number < 0)
                {
                    menu.addItem("Add Column", () =>
                    {
                        showAddColumnWindow(0);
                    });
                }

                //只能在有title的时候创建行
                if (editorData.columnCount > 0)
                {
                    menu.addItem("Add Row", () =>
                    {
                        editorData.insert(number + 1);
                        EditorUtility.SetDirty(target);
                    });
                }


                if (number >= 0)
                {
                    menu.addItem("Delete Row", () =>
                    {
                        editorData.deleteRow(number);
                        EditorUtility.SetDirty(target);
                    });
                }

                PopupWindow.Show(_rowNumRects[number + 1], menu);
            }

            if (Event.current.type == EventType.Repaint)
                _rowNumRects[number + 1] = GUILayoutUtility.GetLastRect();
        }

        private void showAddColumnWindow(int columnIndex)
        {
            AddColumnWindow dialog = new AddColumnWindow(columnIndex);
            dialog.createColumnAction = (string title, E_DATA_TYPE dataType, int index) =>
            {
                editorData.insertColumn(title, dataType, index);
                EditorUtility.SetDirty(target);
            };
            PopupWindow.Show(new Rect(0, 0, 0, 0), dialog);
        }



        private void showRenameColumnWindow(string titleName)
        {

            RenamePopWindow dialog = new RenamePopWindow();
            dialog.renameAction = (string name) =>
            {
                editorData.modifyColumnName(titleName, name);
                EditorUtility.SetDirty(target);
            };
            dialog.titleName = titleName;

            PopupWindow.Show(new Rect(0, 0, 0, 0), dialog);
        }

        private void showDelColumnConfirmWindow(string titleName)
        {

            string content = String.Format("Are you sure DELETE column \'{0}\'?", titleName);

            OKCancelWindow delConfirmWindow = new OKCancelWindow("Warnning", content, () =>
            {
                editorData.deleteColumn(titleName);
                EditorUtility.SetDirty(target);
            }, null);

            PopupWindow.Show(new Rect(0, 0, 0, 0), delConfirmWindow);
        }
    }
}
