# Version 1.1

## Breaking Changes
* Datetime parsing now defaults to en-US style (m/d/y) when it is ambigous. Previously, it used the local settings, and defaulted to en-CA when ambigous.
* Phone number parsing behaves differently in edge cases
* Enum parsing is now more strict - ie if you try to convert an int to an int-based Enum, and that value isn't defined for that Enum, you'll now get an exception
