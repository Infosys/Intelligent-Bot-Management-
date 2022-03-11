'====================================================================================== 
' © 2017 Infosys Limited, Bangalore, India. All Rights Reserved.
' Version: 2.0
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
' FILE: healthCheckServiceStatus.vbs

' USAGE: cscript.exe healthCheckServiceStatus.vbs <ip> <servicename> 

' DESCRIPTION: This script checks the status of WindowsService,It accepts ServerName/SystemIP and Service 
'              Name as parameters and checks if specified service is running on the specified ServerName/SystemIP

' PREREQUISITES: 
'    Windows Server/Desktop
'               Admin rights to user on specified remote machine

' INPUT: ip: System IP of the Target Machine 
'                               servicename: Windows Service Name
' OUTPUT: ishealthy - yes/no
' Author: Kumar Ankit
'======================================================================================
Dim error
On Error Resume Next
error="No Error Found"

'Read input to the script. 
Set colArgs = WScript.Arguments.Named

If (UCase(wscript.arguments(0)) = "-HELP")  Then
                WScript.echo "FILE: healthCheckServiceStatus.vbs"
                WScript.echo "Arguments to be passed"
                WScript.echo " 1) ServerName/SystemIP - Name or Ip of the server on which the service is running"
                WScript.echo " 2) Service Name- Name of the service whose status is to be found"
                WScript.echo "Usage - This script returns service status for the specified service"
                WScript.quit
End If

'validate arguments in case of Null value
If WScript.arguments.count =0 then
                error="[ERROR001]:" & timeStamp() & " :[Missing Arguments]: Server Name/SystemIP and Service Name are missing."
                Wscript.Echo(error)
                Wscript.echo("error=" & error)
                Wscript.quit()
End If
                
If colArgs.Exists("ip") and (colArgs.Item("ip")<>"") then  
                ip = replace(colArgs.Item("ip") , "ip=","")               
Else        
                error= "[ERROR002]:" & timeStamp() & " :[Wrong number of Argument]: ServerName/SystemIP  is missing."
                WSCript.echo(error)
                Wscript.echo("error=" & error)
                WScript.quit()                    
End If

If colArgs.Exists("servicename") and (colArgs.Item("servicename")<>"") then
                servicename=replace(colArgs.Item("servicename") , "servicename=","") 
Else
                error= "[ERROR003]:" & timeStamp() & " :[Wrong number of Argument]: Service Name is missing."
                Wscript.echo(error)
                Wscript.echo("error=" & error)
                WScript.quit()    
End if 

wscript.echo "ip=" &ip
wscript.echo "servicename=" &servicename
'Establishing Remote Connection 
Set objWMIService = GetObject("winmgmts:\\" & ip & "\root\cimv2")

If Err.Number<>0 Then  
                If Err.Number=70 Then
        error="[70]:" & timeStamp() & " :[Connection Failure] : unable to connect to " & ip & " windows Server : Permission denied"
                                Wscript.echo(error)
                                Wscript.echo("error=" & error)  
                                WScript.quit()
                elseif Err.Number = 462 Then
                                error= "[462]:" & timeStamp() & " :[Connection Failure] : unable to connect to " & ip & " windows Server : The remote server machine does not exist or is unavailable"
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

'Validate Service Name
Set strServiceNameCheck = objWMIService.ExecQuery("Select * from Win32_Service where Name = '" & servicename & "'")

If strServiceNameCheck.Count = 0 Then
                error= "[ERROR005]:" & timeStamp() & " :[Not Valid ServiceName]: Please enter a valid Service Name"
                Wscript.echo(error)
                Wscript.echo("error=" & error)
                WSCript.Quit()
End If

	 
Set strServiceState = objWMIService.ExecQuery("Select * from Win32_Service where state = 'Running' and Name = '" & servicename & "'")

'Check if Service is Running
For Each objProcess in strServiceState
    If Instr(objProcess.DisplayName,DisplayName) > 0 And objProcess.State = "Running" Then 
                                Status = true
    End If
Next

If Status = true Then
                ishealthy = "healthy"
                Wscript.echo ("ishealthy=Yes")
Else
                ishealthy = "unhealthy"
                Wscript.echo ("ishealthy=No")
End If

Wscript.echo ("servicestatus=" & ishealthy)
Wscript.echo("error=" & error)
Set objWMIService=Nothing
Set strServiceNameCheck=Nothing
Set strServiceState=Nothing

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
