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
# FILE: createServiceNowIncident.ps1
#
# USAGE: PowerShell.exe -File <location of the script>.\createServiceNowIncident.ps1 -snowURL <ServiceNow URL> 
# -snowUsername <ServiceNow User Name> -snowPassword <ServiceNow Password> -detailsJson <Input details in Json format String> 
#
# DESCRIPTION: This script Create Incident in ServiceNow system and return the IncidentID.
#
# INPUT: snowURL,snowUsername,snowPassword,detailsJson
# NOTE: Kindly enter the inputs in following sequence :
# snowURL  snowUsername  snowPassword  detailsJson 
# OUTPUT: ServiceNow IncidentID.
# Author: Ananthkumar S
#=================================================================================================

#validate the arguments.
[cmdletbinding()]
param(
    [string] $snowURL,
	[string] $snowUsername, 
	[string] $snowPassword,
	[string] $detailsJson
)

# Variable to display error message
[string] $errordata = "No Error Found"

[string] $incidentURL = $snowURL+"api/now/table/incident"
#Write-Output ($incidentURL)

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
If (-not $PSBoundParameters.ContainsKey('snowURL')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - snowURL"
    Write-Output ("error=" + $errordata)
	break
}
elseif (-not $PSBoundParameters.ContainsKey('snowUsername')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - snowUsername"
    Write-Output ("error=" + $errordata)
	break
}
elseif (-not $PSBoundParameters.ContainsKey('snowPassword')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - snowPassword"
    Write-Output ("error=" + $errordata)
	break
}
elseif (-not $PSBoundParameters.ContainsKey('detailsJson')){
	$date = timestamp
	$errordata = "[ERROR001]: [$($date)]: [Invalid Arguments]: Please pass required parameter - detailsJson"
    Write-Output ("error=" + $errordata)
	break
}

###############################################
# Creating Incident
###############################################


# Building Authentication Header & setting content type

$HeaderAuth = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $SNOWUsername, $SNOWPassword)))
$SNOWSessionHeader = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
$SNOWSessionHeader.Add('Authorization',('Basic {0}' -f $HeaderAuth))
$SNOWSessionHeader.Add('Accept','application/json')
$Type = "application/json"

# POST to API
Try 
{
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$IncidentPOSTResponse = Invoke-RestMethod -Method POST -Uri $incidentURL -Body $detailsJson -TimeoutSec 300 -Headers $SNOWSessionHeader -ContentType $Type
}
Catch 
{
	$errordata = "[ERROR002]: [$($date)]: [Unable to Create Incident in ServiceNow]: Exception occurred. Exception is : "  + $_.Exception.Message
	write-output($errordata)
}

# Pulling ticket ID from response
$IncidentID = $IncidentPOSTResponse.result.number

# Verifying Incident created and show ID

IF ($IncidentID -ne $null)
{
	write-output("status=success")
	write-output("Created Incident With ID:$IncidentID")
}
ELSE
{
	write-output("status=failure")
	write-output("Incident Not Created")
}

# End of script

