using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CutsceneActionsItem))]
public class CusteneActionsItemsDrawer : PropertyDrawer
{
    private const float Spacing = 2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect rect = new(position.x, position.y, position.width, 0);

        DrawProperty(ref rect, property.FindPropertyRelative("_actionType"));
        DrawProperty(ref rect, property.FindPropertyRelative("_duration"));

        var actionType = property.FindPropertyRelative("_actionType");
        Actions type = (Actions)actionType.enumValueIndex;

        switch (type)
        {
            case Actions.MoveCamera:
                DrawProperty(ref rect, property.FindPropertyRelative("_targetCameraPosition"));
                //DrawProperty(ref rect, property.FindPropertyRelative("_speed"));
                break;

            case Actions.ResizeCamera:
                DrawProperty(ref rect, property.FindPropertyRelative("_targetCameraSize"));
                //DrawProperty(ref rect, property.FindPropertyRelative("_speed"));
                break;

            case Actions.ShakeCamera:
                DrawProperty(ref rect, property.FindPropertyRelative("_pauseTime"));
                DrawProperty(ref rect, property.FindPropertyRelative("_shakeStrength"));
                break;
        }

        EditorGUI.EndProperty();
    }

    private void DrawProperty(ref Rect rect, SerializedProperty property)
    {
        float height = EditorGUI.GetPropertyHeight(property, true);
        rect.height = height;

        EditorGUI.PropertyField(rect, property, true);

        rect.y += height + Spacing;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float totalHeight = 0f;

        totalHeight += GetHeight(property.FindPropertyRelative("_actionType"));
        totalHeight += GetHeight(property.FindPropertyRelative("_duration"));

        var actionType = property.FindPropertyRelative("_actionType");
        Actions type = (Actions)actionType.enumValueIndex;

        switch (type)
        {
            case Actions.MoveCamera:
                totalHeight += GetHeight(property.FindPropertyRelative("_targetCameraPosition"));
                //totalHeight += GetHeight(property.FindPropertyRelative("_speed"));
                break;

            case Actions.ResizeCamera:
                totalHeight += GetHeight(property.FindPropertyRelative("_targetCameraSize"));
                break;

            case Actions.ShakeCamera:
                totalHeight += GetHeight(property.FindPropertyRelative("_pauseTime"));
                totalHeight += GetHeight(property.FindPropertyRelative("_shakeStrength"));
                break;
        }

        return totalHeight;
    }

    private float GetHeight(SerializedProperty property)
    {
        return EditorGUI.GetPropertyHeight(property, true) + Spacing;
    }
}