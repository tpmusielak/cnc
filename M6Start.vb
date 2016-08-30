tool = GetSelectedTool()
SetCurrentTool( tool )
  
SetVar(1, GetOEMDRO(800)) 'Set X
SetVar(2, GetOEMDRO(801)) 'Set Y
SetVar(3, GetOEMDRO(802)) 'Set Z

TCX=GetOEMDRO(1200) 'Get tool change X
TCZ=GetOEMDRO(1202) 'Get tool change Y
TCY=GetOEMDRO(1201) 'Get tool change Z


Code("G53 G0 Z " & TCZ)
WaitForMove

Code("G53 G0 X" & TCX & "Y" & TCY)
WaitForMove

'------------------------------------------------------------------

Sub WaitForMove () 
While IsMoving() 
    Sleep(15) 
Wend 
End Sub    