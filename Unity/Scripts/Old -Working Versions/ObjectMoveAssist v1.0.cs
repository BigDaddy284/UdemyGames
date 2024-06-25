// Unity tool to make moving/aligning objects easier 
// v1.0 only works with single object selcetion , working on multi object slection for v2.0 

using UnityEngine;
using UnityEditor;

public class ObjectMoveAssist : EditorWindow
{
    // State variables for rounding and axis locking
    private static bool isRoundingEnabled = true;
    private static bool lockXAxis = false;
    private static bool lockYAxis = false;
    private static bool lockZAxis = false;

    // Options and selection index for rounding precision
    private static string[] roundingOptions = new string[] { "Whole Number", "Tenths", "Hundredths" };
    private static int selectedRoundingOption = 0;

    // Initial position to store the locked axis positions
    private static Vector3 initialPosition;

    // Menu item to show the window in the Unity Editor
    [MenuItem("Window/Object Move Assist")]
    public static void ShowWindow()
    {
        GetWindow<ObjectMoveAssist>("Object Move Assist");
    }

    private void OnGUI()
    {
        // Section for rounding settings with a tooltip
        GUILayout.Label(new GUIContent("Round Position Settings", "Settings to enable/disable and configure position rounding"), EditorStyles.boldLabel);

        // Button to toggle rounding on or off
        if (GUILayout.Button(new GUIContent(
            isRoundingEnabled ? "Disable Rounding" : "Enable Rounding",
            "Toggle position rounding on or off")))
        {
            isRoundingEnabled = !isRoundingEnabled;
        }

        // Heading and dropdown to select rounding precision
        GUILayout.Label(new GUIContent("Rounding Precision", "Select the precision for rounding"), EditorStyles.boldLabel);
        selectedRoundingOption = EditorGUILayout.Popup(selectedRoundingOption, roundingOptions);

        GUILayout.Space(10);

        // Section for axis lock settings with a tooltip
        GUILayout.Label(new GUIContent("Axis Lock Settings", "Settings to lock movement on specific axes"), EditorStyles.boldLabel);

        // Buttons to lock/unlock each axis
        if (GUILayout.Button(new GUIContent(
            lockXAxis ? "Unlock X Axis" : "Lock X Axis",
            "Toggle locking movement on the X axis")))
        {
            lockXAxis = !lockXAxis;
        }

        if (GUILayout.Button(new GUIContent(
            lockYAxis ? "Unlock Y Axis" : "Lock Y Axis",
            "Toggle locking movement on the Y axis")))
        {
            lockYAxis = !lockYAxis;
        }

        if (GUILayout.Button(new GUIContent(
            lockZAxis ? "Unlock Z Axis" : "Lock Z Axis",
            "Toggle locking movement on the Z axis")))
        {
            lockZAxis = !lockZAxis;
        }
    }

    // Menu item to toggle position rounding with a keyboard shortcut
    [MenuItem("Tools/Toggle Position Rounding %#r")]
    private static void TogglePositionRounding()
    {
        isRoundingEnabled = !isRoundingEnabled;
    }

    // Register update method when the window is enabled
    private void OnEnable()
    {
        EditorApplication.update += Update;
    }

    // Unregister update method when the window is disabled
    private void OnDisable()
    {
        EditorApplication.update -= Update;
    }

    // Update method called every frame
    private void Update()
    {
        if (Selection.activeTransform != null)
        {
            Transform selectedTransform = Selection.activeTransform;
            Vector3 position = selectedTransform.position;

            // Check if any axis is locked and store the initial position if needed
            if (lockXAxis || lockYAxis || lockZAxis)
            {
                if (initialPosition == Vector3.zero)
                {
                    initialPosition = position;
                }
            }
            else
            {
                initialPosition = Vector3.zero;
            }

            // Apply rounding if enabled
            if (isRoundingEnabled)
            {
                position = new Vector3(
                    RoundToPrecision(position.x, GetRoundingPrecision()),
                    RoundToPrecision(position.y, GetRoundingPrecision()),
                    RoundToPrecision(position.z, GetRoundingPrecision())
                );
            }

            // Apply axis locks
            if (lockXAxis)
                position.x = initialPosition.x;
            if (lockYAxis)
                position.y = initialPosition.y;
            if (lockZAxis)
                position.z = initialPosition.z;

            // Update the position of the selected transform
            selectedTransform.position = position;
        }
    }

    // Method to round a value to the specified precision
    private float RoundToPrecision(float value, float precision)
    {
        return Mathf.Round(value / precision) * precision;
    }

    // Method to get the rounding precision based on the selected option
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