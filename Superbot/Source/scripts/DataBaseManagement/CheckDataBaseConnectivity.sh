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
#FILE : <CheckDataBaseConnectivity.sh>
#USAGE : sh CheckDataBaseConnectivity.sh [param1] [param2] [param3] [param4] [param5]
    #Eg: sh CheckDataBaseConnectivity.sh dbusername=orauser dbpassword=***** ip=10.177.120.76 port=1521 serviceid=XE
#DESCRIPTION  : It is used to check whether the connection to Oracle database is 
#               established and sends a success or failure message on execution.
#REQUIREMENTS : Establish Oracle DB Connection 
#INPUT :
#    param1 - dbusername
#    param2 - dbpassword
#    param3 - ip (optional, If Oracle is running in remote m/c it is required)
#    param4 - port (optional, If Oracle is running in remote m/c it is required)
#    param5 - serviceid (optional, If Oracle is running in remote m/c it is required)
#OUTPUT : If database connection obtained - "ishealthy=yes".
#         If database connection failed - "ishealthy=no" along with error 
#details
#         If provided dbusername is incorrect - "error=invalid_dbusername/dbpassword"
#         If provided dbpassword is incorrect - "error=invalid_dbusername/dbpassword"
#         If provided ip is incorrect - "error=invalid_ip"
#         If provided port is incorrect - "error=invalid_port"
#         If provided serviceid/Service name is incorrect - "error=invalid_sid"
#CREATED DATE : 13/02/2017 
#REVISION DATE: 20/02/2020
# Exception: NA
# Script starts
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
    echo -e "Error001: Username not provided.\nPlease provide arguments in the format
- [dbusername] [dbpassword] [ip] [port] [serviceid]"
elif [[ $dbpassword = "" ]]; then
    echo -e "Error002: Password not provided.\nPlease provide arguments in the format
- [dbusername] [dbpassword] [ip] [port] [serviceid]"
else

if [ -z "$serverip" ]; then
    connStr=$dbusername/$dbpassword
else
    connStr=$dbusername/$dbpassword@$serverip:$port/$sid
fi

#echo "INFO:`date`:connection string $connStr"


export ORACLE_HOME=/u01/app/oracle/product/11.2.0/xe
export ORACLE_SID=XE
export NLS_LANG=`$ORACLE_HOME/bin/nls_lang.sh`
export PATH=$ORACLE_HOME/bin:$PATH

status=`sqlplus -s /nolog<<EOF
CONNECT $connStr
select sysdate from dual;
EXIT;
EOF`
case $status in
    *"ORA-01017"*) echo "error=ORA-01017:`date`:invalid_username/password";;
    *"ORA-12170"*) echo "error=ORA-12170:`date`:invalid_ip";;
    *"ORA-12154"*) echo "error=ORA-12154:`date`:invalid_ip";;
    *"ORA-12541"*) echo "error=ORA-12541:`date`:invalid_port";;
    *"ORA-12514"*) echo "error=ORA-12514:`date`:invalid_sid";;
    *"SYSDATE"*) echo "ishealthy=yes";;
    *) echo -e "ishealthy=no\nError Details:\n--------------\n $status";;
esac 
fi
# Script Ending