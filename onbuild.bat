set BUILD_TYPE=Debug
set MOD_AUTHOR=eli
set MOD_NAME=elisShenanigans
set PROFILE_NAME=Default
set THUNDERSTORE_LOCAL_MOD_PATH=C:\Users\User\AppData\Roaming\r2modmanPlus-local\STRAFTAT\profiles\%PROFILE_NAME%

set LOCAL_MOD_PATH=%THUNDERSTORE_LOCAL_MOD_PATH%\BepInEx\plugins\%MOD_AUTHOR%-%MOD_NAME%\

IF EXIST %LOCAL_MOD_PATH% (
    xcopy /s /i /y "bin\%BUILD_TYPE%\netstandard2.1\%MOD_NAME%.dll" "%LOCAL_MOD_PATH%\"
    echo "copied bin\Release\netstandard2.1\%MOD_NAME%.dll to %LOCAL_MOD_PATH%"
    IF "%BUILD_TYPE%"=="Debug" (
        xcopy /s /i /y "bin\%BUILD_TYPE%\netstandard2.1\%MOD_NAME%.pdb" "%LOCAL_MOD_PATH%\"
        echo "copied bin\Release\netstandard2.1\%MOD_NAME%.pdb to %LOCAL_MOD_PATH%"
    )
    echo "(BUILD MODE %BUILD_TYPE%) Updated files in Thunderstore".
)