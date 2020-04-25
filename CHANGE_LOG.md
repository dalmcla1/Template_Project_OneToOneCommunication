# Changelog for EVE

Find an overview of all new additions.

## EVE1.3

###Database support for MySQL8

Simplify database usage by upgrading to MySQL8.

- Easier download of MySQL database as the version is no longer legacy.
- Encrypted password exchange with database.
- Safer DB use with Prepared queries.

###Move to LTS University

Starting with this version, EVE always stays on the long-term support (LTS) Version of Unity, currently 2018.4.X.

###Update tutorial

The tutorial has been revamped and stream-lined based on the changes above.
- Simplified some Prefabs.

###Preparation for XR

To make EVE more usable with XR, the camera in all menus has been moved to
world space.  

## EVE1.2

### Energyscape & Globalscape integration

New features for the energyscape experiments

- New Question Type: VisualStimuli
- New Choice question option: Choice with image
- New Scale question option: dis/enable labels on toggle buttons

### Menu Overhaul

The menu is standardised to make expansion and use more simple

- The loader is renamed launcher (to match with the LaunchManager)
- All menus are now moved to the launcher
  - The evaluation scene is deleted and contents moved to the launcher
  - The questionnaire scene is deleted and contents are moved to the launcher
- All menus are instantiated instead of kept in the background
- The questionnaire manager no longer manages menus but defers everything to the MenuManager.
- All function used within one menu panel are now on the Menu Game Object
- The database can be reseted from the menu
- Experiments can be started from the parameter session screen (given that a participant id is assigned)

### Questionnaire Overhaul

- Simplify push to database
- Cleanup and standardise of Question base class

### LaunchManager Overhaul

- The Awake function is cleaned up so that now the first database connection does not require a restart of EVE.
- Add OSC to enable high quality audio with external audio system.

### General Overhaul

- Add more documentation
- Follow Reshaper recommendations on code design
- Upgraded to MiddleVR 1.7.1.1

### Linux support
EVE now runs on Ubuntu.

- Large files are located at http://www.files.ethz.ch/cog/LargeFilesLinux.rar


### Community Request

- Add custom images option for scale questions
  - See Experiment/Resources/QuestionSets/TestSet.xml for details
