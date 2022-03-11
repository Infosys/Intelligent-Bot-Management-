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
#FILE : <FileSystemCleanUp.sh>
#USAGE : sh FileSystemCleanUp.sh [param1] 
#EXAMPLE: VALUES : sh FileSystemCleanUp.sh path=/home/manauser/temp/log/ filetype=*.log
#DESCRIPTION  : The Script checks the status of the linux file system and clean up the log file and 
#  Check the file It displays the used percentage of the file system.
#REQUIREMENTS : File System Status
#INPUT : 
#    param1 - path
#    param2 - filetype (optional, if not specify, default value *.log)
#OUTPUT : If path not provided error message is displayed.
#         If path is not valid then error message is displayed. 
#         If valid path is provide it displayes 
#          status=success
#          Before cleanup - Disk Space Used Percentage=50%
#          Number of File Deleted=100    
#          After cleanup - Disk Space Used Percentage=25%    
#CREATED DATE : 20/02/2020
#REVISION DATE: 
#Exception: NA
#Script starts


for var in "$@"
do
IFS='=' read -r -a param <<< "$var"
    if [ ${param[0]} == "path" ]; then
        path=${param[1]}
    elif [ ${param[0]} == "filetype" ]; then
        filetype=${param[1]}
    fi
done

if [[ $filetype = "" ]]; then
    filetype="*.log"
fi

if [[ $path = "" ]]; then
    error="Error001: Path not provided.\nPlease provide arguments in the format -[path] [filetype] [target]"
    echo -e $error
else
        if [[ -d $path ]] ; then
                cd $path             
                used_percentage=`df . -h | tail -1 |awk '{print $5}'`
                no_of_files_deleted=0                            
                for logfile in $filetype
                do
                    filename=${logfile}
                    if [ -f "$filename" ]; then
                        rm $filename
                        no_of_files_deleted=`expr $no_of_files_deleted + 1`
                    fi
                done                 
                echo "status=success"
                echo "Before cleanup - Disk Space Used Percentage=$used_percentage"                      
                used_percentage=`df . -h | tail -1 |awk '{print $5}'`
                echo "After cleanup - Disk Space Used Percentage=$used_percentage"     
                echo "Details: \r\n Number of File Deleted=$no_of_files_deleted "  
        else
            error="Error= Directory $path does not exists."
            echo $error
        fi
fi
