'====================================================================================== 
' © 2017 Infosys Limited, Bangalore, India. All Rights Reserved.
' Version: 1.4
' Except for any open source software components embedded in this Infosys proprietary 
' software program ("Program"),this Program is protected by copyright laws, 
' international treaties and other pending or existing intellectual property rights in
' India, the United States and other countries. Except as expressly permitted, any 
' unauthorized reproduction, storage, transmission in any form or by any means 
'(including without limitation electronic, mechanical,printing, photocopying, recording
' or otherwise), or any distribution of this Program, or any portion of it, may
' results in severe civil and criminal penalties, and will be prosecuted to the maximum
' extent possible under the law.
'======================================================================================
'
' FILE: healthCheckServerAvailability.vbs
'
' USAGE: cscript healthCheckServerAvailability.vbs /ip:[ip] 
'
' DESCRIPTION: Check VM/Server availability is the scenario where we have to check that the windows server is available or not. 
'              If it is available, then the windows server is healthy.
'              If the windows server is not available, then that server is not healthy
'
' PREREQUISITES: 
' Windows Machine
'
' INPUT: ip - IP address of the machine
'
' OUTPUT: ishealthy - (yes/no) Gives information about Server availability
'
' Author: Nivedita Raut
'======================================================================================
wscript.echo ("[INFO001]:" & timeStamp() & ": Script Started Execution.")

On Error Resume Next
'Declare local variables '
Dim error
Dim ip
Dim ishealthy
Dim colArgs
Dim objShell
Dim objPing
Dim pingOut

error = "No Error Found"
Set colArgs = WScript.Arguments.Named

'Read argument which has server or IP address
If colArgs.Exists("ip") and (colArgs.Item("ip")<>"") Then  
   ip = replace(colArgs.Item("ip") , "ip=","")
Else
    error= "[ERROR001]: " & timeStamp() & " : [Wrong number of Arguments]: servername is required. " 
	wscript.echo (error)
    WScript.echo ("error=" & error )
	WScript.quit
End If

WScript.echo("Server Name is " & ip)

If (UCase(WScript.arguments(0)) = "-HELP") Then
    'Place usage guide in this section
	Wscript.Echo("FILE: healthCheckServerAvailability.vbs")
	Wscript.Echo("USAGE: cscript.exe healthCheckServerAvailability.vbs [ip]  ")
	Wscript.Echo("PREREQUISITES: ")
	Wscript.Echo("Machine must be Windows machine")
	Wscript.Echo("INPUT: IP address")
	Wscript.Echo("OUTPUT: ishealthy of the machine")
	Wscript.Echo("How to run:")
	Wscript.Echo("From the command prompt in Windows type")
	Wscript.Echo("prompt> cscript.exe healthCheckServerAvailability.vbs ip")
	WSCript.Quit()	
End If

'Read input to the script. 
'Input will be ip of the machine
	set objShell = CreateObject("Wscript.Shell")
	set objPing = objShell.Exec("ping " & ip)
	pingOut = objPing.StdOut.ReadAll
	WSCript.Echo(LCase(pingOut))
	If instr(LCase(pingOut), "reply") Then
	   WSCript.Echo("ishealthy=yes")
	   WSCript.Echo("serveravailability=healthy")
	Else
	   WSCript.Echo("ishealthy=no")
	   WSCript.Echo("serveravailability=unhealthy")
	End if

	Set objShell = Nothing
	set objPing = Nothing
	WScript.echo ("error=" & error )
    Wscript.echo ("[INFO002]:" & timeStamp() & ":Completed resolution.")
'=== FUNCTION ==============================
' NAME: timeStamp()
' DESCRIPTION: This Function gives Output of current time in yyyy-mm-dd HH:MM Format
' PARAMETER : NA 
' INPUT: NA
' RETURN: timeStamp in yyyy-mm-dd HH:MM
Function timeStamp() 
    timeStamp = Year(Now) & ":" & _
    Right("0" & Month(Now),2)  & ":" & _
    Right("0" & Day(Now),2)  & " " & _  
    Right("0" & Hour(Now),2) & ":" & _
    Right("0" & Minute(Now),2)  
End Function