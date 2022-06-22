# Intelligent-Bot-Management-
Intelligent Bot Management is an innovative way to automatically monitor the RPA system. It is bot of bots. It is meant for controlling monitoring of other bots. It diagnoses the failure of RPA components and promptly execute the remediation action to resolve the issue and notify the respective team about the failure and action taken against those failures.

# Build Instructions
1.Download source code
2.Go to properties of the zip file and unblock the file
3.Extract the files from the zip to D:\GIT
4.Open visual studio cmd Prompt and execute
	MSBuild "D:\GIT\Intelligent-Bot-Management--main\Superbot\Source\SuperBot.sln"
	OR
	Open the solution Superbot.sln in visual studio and right click on the solution > Build Solution
5. Open visual studio cmd Prompt and execute
	MSBuild "D:\GIT\Intelligent-Bot-Management--main\Resource Configurator\ConfigurationManagement\ConfigurationManagement.sln"
	OR
	Open the solution ConfigurationManagement.sln in visual studio and right click on the solution > Build Solution
6. Open visual studio cmd Prompt and execute
	MSBuild "D:\GIT\Intelligent-Bot-Management--main\Data Integration Framework\Code.sln"
	OR
	Open the solution Code.sln in visual studio and right click on the solution > Build Solution