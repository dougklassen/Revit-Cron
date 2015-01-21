Revit-Cron
==========

A task scheduling application for Revit
---------------------------------

- Plot specified print sets
- Package the model with e-transmit
- Export specified print set to CAD
- Run external commands on the model

The application consists of several parts:

- A schedule file that specifies when tasks should run and records the last run time
- A dispatcher that batches tasks, records the batches in batch files, and launches Revit sessions to run them
- Batch files that specify what tasks should be run during the current session
- An external application Revit add-in that checks for a batch file on startup, runs if found, deletes the batch file on completion, and finally closes Revit.
