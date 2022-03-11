#===============================================================================================
# 2019 Infosys Limited, Bangalore, India. All Rights Reserved.
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
# FILE: healthCheckDeployedDevicesStatus.ps1
#
# USAGE: PowerShell.exe -File <location of the script>\getDevicesList.ps1 -controlRoomBaseURI <ControlRoom URL> -controlRoomUsername<ControlRoom Username>  
# -controlRoomPassword<ControlRoom Password>
#
# DESCRIPTION: This script generates an authentication token using the Automation Anywhere ControlRoom
# Details and passes this token to get devices list which then retrieves the status of the deployed devices on
# the Automation Anywhere control tower
#
# PRE-REQUISITES: 
#     Machine must be Windows machine
#
# INPUT: controlRoomBaseURI,ControlRoomUsername,ControlRoomPassword.
# NOTE: Kindly enter the inputs in following sequence :
# controlRoomBaseURI  ControlRoomUsername  ControlRoomPassword 
# OUTPUT: List of devices attached to the control tower.
# Author: Ananthkumar_S
#=================================================================================================

#validate the arguments.
[cmdletbinding()]
param(
	
	[string] $controlRoomBaseURI ,
	[string] $controlRoomUsername , 
	[string] $controlRoomPassword
)


# Variable to display error message
[string] $errordata = "No Error Found"
[string] $authenticationURL= $controlRoomBaseURI+"v1/authentication"
[string] $devicesListURL = $controlRoomBaseURI+"v2/devices/list"

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
	$errordata = "[ERROR001]: [$($date)]: [Missing Arguments]: Please pass required parameter - controlRoomBaseURI"
    Write-Output("error=" + $errordata)
	break

}

elseif(-not $PSBoundParameters.ContainsKey('controlRoomUsername')){
	$date = timestamp
	$errordata = "[ERROR002]: [$($date)]: [Missing Arguments]: Please pass required parameter - controlRoomUsername"
    Write-Output("error=" + $errordata)
	break
}

elseif (-not $PSBoundParameters.ContainsKey('controlRoomPassword')){
	$date = timestamp
	$errordata = "[ERROR003]: [$($date)]: [Missing Arguments]: Please pass required parameter - controlRoomPassword"
    Write-Output("error=" + $errordata)
	break
}

#=== FUNCTION ========================================================================================
# NAME: generateToken()
# DESCRIPTION: Function will generate Authorization Token Using Control Room URL Credentials
# INPUT: NA
# RETURN: Returns authorization token required to get the devices list.
Function generateToken{

Try
{
	$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
	$headers.Add("Accept-Charset", 'UTF-8')
	$headers.Add("Accept", 'application/json')
	$params = @{"@type"="login";
				"username"=$controlRoomUsername;
				"password"=$controlRoomPassword;
	}
	$URL = $authenticationURL

	$httpRequest =  Invoke-WebRequest -Uri $URL -UseBasicParsing -Method POST -Body ($params|ConvertTo-Json) -ContentType "application/json" | ConvertFrom-Json | Select token
	$headers['token'] = $httpRequest.token
	getDevicesList $headers['token']
}
# Perform Error handling
Catch
{
		
		$errordata = "[ERROR004]: [$($date)]: [Service Invocation Error]: Exception occurred in generateToken method. Exception is : "  + $_.Exception.Message
		write-output($errordata)
}
finally
{
		#close the response object
		if($httpResponce -ne $null)
		{
			$httpResponce.Close()
		}
		
}

}
#=== FUNCTION ====================================================================================
# NAME: getDevicesList()
# DESCRIPTION: Function will get device details from the control tower
# INPUT: authorizationToken
# RETURN: NA.

Function getDevicesList($authorizationToken)
{
Try{
	$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
	$headers.Add("Accept-Charset", 'UTF-8')
	$headers.Add("Accept", 'application/json')
	$headers.Add("X-Authorization", $authorizationToken)
	$URL = $devicesListURL
  
	$body = [ordered]@{
	"sort" = @(
		@{
		    "field"="hostName";
			"direction"="asc";		
		}
		)
	}
	  
		  
	$httpRequest =  Invoke-WebRequest -Uri $URL -UseBasicParsing -Headers $headers -Method POST -Body ($body|ConvertTo-Json) -ContentType "application/json" 

	#check the status code
	if ($httpRequest.statusCode -eq 200) {		
		write-output("status=success")		
	}
	else {		
		write-output("status=failure")
		write-output("[ERROR005]: [$($date)]:[Service Invocation Error]: Error occurred in getDevicesList method.")
	}
	
	write-output("deployeddevicesstatus:" + $httpRequest)
			
}
# Perform Error handling
Catch
{
				
		$errordata = "[ERROR006]: [$($date)]: [Service Invocation Error]: Exception occurred in getDevicesList method. Exception is : "  + $_.Exception.Message
		write-output($errordata)
}
finally
{
		#close the response object
		if($httpResponce -ne $null)
		{
			$httpResponce.Close()
		}
		
}

}

Try
{
 generateToken
}
Catch
{
		$errordata = "[ERROR007]: [$($date)]: [Unable to Connect to Website]: Exception occurred in generateToken method. Exception is : "  + $_.Exception.Message
		write-output($errordata)
		
}






