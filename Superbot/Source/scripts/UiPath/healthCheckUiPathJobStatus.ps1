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
# FILE: healthCheckUiPathJobStatus.ps1
#
# USAGE: PowerShell.exe -File <location of the script>\healthCheckUiPathJobStatus.ps1 -authenticationURI <Authentication URI> -jobStatusServiceURI <Job Status Service URI>  -clientId <Client Id> -refreshToken <Refresh Token> -uiPathTenantName <uiPath TenantName> -robotId <Robot Id> -releaseName <Release Name>  -userName <User Name> -password <password>
#
# DESCRIPTION: This script generates an authentication token using the authentication URL
# and passes this token to get Job execution details based on the Robot ID , Release Name
#
# PRE-REQUISITES: 
#     Machine must be Windows machine
#
# INPUT: authenticationURI,jobStatusServiceURI,clientId,refreshToken,uiPathTenantName,robotId,releaseName,userName,password
# NOTE: Kindly enter the inputs in following sequence :
# authenticationURI,jobStatusServiceURI,clientId,refreshToken,uiPathTenantName,robotId,releaseName,userName,password
# Example: authenticationURI = "https://account.uipath.com/oauth/token"
#          Final jobStatusServiceURI = #https://cloud.uipath.com/infoszwekhof/InfosysDefapyud470494/odata/Jobs?$top=1&$filter=Robot/Id eq 145875 and ReleaseName eq 'testProcess2_TestEnvironment'&$orderby=EndTime desc
# OUTPUT: Get Job execution details for the Robot based on the Robot ID , Release Name
# Author: Ananthkumar_S
#=================================================================================================

#validate the arguments.
[cmdletbinding()]
param(
    [string] $authenticationURI ,
	[string] $jobStatusServiceURI , 
	[string] $clientId ,
	[string] $refreshToken ,
	[string] $uiPathTenantName,
	[string] $robotId,
	[string] $releaseName,
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

elseif (-not $PSBoundParameters.ContainsKey('jobStatusServiceURI')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - jobStatusServiceURI"
    Write-Output ("error=" + $errordata)
	break

}


elseif(-not $PSBoundParameters.ContainsKey('uiPathTenantName')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - uiPathTenantName"
    Write-Output ("error=" + $errordata)
	break
}

elseif(-not $PSBoundParameters.ContainsKey('robotId')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - robotId"
    Write-Output ("error=" + $errordata)
	break
}

elseif(-not $PSBoundParameters.ContainsKey('releaseName')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - releaseName"
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
	getJobExecutionDetails $accessToken
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
	getJobExecutionDetails $accessToken
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
# NAME: getJobExecutionDetails()
# DESCRIPTION: Function will get job execution details from the Orchestrator
# INPUT: authorizationToken
# RETURN: NA.

Function getJobExecutionDetails($authorizationToken)
{
Try{
    #write-output($authorizationToken)
	$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
	$headers.Add("Authorization", $authorizationToken)
	$headers.Add("X-UIPATH-TenantName", $uiPathTenantName)
	
	$filter1='?$top=1&$filter=Robot/Id eq '
	$filter2=" and ReleaseName eq '"
	$filter3="'&"
	$filter4='$orderby=EndTime desc'
	#Construct full Service URL
	$URL = $jobStatusServiceURI+$filter1+$robotId+$filter2+$releaseName+$filter3+$filter4
	#write-output("$URL") 
	[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12	  
	$httpRequest =  Invoke-WebRequest -Uri $URL -UseBasicParsing -Headers $headers -Method GET -ContentType "application/json" 

    #write-output($httpRequest.statusCode)	
	#check the status code
	if ($httpRequest.statusCode -eq 200) {		
		write-output("status=success")		
	}
	else {		
		write-output("status=failure")
		write-output("[ERROR005]: [$($date)]:[Service Invocation Error]: Error occurred in getJobExecutionDetails method.")
	}
	
	write-output("JobStatus:" + $httpRequest)
			
}
# Perform Error handling
Catch
{
				
		$errordata = "[ERROR006]: [$($date)]: [Service Invocation Error]: Exception occurred in getJobExecutionDetails method. Exception is : "  + $_.Exception.Message
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






