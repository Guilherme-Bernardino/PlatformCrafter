<p align="center">
 <img width="400px" src="https://github.com/user-attachments/assets/49d4b92f-da57-4856-a2b8-6189994a493b" align="center" alt="Icon" />
</p>
<br />
<p align="center">
 <img src="https://img.shields.io/badge/build-v0.9.10-ffffff" />
   <a href="https://github/Guilherme-Bernardino/PlatformCrafter/downloads">
    <img alt="Total" src="https://img.shields.io/github/downloads/Guilherme-Bernardino/PlatformCrafter/total.svg" />
   </a>
   <br />
 </p>
 
# Description
Unity package for 2D platformer and platformer-like games.
Meant for unexperienced and experienced game devs that want an easy way to create game experiences.

# Contributor(s)
Guilherme Bernardino

# Change Log
All notable changes of the project

## [0.9.10] - 1-10-2024
### Added
- Added a new Vertical Module Action named WallGrab & Wall Jump, where you can hold a wall or slowly slide and jump from the wall.
- Added a new Vertical Module state named Wall Jump, that can only ocurr when Wall Grabbing or Ledge Grabbing, perform a unique jump different from the standard jump action.
- Added a new Vertical Module state named Ledge Grab on the WallGrab action, with a different tag, allows for grabbing and holding a grab on a ledge.
- Added automatic mode only for Ledge Grabbing.
- Added Animation and Sound Effect Modules settings for WallGrab, Ledge Grab and Wall Jump states.
- Added a Fall State (toggle), that replaces the falling states of the jump actions, like the Jump and AirJump or Natural Fall on Idle.
- Added the Fall state (toggle) settings on the Animation Module settings.
- Added Dash option called AlwaysHorizontal, where the Y velocity doen'st affect the dash.

### Changed
- Changed a few properties for clarity.

### Fixed
- Fixed Air jump insconsistent jump height.
- Fixed Jumps through platforms.

## [0.9.9] - 23-09-2024

### Added
- Added a new type of action on Action Module called Special Effect, that deals with visual and audio effects as an action, such as Sound Effect, Animation and Particles.
- Added a getter for a list of particle systems on the Modular Brain.
- Added an option named CanCrawl to the Crouch Action on VM Module, that allows the entity to Crawl, or move horizontally without leaving the Crouching state.

### Changed
- Changed a few properties for clarity.

### Fixed
- Fixed Air Jump bug where Time Between Jumps was making the derivative air jump a small bump in the air, than proceed to the long derivative jump with the amount of time pressed.
- Fixed bug where the animation on Special Effect Type Action wasn't prioritized and played until the end.

### Removed
- Removed the Animation and Sound Effect type action.

## [0.9.8] - 20-09-2024

### Added
- Added new Action type to Action Module named Wield, that allows for the wielding of a tool or weapon close to the entity.

### Changed
- Changed a few settings names for clarity.

### Fixed
- Fixed Air Jump auto mode not working.

## [0.9.7] - 19-09-2024

### Added
- Added a new HM Action called Slide.
- Added two modes for Slide: Roll and Long Slide.
- Added Transition from Walk state to Sprint and vice-versa through speed treshold.
- Added Shadow Effect setting to all HM Module's actions and Jump and AirJump on VM Module.
- Added a Shadow Effect getter on Modular Brain.
- Added HM property that allows to adjust sprite's facing direction.
- Added Natural Fall Gravity Scale property to the VM Module.
- Added Time Between Jump on the Air Jump settings.
- Added automatic modes for every actions on HM and VM. HM isAutomatic setting allows for No automatic, Right direction and Left direction. VM isAutomatic settings is a bool and Climb is "No" , "Up" & "Down".
- Added Slide Action settings to Animation and Sound Effect Modules.

### Changed
- Changed the Physics Category Modules to now having all in the same script, and utilizing FixedUpdate for physics and Update for inputs.
- Changed the use of Module Actions to none.
- Changed the range values of all properties.
- Changed ground check range to ground check as a box check, with now width(X) and height(y) on both HM and VM modules.

### Fixed
- Fixed Shadow Effect Shadows not being the same scale as the original sprite.

## [0.9.6] - 10-09-2024

### Added
- Added the Platform Crafter's User Guide.

### Changed
- Changed the Sound Effects and Animation Modules fused actions from CrouchWalk to Crawl.

## [0.9.5] - 09-09-2024

### Added
- Added a Size label that tells the amount of slots on an Inventory.
- Added Icons for the Shadow Effect, Solid, Interaction Channel and Interaction Receptor.
- Added FixedUpdate and LateUpdate cycles for the Modular Brain and the Modules.
- Added a smoothness and offset vector to the CameraFollow custom module.
- Added a new Asset menu named Utils and added create assets of Inventory Item, External Action Test and Interaction Channel.
- Added a new create menu tab named "Custom" and added the CameraFollow custom module to it.

### Changed
- Changed the general settings of the Sound Effect Module, like sound, pitch and loop to individual settings for each SFX.
- Changed all create assets for the modules.
- Changed Abstract folder to Utils.

## [0.9.4] - 28-08-2024

### Added
- Added a new combined animation and SFX named AirDash, that can be performed when jumping or air jumping and dashing.

