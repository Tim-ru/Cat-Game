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

        var actionType = property.FindPropertyRelative("_actionType");
        Actions type = (Actions)actionType.enumValueIndex;

        switch (type)
        {
            case Actions.MoveCamera:
                DrawProperty(ref rect, property.FindPropertyRelative("_duration"));
                DrawProperty(ref rect, property.FindPropertyRelative("_pauseTime"));
                DrawProperty(ref rect, property.FindPropertyRelative("_targetCameraPosition"));
                //DrawProperty(ref rect, property.FindPropertyRelative("_speed"));
                break;

            case Actions.ResizeCamera:
                DrawProperty(ref rect, property.FindPropertyRelative("_unfollowTargetDuringScene"));
                DrawProperty(ref rect, property.FindPropertyRelative("_duration"));
                DrawProperty(ref rect, property.FindPropertyRelative("_pauseTime"));
                DrawProperty(ref rect, property.FindPropertyRelative("_targetCameraSize"));
                //DrawProperty(ref rect, property.FindPropertyRelative("_speed"));
                break;

            case Actions.ShakeCamera:
                DrawProperty(ref rect, property.FindPropertyRelative("_duration"));
                DrawProperty(ref rect, property.FindPropertyRelative("_pauseTime"));
                DrawProperty(ref rect, property.FindPropertyRelative("_shakeStrength"));
                break;

            case Actions.MovePlayer:
                DrawProperty(ref rect, property.FindPropertyRelative("_duration"));
                DrawProperty(ref rect, property.FindPropertyRelative("_pauseTime"));
                DrawProperty(ref rect, property.FindPropertyRelative("_playerMovementVector"));
                break;

            case Actions.AddVelocityToPlayer:
                DrawProperty(ref rect, property.FindPropertyRelative("_duration"));
                DrawProperty(ref rect, property.FindPropertyRelative("_pauseTime"));
                DrawProperty(ref rect, property.FindPropertyRelative("_playerMovementVector"));
                break;

            case Actions.JumpPlayer:
                DrawProperty(ref rect, property.FindPropertyRelative("_pauseTime"));
                break;

            case Actions.FollowNewObject:
                DrawProperty(ref rect, property.FindPropertyRelative("_duration"));
                DrawProperty(ref rect, property.FindPropertyRelative("_pauseTime"));
                DrawProperty(ref rect, property.FindPropertyRelative("_followingObject"));
                DrawProperty(ref rect, property.FindPropertyRelative("_followingOffset"));
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

        var actionType = property.FindPropertyRelative("_actionType");
        Actions type = (Actions)actionType.enumValueIndex;

        switch (type)
        {
            case Actions.MoveCamera:
                totalHeight += GetHeight(property.FindPropertyRelative("_duration"));
                totalHeight += GetHeight(property.FindPropertyRelative("_pauseTime"));
                totalHeight += GetHeight(property.FindPropertyRelative("_targetCameraPosition"));
                //totalHeight += GetHeight(property.FindPropertyRelative("_speed"));
                break;

            case Actions.ResizeCamera:
                totalHeight += GetHeight(property.FindPropertyRelative("_unfollowTargetDuringScene"));
                totalHeight += GetHeight(property.FindPropertyRelative("_duration"));
                totalHeight += GetHeight(property.FindPropertyRelative("_pauseTime"));
                totalHeight += GetHeight(property.FindPropertyRelative("_targetCameraSize"));
                break;

            case Actions.ShakeCamera:
                totalHeight += GetHeight(property.FindPropertyRelative("_duration"));
                totalHeight += GetHeight(property.FindPropertyRelative("_pauseTime"));
                totalHeight += GetHeight(property.FindPropertyRelative("_shakeStrength"));
                break;

            case Actions.MovePlayer:
                totalHeight += GetHeight(property.FindPropertyRelative("_duration"));
                totalHeight += GetHeight(property.FindPropertyRelative("_pauseTime"));
                totalHeight += GetHeight(property.FindPropertyRelative("_playerMovementVector"));
                break;

            case Actions.AddVelocityToPlayer:
                totalHeight += GetHeight(property.FindPropertyRelative("_duration"));
                totalHeight += GetHeight(property.FindPropertyRelative("_pauseTime"));
                totalHeight += GetHeight(property.FindPropertyRelative("_playerMovementVector"));
                break;

            case Actions.JumpPlayer:
                totalHeight += GetHeight(property.FindPropertyRelative("_pauseTime"));
                break;

            case Actions.FollowNewObject:
                totalHeight += GetHeight(property.FindPropertyRelative("_pauseTime"));
                totalHeight += GetHeight(property.FindPropertyRelative("_duration"));
                totalHeight += GetHeight(property.FindPropertyRelative("_followingObject"));
                totalHeight += GetHeight(property.FindPropertyRelative("_followingOffset"));
                break;
        }

        return totalHeight;
    }

    private float GetHeight(SerializedProperty property)
    {
        return EditorGUI.GetPropertyHeight(property, true) + Spacing;
    }
}