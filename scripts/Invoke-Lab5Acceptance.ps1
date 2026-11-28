param([string]$BaseUrl='http://localhost:5080')
$ErrorActionPreference='Stop'
if(-not $env:PGPASSWORD){throw 'Set PGPASSWORD locally for the independent PostgreSQL reader.'}
$root=Split-Path $PSScriptRoot -Parent
$evidence=Join-Path $root 'docs/activity5/evidence'
$records=[Collections.Generic.List[object]]::new()
function Call($test,$method,$path,$body,$expected){
 $json=if($null -ne $body){$body|ConvertTo-Json -Depth 10 -Compress}else{$null}
 $args=@{Uri="$BaseUrl/api/v1/$path";Method=$method;SkipHttpErrorCheck=$true}
 if($json){$args.Body=$json;$args.ContentType='application/json'}
 $r=Invoke-WebRequest @args
 $records.Add([pscustomobject]@{test=$test;method=$method;path=$path;request=$body;expected=$expected;status=[int]$r.StatusCode;body=$r.Content;location=$r.Headers.Location;timestamp=(Get-Date -Format o)})
 if([int]$r.StatusCode -ne $expected){throw "$test $method $path expected $expected received $($r.StatusCode)"}
 if($r.Content){return $r.Content|ConvertFrom-Json}
}
function Reset {
 & dotnet "$root/src/EquipmentBorrowing.Api/bin/Debug/net10.0/EquipmentBorrowing.Api.dll" --reset-acceptance *> $null
 if($LASTEXITCODE -ne 0){throw 'Dataset reset failed'}
 $script:students=Call Setup GET students $null 200
 $script:equipment=Call Setup GET equipment $null 200
 $script:s1=($students|Where-Object studentNumber -eq ST-100).id
 $script:s2=($students|Where-Object studentNumber -eq ST-200).id
 $script:s3=($students|Where-Object studentNumber -eq ST-300).id
 $script:s4=($students|Where-Object studentNumber -eq ST-400).id
 $script:e1=($equipment|Where-Object equipmentCode -eq EQ-001).id
 $script:e2=($equipment|Where-Object equipmentCode -eq EQ-002).id
 $script:e6=($equipment|Where-Object equipmentCode -eq EQ-006).id
}
function Snapshot($label){
 $sql='SELECT json_build_object(''students'',(SELECT json_agg(s ORDER BY "Id") FROM "Students" s),''equipment'',(SELECT json_agg(e ORDER BY "Id") FROM "Equipment" e),''borrowings'',(SELECT json_agg(b ORDER BY "Id") FROM "Borrowings" b));'
 $state=& 'C:\Program Files\PostgreSQL\19\bin\psql.exe' -X -w -h localhost -U postgres -d equipment_borrowing_lab5_acceptance -tAc $sql
 if($LASTEXITCODE -ne 0){throw 'Independent PostgreSQL query failed'}
 $state|Set-Content "$evidence/$label.json"

 return ($state -join "`n")
}
$due=(Get-Date).Date.AddDays(2).ToString('yyyy-MM-dd')
try {
 Reset; $before=Snapshot T01-before
 foreach($path in @('equipment','equipment?available=true','students',"students/$s1","equipment/$e1",'borrowings',"students/$s1/borrowings","students/$s4/borrowings","equipment/$e1/borrowings",'borrowings?status=Active')){Call T01 GET $path $null 200|Out-Null}
 foreach($path in @('students/2147483647','equipment/2147483647','borrowings/2147483647','students/2147483647/borrowings','equipment/2147483647/borrowings','missing')){Call T01 GET $path $null 404|Out-Null}
 Call T01 DELETE equipment $null 405|Out-Null
 $after=Snapshot T01-after;if($before -ne $after){throw 'T01 changed data'}
 Invoke-WebRequest "$BaseUrl/openapi/v1.json" -OutFile "$root/docs/activity5/openapi.json"
 Reset; Snapshot T02-before|Out-Null
 $new=Call T02 POST students @{studentNumber='ST-500';fullName='Evan Test';contact=$null} 201
 Call T02 GET "students/$($new.id)" $null 200|Out-Null
 Call T02 PUT "students/$($new.id)" @{studentNumber='ST-500';fullName='Evan Updated';contact='test@example.edu'} 200|Out-Null
 Call T02 POST students @{studentNumber=' st-100 ';fullName='Duplicate'} 409|Out-Null
 Call T02 POST students @{studentNumber='ST-600';fullName=' '} 400|Out-Null
 Call T02 PATCH "students/$s1/eligibility" @{isEligible=$false} 200|Out-Null
 Call T02 POST borrowings @{studentId=$s1;equipmentId=$e2;expectedReturnDate=$due} 409|Out-Null
 $loan=(Call T02 GET "students/$s1/borrowings" $null 200|Where-Object status -eq Active)[0]
 Call T02 POST "borrowings/$($loan.id)/return" @{} 200|Out-Null
 Call T02 PATCH "students/$s1/eligibility" @{isEligible=$true} 200|Out-Null
 Call T02 PATCH "students/$s1/eligibility" @{isEligible=$true} 200|Out-Null
 Call T02 POST borrowings @{studentId=$s1;equipmentId=$e2;expectedReturnDate=$due} 201|Out-Null
 Snapshot T02-after|Out-Null
 Reset; Snapshot T03-before|Out-Null
 $new=Call T03 POST equipment @{equipmentCode='EQ-008';name='Monitor';description=$null} 201
 Call T03 GET "equipment/$($new.id)" $null 200|Out-Null
 Call T03 PUT "equipment/$e1" @{equipmentCode='EQ-001';name='Camera updated';description=$null} 200|Out-Null
 Call T03 PUT "equipment/$($new.id)" @{equipmentCode='EQ-008';name='Monitor updated';description=$null} 200|Out-Null
 Call T03 POST equipment @{equipmentCode=' eq-001 ';name='Duplicate'} 409|Out-Null
 Call T03 PUT "equipment/$e1" @{equipmentCode='EQ-001';name=' ';description=$null} 400|Out-Null
 Call T03 PUT "equipment/$e1" @{equipmentCode='EQ-001';name='Camera';isAvailable=$true} 400|Out-Null
 Snapshot T03-after|Out-Null
 Reset;Snapshot T04-before|Out-Null
 $loan=Call T04 POST borrowings @{studentId=$s4;equipmentId=$e2;expectedReturnDate=$due} 201
 Call T04 GET "borrowings/$($loan.id)" $null 200|Out-Null
 Snapshot T04-active|Out-Null
 foreach($case in @(@{s=$s2;e=$e6;d=$due;status=409},@{s=$s4;e=$e1;d=$due;status=409},@{s=2147483647;e=$e6;d=$due;status=404},@{s=$s4;e=$e6;d='2000-01-01';status=400})){
 Call T04 POST borrowings @{studentId=$case.s;equipmentId=$case.e;expectedReturnDate=$case.d} $case.status|Out-Null }
 Call T04 POST "borrowings/$($loan.id)/return" @{} 200|Out-Null
 Call T04 POST "borrowings/$($loan.id)/return" @{} 409|Out-Null
 Snapshot T04-returned|Out-Null
 foreach($case in @('success','limit-single','limit-multi','over-limit','duplicates','one','unavailable','ineligible','missing','date')){
 Reset;$before=Snapshot "T05-$case-before"
 $student=$s4;$items=@($e2,$e6);$date=$due;$expected=201;$path='checkout-requests'
 switch($case){'limit-single'{$student=$s3;$path='borrowings'} 'limit-multi'{$student=$s1} 'over-limit'{$student=$s3;$expected=409} 'duplicates'{$items=@($e2,$e2);$expected=400} 'one'{$items=@($e2);$expected=400} 'unavailable'{$items=@($e1,$e2);$expected=409} 'ineligible'{$student=$s2;$expected=409} 'missing'{$items=@($e2,2147483647);$expected=404} 'date'{$date='2000-01-01';$expected=400}}
 $body=@{studentId=$student;equipmentIds=$items;expectedReturnDate=$date};if($path -eq 'borrowings'){$body=@{studentId=$student;equipmentId=$e2;expectedReturnDate=$date}}
 Call T05 POST $path $body $expected|Out-Null;$after=Snapshot "T05-$case-after";if($expected -ne 201 -and $before -ne $after){throw "T05 $case wrote data"}
 }
 Reset;$before=Snapshot T06-before
 foreach($path in @("students/$s1/borrowings","students/$s4/borrowings","equipment/$e1/borrowings",'borrowings?search=aDa','borrowings?search=st-100','borrowings?search=CAM','borrowings?search=no-match','borrowings?status=Active','borrowings?status=Returned','borrowings?status=All','borrowings?search=Ada&status=Active&sort=expectedReturnDate&direction=desc','borrowings')){Call T06 GET $path $null 200|Out-Null}
 foreach($sort in @('borrowedDate','expectedReturnDate')){foreach($dir in @('asc','desc')){Call T06 GET "borrowings?sort=$sort&direction=$dir" $null 200|Out-Null}}
 Call T06 GET 'borrowings?status=invalid' $null 400|Out-Null
 Call T06 GET 'equipment?available=invalid' $null 400|Out-Null
 $after=Snapshot T06-after;if($before -ne $after){throw 'T06 changed data'}
 Write-Host "Completed $($records.Count) HTTP checks. Further semantic, rollback, restart and UI evidence is recorded separately."
} finally {$records|ConvertTo-Json -Depth 20|Set-Content "$evidence/http-execution.json"}



