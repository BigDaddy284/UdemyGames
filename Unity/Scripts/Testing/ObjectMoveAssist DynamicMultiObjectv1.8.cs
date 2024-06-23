// Unity tool to make moving/aligning objects easier (like your're on a grid)
// Rotation Snap button to remove the need to hold CTRL
// v1.8 unstable, working on scale axis locking for v2.0 

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ObjectMoveAssist : EditorWindow
{
    // State variables for rounding
    private static bool isRoundingEnabled = true;

    // Options and selection index for rounding precision
    private static string[] roundingOptions = new string[] { "Whole Number", "Tenths", "Hundredths" };
    private static int selectedRoundingOption = 0;

    // Dictionary to store initial positions and scales of objects
    private static Dictionary<Transform, Vector3> initialPositions = new Dictionary<Transform, Vector3>();
    private static Dictionary<Transform, Vector3> initialScales = new Dictionary<Transform, Vector3>();

    // Dictionary to store locked scales of objects
    private static Dictionary<Transform, Vector3> lockedScales = new Dictionary<Transform, Vector3>();

    // Dictionary to store axis lock states for each object
    private static Dictionary<Transform, (bool lockX, bool lockY, bool lockZ)> positionLocks = new Dictionary<Transform, (bool, bool, bool)>();
    private static Dictionary<Transform, (bool lockX, bool lockY, bool lockZ)> scaleLocks = new Dictionary<Transform, (bool, bool, bool)>();

    // Menu item to show the window in the Unity Editor
    [MenuItem("Window/Object Move Assist")]
    public static void ShowWindow()
    {
        GetWindow<ObjectMoveAssist>("Object Move Assist");
    }

    private void OnGUI()
    {
        // Start a scroll view
        EditorGUILayout.BeginScrollView(Vector2.zero, GUILayout.Width(position.width), GUILayout.Height(position.height));

        // Section for rounding settings with a tooltip
        GUILayout.Label(new GUIContent("Round Position and Scale Settings", "Settings to enable/disable and configure position and scale rounding"), EditorStyles.boldLabel);

        // Button to toggle rounding on or off
        if (GUILayout.Button(new GUIContent(
            isRoundingEnabled ? "Disable Rounding" : "Enable Rounding",
            "Toggle position and scale rounding on or off")))
        {
            isRoundingEnabled = !isRoundingEnabled;
        }

        // Heading and dropdown to select rounding precision
        GUILayout.Label(new GUIContent("Rounding Precision", "Select the precision for rounding"), EditorStyles.boldLabel);
        selectedRoundingOption = EditorGUILayout.Popup(selectedRoundingOption, roundingOptions);

        GUILayout.Space(10);

        // Section for position axis lock settings with a tooltip
        GUILayout.Label(new GUIContent("Position Axis Lock Settings", "Settings to lock movement on specific axes"), EditorStyles.boldLabel);

        // Buttons to lock/unlock each position axis
        foreach (var selectedTransform in Selection.transforms)
        {
            GUILayout.Label(selectedTransform.name, EditorStyles.boldLabel);

            if (GUILayout.Button(new GUIContent(
                GetPositionLockState(selectedTransform).lockX ? "Unlock X Axis" : "Lock X Axis",
                "Toggle locking movement on the X axis")))
            {
                TogglePositionLock(selectedTransform, "X");
                CaptureCurrentPositions();
            }

            if (GUILayout.Button(new GUIContent(
                GetPositionLockState(selectedTransform).lockY ? "Unlock Y Axis" : "Lock Y Axis",
                "Toggle locking movement on the Y axis")))
            {
                TogglePositionLock(selectedTransform, "Y");
                CaptureCurrentPositions();
            }

            if (GUILayout.Button(new GUIContent(
                GetPositionLockState(selectedTransform).lockZ ? "Unlock Z Axis" : "Lock Z Axis",
                "Toggle locking movement on the Z axis")))
            {
                TogglePositionLock(selectedTransform, "Z");
                CaptureCurrentPositions();
            }

            GUILayout.Space(10);
        }

        // Section for scale axis lock settings with a tooltip
        GUILayout.Label(new GUIContent("Scale Axis Lock Settings", "Settings to lock scaling on specific axes"), EditorStyles.boldLabel);

        foreach (var selectedTransform in Selection.transforms)
        {
            GUILayout.Label(selectedTransform.name, EditorStyles.boldLabel);

            if (GUILayout.Button(new GUIContent(
                GetScaleLockState(selectedTransform).lockX ? "Unlock Scale X Axis" : "Lock Scale X Axis",
                "Toggle locking scaling on the X axis")))
            {
                ToggleScaleLock(selectedTransform, "X");
                CaptureCurrentScales();
            }

            if (GUILayout.Button(new GUIContent(
                GetScaleLockState(selectedTransform).lockY ? "Unlock Scale Y Axis" : "Lock Scale Y Axis",
                "Toggle locking scaling on the Y axis")))
            {
                ToggleScaleLock(selectedTransform, "Y");
                CaptureCurrentScales();
            }

            if (GUILayout.Button(new GUIContent(
                GetScaleLockState(selectedTransform).lockZ ? "Unlock Scale Z Axis" : "Lock Scale Z Axis",
                "Toggle locking scaling on the Z axis")))
            {
                ToggleScaleLock(selectedTransform, "Z");
                CaptureCurrentScales();
            }

            GUILayout.Space(10);
        }

        // Button to reset position
        if (GUILayout.Button(new GUIContent(
            "Reset Position",
            "Reset the position of the selected object(s) to zero or initial position")))
        {
            ResetPosition();
        }

        // Button to reset scale
        if (GUILayout.Button(new GUIContent(
            "Reset Scale",
            "Reset the scale of the selected object(s) to the initial scale")))
        {
            ResetScale();
        }

        // End the scroll view
        EditorGUILayout.EndScrollView();
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
        Selection.selectionChanged += OnSelectionChanged;
    }

    // Unregister update method when the window is disabled
    private void OnDisable()
    {
        EditorApplication.update -= Update;
        Selection.selectionChanged -= OnSelectionChanged;
    }

    // Update method called every frame
    private void Update()
    {
        if (Selection.transforms.Length > 0)
        {
            foreach (var selectedTransform in Selection.transforms)
            {
                ModifyTransformPosition(selectedTransform);
                ModifyTransformScale(selectedTransform);
            }
        }
    }

    // Method to handle selection changes
    private void OnSelectionChanged()
    {
        UpdateInitialPositions();
        UpdateInitialScales();
        ClearLockedScales();
    }

    // Method to update the initial positions dictionary
    private void UpdateInitialPositions()
    {
        foreach (var selectedTransform in Selection.transforms)
        {
            if (!initialPositions.ContainsKey(selectedTransform))
            {
                initialPositions[selectedTransform] = selectedTransform.position;
                positionLocks[selectedTransform] = (false, false, false);
            }
        }

        // Remove any deselected objects from the dictionary
        var keysToRemove = new List<Transform>();
        foreach (var key in initialPositions.Keys)
        {
            if (!System.Array.Exists(Selection.transforms, t => t == key))
            {
                keysToRemove.Add(key);
            }
        }

        foreach (var key in keysToRemove)
        {
            initialPositions.Remove(key);
            positionLocks.Remove(key);
        }
    }

    // Method to update the initial scales dictionary
    private void UpdateInitialScales()
    {
        foreach (var selectedTransform in Selection.transforms)
        {
            if (!initialScales.ContainsKey(selectedTransform))
            {
                initialScales[selectedTransform] = selectedTransform.localScale;
                scaleLocks[selectedTransform] = (false, false, false);
            }
        }

        // Remove any deselected objects from the dictionary
        var keysToRemove = new List<Transform>();
        foreach (var key in initialScales.Keys)
        {
            if (!System.Array.Exists(Selection.transforms, t => t == key))
            {
                keysToRemove.Add(key);
            }
        }

        foreach (var key in keysToRemove)
        {
            initialScales.Remove(key);
            scaleLocks.Remove(key);
        }
    }

    // Method to clear the locked scales dictionary
    private void ClearLockedScales()
    {
        lockedScales.Clear();
    }

    // Method to capture the current positions of selected objects
    private void CaptureCurrentPositions()
    {
        foreach (var selectedTransform in Selection.transforms)
        {
            if (initialPositions.ContainsKey(selectedTransform))
            {
                initialPositions[selectedTransform] = selectedTransform.position;
            }
        }
    }

    // Method to capture the current scales of selected objects
    private void CaptureCurrentScales()
    {
        foreach (var selectedTransform in Selection.transforms)
        {
            if (initialScales.ContainsKey(selectedTransform))
            {
                initialScales[selectedTransform] = selectedTransform.localScale;
            }
        }
    }

    // Method to modify the position of the given transform
    private void ModifyTransformPosition(Transform selectedTransform)
    {
        Vector3 position = selectedTransform.position;

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
        if (initialPositions.TryGetValue(selectedTransform, out Vector3 initialPosition))
        {
            var lockState = GetPositionLockState(selectedTransform);
            if (lockState.lockX)
                position.x = initialPosition.x;
            if (lockState.lockY)
                position.y = initialPosition.y;
            if (lockState.lockZ)
                position.z = initialPosition.z;
        }

        // Update the position of the selected transform
        selectedTransform.position = position;
    }

    // Method to modify the scale of the given transform
    private void ModifyTransformScale(Transform selectedTransform)
    {
        Vector3 scale = selectedTransform.localScale;

        // Apply rounding if enabled
        if (isRoundingEnabled)
        {
            scale = new Vector3(
                RoundToPrecision(scale.x, GetRoundingPrecision()),
                RoundToPrecision(scale.y, GetRoundingPrecision()),
                RoundToPrecision(scale.z, GetRoundingPrecision())
            );
        }

        // Apply scale locks
        if (lockedScales.TryGetValue(selectedTransform, out Vector3 lockedScale))
        {
            var lockState = GetScaleLockState(selectedTransform);
            if (lockState.lockX)
                scale.x = lockedScale.x;
            if (lockState.lockY)
                scale.y = lockedScale.y;
            if (lockState.lockZ)
                scale.z = lockedScale.z;
        }

        // Update the scale of the selected transform
        selectedTransform.localScale = scale;
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

    // Method to reset the position of the selected objects
    private void ResetPosition()
    {
        foreach (var selectedTransform in Selection.transforms)
        {
            if (initialPositions.TryGetValue(selectedTransform, out Vector3 initialPosition))
            {
                selectedTransform.position = initialPosition;
            }
        }
    }

    // Method to reset the scale of the selected objects
    private void ResetScale()
    {
        foreach (var selectedTransform in Selection.transforms)
        {
            if (initialScales.TryGetValue(selectedTransform, out Vector3 initialScale))
            {
                selectedTransform.localScale = initialScale;
            }
        }
    }

    // Method to get the lock state for position of a specific transform
    private (bool lockX, bool lockY, bool lockZ) GetPositionLockState(Transform transform)
    {
        if (positionLocks.TryGetValue(transform, out var state))
        {
            return state;
        }
        return (false, false, false);
    }

    // Method to get the lock state for scale of a specific transform
    private (bool lockX, bool lockY, bool lockZ) GetScaleLockState(Transform transform)
    {
        if (scaleLocks.TryGetValue(transform, out var state))
        {
            return state;
        }
        return (false, false, false);
    }

    // Method to toggle position lock state for a specific axis
    private void TogglePositionLock(Transform transform, string axis)
    {
        var state = GetPositionLockState(transform);
        switch (axis)
        {
            case "X":
                state.lockX = !state.lockX;
                break;
            case "Y":
                state.lockY = !state.lockY;
                break;
            case "Z":
                state.lockZ = !state.lockZ;
                break;
        }
        positionLocks[transform] = state;
    }

    // Method to toggle scale lock state for a specific axis
    private void ToggleScaleLock(Transform transform, string axis)
    {
        var state = GetScaleLockState(transform);
        switch (axis)
        {
            case "X":
                state.lockX = !state.lockX;
                break;
            case "Y":
                state.lockY = !state.lockY;
                break;
            case "Z":
                state.lockZ = !state.lockZ;
                break;
        }
        scaleLocks[transform] = state;
    }
}
