Revit-Cron
==========

A task scheduling suite for Revit
---------------------------------

- Plot specified print sets
- Package the model with e-transmit
- Export specified print set to CAD
- Run external commands

The application will consist of five parts

- A calendar file that schedules when tasks should run and records the last run time
- An editor for calendar file
- A dispatcher that will run Revit at a specified time and generate a work file
- A work file that specifies what tasks should be run during the current session
- An external application add-in that will check the work file, run current tasks, update the calendar file, and delete the work file
