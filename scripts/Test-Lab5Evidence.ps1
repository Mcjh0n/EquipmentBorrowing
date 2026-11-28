$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$dir="$root/docs/activity5/evidence"
$checks=[Collections.Generic.List[object]]::new()
function Assert($name,$condition){$checks.Add(@{name=$name;passed=[bool]$condition});if(-not $condition){throw "Assertion failed: $name"}}
try {
 $records=Get-Content "$dir/http-execution.json" -Raw|ConvertFrom-Json
 function Response($test,$path){($records|Where-Object {$_.test -eq $test -and $_.path -eq $path}|Select-Object -First 1).body|ConvertFrom-Json}
 $all=Response T01 equipment;$available=Response T01 'equipment?available=true'
 Assert 'T01 catalog seven items' ($all.Count -eq 7)
 Assert 'T01 available item set' ((($available.equipmentCode|Sort-Object)-join ',') -eq 'EQ-002,EQ-003,EQ-006,EQ-007')
 $openapi=Get-Content "$root/docs/activity5/openapi.json" -Raw|ConvertFrom-Json
 $operationCount=0;foreach($path in $openapi.paths.PSObject.Properties){foreach($method in $path.Value.PSObject.Properties){if($method.Name -in @('get','post','put','patch')){$operationCount++}}}
 Assert 'T01 OpenAPI sixteen operations' ($operationCount -eq 16)
 foreach($case in @('success','limit-single','limit-multi')){
 $before=Get-Content "$dir/T05-$case-before.json" -Raw|ConvertFrom-Json
 $after=Get-Content "$dir/T05-$case-after.json" -Raw|ConvertFrom-Json
 $expected=if($case -eq 'limit-single'){1}else{2}
 Assert "T05 $case exact added rows" (($after.borrowings.Count-$before.borrowings.Count) -eq $expected)
 $student=if($case -eq 'limit-single'){'ST-300'}elseif($case -eq 'limit-multi'){'ST-100'}else{'ST-400'}
 $id=($after.students|Where-Object StudentNumber -eq $student).Id
 $count=@($after.borrowings|Where-Object { $_.StudentId -eq $id -and $_.Status -eq 0 }).Count
 Assert "T05 $case exact active count" ($count -eq $(if($case -eq 'success'){2}else{3}))
 foreach($b in $after.borrowings|Where-Object Status -eq 0){Assert "Active borrowing $($b.Id) equipment unavailable" (($after.equipment|Where-Object Id -eq $b.EquipmentId).IsAvailable -eq $false)}
 }
 foreach($r in $records|Where-Object {$_.test -eq 'T06' -and $_.status -eq 200}){
 $rows=@($r.body|ConvertFrom-Json)
 if($r.path -match 'search=no-match'){Assert 'T06 empty search array' ($rows.Count -eq 0)}
 if($r.path -match 'search=aDa|search=st-100|search=Ada'){Assert 'T06 student search membership' (@($rows|Where-Object {$_.student.studentNumber -ne 'ST-100'}).Count -eq 0)}
 if($r.path -match 'search=CAM'){Assert 'T06 equipment search membership' (@($rows|Where-Object {$_.equipment.name -ne 'Camera'}).Count -eq 0)}
 if($r.path -match 'status=(Active|Returned)'){ $status=$Matches[1];Assert "T06 $status membership" (@($rows|Where-Object status -ne $status).Count -eq 0)}
 if($r.path -match 'sort=(borrowedDate|expectedReturnDate)&direction=(asc|desc)'){
 $field=$Matches[1];$desc=$Matches[2] -eq 'desc';$sorted=@($rows|Sort-Object @{Expression={$_.PSObject.Properties[$field].Value};Descending=$desc},@{Expression={$_.id};Descending=$false})
 Assert "T06 $field $desc stable ordering" (($rows.id -join ',') -eq ($sorted.id -join ','))
 }
 }
 $returned=Get-Content "$dir/T04-returned.json" -Raw|ConvertFrom-Json
 $new=($records|Where-Object {$_.test -eq 'T04' -and $_.method -eq 'POST' -and $_.status -eq 201}).body|ConvertFrom-Json
 $row=$returned.borrowings|Where-Object Id -eq $new.id
 Assert 'T04 same record returned with date' ($row.Status -eq 1 -and $null -ne $row.ReturnedDate)
 Assert 'T04 equipment available after return' (($returned.equipment|Where-Object Id -eq $row.EquipmentId).IsAvailable)
 Write-Host "Passed $($checks.Count) semantic assertions."
} finally {$checks|ConvertTo-Json -Depth 5|Set-Content "$dir/semantic-checks.json"}
