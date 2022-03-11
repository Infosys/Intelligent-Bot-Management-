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
# FILE: performDeployBot.ps1
#
# USAGE: PowerShell.exe -File <location of the script>.\performDeployBot.ps1 -taskRelativePath <Relative path of bot> 
# -botClient <Client Machine> -botUser <Bot Username> -controlRoomBaseURI <ControlRoom Base URI> -controlRoomUsername<ControlRoom Username>  
# -controlRoomPassword<ControlRoom Password>
#
# DESCRIPTION: This script generates an authentication token using the Automation Anywhere ControlRoom
# Details and passes this token to deploy the Bot specified by the user in the specified client.
#
# PREREQUISITES: 
#     Machine must be Windows machine

# INPUT: taskRelativePath,botClient,botUser,controlRoomBaseURI,controlRoomUsername,controlRoomPassword.
# NOTE: Kindly enter the inputs in following sequence :
# taskRelativePath  botClientUser  controlRoomBaseURI  ControlRoomUsername  ControlRoomPassword 
# OUTPUT: Deploys Bot In Client Specified by User.
# Author: Gauri Joshi
#=================================================================================================

#validate the arguments.
[cmdletbinding()]
param(
    [string] $taskRelativePath ,
	[string] $botClient , 
	[string] $botUser ,
	[string] $controlRoomBaseURI ,
	[string] $controlRoomUsername , 
	[string] $controlRoomPassword
)


# Variable to display error message
[string] $errordata = "No Error Found"
[string] $authenticationURL= $controlRoomBaseURI+"v1/authentication"
[string] $deploymentURL = $controlRoomBaseURI+"v1/schedule/automations/deploy"

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
If (-not $PSBoundParameters.ContainsKey('taskRelativePath')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - taskRelativePath"
    Write-Output ("error=" + $errordata)
	break

}

elseif (-not $PSBoundParameters.ContainsKey('botClient')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - botUser"
    Write-Output ("error=" + $errordata)
	break

}

elseif (-not $PSBoundParameters.ContainsKey('botUser')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - botUser"
    Write-Output ("error=" + $errordata)
	break

}

elseif(-not $PSBoundParameters.ContainsKey('controlRoomBaseURI')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - controlRoomBaseURI"
    Write-Output ("error=" + $errordata)
	break

}

elseif(-not $PSBoundParameters.ContainsKey('controlRoomUsername')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - controlRoomUsername"
    Write-Output ("error=" + $errordata)
	break
}

elseif (-not $PSBoundParameters.ContainsKey('controlRoomPassword')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - controlRoomPassword"
    Write-Output ("error=" + $errordata)
	break
}

#=== FUNCTION ========================================================================================
# NAME: generateToken()
# DESCRIPTION: Function will generate Authorization Token Using Control Room URL Credentials
# INPUT: NA
# RETURN: Returns authorization token required to deploy bot in client.
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
	deployBot $headers['token']
}
# Perform Error handling
Catch
{
		
		$errordata = "[ERROR002]: [$($date)]: [Unable to Connect to Website]: Exception occurred. Exception is : "  + $_.Exception.Message
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
# NAME: deployBot()
# DESCRIPTION: Function will deploy bot in specified client provided by user
# INPUT: authorizationToken
# RETURN: NA.

Function deployBot($authorizationToken)
{
Try{
	
	$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
	$headers.Add("Accept-Charset", 'UTF-8')
	$headers.Add("Accept", 'application/json')
	$headers.Add("X-Authorization", $authorizationToken)
	$URL = $deploymentURL
	$subBotRunners=@()
	$subBotRunners += [pscustomobject]@{
    'client'=$botClient;
    'user'=$botUser;
	
}
	$params = [pscustomobject]@{
				"taskRelativePath"=$taskRelativePath;
				"botRunners"=$subBotRunners;
      }
	  
	  
	  $httpRequest =  Invoke-WebRequest -Uri $URL -UseBasicParsing -Headers $headers -Method POST -Body ($params|ConvertTo-Json) -ContentType "application/json" 
	  
write-output("Status code is " + $httpRequest.statusCode)
	
	
	#check the status code
		if ($httpRequest.statusCode -eq 200) {
			write-output("Bot Successfully Deployed To Specified Client")
			write-output("status=success")
			
			
		}
		else {
			
			write-output("status=failure")
		}
}
# Perform Error handling
Catch
{
		write-output("Inside second catch")
		
		$errordata = "[ERROR002]: [$($date)]: [Unable to Connect to Website]: Exception occurred. Exception is : "  + $_.Exception.Message
		write-output($errordata)
}
finally
{
		#close the response object
		if($httpResponce -ne $null)
		{
			$httpResponce.Close()
		}
		#
}

}

Try
{
 generateToken
}
Catch
{
		
		$errordata = "[ERROR002]: [$($date)]: [Unable to Connect to Website]: Exception occurred. Exception is : "  + $_.Exception.Message
		write-output($errordata)
		
}






