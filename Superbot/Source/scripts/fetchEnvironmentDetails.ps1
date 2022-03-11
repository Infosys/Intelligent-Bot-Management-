#====================================================================================== 
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
#======================================================================================
#
# FILE: fetchEnvironmentDetails.ps1
#
# USAGE: PowerShell.exe -File <location of the script>.\fetchEnvironmentDetails.ps1
#
# DESCRIPTION: This script returns the list of installed softwares along with OS details and Screen Resolution Details in json format.
#
# PREREQUISITES: 
#     Machine must be Windows machine

# INPUT: MachineName for which list of installed softwares is required.
# OUTPUT: List of installed softwares along with OS details and Screen Resolution Details in json format.
# Author: Gauri Joshi
#========================================================================================


Try{
$value1 = Get-ItemProperty -Path "HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*" -ErrorAction Stop | Select-Object DisplayName,DisplayVersion,Publisher,InstallDate 
$value2 = Get-ItemProperty -Path "HKLM:\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*" -ErrorAction Stop | Select-Object DisplayName,DisplayVersion,Publisher,InstallDate 
$value3 = Get-ItemProperty -Path "HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*" -ErrorAction Stop | Select-Object DisplayName,DisplayVersion,Publisher,InstallDate 
}

Catch {
$errordata = "[ERROR005]: Error occured in command invocation Machine name is incorrect or you dont have authentication to use the machine"
    Write-Output ("error=" + $errordata)
}

$global:a=1;
$global:jsonDoc = @();
[string] $errordata = "No Error Found"


#=== FUNCTION ============================================================================
# NAME: getOSDetails()
# DESCRIPTION: This Function returns the OS details
# INPUT: NA
# RETURN: OS details

function getOSDetails([REF]$jsonDoc){
Try{
$OsDetails =  Get-WmiObject Win32_OperatingSystem -ErrorAction Stop | Select PSComputerName, Caption, OSArchitecture, Version, BuildNumber 

$trialJSON += [pscustomobject]@{
"metricvalue" = $OsDetails;
}

$metricvalue = @()
$computername = $trialJSON.metricvalue[0].PSComputerName;
$caption = $trialJSON.metricvalue[0].Caption;
$architecture = $trialJSON.metricvalue[0].OSArchitecture;
$version = $trialJSON.metricvalue[0].Version;
$BuildNumber = $trialJSON.metricvalue[0].BuildNumber;


$metricvalue += [pscustomobject]@{
'attributename'='Computer name';
'attributevalue'="$computername";
'displayname'= 'Computer Name'
}
$metricvalue += [pscustomobject]@{
'attributename'='Caption';
'attributevalue'="$caption";
'displayname'= 'OS Name'
}
$metricvalue += [pscustomobject]@{
'attributename'='OS Architecture';
'attributevalue'="$architecture";
'displayname'= 'System Type'
}
$metricvalue += [pscustomobject]@{
'attributename'='Version';
'attributevalue'="$version";
'displayname'= 'Version'
}
$metricvalue += [pscustomobject]@{
'attributename'='Build Number';
'attributevalue'="$BuildNumber";
'displayname'= 'Build Number'
}

$jsonDoc.Value += [pscustomobject]@{
    metricid = '1';
    metricname = 'OSDetails';
	metrickey = $caption;
    metricvalue = $metricvalue
} 

return $jsonDoc.Value;

}

Catch {
$errordata = "[ERROR002]: Error occured gathering OS Details"
Write-Output ("error=" + $errordata)
}
}

#=== FUNCTION ============================================================================
# NAME: getScreenResolutionDetails()
# DESCRIPTION: This Function returns the Screen Resolution details
# INPUT: NA
# RETURN: Screen Resolution details.

function getScreenResolutionDetails([REF]$jsonDoc){
Try{
$ScreenResolution = Get-WmiObject Win32_DisplayConfiguration -ErrorAction Stop | Select PelsHeight, PelsWidth 
$trialJSON += [pscustomobject]@{
"metricvalue" = $ScreenResolution;
}
$metricvalue =@()
$pelsHeight = $trialJSON.metricvalue[0].PelsHeight;
$pelsWidth = $trialJSON.metricvalue[0].PelsWidth;
$keyvalue = "$($trialJSON.metricvalue[0].PelsHeight) * $($trialJSON.metricvalue[0].PelsWidth)";

$metricvalue += [pscustomobject]@{
'attributename'='PelsHeight';
'attributevalue'="$pelsHeight";
'displayname'= 'PelsHeight'
}

$metricvalue += [pscustomobject]@{
'attributename'='PelsWidth';
'attributevalue'="$pelsWidth";
'displayname'= 'PelsWidth'
}


$jsonDoc.Value += [pscustomobject]@{
    metricid = '1';
    metricname = 'Screen Resolution';
	metrickey = $keyvalue;
    metricvalue = $metricvalue
} 

return $jsonDoc.Value;

}
Catch{

$errordata = "[ERROR003]: Error gathering Screen Resolution Details"
    Write-Output ("error=" + $errordata)

}
}


#=== FUNCTION ============================================================================
# NAME: getInstalledSoftwareList()
# DESCRIPTION: This Function returns the final metric containing list of installed softwares along with OS details and Screen Resolution details in JSON format.
# INPUT: NA
# RETURN: List of installed softwares along with OS details and Screen Resolution Details in json format.

function getInstalledSoftwareList($value,[REF]$jsonDoc){
Try{
$trialJSON += [pscustomobject]@{
"metricvalue" = $value;
}

$b=0;
do {

$metricvalue = @()

$displayname=$trialJSON.metricvalue[$b].DisplayName;
$version=$trialJSON.metricvalue[$b].DisplayVersion;
$installeddate=$trialJSON.metricvalue[$b].InstallDate;
$publisher=$trialJSON.metricvalue[$b].Publisher;

$metricvalue += [pscustomobject]@{
    'attributename'='Display name';
    'attributevalue'="$displayname";
                'displayname'= 'Software Name'
}
$metricvalue += [pscustomobject]@{
    'attributename'='Version';
    'attributevalue'="$version";
                'displayname'= 'Software Version'
}
$metricvalue += [pscustomobject]@{
    'attributename'='InstallDate';
    'attributevalue'="$installeddate";
                'displayname'= 'Installed Date'
}
$metricvalue += [pscustomobject]@{
    'attributename'='Publisher';
    'attributevalue'="$publisher";
                'displayname'= 'Publisher'
}

$jsonDoc.Value += [pscustomobject]@{
    metricid = "$a";
    metricname = 'Installed Software';
	metrickey = $displayname;
    metricvalue = $metricvalue
}

$b++;
$global:a++;
}while($b -le $trialJSON.metricvalue.length) 

return $jsonDoc.Value;

}

Catch{
$errordata = "[ERROR004]:Error gathering list of installed softwares"
Write-Output ("error=" + $errordata)

}
}	

$jsonFile = @();
Try{
$jsonFile = getInstalledSoftwareList $value1 ([REF]$global:jsonDoc)   ;
$jsonFile = getInstalledSoftwareList $value2 ([REF]$global:jsonDoc) ;
$jsonFile = getInstalledSoftwareList $value3 ([REF]$global:jsonDoc) ;

$jsonFile = getOSDetails ([REF]$global:jsonDoc) ;
$jsonFile = getScreenResolutionDetails ([REF]$global:jsonDoc) ;



write-output($jsonFile | ConvertTo-Json -Depth 10)
#$jsonFile | ConvertTo-Json -Depth 10 | Out-File ".\customers.json"

}
Catch{

write-output("Some error occured")
}
