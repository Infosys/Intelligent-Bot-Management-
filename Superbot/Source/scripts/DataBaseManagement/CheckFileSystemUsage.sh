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
#
#======================================================================================
#
#FILE : <CheckFileSystemUsage.sh>
#USAGE : sh CheckFileSystemUsage.sh [param1] 
#EXAMPLE: VALUES : sh CheckFileSystemUsage.sh path=/u01/app/oracle/product/11.2.0/xe
#DESCRIPTION  : The Script checks the status of the oracle file system. It displays the used percentage of the storage mount that is used by oracle as its file system. .
#REQUIREMENTS : File System Status
#INPUT : 
#    param1 - path
#OUTPUT : If path not provided error message is displayed.
#         If path is not valid then error message is displayed. 
#         If valid path is provide it displayes the 
#				Total space used in percentage
#CREATED DATE : 20/02/2020
#REVISION DATE: 
#Exception: NA
#Script starts
#This function is used to navigate to the specified location
IFS='=' read -r -a param <<< "$1"
path=${param[1]}
if [[ $path = "" ]]; then
	error="Error001: Path not provided.\nPlease provide arguments in the format -[path]"
    echo -e $error
else
		if [[ -d $path ]] ; then
				cd $path
#======================================================================================
#Calculate file system space in $path location
#======================================================================================
				totalspace=`df . -h | tail -1 |awk '{print$2}'`
				usedspace=`df . -h | tail -1 |awk '{print$3}'`
				freespace=`df . -h | tail -1 |awk '{print $4}'`
				usedpercentage=`df . -h | tail -1 |awk '{print $5}'`
				mountedon=`df . -h | tail -1 |awk '{print $6}'`
				pathusedsize=`du -sh . |awk '{print ($1)}'`
				#echo "totalspace=$totalspace"
				#echo "usedspace=$usedspace"
				#echo "freespace=$freespace"
				#echo "spaceusedbydb:${spaceusedbydb}G"
				#echo "usedpercentage:$usedpercentage"
				#if [[ $pathusedsize =~ M ]]; then
				#	dbusage=`du -sh . |awk '{print ($1/1024)}'`
				#elif [[ $pathusedsize =~ K ]]; then
				#	dbusage = `du -sh . |awk '{print ($1/1024/1024)}'`
				#else 
				#echo "space calculated"
				#fi
				#echo "mountedon:$mountedon"
				result=$( echo "$usedpercentage" | cut -d '%' -f 1 )
				echo "status=success"
				echo "Disk Space Used Percentage=$result"
		else
			error="Error= Directory $path does not exists."
			echo $error
		fi
fi
