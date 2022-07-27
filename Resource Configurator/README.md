# Build Instructions
1.Download the release package <br/>
2.Go to properties of the zip file and unblock the file <br/>
3.Extract the files from the zip <br/>
4.Open visual studio cmd Prompt and execute <br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;MSBuild "..\Source\Source\ConfigurationManagement\ConfigurationManagement.sln" <br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;OR <br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Open the solution ConfigurationManagement.sln in visual studio and right click on the solution > Build Solution <br/>
	  
# Release Instructions
1.Follow the build instructions for ConfigurationManagement.sln <br/>
2.Go to ..\Source\Source\ConfigurationManagement\ConfigurationManagement.API\bin\ <br/>
3.Copy all the runtime files to configurationmanagementapi directory of the release package <br/>
