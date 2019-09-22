aws s3api list-objects --bucket andreikstaticwebsite --output text  --query "Contents[].{Key: Key, Size: Size}" --profile s3_ra_user >> out.csv 

