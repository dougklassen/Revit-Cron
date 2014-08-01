RCronD plan
---

- load tasks file
- calculate most recent requested run time for all tasks
- every minute:
    - iterate all tasks
    - if last run is greater than requested run time, add to run queue
    - if queue is running, wait 
    - otherwise, mark queue as running and run it
    - mark queue as not running

- watch tasks file and reload if changed
