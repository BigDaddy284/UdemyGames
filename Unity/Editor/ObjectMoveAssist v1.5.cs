// Unity tool to make moving/aligning objects easier (like your're on a grid)
// Rotation Snap button to remove the need to hold CTRL
// v1.5 working on multi object slection as I work towards v2.0 

    //BUGS
        //If an axis lock is enabled before selecting an object, it has no affect. unlock and relock axis while object is slected.
        //Same issue as above for multipole object selection. Select all objects first, then enable the axis lock

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq; // Add this namespace for LINQ queries

public class ObjectMoveAssist : EditorWindow
{
    // State variables for rounding and axis locking
    private bool isRoundingEnabled = true;
    private bool lockXAxis = false;
    private bool lockYAxis = false;
    private bool lockZAxis = false;

    // Options and selection index for rounding precision
    private static string[] roundingOptions = new string[] { "Whole Number", "Tenths", "Hundredths" };
    private int selectedRoundingOption = 0;

    // Dictionary to store initial positions for each selected transform
    private Dictionary<Transform, Vector3> initialPositions = new Dictionary<Transform, Vector3>();

    // Dictionary to store initial locked axis positions
    private Dictionary<Transform, Vector3> initialLockedPositions = new Dictionary<Transform, Vector3>();

    // Menu item to show the window in the Unity Editor
    [MenuItem("Window/Object Move Assist")]
    public static void ShowWindow()
    {
        GetWindow<ObjectMoveAssist>("Object Move Assist");
    }

    private void OnGUI()
    {
        // Section for rounding settings with a tooltip
        GUILayout.Label(new GUIContent("Position and Scale Rounding", "Settings to enable/disable and configure position and scale rounding"), EditorStyles.boldLabel);

        // Button to toggle rounding on or off for both position and scale
        if (GUILayout.Button(new GUIContent(
            isRoundingEnabled ? "Disable Rounding" : "Enable Rounding",
            "Toggle position and scale rounding on or off")))
        {
            isRoundingEnabled = !isRoundingEnabled;
            EditorPrefs.SetBool("IsRoundingEnabled", isRoundingEnabled);
        }

        // Heading and dropdown to select rounding precision
        GUILayout.Label(new GUIContent("Rounding Precision", "Select the precision for rounding"), EditorStyles.boldLabel);
        selectedRoundingOption = EditorGUILayout.Popup(selectedRoundingOption, roundingOptions);
        EditorPrefs.SetInt("SelectedRoundingOption", selectedRoundingOption);

        GUILayout.Space(10);

        // Section for axis lock settings with a tooltip
        GUILayout.Label(new GUIContent("Axis Locks", "Settings to lock movement on specific axes"), EditorStyles.boldLabel);

        // Buttons to lock/unlock each axis
        if (GUILayout.Button(new GUIContent(
            lockXAxis ? "Unlock X Axis" : "Lock X Axis",
            "Toggle locking movement on the X axis")))
        {
            ToggleAxisLock("X");
        }

        if (GUILayout.Button(new GUIContent(
            lockYAxis ? "Unlock Y Axis" : "Lock Y Axis",
            "Toggle locking movement on the Y axis")))
        {
            ToggleAxisLock("Y");
        }

        if (GUILayout.Button(new GUIContent(
            lockZAxis ? "Unlock Z Axis" : "Lock Z Axis",
            "Toggle locking movement on the Z axis")))
        {
            ToggleAxisLock("Z");
        }

        GUILayout.Space(10);

        // Button to reset position to initial locked position
        if (GUILayout.Button(new GUIContent(
            "Reset Position",
            "Reset position to initial locked position")))
        {
            ResetPosition();
        }
    }

    // Menu item to toggle position and scaling rounding with a keyboard shortcut
    [MenuItem("Tools/Toggle Position and Scaling Rounding %#r")]
    private static void TogglePositionAndScalingRounding()
    {
        bool isRoundingEnabled = EditorPrefs.GetBool("IsRoundingEnabled", true);
        EditorPrefs.SetBool("IsRoundingEnabled", !isRoundingEnabled);
    }

