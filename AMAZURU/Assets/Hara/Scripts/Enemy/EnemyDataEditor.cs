#if UNITY_EDITOR
using Enemy;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyData))]
public class EnemyDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EnemyData enemyData = target as EnemyData;

        EditorGUI.BeginChangeCheck();

        // 共通の設定項目
        enemyData.Type = (EnemyData.EnemyType)EditorGUILayout.EnumPopup("敵の種類", enemyData.Type);
        enemyData.StartRotate = (EnemyData.RotateDirection)EditorGUILayout.EnumPopup("スタート時の向き", enemyData.StartRotate);
        enemyData.Size = Mathf.Max(1.0f, EditorGUILayout.FloatField("サイズ倍率", Mathf.Min(5.0f, enemyData.Size)));
        SerializedProperty property = serializedObject.FindProperty("MovePlan");
        EditorGUILayout.PropertyField(property, true);
        enemyData.MoveType = (EnemyMoveType)EditorGUILayout.EnumPopup("巡回方式", enemyData.MoveType);
        enemyData.NomalSpeed = Mathf.Max(1, EditorGUILayout.FloatField("通常時の移動速度", Mathf.Min(20.0f, enemyData.NomalSpeed)));
        enemyData.WaterSpeed = Mathf.Max(1, EditorGUILayout.FloatField("水中時の移動速度", Mathf.Min(20.0f, enemyData.WaterSpeed)));

        // 個別の設定項目
        if (enemyData.Type != EnemyData.EnemyType.Nomal)
        {
            GUIStyle myStyle = new GUIStyle();
            myStyle.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField("-----以下、特殊設定-----", myStyle);
            if (enemyData.Type == EnemyData.EnemyType.Dry)
            {
                enemyData.BlockSetPosY = Mathf.Max(0, EditorGUILayout.FloatField("ブロックの設置位置", enemyData.BlockSetPosY));
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(enemyData);
        }
    }
}
#endif
