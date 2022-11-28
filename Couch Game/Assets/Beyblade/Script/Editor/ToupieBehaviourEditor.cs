using UnityEngine;
using UnityEditor;

// [CustomEditor(typeof(Transform))]
public class ToupieBehaviourEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ToupieBehaviour playerB = (ToupieBehaviour) target;
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("param"));
        
        EditorGUILayout.Space(10);
        playerB.controller = (CharacterController)EditorGUILayout.ObjectField("Character Controller", 
            playerB.controller, typeof(CharacterController), true);
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("GroundCheck",EditorStyles.boldLabel);
        EditorGUILayout.Space(3);

        EditorGUILayout.Space(10);
        playerB.repulseForce = EditorGUILayout.Slider("Repulse Force",playerB.repulseForce, .1f, 10f);
        
        
        serializedObject.ApplyModifiedProperties();
        
        
    }
}
