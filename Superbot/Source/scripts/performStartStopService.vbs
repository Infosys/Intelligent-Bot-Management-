'======================================================================================
' © 2017 Infosys Limited, Bangalore, India. All Rights Reserved.
' Version: 1.0
' Except for any open source software components embedded in this Infosys proprietary
'software program ("Program"),this Program is protected by copyright laws,
'international treaties and other pending or existing intellectual property rights in
' India, the United States and other countries. Except as expressly permitted, any
'unauthorized reproduction, storage, transmission in any form or by any means
'(including without limitation electronic, mechanical,printing, photocopying, recording
' or otherwise), or any distribution of this Program, or any portion of it, may
' results in severe civil and criminal penalties, and will be prosecuted to the maximum
' extent possible under the law.
'======================================================================================
'
' FILE: performStartStopService.vbs
' USAGE: cscript performStartStopService.vbs [ip],[servicename],[action]

' DESCRIPTION:The script restarts/stops the specified windows service on specified windows desktop or server.It accepts 
'             IP (or Servername), action and service name as input parameters.

' PREREQUISITES:	1	OS:  Windows 7 & above
'					3	Admin rights :Need admin rights on specified remote machine.

' INPUT: ip:- Specified windows desktop or server where service has to be started.
'        servicename:- specified windows service.
'		 action:- Action to taken for the service,Start/Stop
' OUTPUT: status :- Result of performStartStopService.vbs:(Success/Failure)
'
'======================================================================================

'Variable declaration
Dim objService
Dim objServiceConnection
Dim ip
Dim servicename
Dim action
Dim objLocator
Dim error
error="No Error Found"

'Help: ReadMe
If wscript.arguments(0) = "-help" Then
	'Place usage guide in this section
	Wscript.Echo("FILE: performStartStopService.vbs")
	Wscript.Echo("USAGE: cscript.exe performStartStopService [ip] [servicename] [action]")
	Wscript.Echo("DESCRIPTION: The script restarts/stops the specified windows service on specified windows desktop or server.It accepts IP(or Servername) ,action and service name as input parameters.")
	Wscript.Echo("PREREQUISITES: ")
	Wscript.Echo("1 OS:  Windows 7 & above")
	Wscript.Echo("3	Admin rights:	Need admin rights")
	Wscript.Echo("INPUT: ip, servicename, action")
	Wscript.Echo("OUTPUT: Result of service restart (Success/Failure)")
	Wscript.Echo(" ")
	Wscript.Echo("How to run:")
	Wscript.Echo("From the command prompt in Windows type...")
	Wscript.Echo("cscript performStartStopService.vbs /ip:blrkec350285d /servicename:Public Service /action:Start")
End If

On Error Resume Next
WScript.Echo("**********Service Restart Task Started**********")
'Fetching Server Name from Passed Input arguments
Set colArgs = WScript.Arguments.Named

'validate arguments in case of Null value
If WScript.arguments.count =0 then
	error="[ERROR001]:" & timeStamp() & " :[Missing Arguments]: ServerName/SystemIP , Action and Service Name are missing."
	Wscript.Echo(error)
	Wscript.echo("error=" & error)
	Wscript.quit()
End If

'Fetching Server Name from Passed Input arguments
If colArgs.Exists("ip") and (colArgs.Item("ip")<>"") then
	ip=replace(colArgs.Item("ip") , "ip=","")
Else
	error= "[ERROR002]:" & timeStamp() & " :[Missing Arguments]: ServerName/SystemIP  is not passed."
	Wscript.echo(error)
	Wscript.echo("error=" & error)
	WScript.quit()
End If

'Fetching Service Name from Passed Input arguments
If colArgs.Exists("servicename")and (colArgs.Item("servicename")<>"") then
	servicename=replace(colArgs.Item("servicename") , "servicename=","")
Else
	error= "[ERROR003]:" & timeStamp() & " :[Missing Arguments]: Service Name is not passed."
	Wscript.echo(error)
	Wscript.echo("error=" & error)
	WScript.quit()
End if

'Fetching the Action to be taken from Passed Input arguments
If colArgs.Exists("action") and (colArgs.Item("action")<>"") then
	action=replace(colArgs.Item("action") , "action","")
