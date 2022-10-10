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

        switch (playerB.param)
        {
            case ToupieBehaviour.StateParam.MOVE:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("moveParam"));
                break;
            
            case ToupieBehaviour.StateParam.CHARGE:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("chargeParam"));
                break;
        }
        
        EditorGUILayout.Space(10);
        playerB.controller = (CharacterController)EditorGUILayout.ObjectField("Character Controller", 
            playerB.controller, typeof(CharacterController), true);
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("GroundCheck",EditorStyles.boldLabel);
        EditorGUILayout.Space(3);
        playerB.groundCheck = (Transform)EditorGUILayout.ObjectField("Character Controller", 
            playerB.groundCheck, typeof(Transform), true);
        
        playerB.groundDistance = EditorGUILayout.Slider("GroundCheck Distance",playerB.groundDistance, 0f, 1f);
        
        
        playerB.groundMask = EditorGUILayout.LayerField("Ground Layer", playerB.groundMask);
        
        EditorGUILayout.Space(10);
        playerB.repulseForce = EditorGUILayout.Slider("Repulse Force",playerB.repulseForce, .1f, 10f);
        
        
        serializedObject.ApplyModifiedProperties();
        
        
    }
}
