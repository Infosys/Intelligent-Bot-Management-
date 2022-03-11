#===============================================================================================
# 2020 Infosys Limited, Bangalore, India. All Rights Reserved.
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
# FILE: getUiPathProcessDetails.ps1
#
# USAGE: PowerShell.exe -File <location of the script>\getUiPathProcessDetails.ps1 -authenticationURI <Authentication URI> -processSerivceURI <Process Service URI>  -clientId <Client Id> -refreshToken <Refresh Token> -uiPathTenantName <uiPath TenantName> -userName <User Name> -password <password>
#
# DESCRIPTION: This script generates an authentication token using the authentication URL
# and passes this token to get Process details from Orchestrator
#
# PRE-REQUISITES: 
#     Machine must be Windows machine
#
# INPUT: authenticationURI,processSerivceURI,clientId,refreshToken,uiPathTenantName,userName,password
# NOTE: Kindly enter the inputs in following sequence :
# authenticationURI,processSerivceURI,clientId,refreshToken,uiPathTenantName,userName,password
# Example: authenticationURI = "https://account.uipath.com/oauth/token"
#          Final processSerivceURI = "https://cloud.uipath.com/infoszwekhof/InfosysDefapyud470494/odata/Releases"
# OUTPUT: List of Process details from the Orchestrator.
# Author: Ananthkumar_S
#=================================================================================================

#validate the arguments.
[cmdletbinding()]
param(
    [string] $authenticationURI ,
	[string] $processSerivceURI , 
	[string] $clientId ,
	[string] $refreshToken ,
	[string] $uiPathTenantName,
	[string] $userName,
	[string] $password	
)


# Variable to display error message
[string] $errordata = "No Error Found"
[string] $useToken="NA"


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
If (-not $PSBoundParameters.ContainsKey('authenticationURI')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - authenticationURI"
    Write-Output ("error=" + $errordata)
	break

}

elseif (-not $PSBoundParameters.ContainsKey('processSerivceURI')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - processSerivceURI"
    Write-Output ("error=" + $errordata)
	break

}

elseif(-not $PSBoundParameters.ContainsKey('uiPathTenantName')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - uiPathTenantName"
    Write-Output ("error=" + $errordata)
	break
}

If ($PSBoundParameters.ContainsKey('clientId') -And $PSBoundParameters.ContainsKey('refreshToken') -And  ![string]::IsNullOrWhiteSpace($clientId) -And ![string]::IsNullOrWhiteSpace($refreshToken)){
	$useToken = "Yes"
}
ElseIf ($PSBoundParameters.ContainsKey('userName') -And $PSBoundParameters.ContainsKey('password') -And  ![string]::IsNullOrWhiteSpace($userName) -And ![string]::IsNullOrWhiteSpace($password)){
	$useToken = "No"
}

If ($useToken -eq "NA") {
    $date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameters - either clientId,refreshToken or userName,password"
    Write-Output ("error=" + $errordata)
	break
}



#=== FUNCTION ========================================================================================
# NAME: generateToken()
# DESCRIPTION: Function will generate Authorization Token Using Orchestrator URL Credentials
# INPUT: NA
# RETURN: Returns authorization token required to call other services.
Function generateToken{

Try
{
	$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
	$headers.Add("Content-Type", 'application/json')
	$headers.Add("X-UIPATH-TenantName", $uiPathTenantName)
	$params = @{
				"grant_type"="refresh_token";
				"client_id"=$clientId;
				"refresh_token"=$refreshToken;
	}
	$URL = $authenticationURI

    #write-output($URL)
	#write-output($params|ConvertTo-Json)
	$httpRequest =  Invoke-WebRequest -Uri $URL -UseBasicParsing -Headers $headers -Method POST  -Body ($params|ConvertTo-Json) -ContentType "application/json" | ConvertFrom-Json | Select access_token
	$token = $httpRequest.access_token
	$accessToken = "Bearer $token"
	#write-output($accessToken)
	getProcessDetails $accessToken
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
#=== FUNCTION ========================================================================================
# NAME: generateTokenUsingUserName()
# DESCRIPTION: Function will generate Authorization Token Using Orchestrator user name and password
# INPUT: NA
# RETURN: Returns authorization token required to call other services.
Function generateTokenUsingUserName{

Try
{
	$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
	$headers.Add("Content-Type", 'application/json')
	$params = @{
				"tenancyName"=$uiPathTenantName;
				"usernameOrEmailAddress"=$userName;
				"password"=$password;
	}
	$URL = $authenticationURI

    #write-output($URL)
	#write-output($params|ConvertTo-Json)
	[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
	$httpRequest =  Invoke-WebRequest -Uri $URL -UseBasicParsing -Headers $headers -Method POST  -Body ($params|ConvertTo-Json) -ContentType "application/json" | ConvertFrom-Json | Select result
	$token = $httpRequest.result
	$accessToken = "Bearer $token"
	#write-output($accessToken)
	getProcessDetails $accessToken
}
# Perform Error handling
Catch
{
		
		$errordata = "[ERROR004]: [$($date)]: [Service Invocation Error]: Exception occurred in generateTokenUsingUserName method. Exception is : "  + $_.Exception.Message
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
# NAME: getProcessDetails()
# DESCRIPTION: Function will get Process details from the Orchestrator
# INPUT: authorizationToken
# RETURN: NA.

Function getProcessDetails($authorizationToken)
{
Try{
    #write-output($authorizationToken)
	$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
	$headers.Add("Authorization", $authorizationToken)
	$headers.Add("X-UIPATH-TenantName", $uiPathTenantName)
	$URL = "$processSerivceURI"
	#write-output("$URL") 
	[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12	  
	$httpRequest =  Invoke-WebRequest -Uri $URL -UseBasicParsing -Headers $headers -Method GET -ContentType "application/json" 

	#check the status code
	#write-output($httpRequest.statusCode)
	if ($httpRequest.statusCode -eq 200) {		
		write-output("status=success")		
	}
	else {		
		write-output("status=failure")
		write-output("[ERROR005]: [$($date)]:[Service Invocation Error]: Error occurred in getProcessDetails method.")
	}
	
	write-output("ProcessDetails:" + $httpRequest)
			
}
# Perform Error handling
Catch
{
				
		$errordata = "[ERROR006]: [$($date)]: [Service Invocation Error]: Exception occurred in getProcessDetails method. Exception is : "  + $_.Exception.Message
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
	If ($useToken -eq "Yes") {
		generateToken
	} 
	ElseIf ($useToken -eq "No") {
	  generateTokenUsingUserName
	}
}
Catch
{
		$errordata = "[ERROR007]: [$($date)]: [Unable to Connect to Website]: Exception occurred in generateToken method. Exception is : "  + $_.Exception.Message
		write-output($errordata)
		
}






