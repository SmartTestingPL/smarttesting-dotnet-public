$body = Get-Content .\fraud.json
try
{
    Invoke-RestMethod 'http://localhost:8765/fraud/fraudCheck' -Method 'POST' -ContentType "application/json" -Body $body
}
catch
{
    Write-Host "StatusCode:" $_.Exception.Response.StatusCode.value__ 
    Write-Host "StatusDescription:" $_.Exception.Response.StatusDescription
}