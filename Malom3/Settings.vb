' Malom, a Nine Men's Morris (and variants) player and solver program.
' Copyright(C) 2007-2016  Gabor E. Gevay, Gabor Danner
' 
' See our webpage (and the paper linked from there):
' http://compalg.inf.elte.hu/~ggevay/mills/index.php
' 
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program.  If not, see <http://www.gnu.org/licenses/>.


Public Class FrmSettings
    Dim AIStrength As Byte
    'Public CalcNodeValues, ShowEv, ShowLastMove As Boolean
    Public CalcNodeValues As Boolean 'mezőértékek alapján gondolkodjon-e fölrakáskor
    Public ShowEv As Boolean 'mutassa-e a helyzetértékelést
    Public ShowLastMove As Boolean 'mutassa-e a legutóbbi lépést 
    Public timelimit As Double 'meddig növelje a mélységet

    Private Sub Settings_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LoadSettings()
        Tabs.SelectTab(1)
    End Sub

    'Private Sub CmdOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    SaveSettings()
    '    ApplySettings()
    '    Me.Hide()
    'End Sub

    Private Sub ApplyAndSave()
        ApplySettings()
        SaveSettings()
    End Sub

    Public Sub LoadSettings()
        Try 'egyszer talán Mono-n is futni fog...
            UDShowInterval.Value = GetSetting(My.Application.Info.AssemblyName, "Appearance", "ShowInterval", "1500")
            ChkShowLastIrrev.Checked = GetSetting(My.Application.Info.AssemblyName, "Appearance", "ChkShowLastIrrev", False)
            AIStrength = GetSetting(My.Application.Info.AssemblyName, "Computer", "Difficulty", 4)
            Debug.Assert(AIStrength <> 0)
            If AIStrength = 1 Then RdBeginner.Checked = True
            If AIStrength = 2 Then RdNovice.Checked = True
            If AIStrength = 3 Then RdIntermediate.Checked = True
            If AIStrength = 4 Then RdExpert.Checked = True
            ChkCalcNodeValues.Checked = GetSetting(My.Application.Info.AssemblyName, "Computer", "CalcNodeValues", False)
            ChkCalcLLNum.Checked = GetSetting(My.Application.Info.AssemblyName, "Computer", "CalcLLNum", True)
            ChkShowEv.Checked = GetSetting(My.Application.Info.AssemblyName, "Computer", "ShowEv", True)
            UDIncTimeLimit.Value = GetSetting(My.Application.Info.AssemblyName, "Computer", "IncTimeLimit", 0.2)
            ChkIgnoreDD.Checked = GetSetting(My.Application.Info.AssemblyName, "Perfect", "IgnoreDD", False)
        Catch ex As NotImplementedException
            UDShowInterval.Value = 1500
            ChkShowLastIrrev.Checked = False
            AIStrength = 4
            ChkCalcNodeValues.Checked = False
            ChkCalcLLNum.Checked = True
            ChkShowEv.Checked = True
            UDIncTimeLimit.Value = 0.2
            ChkIgnoreDD.Checked = False
        End Try
        ApplySettings()
    End Sub

    Private Sub SaveSettings()
        Try
            If Not Main.Loaded Then Return
            SaveSetting(My.Application.Info.AssemblyName, "Appearance", "ShowInterval", UDShowInterval.Value)
            SaveSetting(My.Application.Info.AssemblyName, "Appearance", "ChkShowLastIrrev", ChkShowLastIrrev.Checked)
            SaveSetting(My.Application.Info.AssemblyName, "Computer", "Difficulty", AIStrength)
            SaveSetting(My.Application.Info.AssemblyName, "Computer", "CalcNodeValues", ChkCalcNodeValues.Checked)
            SaveSetting(My.Application.Info.AssemblyName, "Computer", "CalcLLNum", ChkCalcLLNum.Checked)
            SaveSetting(My.Application.Info.AssemblyName, "Computer", "ShowEv", ChkShowEv.Checked)
            SaveSetting(My.Application.Info.AssemblyName, "Computer", "IncTimeLimit", UDIncTimeLimit.Value)
            SaveSetting(My.Application.Info.AssemblyName, "Perfect", "IgnoreDD", ChkIgnoreDD.Checked)
        Catch ex As NotImplementedException
        End Try
    End Sub

    Private Sub ApplySettings()
        If UDShowInterval.Value > 0 Then
            ShowLastMove = True
        Else
            ShowLastMove = False
        End If
        Main.AdjustStatusStrip1Visibility()
        CalcNodeValues = ChkCalcNodeValues.Checked
        ShowEv = ChkShowEv.Checked
        FrmMain.LblEv.Visible = ChkShowEv.Checked
        timelimit = UDIncTimeLimit.Value
        Wrappers.gui_eval_elem2.ignore_DD = ChkIgnoreDD.Checked
    End Sub

    'Private Sub CmdApply_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    ApplySettings()
    'End Sub

    'Private Sub CmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Me.Hide()
    '    LoadSettings()
    'End Sub

    Private Sub Settings_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles Me.KeyPress
        'If Asc(e.KeyChar) = 27 Then
        '    Me.Hide()
        '    LoadSettings()
        'End If
        If Asc(e.KeyChar) = 13 Then
            'SaveSettings()
            'ApplySettings()
            ApplyAndSave()
            Me.Hide()
        End If
    End Sub

    Private Sub Settings_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        LoadSettings()
    End Sub

    Private Sub RdBeginner_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RdBeginner.CheckedChanged
        If RdBeginner.Checked Then AIStrength = 1
        ApplyAndSave()
    End Sub

    Private Sub RdNovice_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RdNovice.CheckedChanged
        If RdNovice.Checked Then AIStrength = 2
        ApplyAndSave()
    End Sub

    Private Sub RdIntermediate_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RdIntermediate.CheckedChanged
        If RdIntermediate.Checked Then AIStrength = 3
        ApplyAndSave()
    End Sub

    Private Sub RdExpert_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RdExpert.CheckedChanged
        If RdExpert.Checked Then AIStrength = 4
        ApplyAndSave()
    End Sub

    Private Sub UDShowInterval_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UDShowInterval.ValueChanged
        ApplyAndSave()
    End Sub

    Private Sub ChkCalcNodeValues_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChkCalcNodeValues.CheckedChanged
        If ChkCalcNodeValues.Checked Then ChkCalcLLNum.Checked = False
        ApplyAndSave()
    End Sub

    Private Sub ChkCalcLLNum_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChkCalcLLNum.CheckedChanged
        If ChkCalcLLNum.Checked Then ChkCalcNodeValues.Checked = False
        ApplyAndSave()
    End Sub

    Private Sub ChkIgnoreDD_CheckedChanged(sender As Object, e As EventArgs) Handles ChkIgnoreDD.CheckedChanged
        ApplyAndSave()
    End Sub

    Private Sub UDIncTimeLimit_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UDIncTimeLimit.ValueChanged
        Dim v As Decimal = UDIncTimeLimit.Value
        Dim i As Byte = 0
        While Not System.Math.Round(v) = v
            i += 1
            v *= 10
        End While
        UDIncTimeLimit.DecimalPlaces = i
        ApplyAndSave()
    End Sub

    Private Sub FrmSettings_FormClosing(sender As System.Object, e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        e.Cancel = True
        Me.Hide()
    End Sub

    Public Function CalcDepths(ByVal s As GameState) As Integer
        CalcDepths = -1

        Select Case AIStrength
            Case 1
                If s.phase = 1 Then
                    If s.StoneCount(0) + s.StoneCount(1) >= 0 Then CalcDepths = 2 '
                    If s.StoneCount(0) + s.StoneCount(1) >= 2 Then CalcDepths = 2
                End If
                If s.phase = 2 Then
                    If s.StoneCount(0) > 3 And s.StoneCount(1) > 3 Then
                        CalcDepths = 3
                    Else
                        CalcDepths = 2
                    End If
                End If
            Case 2
                If s.phase = 1 Then
                    If s.StoneCount(0) + s.StoneCount(1) >= 0 Then CalcDepths = 2 '
                    If s.StoneCount(0) + s.StoneCount(1) >= 2 Then CalcDepths = 4
                    If s.StoneCount(0) + s.StoneCount(1) >= 8 Then CalcDepths = 5
                End If
                If s.phase = 2 Then
                    If s.StoneCount(0) > 3 And s.StoneCount(1) > 3 Then
                        CalcDepths = 5
                    Else
                        CalcDepths = 4
                    End If
                End If
            Case 3
                If s.phase = 1 Then
                    If s.StoneCount(0) + s.StoneCount(1) >= 0 Then CalcDepths = 2 '
                    If s.StoneCount(0) + s.StoneCount(1) >= 2 Then CalcDepths = 6 '5            
                    If s.StoneCount(0) + s.StoneCount(1) >= 8 Then CalcDepths = 7 '6
                    If s.StoneCount(0) + s.StoneCount(1) >= 16 Then CalcDepths = 8 '7
                End If
                If s.phase = 2 Then
                    If s.StoneCount(0) > 3 And s.StoneCount(1) > 3 Then
                        If s.StoneCount(0) + s.StoneCount(1) >= 12 Then CalcDepths = 7 Else CalcDepths = 6 '6 5
                        If s.StoneCount(0) + s.StoneCount(1) >= 16 Then CalcDepths = 8 '7                        
                    Else
                        CalcDepths = 5
                    End If
                End If
            Case 4 'This is handled separately by the engine.
                CalcDepths = 100 '
        End Select

        'CalcDepths = -1
    End Function

    Private Sub ChkShowEv_CheckedChanged(sender As Object, e As EventArgs) Handles ChkShowEv.CheckedChanged
        ApplyAndSave()
    End Sub

    Private Sub ChkShowLastIrrev_CheckedChanged(sender As Object, e As EventArgs) Handles ChkShowLastIrrev.CheckedChanged
        ApplyAndSave()
    End Sub
End Class