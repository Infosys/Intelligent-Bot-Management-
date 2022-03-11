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
#FILE : <ReportListOfLinuxProcesses.sh>
#USAGE : sh ReportListOfLinuxProcesses.sh 
#EXAMPLE: VALUES : sh ReportListOfLinuxProcesses.sh 
#DESCRIPTION  : This Script used to show the Linux processes. It provides a dynamic real-time view of the running system. 
# It shows the summary information of the system and the list of processes or threads which are currently managed by the Linux Kernel.
#REQUIREMENTS : Find CPU bottlenecks
#INPUT : no parameters
#    
#OUTPUT : If CPU bottle necks exist - "status=success" and displays 
#         number of bottlenecks.
#         If CPU bottle necks doesnot exist 
#          status=success
#          Details: <<Top command output>>
#CREATED DATE : 03/09/2020
#REVISION DATE: 03/09/2020
#Exception: NA
#Script starts

NOW=$(date +"%F_%T")
top_output="top_output_$NOW.txt"   
top -b -n 1 > $top_output
first_line="YES"
while read line
    do
      if [[ $first_line = "YES" ]]; then 
            echo "status=success"
            echo "Details:"
            first_line="NO"       
        fi  
         echo " \r\n "
         echo "$line"
done < $top_output

if [ -f "$top_output" ]; then
   # Remove the created file
   rm $top_output
fi

#Script Ending