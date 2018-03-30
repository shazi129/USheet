using System;
using UnityEditor;
using UnityEngine;

namespace USheet
{

    public class OKCancelWindow : PopupWindowContent
    {
        private Action _okAction = null;
        private Action _cancelAction = null;

        private String _title = "";
        private String _content = "";

        public OKCancelWindow(string title, string content, Action okAction, Action cancelAction)
        {
            _title = title;
            _content = content;
            _okAction = okAction;
            _cancelAction = cancelAction;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(190, 90);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label(_title, EditorStyles.boldLabel);
            EditorGUILayout.Separator();

            GUILayout.Label(_content, EditorStyles.label);

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("OK", GUILayout.Width(80)))
            {
                if (_okAction != null)
                {
                    _okAction();
                }

                editorWindow.Close();
            }

            if (GUILayout.Button("Cancel", GUILayout.Width(80)))
            {
                if (_cancelAction != null)
                {
                    _cancelAction();
                }
                editorWindow.Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
