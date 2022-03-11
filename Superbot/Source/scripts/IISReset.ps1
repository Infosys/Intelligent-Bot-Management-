#===============================================================================================
# © 2019 Infosys Limited, Bangalore, India. All Rights Reserved.
# Version: 1.0
# Except for any open source software components embedded in this Infosys proprietary 
#software program ("Program"),this Program is protected by copyright laws, 
#international treaties and other pending or existing intellectual property rights in
# India, the United States and other countries. Except as expressly permitted, any 
#unauthorized reproduction, storage, transmission in any form or by any means 
#(including without limitation electronic, mechanical,printing, photocopying, recording
# or otherwise), or any distribution of this Program, or any portion of it, may
# results in severe civil and criminal penalties, and will be prosecuted to the maximum
# extent possible under the law.
#================================================================================================
#
# FILE: IISReset.ps1
#
# USAGE: PowerShell.exe -File <location of the script>\IISReset.ps1 -serverName <Server Name> 
#
# DESCRIPTION: This script resets the IIS on the remote server.
#
# PREREQUISITES: 
#     Machine must be Windows machine.
#	  User must be an admin for the remote server.

# INPUT: serverName
# NOTE: Kindly enter the inputs in following sequence :
# serverName
# OUTPUT: Stops the IIS and then Starts it again on the specified server.
# Author: Vedika Shrivastava
#=================================================================================================

#validate the arguments.
[cmdletbinding()]
param(
    [string] $serverName
	
)

#=== FUNCTION ======================================================================================
# NAME: Reset()
# DESCRIPTION: This Function first stops the IIS and then starts it.
# INPUT: NA
# RETURN: NA
Function Reset{
invoke-command -computername $serverName {cd C:\Windows\System32\; ./cmd.exe /c "iisreset /noforce /stop" }
invoke-command -computername $serverName {cd C:\Windows\System32\; ./cmd.exe /c "iisreset /noforce /start" }
}

Try{
#Function Call
Reset
}
#Perform Error Handling
Catch{
write-output("Inside catch")
Write-Host $_
}



