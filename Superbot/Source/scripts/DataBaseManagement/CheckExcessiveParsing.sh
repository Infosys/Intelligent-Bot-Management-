# ====================================================================================== 
# © 2020 Infosys Limited, Bangalore, India. All Rights Reserved.
# Version: 1.0
# Except for any open source software components embedded in this Infosys proprietary 
# software program ("Program"),this Program is protected by copyright laws, 
# international treaties and other pending or existing intellectual property rights in
# India, the United States and other countries. Except as expressly permitted, any 
# unauthorized reproduction, storage, transmission in any form or by any means 
# (including without limitation electronic, mechanical,printing, photocopying, recording
# or otherwise), or any distribution of this Program, or any portion of it, may
# results in severe civil and criminal penalties, and will be prosecuted to the maximum
# extent possible under the law.
#
# ======================================================================================
# FILE : <CheckExcessiveParsing.sh>
# USAGE : sh CheckExcessiveParsing.sh [param1] [param2] [param3] [param4] [param5]
# Eg: sh CheckExcessiveParsing.sh dbusername=orauser dbpassword=***** ip=ip-10-177-120-76.ec2.internal port=1521 serviceid=XE
# DESCRIPTION  : This script is used to find the number of  SQL queries experiencing excessive parsing.
# REQUIREMENTS : Fetch excessively parsed SQL Queries 
# INPUT :
#    param1 - dbusername
#    param2 - dbpassword
#    param3 - ip (optional, If Oracle is running in remote m/c it is required)
#    param4 - port (optional, If Oracle is running in remote m/c it is required)
#    param5 - serviceid (optional, If Oracle is running in remote m/c it is required)
# OUTPUT :
#          status=success
#          Excessive Parsing Query=1
#         If provided dbusername is incorrect - "error=invalid_dbusername/dbpassword"
#         If provided dbpassword is incorrect - "error=invalid_dbusername/dbpassword"
#         If provided serverip is incorrect - "error=invalid_serverip"
#         If provided port is incorrect - "error=invalid_port"
#         If provided sid/Service name is incorrect - "error=invalid_sid"
#         If any error occurs while fetching - "status=operation_failed"
# CREATED DATE : 20/02/2020
# REVISION DATE: 
# Exception: NA
# Script starts
export ORACLE_HOME=/u01/app/oracle/product/11.2.0/xe
export PATH=$ORACLE_HOME/bin:$PATH
sqlarea="v\$sqlarea"
for var in "$@"
do
IFS='=' read -r -a param <<< "$var"
                if [ ${param[0]} == "dbusername" ]; then
                                dbusername=${param[1]}
                elif [ ${param[0]} == "dbpassword" ]; then
                                dbpassword=${param[1]}
                elif [ ${param[0]} == "ip" ]; then
                                serverip=${param[1]}
                elif [ ${param[0]} == "port" ]; then
                                port=${param[1]}
                elif [ ${param[0]} == "serviceid" ]; then
                                sid=${param[1]}
                fi
done


if [[ $dbusername = "" ]]; then
    echo -e "Error001: Username not provided.\nPlease provide arguments in the format -
    [dbusername] [dbpassword] [ip] [port] [serviceid]"
elif [[ $dbpassword = "" ]]; then
    echo -e "Error002: Password not provided.\nPlease provide arguments in the format - 
    [dbusername] [dbpassword] [ip] [port] [serviceid]"
else

if [ -z "$serverip" ]; then
    connStr=$dbusername/$dbpassword
else
    connStr=$dbusername/$dbpassword@$serverip:$port/$sid
fi

export ORACLE_HOME=/u01/app/oracle/product/11.2.0/xe
export ORACLE_SID=XE
export NLS_LANG=`$ORACLE_HOME/bin/nls_lang.sh`
export PATH=$ORACLE_HOME/bin:$PATH

sqldetails=`sqlplus -s /nolog<<EOF
CONNECT $connStr
set echo off
set heading off
spool sqldetails.txt
select 'Metric Value=' as MetricValue, count(*) as NoOfExcessiveParsingQuery from (select sql_id
from $sqlarea
where ROWNUM <= 20 and parse_calls > 100
and kept_versions = 0
and executions > 50*parse_calls order by PARSE_CALLS desc);
exit;
EOF`


#======================================================================================
#convert the output to expected format
#======================================================================================
    while read line
    do
        if [[ $line == *"Metric Value="* ]]; then
            result=$( echo "$line" |cut -d '=' -f 2 | tr -d "[:space:]" )            
        fi    
    done < sqldetails.txt

    #rm -rf ioreads.txt;


case $sqldetails in
   *"ORA-01017"*) echo "error=ORA-01017:`date`:invalid_username/password";;
    *"ORA-12170"*) echo "error=ORA-12170:`date`:invalid_ip";;
    *"ORA-12154"*) echo "error=ORA-12154:`date`:invalid_ip";;
    *"ORA-12541"*) echo "error=ORA-12541:`date`:invalid_port";;
    *"ORA-12514"*) echo "error=ORA-12514:`date`:invalid_serviceid";;
    *"Metric Value="*) echo "status=success"
        echo "Excessive Parsing Query=$result";;
    *) echo -e "status=failure\nError Details:\n--------------\n$sqldetails";; 
esac
fi


