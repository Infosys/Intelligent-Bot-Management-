#====================================================================================== 
# © 2020  Infosys Limited, Bangalore, India. All Rights Reserved.
# Version: 1.0
# Except for any open source software components embedded in this Infosys proprietary 
#software program ("Program"),this Program is protected by copyright laws, 
#international treaties and other pending or existing intellectual property rights in
#India, the United States and other countries. Except as expressly permitted, any 
#unauthorized reproduction, storage, transmission in any form or by any means 
#(including without limitation electronic, mechanical,printing, photocopying, recording
#or otherwise), or any distribution of this Program, or any portion of it, may
#results in severe civil and criminal penalties, and will be prosecuted to the maximum
#extent possible under the law.
#
#======================================================================================
#
#FILE : <CheckPGAMemoryUsage.sh>
#USAGE : sh CheckPGAMemoryUsage.sh [param1] [param2] [param3] [param4] [param5]
    #Eg: sh CheckPGAMemoryUsage.sh dbusername=orauser dbpassword=orapasswd1 ip=10.177.120.76 port=1521 serviceid=XE
#DESCRIPTION  : This script displays Program Global Area (PGA) Memory Used Percentage in the oracle database 
#               
#REQUIREMENTS : Memory Reporting
#INPUT : 
#    param1 - dbusername
#    param2 - dbpassword
#    param3 - ip (optional, If Oracle is running in remote m/c it is required)
#    param4 - port (optional, If Oracle is running in remote m/c it is required)
#    param5 - serviceid (optional, If Oracle is running in remote m/c it is required)
#OUTPUT : 
#along with error details details.
#         If provided dbusername is incorrect - "error=invalid_dbusername/dbpassword"
#         If provided dbpassword is incorrect - "error=invalid_dbusername/dbpassword"
#         If provided serverip is incorrect - "error=invalid_serverip"
#         If provided port is incorrect - "error=invalid_port"
#         If provided sid/Service name is incorrect - "error=invalid_sid"
#         status=success
#         PGA Memory Used Percentage=70
#CREATED DATE : 20/02/2020
#REVISION DATE: 
#Exception: NA
#Script starts
process="v\$process"
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

export ORACLE_HOME=/u01/app/oracle/product/11.2.0/xe
export ORACLE_SID=XE
export NLS_LANG=`$ORACLE_HOME/bin/nls_lang.sh`
export PATH=$ORACLE_HOME/bin:$PATH

if [ -z "$serverip" ]; then
    connStr=$dbusername/$dbpassword
else
    connStr=$dbusername/$dbpassword@$serverip:$port/$sid
fi
	

result_data_memory=`sqlplus -s /nolog<<EOF
CONNECT $connStr
set echo off 
set heading off
spool usedmemory.txt
select 'Metric Value=' as MetricValue,(used*100)/alloc usedpercentage from ( 
SELECT sum(p.pga_max_mem) max, sum(p.pga_alloc_mem) alloc, sum(p.pga_used_mem) used,
sum(p.pga_freeable_mem) free FROM $process p where p.program != 'PSEUDO' );
exit;
EOF`

while read line
do
	if [[ $line == *"Metric Value="* ]]; then
		result=$( echo "$line" |cut -d '=' -f 2 | tr -d "[:space:]" )
		result=$( echo "$result" | cut -d '.' -f 1 )
	fi	
done < usedmemory.txt

case $result_data_memory in
    *"ORA-01017"*) echo "error=ORA-01017:`date`:invalid_username/password";;
    *"ORA-12170"*) echo "error=ORA-12170:`date`:invalid_ip";;
    *"ORA-12154"*) echo "error=ORA-12154:`date`:invalid_ip";;
    *"ORA-12541"*) echo "error=ORA-12541:`date`:invalid_port";;
    *"ORA-12514"*) echo "error=ORA-12514:`date`:invalid_sid";;
    *"Metric Value="*) echo "status=success"
		echo "PGA Memory Used Percentage=$result";;
    *) echo -e "status=failure\nError Details:\n--------------\n$result_data_memory";;
esac
fi


# Script Ending
