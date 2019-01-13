# RAGEMP Base Scripts

---

Uses **Serilog** for rolling file logs if enabled with an attribute. 

## Build

This resource will build to RAGEMP's server runtime directory.

 As this is a common base resource there's no need to include it in every server resource directory.

	  <Target Name="PostBuild" AfterTargets="PostBuildEvent">
	    <Exec Command="xcopy /E /Y /R &quot;$(TargetDir)*.*&quot; &quot;C:/RAGEMP\server-files\bridge\runtime\*.*&quot;&#xD;&#xA;del &quot;C:\RAGEMP\server-files\bridge\runtime\$(TargetName).deps.json&quot;&#xD;&#xA;" />
	  </Target>

---

## Script usage:
	
	
	[Logger(RageLogLevel.Debug)]
	public class MyScript : RageScript
	{
		public MyScript()
		{
			Log("Script init");

            //Template message
            Log("Script innit error {warn}", RageLogLevel.Warning, warn);

            //Log to server console and file
            LogIncludeConsole("Script innit error {warn}", args: warn);

			//Get settings for this resource
			string[] resourceSettings = GetResourceSettings();
		}
	}


## Notes:

- If you're not using the default **C:/RAGEMP/** edit the csproj
- Logs to **RAGEMP\server-files\Logs**
- Highly Recommend to remove the **server-logs.txt** produced by GTAN (I choose one main script to delete this file each time it's run)
-  When this **server-logs.txt** gets above 800K the server load up time is significantly slower to boot up.
- Logging methods are overridable




