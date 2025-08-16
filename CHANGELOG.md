# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

<!-- ## [MAJOR.MINOR.PATCH] - yyyy-mm-dd

### Added
- 

### Fixed
- 

### Changed
- 

### Removed
-  -->

## [3.1.0] - 2025-08-16

### Added
- Comprehensive XML documentation for all classes and public members across the entire codebase
- Tooltips for all serialized fields to improve Unity Inspector user experience
- Complete test suite using Unity Test Framework with extensive coverage:
  - Vector2Utils, Vector2IntUtils, Vector3Utils, Vector3IntUtils tests
  - EnumUtils tests with multiple test scenarios
  - SceneMask tests covering all bitwise operations and enumeration
  - MonoBehaviourExt tests for cached component access
  - InputManager.DeviceType enum validation tests
  - HapticsTests for HapticsReference behavior and timed vs infinite haptic logic
  - InactivityTimerTests for component initialization and API validation
  - UIGroupTests for Opacity and Offset functionality with Unity components
  - DeviceControlsListenerTests for component dependency verification
  - SceneLoading namespace tests (SceneEntryTests, SceneTypeTests) for configuration validation
  - UniTaskUtilsTests for async utility method validation
- Enhanced README.md with comprehensive feature documentation, installation instructions, and usage examples
- File links in README.md for easy navigation to source code

### Fixed
- SceneMask now properly implements IEnumerable<SceneType> interface with both generic and non-generic GetEnumerator methods
- InactivityTimer implementation simplified using UniTask.Delay for more efficient and reliable operation

### Changed
- Enhanced code documentation following C# XML documentation standards

## [3.0.0] - 2025-08-02

### Added
- InputManager: Complete input management system that can detect controller disconnection and device type changes
- DeviceControlsListener: Listener for device control changes and controller type updates
- Haptics: Comprehensive haptic feedback system with intensity control and reset functionality
- InactivityTimer: Timer system that triggers actions after periods of inactivity
- MonoBehaviourExt: Extended MonoBehaviour with cached transform and gameObject references
- UIGroup: Canvas group wrapper with clear API for moving UI groups
- SceneLoading namespace with improved scene management:
  - SceneEntry: Individual scene entry management
  - SceneMask: Scene masking functionality
  - SceneType: Enhanced scene type system
- UniTaskUtils: Utility methods for UniTask operations including delay and interpolation methods
- Enhanced SceneLoader with options for default entry/exit transitions and minimum load time

### Fixed
- SceneMask enumerator bug: Fixed incorrect bit shifting logic in GetEnumerator() method that was causing incorrect scene type enumeration

### Changed
- Refactored SceneLoader into SceneLoading namespace for better organization
- Changed SceneLoader to not load loadOnStart scenes if there are already loaded scenes when it is loaded
- Replaced Coroutine system with UniTask-based implementation for better performance and cancellation support

### Removed
- Coroutine.cs: Replaced with UniTask-based UniTaskUtils.cs


## [2.0.0] - 2023-05-22

### Added

- Coroutines:
  - Add Interpolate overload that is able to break mid-interpolation.
  - Add InterpolateUnscaleTime methods that use unscaled time instead of scaled time.
- SceneLoader:
  - Add public getter to Scenes array.
  - Add Reset method that unloads all but SceneLoader scene and reloads loadOnStart scenes.
  - Add SceneLoader.SwitchScene method that gets a collection of scenes to load.
  - Add SceneLoader.UnloadScenes method.

### Fixed

- Fix Vector3Utils.ToVector3XZ return type.
- Fix SceneLoader.SwitchScene's specific scenes unloading.

### Changed

- SceneLoader:
  - Change SwitchScene API to get a collection of scenes to unload instead of a single scene.
  - Changed SwitchScene coroutine to use unscaled time.

## [1.5.0] - 2023-05-11

### Added

- Add Vector3Utils.ToVector3XZ function.
- Add documentation.

### Fixed

- Fix SceneLoader.GetActiveScenes to use the given type.
- Fix Vector3IntUtils's L1Norm and L1Distance functions calculation.

## [1.4.0] - 2023-05-10

### Added

- Add SceneLoader.GetActiveScenes method.
- Add PureAttribute to functions in Utils classes.

## [1.3.0] - 2023-05-04

### Added

- New SceneManager.SceneType: ConstantReload.
  Which is like Constant except it is reloaded on each scene switch.
- SceneManager.SwitchScene can now get a callback to perform on switch end.

## [1.2.0] - 2023-05-04

### Added

- GizmosExt static class with a DrawWireCapsule method.
- CHANGELOG.md (this) and LICENSE.md files.

### Changed

- SceneManager now adds pre-loaded scenes on Awake.

## [1.1.0] - 2023-03-15

### Added

- Vector2Utils static class.

## [1.0.0] - 2023-03-13

### Added

- Coroutine static class.
- SceneLoader static class.
- EnumUtils static class.
- Vector3Utils static class.
- Vector2IntUtils static class.
- Vector3IntUtils static class.