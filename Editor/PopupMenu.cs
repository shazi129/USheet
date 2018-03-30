using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace USheet
{
    public class PopupMenu : PopupWindowContent
    {
        private Dictionary<string, Action> _menuActionDic = new Dictionary<string, Action>();
        private GUIStyle _itemStyle;

        public PopupMenu()
        {
            _itemStyle = new GUIStyle(GUI.skin.label);
            _itemStyle.padding.left = 10;
            _itemStyle.hover.background = createTexture2D(Color.blue, 1, 1);
            _itemStyle.hover.textColor = Color.white;

        }

        public Texture2D createTexture2D(Color color, int width, int height)
        {
            Color[] colors = new Color[width * height];
            for (int i = 0; i < width * height; i++)
            {
                colors[i] = color;
            }
            Texture2D textrue = new Texture2D(width, height);
            textrue.SetPixels(colors);

            return textrue;
        }

        public void addItem(string itemName, Action action)
        {
            if (_menuActionDic.ContainsKey(itemName))
            {
                Debug.LogError("Add duplicate item in PopupMenu");
                return;
            }

            _menuActionDic.Add(itemName, action);
        }

        public override void OnGUI(Rect rect)
        {
            foreach (var item in _menuActionDic)
            {
                if (GUILayout.Button(item.Key, _itemStyle))
                {
                    if (item.Value != null)
                    {
                        this.editorWindow.Close();
                        item.Value();
                    }
                    Debug.Log("OnClick menu:" + item.Key);
                }
            }

            if (Event.current.type == EventType.MouseMove)
                editorWindow.Repaint();

        }

    }
}
