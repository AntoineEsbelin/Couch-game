using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class ToolboxWindow : EditorWindow
{
    [MenuItem("Tools/Custom Tool Box")]
    static void Initbox()
    {
        ToolboxWindow newWindow = GetWindow<ToolboxWindow>();
        
        newWindow.titleContent = new GUIContent("Tool Box");

        newWindow.Show();
    }


    private string aled = "Assets/Scenes/";
    private string entitiesPath = "Script/BattleRelated/Entity/";
    private Vector2 scrollPos;
    private void OnGUI()
    {
        if(!EditorApplication.isPlaying)
        {
            //Scenes TP
            aled = EditorGUILayout.TextField("Path mark last / in the end", aled);
            
            if(System.IO.Directory.Exists(aled))
            {
                scrollPos = GUILayout.BeginScrollView(scrollPos,true, false);
                GUILayout.BeginHorizontal();
                GUILayout.Label("GO TO :");

                string[] allScene = Directory.GetFiles(aled, "*.unity");
                if(allScene.Length > 0)
                {

                    foreach(string file in allScene)
                    {
                        //Debug.Log(Path.GetFileName(file));
                        string sceneName = Path.GetFileName(file);
                        
                        Scene scene;
                        if(!EditorSceneManager.GetSceneByPath(aled + sceneName).isSubScene && !EditorSceneManager.GetSceneByPath(aled + sceneName).IsValid())
                        {
                            //Debug.Log(removedExtension);
                            scene = EditorSceneManager.GetSceneByName(NameWithoutExt(sceneName));
                        }
                        else
                        {
                            if(SceneManager.sceneCount < allScene.Length)scene = EditorSceneManager.OpenScene(aled + sceneName, OpenSceneMode.AdditiveWithoutLoading);
                            else scene = EditorSceneManager.GetSceneByName(NameWithoutExt(sceneName));
                            //Debug.Log(scene.name);
                        }
                        //Debug.Log(sceneName);
                        if(GUILayout.Button(scene.name))
                        {
                            if(EditorSceneManager.GetActiveScene() != EditorSceneManager.GetSceneByName(sceneName))
                            {
                                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                                EditorSceneManager.OpenScene(aled + sceneName, OpenSceneMode.Single);
                            }
                            else Debug.LogError($"{SceneManager.GetActiveScene().name} SCENE ALREADY OPEN");
                        }

                        
                    }
                    GUILayout.EndHorizontal();
                    
                }
                else
                {
                    GUILayout.EndHorizontal();
                    EditorGUILayout.HelpBox($"No scenes currently in path {aled}", MessageType.Warning);
                }
                GUILayout.EndScrollView();

            }
            else
            {
                EditorGUILayout.HelpBox($"No path existing in path {aled}", MessageType.Warning);
            }
        }
    }

    private string NameWithoutExt(string sceneName)
    {
        string copy = sceneName;
        string removedExtension = copy.Replace(".unity", "");
        return removedExtension;
    }
}
