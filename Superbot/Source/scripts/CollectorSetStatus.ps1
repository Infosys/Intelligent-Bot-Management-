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
# FILE: CollectorSetStatus.ps1
#
# USAGE: PowerShell.exe -File <location of the script>\CollectorSetStatus.ps1 -dataCollectorSet <Data Collector Set Name> 
# -action <Action> -serverName <Server Name> 
#
# DESCRIPTION: This script starts and stops the Specified Data Collector Set in the remote server.
#
# PREREQUISITES: 
#     Machine must be Windows machine.
#	  User must be an admin for the remote server.

# INPUT: dataCollectorSet,action,serverName
# NOTE: Kindly enter the inputs in following sequence :
# dataCollectorSet  action  serverName
# OUTPUT: Starts/Stops the Data Collector Set in the remote server Specified by User.
# Author: Vedika Shrivastava
#=================================================================================================

#validate the arguments.
[cmdletbinding()]
param(
    [string] $dataCollectorSet ,
	[string] $action ,
	[string] $serverName
	
)

#=== FUNCTION ======================================================================================
# NAME: CollectorSetStatusChange()
# DESCRIPTION: This Function starts/stops the specified Data Collector Set.
# INPUT: NA
# RETURN: NA
Function CollectorSetStatusChange 
{
	$DataCollector = new-object -COM Pla.DataCollectorSet 

	#Query the Collector Set given by user.
	$DataCollector.Query($dataCollectorSet,$serverName)

	#check the Action(start/stop)
		switch($action) 
		{
			"start" {
					#check the current status 
						if ( $DataCollector.Status -eq 0  ){
							$DataCollector.Start(0) 
							Write-Host "Started........"
						}
						else{
						Write-Host "Already Running........"
						}
					}
					
			"stop" {
					#check the current status 
						if ( $DataCollector.Status -ne 0  ){
							$DataCollector.Stop(0) 
							Write-Host "Stopped........"
						}
						else{
						Write-Host "Already Stopped........"
						}				
					}
		}
}

Try{
#Function Call
 CollectorSetStatusChange
}

#Perform Error Handling
Catch{
write-output("Inside catch")
Write-Host $_
}
