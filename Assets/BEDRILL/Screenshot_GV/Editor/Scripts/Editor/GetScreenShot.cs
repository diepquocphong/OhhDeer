using ScreenShot.ResizeTools;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Unity.EditorCoroutines.Editor;

public class GetScreenShot : EditorWindow
{
    [System.Serializable]
    public class ResolutionsClass
    {
        public string NameScreenShot;
        public Vector2 Resolution;
    }
    public List<ResolutionsClass> Resolutions = new List<ResolutionsClass>();
    private string nameFile = "Screenshot";
    private static string SaveFolder = "";
    private string Suffix;
    private string Preffix;
    private bool needAddToNameResolutions = true;
    private bool needAddToNameSuffix = false;
    private bool needAddToNamePreffix = false;
    private bool togglePngOrJPEG = true;
    private bool moreResolution = false;
    private static ResizeTool activeTool;
    private ReorderableList itemList = null;
    static SerializedObject serializedObject = null;
    private static Texture2D textureLogo = null;
    private string RateUs = "Your rating is important!";
    /// <summary>
    /// Function to create and display a dialog box
    /// </summary>
    [MenuItem("Tools/Screenshot_GV")]
    public static void ShowWindow()
    {
        //Check the path to the folder where the screenshots will be stored
        if (SaveFolder == string.Empty || SaveFolder == "")
            SaveFolder = Application.dataPath + "/Screenshot_GV/Editor/Screenshot";
        //Create a dialog box and customize its size, logo, and title
        GetScreenShot window = GetWindow<GetScreenShot>("Screenshot_GV");
        window.minSize = Vector2.one * 150;
        activeTool = new ResizeTool("", 10, 10);
        textureLogo = (Texture2D)Resources.Load("logo", typeof(Texture2D));
        Texture icon = (Texture2D)Resources.Load("icon", typeof(Texture2D));
        GUIContent titleContent = new GUIContent("Screenshot_GV", icon);
        window.titleContent = titleContent;
        //Display dialog box
        window.Show();
    }
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.textArea))
            {
                EditorGUILayout.LabelField("Format:", EditorStyles.helpBox);
                togglePngOrJPEG = GUILayout.Toggle(togglePngOrJPEG, "PNG", EditorStyles.miniButtonLeft, GUILayout.Width(50f), GUILayout.ExpandWidth(false));
                togglePngOrJPEG = !GUILayout.Toggle(!togglePngOrJPEG, "JPG", EditorStyles.miniButtonRight, GUILayout.Width(50f), GUILayout.ExpandWidth(false));
            }

            using (new EditorGUILayout.HorizontalScope(EditorStyles.textArea))
            {
                EditorGUILayout.LabelField("More Resolutions:", EditorStyles.helpBox);
                moreResolution = GUILayout.Toggle(moreResolution, "On", EditorStyles.miniButtonLeft, GUILayout.Width(50f), GUILayout.ExpandWidth(false));
                moreResolution = !GUILayout.Toggle(!moreResolution, "Off", EditorStyles.miniButtonRight, GUILayout.Width(50f), GUILayout.ExpandWidth(false));
            }
            //Check for the variable "moreResolution" if it is true then create a ReorderableList if it is false we keep only the name
            if (moreResolution)
            {
                //Ñreate a ReorderableList
                if (itemList == null)
                {
                    serializedObject = new SerializedObject(this);
                    itemList = new ReorderableList(serializedObject, serializedObject.FindProperty("Resolutions"));
                }
                //Ñustomize a ReorderableList
                itemList.displayRemove = true;
                itemList.draggable = true;
                itemList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Resolutions"); };
                itemList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocus) =>
                {
                    SerializedProperty element = itemList.serializedProperty.GetArrayElementAtIndex(index);

                    EditorGUI.LabelField(new Rect(rect.x, rect.y, 140, EditorGUIUtility.singleLineHeight), "Name Screenshot:");
                    EditorGUI.PropertyField(
                           new Rect(rect.x + 115, rect.y, 100, EditorGUIUtility.singleLineHeight),
                           element.FindPropertyRelative("NameScreenShot"),
                           GUIContent.none
                       );
                    EditorGUI.LabelField(new Rect(rect.x + 230, rect.y, 100, EditorGUIUtility.singleLineHeight), "Resolution:");
                    EditorGUI.PropertyField(
                           new Rect(rect.x + 300, rect.y, 160, EditorGUIUtility.singleLineHeight),
                           element.FindPropertyRelative("Resolution"),
                           GUIContent.none
                       );
                };

                itemList.DoLayoutList();
                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                nameFile = EditorGUILayout.TextField("Name Screenshot:", nameFile);
            }
            GUI.enabled = true;

            //Next, add 3 switches to check if you need a prefix, suffix and whether you need to specify in the name of the file extension screen
            using (new EditorGUILayout.HorizontalScope(EditorStyles.textArea))
            {
                EditorGUILayout.LabelField("Add to name RESOLUTION:", EditorStyles.helpBox);
                needAddToNameResolutions = GUILayout.Toggle(needAddToNameResolutions, "On", EditorStyles.miniButtonLeft, GUILayout.Width(50f), GUILayout.ExpandWidth(false));
                needAddToNameResolutions = !GUILayout.Toggle(!needAddToNameResolutions, "Off", EditorStyles.miniButtonRight, GUILayout.Width(50f), GUILayout.ExpandWidth(false));
            }


            using (new EditorGUILayout.HorizontalScope(EditorStyles.textArea))
            {
                EditorGUILayout.LabelField("Add to name PREFFIX:", EditorStyles.helpBox);
                needAddToNamePreffix = GUILayout.Toggle(needAddToNamePreffix, "On", EditorStyles.miniButtonLeft, GUILayout.Width(50f), GUILayout.ExpandWidth(false));
                needAddToNamePreffix = !GUILayout.Toggle(!needAddToNamePreffix, "Off", EditorStyles.miniButtonRight, GUILayout.Width(50f), GUILayout.ExpandWidth(false));
            }
            if (needAddToNamePreffix)
            {
                Preffix = EditorGUILayout.TextField("Preffix: ", Preffix);
            }


            using (new EditorGUILayout.HorizontalScope(EditorStyles.textArea))
            {
                EditorGUILayout.LabelField("Add to name POSTFFIX:", EditorStyles.helpBox);
                needAddToNameSuffix = GUILayout.Toggle(needAddToNameSuffix, "On", EditorStyles.miniButtonLeft, GUILayout.Width(50f), GUILayout.ExpandWidth(false));
                needAddToNameSuffix = !GUILayout.Toggle(!needAddToNameSuffix, "Off", EditorStyles.miniButtonRight, GUILayout.Width(50f), GUILayout.ExpandWidth(false));
            }
            if (needAddToNameSuffix)
            {
                Suffix = EditorGUILayout.TextField("Postffix: ", Suffix);
            }

            //Create a text box with a file path and 2 buttons, the first with the ability to select the second loop to display this loop in Explorer
            OnGUISaveFolderInput();

            //Create a button to take a screenshot
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(new GUIContent("GET SCREENSHOT"), GUILayout.ExpandHeight(false), GUILayout.Height(25f)))
                {
                    GetScreenshots();
                }
            }

            // We create informative fields
            EditorGUILayout.LabelField("File saved to: " + SaveFolder, EditorStyles.helpBox);
            if (!moreResolution)
            {
                EditorGUILayout.LabelField("Current resolution: " + Handles.GetMainGameViewSize().x + "x" + Handles.GetMainGameViewSize().y, EditorStyles.helpBox);
                EditorGUILayout.LabelField("Resolution takes from Game View", EditorStyles.helpBox);
            }
            else
            {
                string res = "";
                for (int i = 0; i < Resolutions.Count; i++)
                {
                    res += Resolutions[i].Resolution.x + "x" + Resolutions[i].Resolution.y;
                    if (Resolutions.Count -1 != i)
                        res += ", ";
                    else
                        res += ". ";
                }
                EditorGUILayout.LabelField("Current resolution: "+res, EditorStyles.helpBox);
            }
            GUIStyle gUI = new GUIStyle();
            gUI.alignment = TextAnchor.UpperCenter;
            GUILayout.Label(textureLogo, gUI);
            LinkButton(RateUs, "https://assetstore.unity.com/packages/slug/193235");
            EditorGUILayout.EndVertical();
        }

    }
    /// <summary>
    /// Function for taking a screenshot
    /// </summary>
    void GetScreenshots()
    {
        if (moreResolution)
        {
            this.StartCoroutine(CoroutineForResizeGameWindow());
        }
        else
        {
            string folderSave = GetSaveFolderPath(Handles.GetMainGameViewSize(), nameFile); 
            if (!File.Exists(folderSave))
            {
                ScreenCapture.CaptureScreenshot(folderSave);
            } else
            {
                for (int i = 1; i < 100; i++)
                {
                    folderSave = GetSaveFolderPath(Handles.GetMainGameViewSize(), nameFile, i);  
                    if (!File.Exists(folderSave))
                    {
                        ScreenCapture.CaptureScreenshot(folderSave);
                        return;
                    }
                }
            }
        }
    }
    /// <summary>
    /// Coroutine which is used if "moreResolution" is selected true
    /// </summary>
    /// <returns>
    /// </returns>
    IEnumerator CoroutineForResizeGameWindow()
    {
        Vector2 currentRes = Handles.GetMainGameViewSize();
        foreach (var i in Resolutions)
        {
            activeTool.Resize((int)i.Resolution.x, (int)i.Resolution.y);

            string folderSave = GetSaveFolderPath(i.Resolution, i.NameScreenShot);
            if (!File.Exists(folderSave))
            {
                ScreenCapture.CaptureScreenshot(folderSave);
            }
            else
            {
                for (int z = 1; z < 100; z++)
                {
                    folderSave = GetSaveFolderPath(i.Resolution, i.NameScreenShot, z);
                    if (!File.Exists(folderSave))
                    {
                        ScreenCapture.CaptureScreenshot(folderSave);
                        break;
                    }
                }
            }
            yield return 0;
        }
        activeTool.Resize((int)currentRes.x, (int)currentRes.y);
     }

    /// <summary>
    /// Creating a link button
    /// </summary>
    /// <param name="caption">caption button</param>
    /// <param name="url"> link </param>
    private static void LinkButton(string caption, string url)
    {
        var style = GUI.skin.label;
        style.richText = true;

        bool buttonClicked = GUILayout.Button("<i><b><color=#1B69EB>" + caption+"</color></b></i>", style);

        var rect = GUILayoutUtility.GetLastRect();
        rect.width = style.CalcSize(new GUIContent(caption)).x;
        EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);

        if (buttonClicked)
            Application.OpenURL(url);
    }
    /// <summary>
    /// Save Folder Input
    /// </summary>
    private void OnGUISaveFolderInput()
    {
        SaveFolder = EditorGUILayout.TextField("Save to: ",(SaveFolder == string.Empty || SaveFolder == "") ? Application.dataPath + "/Screenshot_GV/Editor/Screenshot" : SaveFolder);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
        {
            SaveFolder = EditorUtility.SaveFolderPanel("Save screenshots to:", SaveFolder, string.Empty);
            GUI.FocusControl("Browse");
        }
        GUI.enabled &= Directory.Exists(SaveFolder);
        if (GUILayout.Button("Show", GUILayout.ExpandWidth(false)))
        {
            Application.OpenURL("file://" + Path.GetFullPath(SaveFolder));
        }


        EditorGUILayout.EndHorizontal();

        if (string.IsNullOrEmpty(SaveFolder) || SaveFolder.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
        {
            EditorGUILayout.HelpBox("Folder path is empty or contains invalid characters.", MessageType.Error);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }
    /// <summary>
    /// Get Save Folder Path
    /// </summary>
    /// <param name="resolution">resolution </param>
    /// <param name="name">name screenshot</param>
    /// <returns></returns>
    string GetSaveFolderPath(Vector2 resolution, string name)
    {
        string resFolder = "/" + (int)resolution.x + "x" + (int)resolution.y;

        string folderSave = SaveFolder + resFolder + "/"
                + ((needAddToNamePreffix) ? Preffix : "")
                + name
                + ((needAddToNameSuffix) ? Suffix : "")
                + ((needAddToNameResolutions) ? ("_" + (int)resolution.x + "x" + (int)resolution.y) : "")
                + (togglePngOrJPEG ? ".png" : ".jpg");

        string dir = SaveFolder + resFolder;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        return folderSave;
    }
    /// <summary>
    /// Get Save Folder Path
    /// </summary>
    /// <param name="resolution">resolution</param>
    /// <param name="name">name screenshot</param>
    /// <param name="index">file index</param>
    /// <returns></returns>
    string GetSaveFolderPath(Vector2 resolution, string name, int index)
    {
        string resFolder = "/" + (int)resolution.x + "x" + (int)resolution.y;

        string folderSave = SaveFolder + resFolder + "/"
                + ((needAddToNamePreffix) ? Preffix : "")
                + name + "_(" + index.ToString() + ")_"
                + ((needAddToNameSuffix) ? Suffix : "")
                + ((needAddToNameResolutions) ? ("_" + (int)resolution.x + "x" + (int)resolution.y) : "")
                + (togglePngOrJPEG ? ".png" : ".jpg");

        string dir = SaveFolder + resFolder;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        return folderSave;
    }
}
