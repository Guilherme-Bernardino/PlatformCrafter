# PlatformCrafter - by Guilherme B.

Unity package for 2D platformer and platformer-like games.
Meant for unexperienced and experienced game devs that want an easy way to create game experiences.


# Change Log
All notable changes of the project

## [0.6.1] - 1-07-2024

### Added
- Added Instantiate Type Action to Action Module v1.0.
- Added Sound Effect Type Action to Action Module v1.0.
- Added AudioSource component to Modular Brain.
- Added a way to obtain the HM Module form the MB.
- Added a Facing Right check for HM module for direction check.

### Changed
- Changed Action class to ExternalAction for comprehension.

## [0.6] - 30-07-2024

### Added
- Added drag movement setting on the Crouch action.
- Added option for braking with left/right key on the Vehicle-Like mode on the Walking Action.
- Added toggle option that allows for mid-way stopping on vertical climb action.
- Added Action Module v1.0 with internal and external actions choices.
- Added External Action class used for actions not hardcoded on the action module.

### Changed
- Changed the way Interact Receptors work, now allowing UnityEvent and a hardcoded option of Interactable Objects.

### Fixed
- Fixed bug where braking didn't work properly for both left and right.
- Fixed modules not keeping the active state upon reload.

## [0.5.1] - 28-07-2024

### Added
- Added Interaction Receptor class used for interactable objects.

### Removed
- Layers and InteractComponent from Interaction Module.

## [0.5] - 27-07-2024

### Added
- Added Resource Module v1.0 with active and passive modes.
- Added Interaction Module v1.0 with active and passive modes.
- Added ScriptableChannels.

## [0.4.1] - 26-07-2024

### Added 
- Added Crouch action to VM Module v1.0.
- Added two modes to crouch: normal crouch and platform that allows to go through platforms while crouching.
- Added Climb action to VM Module v1.0 with a simple vertical climb.
- Added Collider reference on the Modular Brain.

### Changed
- Dash action struture.

### Fixed
- HM and VM action selection where some actions couldn't be selected properly.

### Removed
- Removed MultipleDashes mode to dash action.

## [0.4] - 23-07-2024

### Added
- Added Vertical Movement Type Module V1.0.
- Added Jump and AirJump as ModuleActions for said module.
- Added Constant Height and Derivative Height for both actions.

### Changed
- Changed the way walking works by checking ground and allowing horizontal air movement if needed.

## [0.3.2] - 19-07-2024

### Added
- Added double tap feature for Sprint and Dash actions.
- Added a way to activate/deactivate extra actions through flagging.

### Fixed
- Fixed nesting modes for the actions.

## [0.3.1] - 18-07-2024

### Added
- Added Horizontal Movement Type Module V1.0.
- Added Walk, Sprint and Dash as ModuleActions for said module.

## [0.3] - 15-07-2024

### Added
- Added Modular Brain System component.
- Added Modular Brain Editor.

### Changed
- Changed modular structure to allow module types.
- Changed module mechanics to actions structs.

## [0.2.1] - 10-07-2024
 
### Added
- Added basic shooting mechanic to Toggle Feature (mouse 0 for testing).
- Added basic shooting mechanic to Modular (mouse 0 for testing).
- Added basic animation on both controllers for Idle, Walking and Running.

## [0.2] - 09-07-2024
 
### Added
- Added double jump to Toggle Feature.
- Added basic Modular controller structure for PlatformCrafter.
- Added basic Horizontal Movement Module.
- Added basic Jump Module.

### Changed
- Changed old controller to now be named Toggle Feature.
- Changed Jump button and logic on Toggle Feature.

## [0.1] - 04-07-2024
 
### Added
- Added controller for PlatformCrafter.
- Added horizontal movement enabler and basic walking functionality.
- Added run movement enabler and basic running functionality (left shift for testing).
- Added jump enabler and basic jumping functionality.
 