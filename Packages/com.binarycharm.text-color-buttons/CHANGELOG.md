# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2022-11-16
### Added
- Added editor integration (`GameObject` menu commands)
- Added method `TextColorButtonExtensions.setDisabledTextColor`
- Added integration supporting `BinaryCharm.SemanticColorPalette`
- Added assembly definitions
- Added documentation comments

### Changed
- Restructured directory layout
- Installation as `Package` (no cluttering of the `Assets` folder)
- Adjusted Demo UI layout (previously worked well only at 16:9 aspect ratio)
- Improved code formatting consistency
- Improved documentation (new formatting, new usage instructions)

### Fixed
- Made `TextMeshPro` dependency optional
- Corrected wrong calculation in `TextColorBlockDrawer.GetPropertyHeight`
- Avoided `null` reference access to text component in some circumstances

## [1.0.0] - 2021-08-20
- Initial release of BinaryCharm.UI.TextColorButtons
