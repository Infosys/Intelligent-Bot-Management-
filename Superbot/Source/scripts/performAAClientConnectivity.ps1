#===============================================================================================
# � 2019 Infosys Limited, Bangalore, India. All Rights Reserved.
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
# FILE: performAAClientConnectivity.ps1
#
# USAGE: PowerShell.exe -File <location of the script>\performAAClientConnectivity.ps1 -controlRoomBaseURI <ControlRoom URL> 
# -botUser <bot user name> -botUserPassword <bot user Password> -clientInstallationPath <AA Client Installation Path>
#
# DESCRIPTION: This script establish the connectivity between bot creator/ bot runner to Automation Anywhere ControlRoom.
#
# PREREQUISITES: 
#     Machine must be Windows machine

# INPUT: controlRoomBaseURI,botUser,botUserPassword
# NOTE: Kindly enter the inputs in following sequence :
# controlRoomBaseURI  botUser  botUserPassword clientInstallationPath
# OUTPUT: Success or failure.
# Author: Ananthkumar_S
#=================================================================================================

#validate the arguments.
[cmdletbinding()]
param(
	
	[string] $controlRoomBaseURI ,
	[string] $botUser , 
	[string] $botUserPassword ,
	[string] $clientInstallationPath
)


# Variable to display error message
[string] $errordata = "No Error Found"
[string] $controlRoomBaseURIParam="/c"+$controlRoomBaseURI
[string] $botUserParam="/u"+$botUser
[string] $botUserPasswordParam="/p"+$botUserPassword


#=== FUNCTION ======================================================================================
# NAME: timeStamp()
# DESCRIPTION: This Function gives Output of current time in yyyy-mm-dd HH:MM Format
# INPUT: NA
# RETURN: timeStamp in yyyy-mm-dd HH:MM

Function timestamp {
	$date = get-date -f "yyyy-MM-dd HH:mm"
	Return $date
}



#Check if all required input parameters are passed.
If (-not $PSBoundParameters.ContainsKey('controlRoomBaseURI')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - controlRoomBaseURI"
    Write-Output ("error=" + $errordata)
	break

}

elseif(-not $PSBoundParameters.ContainsKey('botUser')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - botUser"
    Write-Output ("error=" + $errordata)
	break
}

elseif (-not $PSBoundParameters.ContainsKey('botUserPassword')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - botUserPassword"
    Write-Output ("error=" + $errordata)
	break
}

elseif (-not $PSBoundParameters.ContainsKey('clientInstallationPath')){
	$clientInstallationPath="C:\Program Files (x86)\Automation Anywhere\Enterprise\Client\Automation Anywhere.exe"
}

Try
{
	
	$clientPath=$clientInstallationPath
	$closeString="/aclose"	
	$loginString="/llogin"
	
	$command="""$controlRoomBaseURIParam"" ""$botUserParam"" ""$botUserPasswordParam"" ""$closeString"" ""$loginString"""
	
	#write-output($command)
	
	Start-Process -FilePath $clientPath -ArgumentList $command
		
	write-output("status=success")

}
# Perform Error handling
Catch
{
		
		$errordata = "[ERROR002]: [$($date)]: [Unable to perform Start-Process]: Exception occurred. Exception is : "  + $_.Exception.Message
		write-output($errordata)#
		write-output("status=failure")
}finally
{
		#close the session object
		
}








