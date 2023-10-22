<# 
.SYNOPSIS
    Receives a request from an external system.

.DESCRIPTION
    Will call an external URL to retrieve a user.
#>


<# 
.SYNOPSIS
     Will call an external URL to retrieve a user and print its json content
#>
function Request(
    [string] 
    # Specifies an absolute uri to use for the request
    $uri 
)
{
    try
    {
        Invoke-RestMethod -Method 'Get' -Uri $uri | ConvertTo-Json
    }
    catch
    {
        Write-Output "Failed to retrieve the request"
    }
}

$targetUri = $null
if ($args.Count -eq 0)
{
	$targetUri = "https://reqres.in/api/users/2"
}
else
{
	$targetUri = $args[0]
}

Request $targetUri