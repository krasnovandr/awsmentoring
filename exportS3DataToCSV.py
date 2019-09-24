import boto3
import os
import csv

s3 = boto3.client('s3')

class FileInfo:
  fileName = ""
  filePath = ""

def GetS3Keys(bucket):
    keys = []
    resp = s3.list_objects_v2(Bucket=bucket)
    for obj in resp['Contents']:
        keys.append(obj['Key'])
    return keys

def ConvertKeysToExportList(result):
    exportList = []
    for value in result:
        fileinfo = FileInfo()
        fileinfo.fileName =  os.path.basename(value) 
        fileinfo.filePath =  value
        exportList.append(fileinfo)
    return exportList 

def WriteListToCsv(exportList):
    with open('s3-files.csv', 'w', newline='') as writeFile:
        writer = csv.writer(writeFile)
        writer.writerow(['fileName', 'filePath'])
        for exportFile in exportList:
            writer.writerow([exportFile.fileName, exportFile.filePath])

keys = GetS3Keys('andreikstaticwebsite')
exportList = ConvertKeysToExportList(keys)
WriteListToCsv(exportList)




