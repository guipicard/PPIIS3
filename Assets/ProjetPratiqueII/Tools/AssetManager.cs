using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;

public class AssetManager : EditorWindow
{
    private static AssetManager m_Window = null;

    private int m_SelectedTool = 0;
    private string[] m_ToolOptions = { "BitGem", "Prefabs", "Folder Search" };

    private GUIContent[] m_BigtGemItems;
    private GUIContent[] m_PrefabsItems;

    private Vector2 m_ScrollPosition = Vector2.zero;


    private DefaultAsset targetFolder = null;

    [MenuItem("Window/Personnel/AssetWindow")]
    public static void ShowAssetWindow()
    {
        if (m_Window == null)
        {
            m_Window = GetWindow<AssetManager>();
            m_Window.titleContent = new GUIContent("Asset Window");
        }
    }

    private void OnGUI()
    {
        m_SelectedTool = GUILayout.Toolbar(m_SelectedTool, m_ToolOptions);

        if (m_SelectedTool == 0)
        {
            if (GUILayout.Button("Update BitGem") || m_BigtGemItems.Length == 0)
            {
                CreateContent(0);
            }

            EditorGUILayout.LabelField("Items:", EditorStyles.boldLabel);

            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

            foreach (var item in m_BigtGemItems)
            {
                GUILayout.BeginHorizontal();
                if (item.image != null)
                {
                    GUILayout.Label(item.image, GUILayout.Width(60), GUILayout.Height(60));
                }
                else
                {
                    GUILayout.Label("No preview available", GUILayout.Width(40), GUILayout.Height(40));
                }

                string filename = Path.GetFileName(item.text);
                if (GUILayout.Button(filename))
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(item.text);
                    Selection.activeObject = prefab;
                }

                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
        else if (m_SelectedTool == 1)
        {
            if (GUILayout.Button("Update Prefabs") || m_PrefabsItems.Length == 0)
            {
                CreateContent(1);
            }

            EditorGUILayout.LabelField("Items:", EditorStyles.boldLabel);

            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

            foreach (var item in m_PrefabsItems)
            {
                GUILayout.BeginHorizontal();
                if (item.image != null)
                {
                    GUILayout.Label(item.image, GUILayout.Width(60), GUILayout.Height(60));
                }
                else
                {
                    GUILayout.Label("No preview available", GUILayout.Width(40), GUILayout.Height(40));
                }

                string filename = Path.GetFileName(item.text);
                if (GUILayout.Button(filename))
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(item.text);
                    Selection.activeObject = prefab;
                }

                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        if (m_SelectedTool == 2)
        {
            targetFolder = (DefaultAsset)EditorGUILayout.ObjectField(
                "Select Folder",
                targetFolder,
                typeof(DefaultAsset),
                false);

            if (targetFolder != null)
            {
                EditorGUILayout.HelpBox(
                    "Valid folder! Name: " + targetFolder.name,
                    MessageType.Info,
                    true);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Not valid!",
                    MessageType.Warning,
                    true);
            }
        }
    }


    private void CreateContent(int _id)
    {
        if (_id == 0)
        {
            List<GUIContent> contentList = new List<GUIContent>();
            string[] paths = { "Assets/BitGem" };
            string[] guids = AssetDatabase.FindAssets("t:prefab", paths);

            foreach (var itemId in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(itemId);

                GUIContent item = new GUIContent();

                item.text = path;
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var prefabImage = AssetPreview.GetAssetPreview(prefab) as Texture;
                item.image = prefabImage;


                contentList.Add(item);
            }

            List<GUIContent> newArray = new List<GUIContent>();
            for (int i = 0; i < contentList.Count; i++)
            {
                if (contentList[i].image != null)
                {
                    newArray.Add(contentList[i]);
                }
            }

            for (int i = 0; i < contentList.Count; i++)
            {
                if (contentList[i].image == null)
                {
                    newArray.Add(contentList[i]);
                }
            }

            m_BigtGemItems = newArray.ToArray();
        }
        else if (_id == 1)
        {
            List<GUIContent> contentList1 = new List<GUIContent>();
            string[] paths1 = { "Assets/ProjetPratique_H2023/Prefabs" };
            string[] guids1 = AssetDatabase.FindAssets("t:prefab", paths1);

            foreach (var itemId in guids1)
            {
                string path1 = AssetDatabase.GUIDToAssetPath(itemId);

                GUIContent item1 = new GUIContent();

                item1.text = path1;
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path1);
                var prefabImage = AssetPreview.GetAssetPreview(prefab) as Texture;
                item1.image = prefabImage;


                contentList1.Add(item1);
            }

            List<GUIContent> newArray1 = new List<GUIContent>();
            for (int i = 0; i < contentList1.Count; i++)
            {
                if (contentList1[i].image != null)
                {
                    newArray1.Add(contentList1[i]);
                }
            }

            for (int i = 0; i < contentList1.Count; i++)
            {
                if (contentList1[i].image == null)
                {
                    newArray1.Add(contentList1[i]);
                }
            }

            m_PrefabsItems = newArray1.ToArray();
        }
    }
}