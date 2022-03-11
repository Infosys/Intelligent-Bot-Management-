'====================================================================================== 
' © 2019 Infosys Limited, Bangalore, India. All Rights Reserved.
' Version: 1.0
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
' FILE: performServerUpStatusCheck.vbs
'
' USAGE: cscript performServerUpStatusCheck.vbs /ip=[ip] /noOfRetries=[#ofRetries] /waitTime=[WaitTimeinMinutes]
'
' DESCRIPTION: Check VM/Server availability is the scenario where we have to check that the windows server is available or not. 
'              If it is available, then the windows server is healthy.
'              If the windows server is not available, then that server is not healthy
'			   
'
' PREREQUISITES: 
' Windows Machine
'
' INPUT: ip - IP address of the machine, noOfRetries - Number of retries, waitTime - Wait time (msec) for each retry
'
' OUTPUT: status - status of the machine=success/failure,serveravailability= availability of the machine is healthy/unhealthy
'
' Author: Gauri Joshi
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
Dim i
Dim x
Dim y

error = "No Error Found"

If (UCase(WScript.arguments(0)) = "-HELP") Then
    'Place usage guide in this section
	Wscript.Echo("FILE: performServerUpStatusCheck.vbs")
	Wscript.Echo("USAGE: cscript.exe performServerUpStatusCheck.vbs /ip=[ip] /noOfRetries=[noOfRetries] /waitTime=[waitTime]") 
	Wscript.Echo("PREREQUISITES: ")
	Wscript.Echo("Machine must be Windows machine")
	Wscript.Echo("INPUT: ip - IP address of the machine, noOfRetries - Number of retries, waitTime - Wait time (msec) for each retry")
	Wscript.Echo("OUTPUT: status - status of the machine=success/failure,serveravailability= availability of the machine is healthy/unhealthy")
	Wscript.Echo("How to run:")
	Wscript.Echo("From the command prompt in Windows type")
	Wscript.Echo("prompt> cscript.exe performServerUpStatusCheck.vbs ip")
	WSCript.Quit()	
End If

Set colArgs = WScript.Arguments.Named
'Read argument which has server or IP address
If colArgs.Exists("ip") and (colArgs.Item("ip")<>"") Then  
   ip = replace(colArgs.Item("ip") , "ip=","")
Else
    error= "[ERROR001]: " & timeStamp() & " : [Wrong number of Arguments]: serverip is required. " 
	wscript.echo (error)
    WScript.echo ("error=" & error )
	WScript.quit
End If

If colArgs.Exists("noOfRetries") and (colArgs.Item("noOfRetries")<>"") then  
                noOfRetries = replace(colArgs.Item("noOfRetries") , "noOfRetries=","")               
Else        
                error= "[ERROR002]:" & timeStamp() & " :[Wrong number of Argument]: noOfRetries for ping request  is missing."
                WSCript.echo(error)
                Wscript.echo("error=" & error)
                WScript.quit()                    
End If

If colArgs.Exists("waitTime") and (colArgs.Item("waitTime")<>"") then  
                waitTime = replace(colArgs.Item("waitTime") , "waitTime=","")               
Else        
                error= "[ERROR002]:" & timeStamp() & " :[Wrong number of Argument]: waitTime between ping requests is missing."
                WSCript.echo(error)
                Wscript.echo("error=" & error)
                WScript.quit()                    
End If

WScript.echo("Server IP is " & ip)
WScript.echo("No Of Retries is " & noOfRetries)
WScript.echo("Wait Time in msec is " & waitTime)

i=0
x = CInt(noOfRetries)
y= CInt(waitTime)
'Read input to the script. 
'Input will be ip of the machine
Do while i < x 
	set objShell = CreateObject("Wscript.Shell")
	set objPing = objShell.Exec("ping " & ip)
	pingOut = objPing.StdOut.ReadAll
	WSCript.Echo(LCase(pingOut))
	i=i+1
	WSCript.Echo("Iteration :" & i & " at Time:" & timeStamp())
	If instr(LCase(pingOut), "reply") Then
		Exit Do
	End if	
	WScript.Sleep(y)

Loop

If instr(LCase(pingOut), "reply") Then
	WSCript.Echo("status=success")
	WSCript.Echo("serveravailability=healthy")
Else
	WSCript.Echo("status=failure")
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
    Right("0" & Minute(Now),2) & ":" & _
	Right("0" & Second(Now),2)
End Function