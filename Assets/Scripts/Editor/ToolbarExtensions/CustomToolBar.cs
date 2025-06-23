using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

namespace Editor
{
    [InitializeOnLoad]
    public static class CustomToolBar
    {
        static CustomToolBar()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        }

        private static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            
            // Run Official Button
            if (GUILayout.Button(new GUIContent("▶ Run Official", "Run the official Bootstrap scene"),
                    EditorStyles.toolbarButton))
            {
                RunOfficial();
            }

            GUILayout.EndHorizontal();
        }

        private static void OpenScene(string scenePath, bool closeLevelEditor)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            if (closeLevelEditor && LevelEditorWindow.Instance != null)
            {
                LevelEditorWindow.Instance.Close();
            }

            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }
        

        private static void RunOfficial()
        {
            OpenScene("Assets/_Game/Scenes/Bootstrap.unity", true);
            EditorApplication.isPlaying = true;
        }
    }

    public class LevelEditorWindow : EditorWindow
    {
        private static LevelEditorWindow _instance;

        public static LevelEditorWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (LevelEditorWindow)GetWindow(typeof(LevelEditorWindow));
                }

                return _instance;
            }
        }
    }
}
