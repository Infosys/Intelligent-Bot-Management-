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
#FILE : <CheckCPUBottlenecks.sh>
#USAGE : sh CheckCPUBottlenecks.sh 
#EXAMPLE: VALUES : sh CheckCPUBottlenecks.sh 
#DESCRIPTION  : The only way to tell if your server has a CPU bottleneck is when the CPU 
#				runqueue values (per vmstat) exceeds the number of processors on the 
#               server. This script is used to find if CPU bottlenecks exists.
#REQUIREMENTS : Find CPU bottlenecks
#INPUT : no parameters
#    
#OUTPUT : If CPU bottle necks exist - "status=success" and displays 
#         number of bottlenecks.
#         If CPU bottle necks doesnot exist 
#		  status=success
#		  CPU Bottlenecks=0
#CREATED DATE : 04/14/2017
#REVISION DATE: 02/20/2020
#Exception: NA
#Script starts
val=$(vmstat -n 1 | awk 'NR == 1 {next}
NR == 2 {for (i = 1; i <= NF; i++) fields[$i] = i; next}
{split($0, data);
p = data[fields["r"]];}
NR >= 2 + 2 {exit}
END{}
{print p}')
r=`nproc`
echo "status=success"
if [[ $val -gt $r ]]; then
	#echo "status=CPU_bottlenecks_exists\n number of CPU bottlenecks=$val"
	echo "CPU Bottlenecks=$val"
else
	echo "CPU Bottlenecks=0"
fi
#Script Ending