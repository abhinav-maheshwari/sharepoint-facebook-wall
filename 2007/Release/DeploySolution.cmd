@setlocal
@pushd.

@cd %~dp0
 
set url= "<<SERVER-URL>>"
Set FORCE_OPTION=-force

stsadm.exe -o addsolution -filename BrickRed.WebParts.Facebook.Wall.wsp
stsadm.exe -o deploysolution -name BrickRed.WebParts.Facebook.Wall.wsp -immediate -allowGacDeployment -url  %url%

stsadm -o execadmsvcjobs

stsadm.exe -o installfeature -name "BrickRed.WebParts.Facebook.Wall" -force
stsadm.exe -o activatefeature -name "BrickRed.WebParts.Facebook.Wall" -url  %url%
 
@pause
@popd
@endlocal