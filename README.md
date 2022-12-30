# C-security-exercises

I'm just learning C#, here are some leaning examples. Mostly going to be security related.

### C# reverse shell 
C# reverse shell is netcat alike.
command to close connection and exit program: **drop**
client uses https://www.nuget.org/packages/Microsoft.NETFramework.ReferenceAssemblies 
commands are executed in FullLanguage mode. ( check this using $executioncontext.sessionstate.languagemode ).