    // Register update method when the window is enabled
    private void OnEnable()
    {
        isRoundingEnabled = EditorPrefs.GetBool("IsRoundingEnabled", true);
        selectedRoundingOption = EditorPrefs.GetInt("SelectedRoundingOption", 0);
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
        // Update positions and apply rounding if object is selected
        foreach (var transform in Selection.transforms)
        {
            Vector3 position = transform.position;
            Vector3 scale = transform.localScale;

            // Store initial position if not already stored
            if (!initialPositions.ContainsKey(transform))
            {
                initialPositions[transform] = position;
            }

            // Apply rounding if enabled
            if (isRoundingEnabled)
            {
                position = new Vector3(
                    RoundToPrecision(position.x, GetRoundingPrecision()),
                    RoundToPrecision(position.y, GetRoundingPrecision()),
                    RoundToPrecision(position.z, GetRoundingPrecision())
                );

                scale = new Vector3(
                    RoundToPrecision(scale.x, GetRoundingPrecision()),
                    RoundToPrecision(scale.y, GetRoundingPrecision()),
                    RoundToPrecision(scale.z, GetRoundingPrecision())
                );
            }

            // Apply axis locks
            if (lockXAxis)
                position.x = GetInitialLockedPosition(transform, "X").x;
            if (lockYAxis)
                position.y = GetInitialLockedPosition(transform, "Y").y;
            if (lockZAxis)
                position.z = GetInitialLockedPosition(transform, "Z").z;

            // Update the position and scale of the selected transform
            transform.position = position;
            transform.localScale = scale;
        }

        // Clear initial positions for deselected objects
        foreach (var transform in initialPositions.Keys.ToList())
        {
            if (!Selection.transforms.Contains(transform))
            {
                initialPositions.Remove(transform);
                initialLockedPositions.Remove(transform); // Remove locked axis info when deselecting
            }
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

    // Method to toggle axis lock for X, Y, or Z axis
    private void ToggleAxisLock(string axis)
    {
        switch (axis)
        {
            case "X":
                lockXAxis = !lockXAxis;
                UpdateInitialLockedPosition(axis);
                break;
            case "Y":
                lockYAxis = !lockYAxis;
                UpdateInitialLockedPosition(axis);
                break;
            case "Z":
                lockZAxis = !lockZAxis;
                UpdateInitialLockedPosition(axis);
                break;
        }
    }

    // Method to update initial locked position based on current position
    private void UpdateInitialLockedPosition(string axis)
    {
        foreach (var transform in Selection.transforms)
        {
            if (axis == "X" && lockXAxis)
            {
                initialLockedPositions[transform] = new Vector3(transform.position.x, initialLockedPositions.ContainsKey(transform) ? initialLockedPositions[transform].y : transform.position.y, initialLockedPositions.ContainsKey(transform) ? initialLockedPositions[transform].z : transform.position.z);
            }
            else if (axis == "Y" && lockYAxis)
            {
                initialLockedPositions[transform] = new Vector3(initialLockedPositions.ContainsKey(transform) ? initialLockedPositions[transform].x : transform.position.x, transform.position.y, initialLockedPositions.ContainsKey(transform) ? initialLockedPositions[transform].z : transform.position.z);
            }
            else if (axis == "Z" && lockZAxis)
            {
                initialLockedPositions[transform] = new Vector3(initialLockedPositions.ContainsKey(transform) ? initialLockedPositions[transform].x : transform.position.x, initialLockedPositions.ContainsKey(transform) ? initialLockedPositions[transform].y : transform.position.y, transform.position.z);
            }
        }
    }

    // Method to get the initial locked position based on axis
    private Vector3 GetInitialLockedPosition(Transform transform, string axis)
    {
        if (initialLockedPositions.ContainsKey(transform))
        {
            switch (axis)
            {
                case "X":
                    return new Vector3(initialLockedPositions[transform].x, transform.position.y, transform.position.z);
                case "Y":
                    return new Vector3(transform.position.x, initialLockedPositions[transform].y, transform.position.z);
                case "Z":
                    return new Vector3(transform.position.x, transform.position.y, initialLockedPositions[transform].z);
                default:
                    return transform.position;
            }
        }
        else
        {
            return transform.position;
        }
    }

    // Method to reset position to initial locked position for selected objects
    private void ResetPosition()
    {
        foreach (var transform in Selection.transforms.ToList())
        {
            if (initialPositions.ContainsKey(transform))
            {
                transform.position = initialPositions[transform];
            }
        }
    }
}
