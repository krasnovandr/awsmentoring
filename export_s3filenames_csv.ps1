
$objectKeys = aws s3api list-objects --bucket andreikstaticwebsite --output text  --query 'Contents[].{Key: Key}' --profile aws_mentoring_mentee 
$mySplit = $objectKeys -split " "
$obj_list = $mySplit | Select-Object @{Name='Name';Expression={$_}}
$obj_list | Export-Csv 'megaexport.csv' -NoType



