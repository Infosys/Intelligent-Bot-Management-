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
#FILE : <PerformDBStartStop.sh>
#USAGE : sh PerformDBStartStop.sh [param1]
#        Eg: sh PerformDBStartStop.sh dbaction=start/stop/restart
#DESCRIPTION  : This shell script is used to perform Oracle auto start/stop/restart. Target database will be started , stopped and restarted through commands. 
#REQUIREMENTS : To start,stop and restart the service automatically.
#INPUT : 
#    param1 - dbaction
#OUTPUT : If DB service is started - "status=service_started".
#          If DB service is Stopped- "status=service_stopped".
#         If DB service is Retarted- "status=service_restarted".
#         If provided action is incorrect - "status=invalid_dbaction_name"
#x
#CREATED DATE : 03/18/2020
#REVISION DATE: 03/18/2020
#Exception: NA
# Script starts
IFS='=' read -r -a param <<< "$1"
dbaction=${param[1]}
if [[ $dbaction == "" ]]; then
    echo -e "Error001: dbaction not provided.\nPlease provide arguments in the format -
    [dbaction]"
else
 
case $dbaction in
    'start')
       if [ ! -f /etc/init.d/oracle-xe ]
        then
            echo "status=failure"
            echo "Error Details: Oracle startup failed , oracle-xe not found in path - /etc/init.d/"
            exit
        else
            sudo service oracle-xe start
            echo "status=success"
            echo "Details:\nDatabase_started_successfully"           
        fi
    ;;
    'stop')
        if [ ! -f /etc/init.d/oracle-xe ]
        then
            echo "status=failure"
            echo "Error Details: Oracle startup failed , oracle-xe not found in path - /etc/init.d/"
            exit
        else
            sudo service oracle-xe stop
            echo "status=success"
            echo "Details:\nDatabase_stopped_successfully"
            
        fi
    ;;
    'restart')
        if [ ! -f /etc/init.d/oracle-xe ]
        then
            echo "status=failure"
            echo "Error Details: Oracle startup failed , oracle-xe not found in path - /etc/init.d/"
            exit
        else
            sudo service oracle-xe restart
            echo "status=success"
            echo "Details:\nDatabase_restarted_successfully"            
        fi
    ;;
    *)
        echo "status=failure"
        echo "Error Details: Invalid action - $dbaction"
    ;;

esac
fi
# Script Ending

