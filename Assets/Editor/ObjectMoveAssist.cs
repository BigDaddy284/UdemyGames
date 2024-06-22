using UnityEngine;
using UnityEditor;

public class RoundPositionToolbar : EditorWindow
{
    private static bool isRoundingEnabled = false;
    private static bool lockXAxis = false;
    private static bool lockYAxis = false;
    private static bool lockZAxis = false;
    private static string[] roundingOptions = new string[] { "Whole Number", "Tenths", "Hundredths" };
    private static int selectedRoundingOption = 0;

    private static Vector3 lockedPosition;

    [MenuItem("Window/Round Position Toolbar")]
    public static void ShowWindow()
    {
        RoundPositionToolbar window = GetWindow<RoundPositionToolbar>("Round Position Toolbar");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Round Position Settings", EditorStyles.boldLabel);

        isRoundingEnabled = GUILayout.Toggle(isRoundingEnabled, "Enable Rounding");

        GUILayout.Label("Rounding Precision:");
        selectedRoundingOption = EditorGUILayout.Popup(selectedRoundingOption, roundingOptions);

        GUILayout.Space(10);

        GUILayout.Label("Lock Axis While Moving", EditorStyles.boldLabel);
        lockXAxis = GUILayout.Toggle(lockXAxis, "Lock X Axis");
        lockYAxis = GUILayout.Toggle(lockYAxis, "Lock Y Axis");
        lockZAxis = GUILayout.Toggle(lockZAxis, "Lock Z Axis");

        if (Selection.activeTransform != null)
        {
            Transform selectedTransform = Selection.activeTransform;
            if (lockXAxis)
                lockedPosition.x = selectedTransform.position.x;
            if (lockYAxis)
                lockedPosition.y = selectedTransform.position.y;
            if (lockZAxis)
                lockedPosition.z = selectedTransform.position.z;
        }
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        Selection.selectionChanged += OnSelectionChange;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        Selection.selectionChanged -= OnSelectionChange;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (Selection.activeTransform != null)
        {
            Handles.BeginGUI();
            Transform selectedTransform = Selection.activeTransform;
            Vector3 position = selectedTransform.position;

            if (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseMove)
            {
                if (lockXAxis)
                {
                    position.x = lockedPosition.x;
                }
                if (lockYAxis)
                {
                    position.y = lockedPosition.y;
                }
                if (lockZAxis)
                {
                    position.z = lockedPosition.z;
                }

                if (isRoundingEnabled)
                {
                    position = new Vector3(
                        RoundToPrecision(position.x, GetRoundingPrecision()),
                        RoundToPrecision(position.y, GetRoundingPrecision()),
                        RoundToPrecision(position.z, GetRoundingPrecision())
                    );
                }

                selectedTransform.position = position;
            }

            if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp)
            {
                lockedPosition = selectedTransform.position;
            }
            Handles.EndGUI();
        }
    }

    private void OnSelectionChange()
    {
        if (Selection.activeTransform != null)
        {
            Transform selectedTransform = Selection.activeTransform;
            lockedPosition = selectedTransform.position;
        }
    }

    private float RoundToPrecision(float value, float precision)
    {
        return Mathf.Round(value / precision) * precision;
    }

    private float GetRoundingPrecision()
    {
        switch (selectedRoundingOption)
        {
            case 0: // Whole Number
                return 1f;
            case 1: // Tenths
                return 0.1f;
            case 2: // Hundredths
                return 0.01f;
            default:
                return 1f;
        }
    }
}
