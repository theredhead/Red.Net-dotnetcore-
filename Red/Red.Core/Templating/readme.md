Red.Core.Templating
===================

This namespace provides T4 like templating but running the templates as c# scripts in the running AddDomain.

templates are just text with special code sections:

Code blocks: <# ... #>
----------------------
Use these to declare variables and execute logic. Multiple lines are allowed and lines end with semicolons.


Expression blocks: <#= ... #>
-----------------------------
Use these to output the result of any single statement. Only one expression per block and no semicolon.

Example template:
-----------------

The following template outputs a sentence stating the weekday of christmas for the range of years between 1900 and 2100:

````
<# using System; #>
<# var xmas = new DateTime(1900, 12, 25); #>

This is a test template.

<# for (var i = 0; i < 200; i++) { #>
In <#= xmas.Year #>, Christmas <#= xmas > DateTime.Now ? "falls" : "fell" #> on a <#= xmas.DayOfWeek #>.<# xmas = xmas.AddYears(1); #>
<# } #>
````

