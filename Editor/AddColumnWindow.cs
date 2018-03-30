
using System;
using UnityEditor;
using UnityEngine;

namespace USheet
{

    public class AddColumnWindow : PopupWindowContent
    {
        private string _columnTitle = "";
        private E_DATA_TYPE _dataType = E_DATA_TYPE.INT;

        private int labelWidth = 70;
        private int fieldWidth = 100;

        private int _curIndex = 0;

        public Action<string, E_DATA_TYPE, int> createColumnAction = null;

        public AddColumnWindow(int columnIndex)
        {
            _curIndex = columnIndex;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(190, 130);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("Create Column", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Title:", GUILayout.Width(labelWidth));
            _columnTitle = EditorGUILayout.TextField(_columnTitle, GUILayout.Width(fieldWidth));
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Data Type:", GUILayout.Width(labelWidth));
            _dataType = (E_DATA_TYPE)EditorGUILayout.EnumPopup(_dataType, GUILayout.Width(fieldWidth));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("OK", GUILayout.Width(80)))
            {
                if (createColumnAction != null)
                {
                    createColumnAction(_columnTitle, _dataType, _curIndex + 1);
                }

                editorWindow.Close();
            }

            if (GUILayout.Button("Cancel", GUILayout.Width(80)))
            {
                editorWindow.Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
