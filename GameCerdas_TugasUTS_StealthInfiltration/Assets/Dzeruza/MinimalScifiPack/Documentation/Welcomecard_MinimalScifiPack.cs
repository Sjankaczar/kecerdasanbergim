using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

namespace MinimalScifiPack.Editor
{
    [InitializeOnLoad]
    public class WelcomeCardMinimalScifiPack : EditorWindow
    {
        private const string PREF_KEY_DONT_SHOW = "WelcomeCardMinimalScifiPack";
        private const string PREF_KEY_SCENE_VISITED_PREFIX = "MinimalScifiPack_SceneVisited_";

        private static bool initialized = false;
        private static bool dontShowAgain;
        private static Texture2D logo;

        private static readonly string[] requiredScenes =
        {
            "DemoScene_MinimalScifiPack.unity",
        };

        private static readonly HashSet<string> visitedThisSession = new();

        // countdown fields
        private static bool countdownStarted = false;
        private static double countdownStartTime;
        private const double DelaySeconds = 60.0; // 1 minute

        static WelcomeCardMinimalScifiPack()
        {
            EditorApplication.update += DeferredInitialize;
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }

        private static void DeferredInitialize()
        {
            if (initialized) return;
            initialized = true;

            dontShowAgain = EditorPrefs.GetBool(PREF_KEY_DONT_SHOW, false);
            if (!dontShowAgain)
            {
                EditorApplication.update += CheckConditions;
            }

            EditorApplication.update -= DeferredInitialize;
        }

        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            string sceneName = Path.GetFileName(scene.path);

            foreach (string required in requiredScenes)
            {
                if (string.Equals(sceneName, required, System.StringComparison.OrdinalIgnoreCase))
                {
                    visitedThisSession.Add(required);

                    string key = PREF_KEY_SCENE_VISITED_PREFIX + required;
                    if (!EditorPrefs.GetBool(key, false))
                    {
                        EditorPrefs.SetBool(key, true);
                    }

                    
                    countdownStarted = true;
                    countdownStartTime = EditorApplication.timeSinceStartup;

                    break;
                }
            }
        }

        private static void CheckConditions()
        {
            if (dontShowAgain)
            {
                EditorApplication.update -= CheckConditions;
                return;
            }

            if (visitedThisSession.Count >= requiredScenes.Length && countdownStarted)
            {
                if (EditorApplication.timeSinceStartup - countdownStartTime >= DelaySeconds)
                {
                    ShowWindow();
                    EditorApplication.update -= CheckConditions;
                }
            }
        }

        [MenuItem("Tools/MinimalScifiPack/Welcome Card")]
        public static void ShowWindow()
        {
            var window = GetWindow<WelcomeCardMinimalScifiPack>("Welcome");
            window.minSize = new Vector2(360, 400);
            window.Show();
        }

        private void OnEnable()
        {
            dontShowAgain = EditorPrefs.GetBool(PREF_KEY_DONT_SHOW, false);

            if (logo == null)
            {
                logo = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    "Assets/Dzeruza/MinimalScifiPack/Documentation/Logo_MinimalScifiPack.png"
                );
            }
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            if (logo != null)
            {
                float logoWidth = Mathf.Min(position.width - 20, 256);
                float aspect = (float)logo.height / logo.width;

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(logo, GUILayout.Width(logoWidth), GUILayout.Height(logoWidth * aspect));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }

            GUILayout.Label("🚀 Minimal Scifi Pack", EditorStyles.boldLabel);
            GUILayout.Label("Thank you for supporting our work!", EditorStyles.wordWrappedLabel);
            GUILayout.Space(20);

            if (GUILayout.Button("⭐ Leave a Review ⭐", GUILayout.Height(50)))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/3d/environments/minimal-scifi-pack-325328#reviews");
            }

            if (GUILayout.Button("⭐ Get the UltimateMinimalScifiPack ⭐", GUILayout.Height(50)))
            {
                Application.OpenURL("https://u3d.as/3ym6");
            }

            if (GUILayout.Button("📖 More Assets", GUILayout.Height(30)))
            {
                Application.OpenURL("https://assetstore.unity.com/publishers/114715");
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.HelpBox(
                "Your feedback keeps this pack alive. We’d love to hear from you!",
                MessageType.Info
            );

            GUILayout.Space(10);

            EditorGUI.BeginChangeCheck();
            dontShowAgain = EditorGUILayout.ToggleLeft(" Don’t show this window again", dontShowAgain);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(PREF_KEY_DONT_SHOW, dontShowAgain);
            }
        }
    }
}
