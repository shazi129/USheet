using System;
using UnityEditor;
using UnityEngine;

namespace USheet
{
    public class RenamePopWindow : PopupWindowContent
    {
        private string _titleName = "Untitled";
        private Action<string> _renameAction = null;

        public string titleName
        {
            get { return _titleName; }
            set
            {
                _titleName = value;
                if (editorWindow != null)
                {
                    editorWindow.Repaint();
                }
            }
        }

        public Action<string> renameAction
        {
            set { _renameAction = value; }
            get { return _renameAction; }
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(180, 90);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("Rename Column", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _titleName = EditorGUILayout.TextField(_titleName, GUILayout.Width(150));

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("OK", GUILayout.Width(80)))
            {
                if (renameAction != null)
                {
                    renameAction(titleName);
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