${SegmentFile}

${Segment.OnInit}
	System::Call kernel32::GetCurrentProcess()i.s
	System::Call kernel32::IsWow64Process(is,*i.r0)
	${If} $0 == 0
		StrCpy $Bits 32
		Rename "$EXEDIR\App\DBBrowserforSQLite64" "$EXEDIR\App\DBBrowserforSQLite32"
	${Else}
		StrCpy $Bits 64
		Rename "$EXEDIR\App\DBBrowserforSQLite32" "$EXEDIR\App\DBBrowserforSQLite64"
	${EndIf}
!macroend

${SegmentInit}
    ${If} $Bits = 64
        ${SetEnvironmentVariablesPath} FullAppDir "$EXEDIR\App\DBBrowserforSQLite64"
	${Else}
        ${SetEnvironmentVariablesPath} FullAppDir "$EXEDIR\App\DBBrowserforSQLite32"
	${EndIf}
!macroend