#!/bin/bash

wget --no-check-certificate --no-cookies --header "Cookie: oraclelicense=accept-securebackup-cookie" http://download.oracle.com/otn-pub/java/jdk/8u141-b15/336fa29ff2bb4ef291e347e091f7f4a7/jdk-8u141-linux-x64.rpm
yum install -y jdk-8u141-linux-x64.rpm 
yum -y install httpd  
service httpd start 
chkconfig httpd on 
aws s3 cp s3://andreikstaticwebsite /var/www/html --recursive

mkfs -t xfs /dev/sdb
mkdir /data
mount /dev/sdb /data

