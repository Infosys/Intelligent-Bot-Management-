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
#FILE : <CheckActiveHighLoadSQLStatements.sh>
#USAGE : sh CheckActiveHighLoadSQLStatements.sh [param1] [param2] [param3] [param4] [param5]
#Eg: sh CheckActiveHighLoadSQLStatements.sh dbusername=orauser dbpassword=orapasswd1 ip=ip-10-177-120-76.ec2.internal port=1521 serviceid=XE executiontime=500000
#DESCRIPTION  : This script is used to establish Oracle DB Connection and check number of  
#               Active SQL Statements which are executing on High Load
#REQUIREMENTS : Establish Oracle DB Connection 
#INPUT :
#    param1 - dbusername
#    param2 - dbpassword
#    param3 - ip (optional, If Oracle is running in remote m/c it is required)
#    param4 - port (optional, If Oracle is running in remote m/c it is required)
#    param5 - serviceid (optional, If Oracle is running in remote m/c it is required)
#    param6 - executiontime (optional, if not specify, default value 10000000 microsecond)
#OUTPUT : If database connection obtained 
#         status=success
#         High Load Query=0
#         If provided dbusername is incorrect - "error=invalid dbusername/dbpassword"
#         If provided dbpassword is incorrect - "error=invalid dbusername/dbpassword"
#         If provided serverip is incorrect - "error=invalidip"
#         If provided port is incorrect - "error=invalidport"
#         If provided serviceid/Service name is incorrect - "error=invalidserviceid"
#CREATED DATE : 17/03/2020
#REVISION DATE: 17/03/2020
# Exception: NA
# Script starts
sql="v\$sql";
session="v\$session";
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
                                serviceid=${param[1]} 
				elif [ ${param[0]} == "executiontime" ]; then
                                executiontime=${param[1]} 
                fi
done

if [ -z "$serverip" ]; then
    connStr=$dbusername/$dbpassword
else
    connStr=$dbusername/$dbpassword@$serverip:$port/$serviceid
fi


#echo "INFO:`date`:connection string $connStr"
export ORACLE_HOME=/u01/app/oracle/product/11.2.0/xe
export ORACLE_SID=XE
export NLS_LANG=`$ORACLE_HOME/bin/nls_lang.sh`
export PATH=$ORACLE_HOME/bin:$PATH


 if [[ $executiontime = "" ]]; then
    ## It is microsecond 
	executiontime=10*1000*1000 
 fi

if [[ $dbusername = "" ]]; then
    echo -e "Error001: Username not provided.\nPlease provide arguments in the format
	- [dbusername] [dbpassword] [ip] [port] [serviceid] [executiontime]";
elif [[ $dbpassword = "" ]]; then
    echo -e "Error002: Password not provided.\nPlease provide arguments in the format 
	- [dbusername] [dbpassword] [serverip] [port] [serviceid] [executiontime]";
else
sqldetails=`sqlplus -s /nolog<<EOF
CONNECT $connStr
set heading off
set echo off
spool highloadsqls.txt
select 'Metric Value=' as MetricValue, count(*) as NoOfHighLoadQuery from ( 
SELECT S.SID ||','|| Q.SQL_ID ||','|| Q.PLSQL_EXEC_TIME FROM $sql Q,$session S 
WHERE S.USERNAME IS NOT NULL AND S.STATUS = 'ACTIVE' AND S.SQL_ID IS NOT NULL 
AND Q.SQL_ID = S.SQL_ID AND Q.PLSQL_EXEC_TIME>$executiontime
);
EXIT;
EOF`
#======================================================================================
#convert the output to expected format
#======================================================================================
	while read line
	do
		if [[ $line == *"Metric Value="* ]]; then
			result=$( echo "$line" |cut -d '=' -f 2 | tr -d "[:space:]" )			
		fi			
	done < highloadsqls.txt



case $sqldetails in
    *"ORA-01017"*) echo "error=ORA-01017:`date`:invalid_username/password";;
    *"ORA-12170"*) echo "error=ORA-12170:`date`:invalid_ip";;
    *"ORA-12154"*) echo "error=ORA-12154:`date`:invalid_ip";;
    *"ORA-12541"*) echo "error=ORA-12541:`date`:invalid_port";;
    *"ORA-12514"*) echo "error=ORA-12514:`date`:invalid_serviceid";;
    *"Metric Value="*) echo "status=success"
		echo "High Load Query=$result";;
    *) echo -e "status=failure\nError Details:\n--------------\n$sqldetails";;
esac
fi
#Script End

