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
#FILE : <RemoveBlockedUserTransactions.sh>
#USAGE : sh RemoveBlockedUserTransactions.sh [param1] [param2] [param3] [param4] [param5]
    #Eg: sh RemoveBlockedUserTransactions.sh dbusername=orauser dbpassword=*****
    #Eg: sh RemoveBlockedUserTransactions.sh dbusername=orauser dbpassword=***** ip=ip-10-177-120-76.ec2.internal port=1521 serviceid=XE
#DESCRIPTION  : This script is to find the number of locked user  
#which are in pending/locked state and kill those locked session
#REQUIREMENTS : Check user and his/her transactions.
#INPUT : 
#    param1 - dbusername
#    param2 - dbpassword
#    param3 - ip (optional, If Oracle is running in remote m/c it is required)
#    param4 - port (optional, If Oracle is running in remote m/c it is required)
#    param5 - serviceid (optional, If Oracle is running in remote m/c it is required)
#    
#OUTPUT : status=success
#          
#CREATED DATE : 06/Mar/2020
#REVISION DATE: 
#Exception: NA

# Script starts
session="v\$session"
process="v\$process"
transaction="v\$transaction"
rollstat="v\$rollstat"
rollname="v\$rollname"
lock="v\$lock" 


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


export ORACLE_HOME=/u01/app/oracle/product/11.2.0/xe
export ORACLE_SID=XE
export NLS_LANG=`$ORACLE_HOME/bin/nls_lang.sh`
export PATH=$ORACLE_HOME/bin:$PATH
transactions=`sqlplus -s /nolog<<EOF
CONNECT $connStr
set echo off 
set heading off
spool transactions.txt
select distinct s.sid ||','|| s.serial#
        from $session s,$process p,$transaction t,$rollstat r,$rollname n,$lock l 
        where s.paddr = p.addr 
        and s.taddr = t.addr (+)
        and t.xidusn = r.usn (+)
        and r.usn = n.usn (+)
        and s.sid = l.sid  
        and l.block=1;
exit;
EOF`

    error_occured=" ";
    case $transactions in
        *"ORA-01017"*) 
                echo "error=ORA-01017:`date`:invalid_username/password"
                error_occured="YES";;
        *"ORA-12170"*) 
                echo "error=ORA-12170:`date`:invalid_ip"
                error_occured="YES";;
        *"ORA-12154"*) 
                echo "error=ORA-12154:`date`:invalid_ip"
                error_occured="YES";;
        *"ORA-12541"*) 
                echo "error=ORA-12541:`date`:invalid_port"
                error_occured="YES";;
        *"ORA-12514"*) 
                echo "error=ORA-12514:`date`:invalid_sid"
                error_occured="YES";;   
    esac
    
    while read line
        do        
        if [[ $line =~ rows ]]; then
           continue
        elif [ -z "$line" ]; then
           continue
        elif [[ $line =~ ^[0-9]*,[0-9]* ]]; then
            result_data_transactions+=("$line")               
        fi
    done < transactions.txt

    NOW=$(date +"%F_%T")
    sql_file="sql_file_$NOW.sql"   
    sqlcommand="";
    result_size=${#result_data_transactions[*]}
    last_record=$result_size-1
    output_details=""
    newline=" \r\n "
     for ((i=0; i < $result_size; i++))
        do    
           
            if [[ $i -eq 0 ]]; then
                echo "ALTER SYSTEM KILL SESSION '${result_data_transactions[i]}';" > $sql_file
                heading_details="sid,serial#"
                output_details=$newline$heading_details$newline
            else
                echo "ALTER SYSTEM KILL SESSION '${result_data_transactions[i]}';" >> $sql_file
            fi    
            detail=${result_data_transactions[i]}
            output_details=$output_details$detail$newline
           # if [[ $i -eq $last_record ]]; then
           #     echo "exit;" >> $sql_file
           # fi            
     done
        
    if [ -f "$sql_file" ]; then
    
alterdetails=`sqlplus -s /nolog<<EOF
CONNECT $connStr
whenever sqlerror exit sql.sqlcode;
set echo off 
set heading off
spool alterdetails.txt
@$sql_file
exit;
EOF`

        alter_error_occured=" "
        case $alterdetails in
        *"ORA-01017"*) 
                echo "error=ORA-01017:`date`:invalid_username/password"
                alter_error_occured="YES";;
        *"ORA-12170"*) 
                echo "error=ORA-12170:`date`:invalid_ip"
                alter_error_occured="YES";;
        *"ORA-12154"*) 
                echo "error=ORA-12154:`date`:invalid_ip"
                alter_error_occured="YES";;
        *"ORA-12541"*) 
                echo "error=ORA-12541:`date`:invalid_port"
                alter_error_occured="YES";;
        *"ORA-12514"*) 
                echo "error=ORA-12514:`date`:invalid_sid"
                alter_error_occured="YES";;     
        *"altered"*) 
                alter_error_occured="NO";;                
        *)
                alter_error_occured="YES";;     
        esac
              
        if [[ $alter_error_occured = "YES" ]]; then 
            echo "status=failure"
            echo "Error:\n--------------\n$alterdetails"
        else
            echo "status=success"
            echo "Output:\n--------------\n$alterdetails"
            echo "Details:"
            echo "$output_details"
        fi
        # Remove the created sql file
        rm $sql_file
    else  
        if [[ $error_occured = "YES" ]]; then 
            echo "status=failure"
            echo "Error:\n--------------\n$transactions"
        else
            echo "status=success"
            echo "Details: \r\n $transactions"
        fi
    fi

fi

# Script Ending
