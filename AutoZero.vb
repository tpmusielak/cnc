Dim ZOFFSET, ZTOP, ZBOTTOM, ZGODOWN, ZABS, ZUPFINE, Z_SAFE 
Dim ToolLen 
Dim Tool 
Dim SENS_Z, SENS_X, SENS_Y, Z_LIFT, Z_PARK, MAX_DTRAVEL 
Dim SPD_FAST, SPD_FINE 

' ----------------------------------------------------- 
' Configuration parameters 
' ----------------------------------------------------- 
SENS_X = 15.9	' Sensor X position 
SENS_Y = 10.75	' Sensor Y position 
SENS_Z = -43.8031	' Sensor Z position 

Z_SAFE = -5	' Safe Z above sensor (for rapid move) 
MAX_DTRAVEL = 70 ' Max down travel 
SPD_FAST = 1000 
SPD_FINE = 100	' Fast and fine speeds 
Z_LIFT = 1.5	' Z lift for fine probing 
Z_PARK = -5	 ' Park after probe absolute Z position 
' ----------------------------------------------------- 

Message( "Dokonuję pomiaru długości narzędzia..." ) 

SanityCheck
ToolLengthCompensationOff
MoveInPosition
ToolLen = Probe()
SetToolLength(ToolLen)

ToolLengthCompensationOn

Message ("Z Value : " & ToolLen) 

' ----------------------------------------------------------------------------- 

Sub SanityCheck ()
    If GetOEMLed(807) Then 
        Message( "TC ERROR: X Axis not referenced!" ) 
        End 
    End If 
    
    If GetOEMLed(808) Then 
        Message( "TC ERROR: Y Axis not referenced!" ) 
        End 
    End If 
    
    If GetOEMLed(809) Then 
        Message( "TC ERROR: Z Axis not referenced!" ) 
        End 
    End If 
    
    If GetOEMLed(825) Then 
        Message( "TC ERROR: Czujnik pomiaru cały czas załączony! pomiar przerwany." ) 
        End 
    End If 
    
    If GetOEMLed(11) Then 
        Message( "TC ERROR: Do pomiaru narzędzia wyłącz wrzeciono!" ) 
        End 
    End If 

    Tool = GetDRO(24)	' Get current tool index 
    ' narzedzie 0 nie podlega pomiarowi 
    If(Tool = 0) Then 
        Message( "TC INFO: Narzędzie nr:0 nie podlega pomiarom długości" ) 
        End 
    End If   

End Sub

Sub MoveInPosition ()
    ' Move to PARK 
    Code("G0G53 Z" & Z_PARK) 
    WaitForMove 
    ' Move to XY position of tool correction sensor 
    '	Code("G0G53 X" & SENS_X & " Y" & SENS_Y) 
    Code("G0G53 X" & SENS_X) 
    Code("G0G53 Y" & SENS_Y) 
    WaitForMove 
    
    ' Get actual Z offset 
    ZOFFSET = GetOEMDRO(49) 
    ' Rapid go down to safe distance above sensor 
    Code("G0G53 Z" & Z_SAFE) 
    WaitForMove
End Sub

Function Probe ()
    ' Probe 
    ZTOP = GetDRO(2) ' actual Z position 
    ZGODOWN = ZTOP - MAX_DTRAVEL ' max down travel 
    Code("G31 Z" & ZGODOWN & "f" & SPD_FAST) 
    WaitForMove 

    ZBOTTOM = GetProbeActivationPoint()
    
    If ZBOTTOM=GetDRO(2) Then
        Message( "TC ERROR: Probe not hit - tool too short/long")
    End
    End If

    ZUPFINE = GetOEMDRO(85) + Z_LIFT 
    Code("G1G53 Z" & ZUPFINE & "f" & SPD_FAST) ' go up Z_LIFT 
    WaitForMove 
    ' fine probe 
    Code("G31 Z" & ZGODOWN & "f" & SPD_FINE) 
    WaitForMove 
    
    ZBOTTOM = GetProbeActivationPoint()

    ' Lift Z to abs park position 
    Sleep(50) 
    Code("G0G53 Z" & Z_PARK) 
    WaitForMove 

    ZABS = ZBOTTOM + ZOFFSET 
    ToolLen = -(SENS_Z - ZABS) 
    WaitForMove 
    
    Probe=ToolLen
End Function

Sub SetToolLength(length)
    SetOEMDRO(42, length) 
End Sub
    

Sub ToolLengthCompensationOff()
    Code("G43T0") 
    Code("G43H0")	 ' turn off tool lenght compensation 
    WaitForMove 
End Sub

Sub ToolLengthCompensationOn()
    Code("G43T" & Tool)	' G43 to sync value in system 
    Code("G43H" & Tool)	' G43 to sync value in system 
    WaitForMove
End Sub

Sub WaitForMove () 
    While IsMoving() 
        Sleep(15) 
    Wend 
End Sub    

Function GetProbeActivationPoint ()
    GetProbeActivationPoint=GetVar(2002)
End Function
    

  
