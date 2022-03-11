Dim hostname,status
Set arg = WScript.Arguments
hostname = arg(0)
'hostname = "HYDPCM366165D"
Set WshShell = WScript.CreateObject("WScript.Shell")
Ping = WshShell.Run("ping -n 1 " & hostname, 0, True)
Select Case Ping
Case 0
status = "Online"
   WScript.Echo status
Case 1 
status = "Offline"
   WScript.Echo status
End Select
