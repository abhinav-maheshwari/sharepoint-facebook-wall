@setlocal
@pushd


@set url= "<<SERVER-URL>>"
Set FORCE_OPTION=-force

@set PATH=C:\Program Files\Common Files\Microsoft Shared\web server extensions\12\BIN;%PATH% @cd %~dp0


	stsadm.exe -o deactivatefeature -name "BrickRed.WebParts.Facebook.Wall" -url  %url%
	stsadm.exe -o uninstallfeature -name "BrickRed.WebParts.Facebook.Wall" -force

	stsadm.exe -o retractsolution -name BrickRed.WebParts.Facebook.Wall.wsp -immediate -url %url%
	stsadm -o execadmsvcjobs
	stsadm.exe -o deletesolution -name BrickRed.WebParts.Facebook.Wall.wsp
 

@pause
@popd
@endlocal

