# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

[//]: # (## [MAJOR.MINOR.PATCH] - yyyy-mm-dd)

[//]: # ()
[//]: # (### Added)

[//]: # ()
[//]: # (- )

[//]: # ()
[//]: # (### Fixed)

[//]: # ()
[//]: # (- )

[//]: # ()
[//]: # (### Changed)

[//]: # ()
[//]: # (- )

[//]: # ()
[//]: # (### Removed)

[//]: # ()
[//]: # (- )

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