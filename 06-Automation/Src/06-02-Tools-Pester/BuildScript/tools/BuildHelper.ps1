function Install-If-Not-Installed($module, $version)
{
	if (Get-Module -ListAvailable -FullyQualifiedName @{ModuleName=$module;ModuleVersion=$version}) 
	{
		Write-Host $module $version is already installed.
	}
	else
	{
		Install-Module -Name $module -RequiredVersion $version -Force -Scope CurrentUser -SkipPublisherCheck -AllowClobber
		Write-Host $module $version installed successfully.
	}
}

$target = $null
if ($args.Count -eq 0)
{
	$target = 'usage'
}
else
{
	$target = $args[0]
}

Install-If-Not-Installed PowershellGet 2.2.5
Install-If-Not-Installed Execution 1.7.0

switch($target)
{
	'install-script-analyzer'  
	{
		if ((Get-PackageProvider -Name NuGet).version -lt 2.8.5.201 ) 
		{
			Invoke-SelfElevation
			Install-PackageProvider Nuget -MinimumVersion 2.8.5.201 –Force
		}
		else
		{
			Write-Host "Package provider up to date"
		}

		Install-If-Not-Installed PSScriptAnalyzer 1.20.0
	}
	'install-pester' 
	{
		Install-If-Not-Installed Pester 5.3.1
	}
	'usage'
	{
		Write-Output 'Usage: $args[0]: <install-script-analyzer|install-pester>'
	}
}