Else
	error=	"[ERROR004]:" & timeStamp() & " :[Missing Arguments]: Action is not passed."
End If

WScript.Echo("ip: " & ip)
WScript.Echo("servicename: " & servicename)
Wscript.Echo("action: " &action)

'Connecting to the server
Set objLocator = CreateObject("WbemScripting.SwbemLocator")
Set objServiceConnection = objLocator.ConnectServer(ip, "root\cimv2")

If Err.Number <> 0 Then
	If Err.Number=-2147024891 Then
        error="[-2147024891]:" & timeStamp() & " :[Connection Failure] : unable to connect to " & ip & " windows Server :Access is denied."
		Wscript.echo(error)
		Wscript.echo("error=" & error)
		WScript.quit()
	elseif Err.Number = -2147023174 Then
		error= "[-2147023174]:" & timeStamp() & " :[Connection Failure] : unable to connect to " & ip & " windows Server : The RPC server is unavailable. "
		Wscript.echo(error)
		Wscript.echo("error=" & error)
		WScript.quit()
	else
		error= "[ERROR004]:" & timeStamp() & " :[Connection Failure] : unable to connect to " & ip & " windows Server :" & Err.Number & ":" & Err.Description
		Wscript.echo(error)
		Wscript.echo("error=" & error)
		WScript.quit()
	end if
End If

'Check if service exists
Set objService = objServiceConnection.Get("Win32_Service.Name=" & "'" & servicename & "'")
If Err.Number <> 0 Then
	If Err.Number = -2147217406 then
		error="[-2147217406]:" & timeStamp() & ":[Invalid Service Name]: Service Name: " & servicename & " Not found "
		Wscript.echo(error)
		Wscript.echo("error=" & error)
		WScript.Echo("status=Failure")
		WScript.Quit
	else
		error= "[ERROR005]:" & timeStamp() & " :[Invalid Service Name ]: Please enter a valid Service Name: " & Err.Number & ":" & Err.Description
		Wscript.echo(error)
		Wscript.echo("error=" & error)
		WScript.Echo("status=Failure")
		WScript.Quit
	End If
End If

'Converting the user given value of "action parameter" to Upper Case
Wscript.Echo(action)
action=UCase(action)
Wscript.Echo(action)

'Checking the action to be taken(Start/Stop) for the Windows Service
If (action = "START")then
   'Restarting the service
	retVal = objService.StartService()
	If Err.Number <> 0 Then
		error="[Error006]: " & timeStamp & " Unable to restart service : " & servicename & ":" & Err.Number & ":" & Err.Description
		Wscript.echo(error)
		Wscript.echo("error=" & error)
		WScript.Echo("status=Failure")
		WScript.Quit
	Else
		WScript.Echo("Service started successfully...: " & retVal)
		WScript.Echo("status=Success")
		Wscript.echo("error=" & error)
		WScript.Echo("**********Service Restart Task Completed**********")
		WScript.Quit
	End If

Elseif (action = "STOP") then
   'Stopping the service
	retVal=objService.StopService()
	If Err.Number <>0 Then
		error="[Error006]:" & timeStamp & "Unable to stop service : " & servicename & ":" & Err.Number & ":" & Err.Description
		Wscript.echo(error)
		Wscript.echo("error=" & error)
		WScript.Echo("status=Failure")
		WScript.Quit
	Else
		WScript.Echo("Service stopped successfully...: " & retVal)
		WScript.Echo("status=Success")
		Wscript.echo("error=" & error)
		WScript.Echo("**********Service Stop Task Completed**********")
		WScript.Quit
	End If
End If

Set objLocator=Nothing
Set objService=Nothing
Set objServiceConnection=Nothing

'=== FUNCTION ==============================
' NAME: timeStamp()
' DESCRIPTION: This Function gives Output of current time in yyyy-mm-dd HH:MM Format
' PARAMETER : NA
' INPUT: NA
' RETURN: timeStamp in yyyy-mm-dd HH:MM

Function timeStamp()
	timeStamp = Year(Now) & "-" & _
	Right("0" & Month(Now),2)  & "-" & _
	Right("0" & Day(Now),2)  & " " & _
	Right("0" & Hour(Now),2) & ":" & _
	Right("0" & Minute(Now),2)
End Function
