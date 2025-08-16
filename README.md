# CCC - Communal Common Code

A Unity package that aggregates reusable code from various Unity projects, providing essential utilities, systems, and extensions for game development.

## Features

### Core Systems
- **[Input Management](Runtime/InputManager.cs)**: Device type detection with automatic sprite asset switching for UI controls (Xbox, PlayStation, Keyboard)
- **[Haptic Feedback](Runtime/Haptics.cs)**: Advanced controller vibration system with multi-effect support and intensity control
- **[Scene Loading](Runtime/SceneLoading/)**: Flexible scene management with different scene types (Dynamic, Constant, ConstantReload) and transition effects
  - [SceneLoader](Runtime/SceneLoading/SceneLoader.cs) - Main scene management class
  - [SceneEntry](Runtime/SceneLoading/SceneEntry.cs) - Scene configuration data
  - [SceneType](Runtime/SceneLoading/SceneType.cs) - Scene behavior types
  - [SceneMask](Runtime/SceneLoading/SceneMask.cs) - Scene type filtering
- **[Inactivity Timer](Runtime/InactivityTimer.cs)**: Configurable user activity monitoring with automatic timeout actions

### UI Components
- **[UI Group](Runtime/UIGroup.cs)**: Convenient wrapper for CanvasGroup operations with opacity and offset control
- **[Device Controls Listener](Runtime/DeviceControlsListener.cs)**: Automatic UI control icon updates based on active input device

### Utility Extensions
- **[MonoBehaviour Extensions](Runtime/MonoBehaviourExt.cs)**: Cached component access for improved performance
- **Vector Utilities**: Mathematical operations for Vector2/3 and Vector2Int/3Int including projections, rotations, and Manhattan (L₁) distance calculations
  - [Vector2Utils](Runtime/Utils/Vector2Utils.cs) - 2D vector operations and rotations
  - [Vector2IntUtils](Runtime/Utils/Vector2IntUtils.cs) - 2D integer vector operations
  - [Vector3Utils](Runtime/Utils/Vector3Utils.cs) - 3D vector projections
  - [Vector3IntUtils](Runtime/Utils/Vector3IntUtils.cs) - 3D integer vector operations
- **[UniTask Utilities](Runtime/Utils/UniTaskUtils.cs)**: Async operation helpers including delays, interpolation, and conditional execution
- **[Enum Utilities](Runtime/Utils/EnumUtils.cs)**: Generic enum operations for counting and enumeration
- **[Gizmos Extensions](Runtime/GizmosExt.cs)**: Additional debug drawing utilities for scene visualization

## Installation

This package can be installed via Unity Package Manager using Git URL:

1. Open Unity Package Manager (Window → Package Manager)
2. Click the "+" button and select "Add package from git URL"
3. Enter: `https://github.com/ereldebel/CCC.git`

For more detailed instructions, see the [Unity Documentation](https://docs.unity3d.com/Manual/upm-ui-giturl.html).

## Requirements

- Unity 2021.3 or later
- UniTask package (for async utilities)
- TextMeshPro (for UI components)

## Usage Examples

### Input Management
```csharp
// Access the singleton InputManager
var deviceType = InputManager.Instance.CurrentDeviceType;

// Listen for device changes
InputManager.Instance.DeviceTypeChanged += OnDeviceChanged;

// Control haptic feedback
InputManager.Instance.Haptics.StartHaptics(new Haptics.HapticsReference(0.5f, 0.3f, 2f));
```

### Scene Loading
```csharp
// Switch to a specific scene with transitions
await sceneLoader.SwitchScene(sceneIndex);

// Load scenes additively
sceneLoader.LoadScene(sceneEntry);

// Get active scenes of a specific type
var dynamicScenes = sceneLoader.GetActiveScenes(SceneType.Dynamic);
```

### Vector Utilities
```csharp
// Project 3D vector to XZ plane
Vector3 projected = myVector3.ToVector3XZ();

// Rotate 2D vector
Vector2 rotated = myVector2.Rotate(45f);

// Calculate Manhattan distance
int distance = vectorA.L1Distance(vectorB);
```

### UniTask Utilities
```csharp
// Delay with seconds
await UniTaskUtils.Delay(2.5f);

// Interpolate over time
await UniTaskUtils.Interpolate(t => transform.position = Vector3.Lerp(start, end, t), 1f);
```

## License

This project is licensed under the MIT License - see the LICENSE.md file for details.

## Contributing

This package aggregates useful code patterns from Unity projects. Contributions are welcome for additional utilities that follow the established patterns and documentation standards.
