dim status,strComputer,serviceName
Set arg = WScript.Arguments
strComputer = arg(0)
serviceName = arg(1)
'Wscript.Echo strComputer & serviceName

Set objWMIService = GetObject("winmgmts:{impersonationLevel=Impersonate}!\\" & strComputer & "\Root\CIMv2")
Set objWMIService = GetObject("winmgmts:\\" & strComputer& "\Root\CIMv2") 
    
Set colItems = objWMIService.ExecQuery("Select * from Win32_Service",,48) 
For Each objItem in colItems
	If objItem.Name= serviceName then
		status = objItem.State
		'Wscript.Echo "Name: "& objItem.Name &" status: " & status
	End If
Next