### Changed
- Changed the combined action CrouchWalk to now being able to be performed while walking or sprinting and crouching.

### Fixed
- Fixed RigidBody rotation not freezing when holding climb on Climb Action.
- Fixed Crouch Action Platform Crouch not working with a Capsule Collider 2D.

## [0.9.3] - 27-08-2024

### Added
- Added a function to the ModularBrain that adds all the necessary game object components to the entity, such as SpriteRenderer, RigidBody2D, CapsuleCollider2D, Animator, AudioSource and the ShadowEffect.
- Added a new script named ShadowEffect, that instantiates shadows that vanish with time.
- Added a new script named Solid, that represents a shadow.
- Added a way to toggle this effect on the Dash Action of the HM Module, so now the dash can create an effect of after images or shadows.

### Fixed
- Fixed Dash Action not working when Walk Action and Sprint Action are on Constant Speed.
- Fixed Animation and Sound Effect modules playing when unactive.

## [0.9.2] - 25-08-2024

### Added
- (Editor) Added a check change for the GUI to minimize unnecessary repaints.

### Changed
- (Editor) Changed the save and load of the foldout states so it reduces redundancy, improves performance and ensures consistency across different types of modules using a more general method.

## [0.9.1] - 23-08-2024

### Added
- Added the SoundEffect Type Module v1.0, audio for Idle, Walk, Sprint, Dash, Braking, Jump, AirJump, Climb, Crouch and Crouch Walk.
- (Editor) Added an icon to the SoundEffectType Module.
- (Editor) Added buttons to pause and unpause audio on the editor.

### Changed
- Changed HM MovementState to HorizontalState.
- Changed all icons on editor and each module to new ones.

## [0.9] - 22-08-2024

### Added
- Added the Animation Type Module v1.0, with animations for Walk, Sprint, Dash, Brake, Jump, AirJump, Crouch, Climb, and Crouch Walking (Walk + Crouch).
- Added states for HM and VM, so the animations don't conflict with each other.
- (Editor) Added a new category named Visuals & Audio.
- (Editor) Added an icon to the AnimationType Module
- (Editor) Added buttons to pause and unpause animations on the editor.
- (Spelunky Recreation) Added multiple animations to the Spelunky Player entity, namely Walk, Jump, Crouch, Climb and CrouchWalking.

### Changed
- (Editor) Changed the icons and the labels display.

### Fixed
- Fixed HM Module allowing sprites to be flipped when CanMoveOnAir of Walk Action is active.

## [0.8.3] - 20-08-2024

### Added
- Added assets from Bro Force.
- Added a new scene to recreate the levels from Bro Force.
- Added a new player character that represent Bro Force game settings. Currently walk, jump and a shooting action.
- (Editor) Added a limit to the number of Modules displayed on each module type, where it only displays the 10 first modules, and alerts the user that the next modules won't be displayed on the inspector.
- (Editor) Added a toggle editor features on Modular Brain's context menu. 

### Changes
- (Editor) Changed Icon loading to be cached and loaded once.
- (Editor) Changed GUI foldout rendering to minimize 

## [0.8.2] - 19-08-2024

### Added
- Added assets from Rick Dangerous Amiga.
- Added a new scene to recreate the levels from Rick Dangerous.
- Added a new player character that represent Rick Dangerous game settings. Currently walk, jump, climb, crouch and a shooting action.

## [0.8.1] - 16-08-2024

### Added
- Added assets from Spelunky.
- Added a new scene to recreate the levels from Spelunky.
- Added a new player character that represent Spelunky game settings. Currently walk, jump, climb and crouch.
- Added the ability to climb down on the Climb Action of VM Module.
- Added a capsule collider reference to Crouch Action of VM Module.

### Fixed
- Fixed crouch being able to perform mid-air.

## [0.8] - 15-08-2024

### Added
- Added more obstacles and platforms to Testing Scene.
- Added a custom module named Camera Following.

### Fixed
- Fixed crouching bug where box collider doesnt revert to original height.
- Fixed crouching drag on platform crouch.
- Fixed bug where climbing action had rotation on enabled.

## [0.7] - 06-08-2024

### Added
- Added an Inventory Module v1.0 to the system, with Inventory Slots and methods to add and remove items.
- Added InventoryItem scriptable object.
- Added specific getters using name of the module for Modular Brain.
- Added a UseAnItem option on the ConsumptionType of Action Module, where you select a specific InventoryModule, an item and an amount to consume and act.
- Added a method to check if Inventory has an Item and a specific quantity.

## [0.6.3] - 05-08-2024

### Added
- Added Consumption Type Action to Action Module v1.0.

### Removed
- Removed Status Application type.

## [0.6.2] - 03-08-2024

### Added
- Added a Custom Modules list.
- Added a validation to Custom Modules where predefined types of modules can't be added to the list.
- Added getters for all modules.
- Added a count of modules per type and a respective icon for clarity.

### Changed
- Changed modular brain single module list to now having one module for HM and VM, a list for Action, Interaction, Resource, and Inventory.
- Changed editor display to now having categories: Physics Modules with HM and VM types; Action/Interaction Modules with Action and Interaction Types; Container Modules with Resource and Inventory Type; Custom Modules.
- Changed custom modules interface on editor.

## [0.6.1] - 01-08-2024

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
 
