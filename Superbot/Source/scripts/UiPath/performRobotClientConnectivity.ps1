#===============================================================================================
# © 2020 Infosys Limited, Bangalore, India. All Rights Reserved.
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
# FILE: performRobotClientConnectivity.ps1
#
# USAGE: PowerShell.exe -File <location of the script>\performRobotClientConnectivity.ps1 -orchestratorURI <Orchestrator URL> 
# -key <Machine Key> -clientInstallationPath <UiPath Robot Client Installation Path>
#
# DESCRIPTION: This script establish the connectivity between Robot to Orchestrator.
#
# PREREQUISITES: 
#     Machine must be Windows machine

# INPUT: orchestratorURI,key,clientInstallationPath
# NOTE: Kindly enter the inputs in following sequence :
# orchestratorURI  key  clientInstallationPath
# OUTPUT: Success or failure.
# Author: Ananthkumar_S
#=================================================================================================

#validate the arguments.
[cmdletbinding()]
param(
	
	[string] $orchestratorURI ,
	[string] $key , 
	[string] $clientInstallationPath
)


# Variable to display error message
[string] $errordata = "No Error Found"
[string] $orchestratorURIParam=" -url "+$orchestratorURI
[string] $keyParam=" -key "+$key



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
If (-not $PSBoundParameters.ContainsKey('orchestratorURI')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - orchestratorURI"
    Write-Output ("error=" + $errordata)
	break

}

elseif(-not $PSBoundParameters.ContainsKey('key')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - key"
    Write-Output ("error=" + $errordata)
	break
}

elseif (-not $PSBoundParameters.ContainsKey('clientInstallationPath')){
	$clientInstallationPath="C:\Program Files (x86)\UiPath\Studio\UiRobot.exe"
}

Try
{
	
	$clientPath=$clientInstallationPath
	$connectString=" --connect "	
	
	
	$command=$connectString + $orchestratorURIParam + $keyParam
	
	write-output($command)
	
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








