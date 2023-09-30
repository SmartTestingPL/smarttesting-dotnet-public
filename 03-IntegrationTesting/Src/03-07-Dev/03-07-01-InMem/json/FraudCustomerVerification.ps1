$body = Get-Content .\fraudCustomerVerification.json
try
{
    Invoke-RestMethod 'http://localhost:5000/fraud' -Method 'POST' -ContentType "application/json" -Body $body
}
catch
{
    Write-Host "StatusCode:" $_.Exception.Response.StatusCode.value__ 
    Write-Host "StatusDescription:" $_.Exception.Response.StatusDescription
}
