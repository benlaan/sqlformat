param(
    [string]$source
)

function Get-IsAdministrator() {

    $identity = [System.Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = new-object System.Security.Principal.WindowsPrincipal($identity)

    return $principal.IsInRole([System.Security.Principal.WindowsBuiltInRole]::Administrator)
}

if (Get-IsAdministrator) {

    cp -Path "$source\*.dll" -Destination "C:\Program Files (x86)\Laan Software\Laan SQL Add-In for SSMS\"
    cp -Path "$source\*.pdb" -Destination "C:\Program Files (x86)\Laan Software\Laan SQL Add-In for SSMS\"
}
