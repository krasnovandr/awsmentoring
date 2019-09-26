
$s3objectKeys = aws s3api list-objects --bucket andreikstaticwebsite --output text  --query 'Contents[].{Key: Key}' --profile aws_mentoring_mentee 

$splittedObjectKeys = $s3objectKeys -split " "
$obj_list = $splittedObjectKeys | Select-Object @{Name='Name';Expression={$_}}


$resultList = New-Object System.Collections.ArrayList

ForEach ($item in $obj_list) 
{
    $fileInfo = New-Object System.Object
    $fileName = [System.IO.Path]::GetFileName($item.Name) 
    $fileInfo | Add-Member -MemberType NoteProperty -Name "FileName" -Value $fileName
    $fileInfo | Add-Member -MemberType NoteProperty -Name "FilePath" -Value $item.Name

    $resultList.Add($fileInfo) | Out-Null
}


$resultList | Export-Csv 'megaexdasdsaport.csv' -NoType

