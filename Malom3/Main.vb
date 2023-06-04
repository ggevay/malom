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


Imports System.Drawing
Imports System.Windows.Forms

Public Class FrmMain

    Public Shared sing As FrmMain
    Public _Board As New Board(Me)
    Public Game As Game
    Public Settings As New FrmSettings

    Public Loaded As Boolean

    Public StatusGraphics As Graphics

    Public SetupMode As Boolean
    Public SetupGameState As GameState
    Public SetupModeControls As New List(Of ToolStripItem)
    Public SetupModeWhiteToBePlaced As ToolStripNumericUpDown
    Public SetupModeBlackToBePlaced As ToolStripNumericUpDown
    Public SetupModeAutoAdjust As ToolStripCheckBox
    Public SetupModeSet00 As ToolStripButton
    Public SetupModeKLE As ToolStripCheckBox
    Public SetupNoUpdateUI As Boolean = False

    Public PlayerTypeMenuItems As New List(Of ToolStripMenuItem)

    Public Sub New()
        sing = Me
        ' auto-generated:
        ' This call is required by the designer.
        InitializeComponent()
    End Sub

    <System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptionsAttribute>
    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try

            Rules.Main = Me
            InitRules()

            SetVariant()

            AddHandler MnuPly1Human.Click, AddressOf ChangePlayerTypes
            AddHandler MnuPly1Computer.Click, AddressOf ChangePlayerTypes
            AddHandler MnuPly1Perfect.Click, AddressOf ChangePlayerTypes
            AddHandler MnuPly2Human.Click, AddressOf ChangePlayerTypes
            AddHandler MnuPly2Computer.Click, AddressOf ChangePlayerTypes
            AddHandler MnuPly2Perfect.Click, AddressOf ChangePlayerTypes
            AddHandler MnuPly1Combined.Click, AddressOf ChangePlayerTypes
            AddHandler MnuPly2Combined.Click, AddressOf ChangePlayerTypes

            Me.SuspendLayout()
            Me.Height = Screen.PrimaryScreen.WorkingArea.Height '* 768 / 900 'uncomment to test with low screen resolution
            Me.Width = Me.Height - MenuStrip.Height - StatusStrip.Height - SystemInformation.CaptionHeight
            Me.Top = 0
            Me.Left = Screen.PrimaryScreen.WorkingArea.Width / 2 - Me.Width / 2
            Me.ResumeLayout()
            _Board.Anchor = AnchorStyles.Top + AnchorStyles.Bottom + AnchorStyles.Left + AnchorStyles.Right
            _Board.Top = MenuStrip.Height
            _Board.Size = New Size(Me.ClientSize.Width, Me.ClientSize.Height - MenuStrip.Height - StatusStrip.Height)
            Me.Controls.Add(_Board)

            InitStoneIcons()

            SetupMode = False
            CreateSetupModeControls()

            Settings.LoadSettings()

            CreateGameAndPlayers()

            Loaded = True

        Catch ex As TypeInitializationException
            If ex.InnerException IsNot Nothing AndAlso TypeOf ex.InnerException Is IO.FileNotFoundException Then
                MsgBox("An error has happened while starting the program. Exiting." & vbCrLf & vbCrLf & "A possible cause of this error is that a required dll is not found in the same folder as the exe file (or in system folders). (Wrappers.dll, vcruntime140.dll, ucrtbase.dll, msvcp140.dll, concrt140.dll)" & vbCrLf & vbCrLf & ex.ToString, MsgBoxStyle.Critical)
            Else
                MsgBox("An error has happened while starting the program. Exiting." & vbCrLf & ex.ToString, MsgBoxStyle.Critical)
            End If
            Environment.Exit(1)
        Catch ex As Exception
            MsgBox("An error has happened while starting the program. Exiting." & vbCrLf & ex.ToString, MsgBoxStyle.Critical)
            Environment.Exit(1)
        End Try
    End Sub

    Private Sub InitStoneIcons()
        Dim b0 = New Bitmap(StatusStrip.Height, StatusStrip.Height)
        StatusGraphics = Graphics.FromImage(b0)
        LblKov.Image = b0

        Dim b1 = New Bitmap(MnuPl1.Height, MnuPl1.Height)
        Dim g1 = Graphics.FromImage(b1)
        g1.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        DrawWhiteStoneIcon(g1, b1.Height)
        MnuPl1.Image = b1

        Dim b2 = New Bitmap(MnuPl2.Height, MnuPl2.Height)
        Dim g2 = Graphics.FromImage(b2)
        g2.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        DrawBlackStoneIcon(g2, b2.Height)
        MnuPl2.Image = b2
    End Sub

    Private Sub CreateGameAndPlayers()
        'Game = New Game(New ComputerPlayer(False), New ComputerPlayer(True), Me)

        If Sectors.HasDatabase Then
            If AlphaBetaAvailable() Then
                Game = New Game(New HumanPlayer, New CombinedPlayer, Me)
            Else
                Game = New Game(New HumanPlayer, New PerfectPlayer, Me)
            End If
        Else
            If AlphaBetaAvailable() Then
                'MsgBox("Database files not found. Malom.exe should be in the same directory as the database files." & " Falling back to heuristic AI.", MsgBoxStyle.Exclamation)
                Game = New Game(New HumanPlayer, New ComputerPlayer, Me)
            Else
                'MsgBox("Database files not found. Malom.exe should be in the same directory as the database files.", MsgBoxStyle.Exclamation)
                Game = New Game(New HumanPlayer, New HumanPlayer, Me)
            End If
        End If
    End Sub

    Private Sub CreateSetupModeControls()
        SetupModeControls.Add(New ToolStripStatusLabel("Stones to place:"))

        SetupModeControls.Add(New ToolStripStatusLabel("W:"))
        SetupModeWhiteToBePlaced = New ToolStripNumericUpDown
        AddHandler SetupModeWhiteToBePlaced.ValueChanged, AddressOf SetupModeWhiteToBePlaced_ValueChanged
        SetupModeControls.Add(SetupModeWhiteToBePlaced)

        SetupModeControls.Add(New ToolStripStatusLabel(" B:"))
        SetupModeBlackToBePlaced = New ToolStripNumericUpDown
        AddHandler SetupModeBlackToBePlaced.ValueChanged, AddressOf SetupModeBlackToBePlaced_ValueChanged
        SetupModeControls.Add(SetupModeBlackToBePlaced)

        SetupModeControls.Add(New ToolStripStatusLabel(" ")) 'for padding
        SetupModeAutoAdjust = New ToolStripCheckBox("Auto adjust", "Automatically adjust the number of stones to be placed when a stone is added or removed from the board")
        SetupModeControls.Add(SetupModeAutoAdjust)

        SetupModeSet00 = New ToolStripButton("Set 0-0", "Switch to the sliding phase, by setting the number of stones to be placed to 0 for both players")
        AddHandler SetupModeSet00.Click, AddressOf SetupModeSet00_Click
        SetupModeControls.Add(SetupModeSet00)

        Dim SwitchSTM = New ToolStripButton("Switch STM", "Switch the side to move")
        AddHandler SwitchSTM.Click, AddressOf SetupModeSwitchSTM_Click
        SetupModeControls.Add(SwitchSTM)

        Dim EndSetupMode = New ToolStripButton("End Setup", "Exit position setup mode, and play with the current position.")
        AddHandler EndSetupMode.Click, AddressOf EndSetupMode_Click
        SetupModeControls.Add(EndSetupMode)

        SetupModeControls.Add(New ToolStripStatusLabel(" ")) 'for padding
        SetupModeKLE = New ToolStripCheckBox("Stone taking", "The player to move should take a stone, i.e., a mill has just been closed.")
        AddHandler SetupModeKLE.CheckedChanged, AddressOf SetupModeKLE_CheckedChanged
        SetupModeControls.Add(SetupModeKLE)

        SetupModeControls.ForEach(Sub(c) c.BackColor = SystemColors.Control)
        SetupModeControls.ForEach(Sub(c) c.Visible = False)

        StatusStrip1.Items.AddRange(SetupModeControls.ToArray)
    End Sub

    'Azert kommenteztuk ki, mert nem tudjuk, hogy micsoda
    'Public Sub GenerateLookuptables()
    '    SetVariant(RuleVariant.Morabaraba)

    '    Dim r As String = ""

    '    'For i = 0 To 23
    '    '    Dim adj As Integer = 0
    '    '    For j = 1 To Rules.CSLTableGraph(i, 0)
    '    '        adj = adj Or (1 << CSLTableGraph(i, j))
    '    '    Next
    '    '    r = r & adj & ","
    '    'Next

    '    For i = 0 To Rules.MillPos.GetUpperBound(0)
    '        Dim mask As Integer = 0
    '        For j = 0 To 2
    '            mask = mask Or (1 << Rules.MillPos(i, j))
    '        Next
    '        r = r & mask & ","
    '    Next

    '    Clipboard.SetText(r)
    'End Sub

    Public Sub NewGame()
        'Debug.Write(New Diagnostics.StackTrace())

        If Not Game.PlayertypeChangingCmdAllowed() Then Return

        CancelSetup()

        Game.CancelThinking()

        _Board.ClearMezoSelection()

        Dim prevhistory = Game.history
        Game = New Game(Game.Ply(1), Game.Ply(0), Me)
        Dim a = prevhistory.Last
        While a IsNot Nothing
            Game.history.AddBefore(Game.history.First, a.Value)
            a = a.Previous
        End While
    End Sub

    Private Sub MnuNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MnuNew.Click
        NewGame()
    End Sub

    Public Sub UpdateUI(ByVal s As GameState)
        If SetupNoUpdateUI Then Return
        Game.UpdateLabels()
        _Board.UpdateGameState(s)
    End Sub

    Private Sub MnuUndo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MnuUndo.Click
        If Not Game.Undo() Then LblKov.Text = "Cannot undo. " & LblKov.Text
    End Sub
    Private Sub MnuRedo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MnuRedo.Click
        If Not Game.Redo() Then LblKov.Text = "Cannot redo. " & LblKov.Text
    End Sub

    Private Sub MnuCopy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MnuCopy.Click
        Game.copy()
    End Sub
    Private Sub MnuPaste_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MnuPaste.Click
        _Board.ClearMezoSelection()
        If Not SetupMode Then
            Game.paste()
        Else
            CancelSetup()
            Game.paste()
            'EnterSetupMode() 'Not good: if ComputerPlayer is to move in the pasted position, then it would try to make a move in SetupMode
        End If
    End Sub

    Delegate Sub DPrintDepth(ByVal sz As String)
    Public Sub PrintDepth(ByVal sz As String)
        If SetupMode Then Return
        LblCalcDepths.Text = "D: " & sz
    End Sub
    Delegate Sub DSetText(ByVal s As String)
    Public Sub SetText(ByVal s As String)
        Me.Text = s
    End Sub
    Delegate Sub DRegiLblPerfEvalSettext(ByVal s As String)
    Public Sub RegiLblPerfEvalSettext(ByVal s As String)
        LblPerfEvalSetText(s)
    End Sub

    Private Sub MnuSettings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MnuSettings.Click
        Settings.Show()
        Settings.Focus()
    End Sub

    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        For Each p As Player In Game.Plys
            p.Quit()
        Next
    End Sub

    Private Sub MnuSwitchSides_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MnuSwitchSides.Click
        Game.SwitchPlayers()
    End Sub

    Private Sub frmMain_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            If SetupMode Then
                ApplySetup()
            End If
        End If
    End Sub

    <System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptionsAttribute>
    Private Sub frmMain_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles Me.KeyPress
        Try
            If e.KeyChar = " " Then
                If Not SetupMode Then
                    _Board.ShowLastJeloltMezok()
                End If
            End If
            If e.KeyChar = "m" Then
                Game.Ply(0) = New UDPPlayer
            End If
            If e.KeyChar = "M" Then
                Game.Ply(0) = New UDPPlayer
                Game.Ply(1) = New ComputerPlayer
                Game.SwitchPlayers()
            End If
            If e.KeyChar = "a" Then
                _Board.SwitchAdvisor()
            End If
        Catch ex As Exception
            MsgBox("Exception in frmMain_KeyPress" & vbCrLf & ex.ToString, MsgBoxStyle.Critical)
            Environment.Exit(1)
        End Try
    End Sub

    Private Sub ChangePlayerTypes(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If sender.checked Then Return
        If Not Game.PlayertypeChangingCmdAllowed() Then Return

        If Not Sectors.HasDatabase AndAlso (Object.ReferenceEquals(sender, MnuPly1Perfect) Or Object.ReferenceEquals(sender, MnuPly1Combined) Or Object.ReferenceEquals(sender, MnuPly2Perfect) Or Object.ReferenceEquals(sender, MnuPly2Combined)) Then
            MsgBox("Database files not found. Malom.exe should be in the same directory as the database files. Perfect player and combined player are not available.", MsgBoxStyle.Exclamation)
            Return
        End If

        If Object.ReferenceEquals(sender, MnuPly1Human) Then Game.Ply(0) = New HumanPlayer
        If Object.ReferenceEquals(sender, MnuPly1Computer) Then Game.Ply(0) = New ComputerPlayer
        If Object.ReferenceEquals(sender, MnuPly1Perfect) Then Game.Ply(0) = New PerfectPlayer
        If Object.ReferenceEquals(sender, MnuPly1Combined) Then Game.Ply(0) = New CombinedPlayer
        If Object.ReferenceEquals(sender, MnuPly2Human) Then Game.Ply(1) = New HumanPlayer
        If Object.ReferenceEquals(sender, MnuPly2Computer) Then Game.Ply(1) = New ComputerPlayer
        If Object.ReferenceEquals(sender, MnuPly2Perfect) Then Game.Ply(1) = New PerfectPlayer
        If Object.ReferenceEquals(sender, MnuPly2Combined) Then Game.Ply(1) = New CombinedPlayer
        Game.UpdateLabels()
    End Sub

    Private Sub Main_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        If TypeOf Game.Ply(0) Is UDPPlayer OrElse TypeOf Game.Ply(1) Is UDPPlayer Then UDPPlayer.ReportMatchResult()
    End Sub

    'Public UjEval(16777216) As Integer '2^24
    'Private Sub InitUjEval()
    '    For i = 0 To 16777216
    '        UjEval(i) = 1000000
    '    Next
    '    Dim be As New System.IO.StreamReader("eval.txt")
    '    While Not be.EndOfStream
    '        Dim ln = be.ReadLine().Split()
    '        Dim index As Integer = 0
    '        For i = 1 To 12
    '            Select Case ln(i)
    '                Case 0 : index = index Xor (1 << (i - 1))
    '                Case 1 : index = index Xor (1 << (i - 1 + 12))
    '            End Select
    '        Next
    '        UjEval(index) = ln(13)
    '    End While
    'End Sub

    Private Sub MnuCopyHistory_Click(sender As System.Object, e As System.EventArgs) Handles MnuCopyHistory.Click
        Clipboard.SetText(Game.history.Aggregate("", Function(str, s) str & Game.ClpString(s) & vbCrLf))
    End Sub

    Private Sub MnuTikzCopy_Click(sender As Object, e As EventArgs) Handles MnuTikzCopy.Click
        Dim s As New System.Text.StringBuilder
        s.AppendLine("\begin{tikzpicture}[]")
        s.AppendLine(" \tikzstyle{vertex}=[circle,draw,minimum size=10pt,inner sep=0pt]")
        Const scale = 0.01 '0.009
        'Dim angles = {180, 135, 90, 45, 0, 315, 270, 225, _
        '              135, 135, 45, 45, 315, 315, 225, 225, _
        '              0, 135, 270, 45, 180, 315, 90, 225}
        Dim angles = {180, 135, 90, 45, 0, 315, 270, 225, _
                      135, 45, 45, 135, 315, 225, 225, 315, _
                      0, 45, 270, 135, 180, 225, 90, 315}

        Dim AdvisorSetMoves As List(Of Tuple(Of PerfectPlayer.Move, Wrappers.gui_eval_elem2))
        Dim OkMoves As SortedSet(Of Integer)
        If _Board.Advisor IsNot Nothing And Game.s.phase = 1 And Not Game.s.KLE Then
            AdvisorSetMoves = _Board.Advisor.GetMoveList(Game.s).Select(Function(m) Tuple.Create(m, _Board.Advisor.MoveValue(Game.s, m))).ToList
            OkMoves = New SortedSet(Of Integer)(PerfectPlayer.AllMaxBy(Function(mvp) mvp.Item2, AdvisorSetMoves, Wrappers.gui_eval_elem2.min_value(_Board.Advisor.GetSec(Game.s))).Select(Function(mvp) mvp.Item1.hov))
        End If

        For i = 0 To 23
            Const emptySize = 6
            Const stoneSize = 13
            Dim x = _Board.BoardNodes(i).X * scale, y = -_Board.BoardNodes(i).Y * scale
            Dim pos = "(" & x & "," & y & ")"
            Dim valueStr = ""
            If Game.s.T(i) = -1 Then
                Dim ii = i
                If AdvisorSetMoves IsNot Nothing Then
                    If OkMoves.Contains(i) Then
                        valueStr = "\bf{!}"
                    End If
                    valueStr &= "$" & AdvisorSetMoves.Where(Function(mvp) mvp.Item1.hov = ii).Max(Function(mvp) mvp.Item2).ToString() & "$"
                End If

                'valueStr = New SetKorong(i).ToString

                s.AppendLine(String.Format("  \node[vertex] (S-{0}) at {1} [label={2}:\footnotesize {5}, minimum size={3}pt, fill=black] {4};", i, pos, angles(i), emptySize, "{}", valueStr))
            ElseIf Game.s.T(i) = 0 Then
                If AdvisorSetMoves IsNot Nothing Then
                    valueStr = "-"
                End If
                s.AppendLine(String.Format("  \node[vertex] (S-{0}) at {1} [label={4}:\footnotesize {5}, minimum size={2}pt, line width=0.8pt] {3};", i, pos, stoneSize, "{}", angles(i), valueStr))
            Else
                If AdvisorSetMoves IsNot Nothing Then
                    valueStr = "-"
                End If
                s.AppendLine(String.Format("  \node[vertex] (S-{0}) at {1} [label={4}:\footnotesize {5}, minimum size={2}pt, fill=black] {3};", i, pos, stoneSize, "{}", angles(i), valueStr))
            End If
        Next
        For i = 0 To 23
            For j = 1 To Rules.ALBoardGraph(i, 0)
                s.Append(String.Format("\draw (S-{0}) -- (S-{1});", i, Rules.ALBoardGraph(i, j)))
            Next
        Next
        s.AppendLine()
        s.AppendLine("\end{tikzpicture}")
        Clipboard.SetText(s.ToString())
    End Sub

    Private Sub MnuCopyMoveList_Click(sender As Object, e As EventArgs) Handles MnuCopyMoveList.Click
        Game.CopyMoveList()
    End Sub

    Private Sub AdvisorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AdvisorToolStripMenuItem.Click
        _Board.SwitchAdvisor()
    End Sub

    Private Sub WebsiteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WebsiteToolStripMenuItem.Click
        Process.Start("http://compalg.inf.elte.hu/~ggevay/mills/index.php")
    End Sub
    Private Sub ManualToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ManualToolStripMenuItem.Click
        Try
            Process.Start(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory, "Readme.txt"))
        Catch ex As System.ComponentModel.Win32Exception
            MsgBox("Can't find Readme.txt in " & System.IO.Directory.GetCurrentDirectory, MsgBoxStyle.Critical)
        End Try
    End Sub
    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        'AboutBox.ShowDialog()
        If AboutBox.sing Is Nothing Then
            Dim x = New AboutBox
        End If
        AboutBox.sing.ShowDialog()
    End Sub


    Private Sub MnuSetupMode_Click(sender As Object, e As EventArgs) Handles MnuSetupMode.Click
        If Not SetupMode Then
            EnterSetupMode()
        Else
            ApplySetup()
        End If
    End Sub

    Private Sub EnterSetupMode()
        Game.CancelThinking()
        _Board.Enabled = True

        SetupMode = True

        'Copy the current state, and initialize it for setup
        SetupGameState = New GameState(Game.s)
        SetupGameState.InitSetup()

        _Board.ClearMezoSelection()

        SetupModeWhiteToBePlaced.Value = Rules.MaxKSZ - SetupGameState.SetStoneCount(0)
        SetupModeBlackToBePlaced.Value = Rules.MaxKSZ - SetupGameState.SetStoneCount(1)
        SetupModeKLE.Checked = SetupGameState.KLE

        StatusStrip.SuspendLayout()
        StatusStrip1.SuspendLayout()
        LblEv.Visible = False
        LblPerfEval.Visible = False
        MnuSwitchSides.Enabled = False
        MnuTikzCopy.Enabled = False
        PlayerTypeMenuItems.ForEach(Sub(m) m.Enabled = False)
        SetupModeControls.ForEach(Sub(c) c.Visible = True)
        MnuSetupMode.Text = "Exit Setup Mode"
        LblCalcDepths.Text = ""
        LblEv.Text = ""
        LblSpeed.Text = ""
        LblTime.Text = ""
        AdjustStatusStrip1Visibility()
        StatusStrip.ResumeLayout()
        StatusStrip1.ResumeLayout()

        SetupModeSet00.Focus()

        UpdateUI(SetupGameState)
    End Sub

    Private Sub EndSetup()
        StatusStrip1.SuspendLayout()
        SetupMode = False
        LblEv.Visible = True
        LblPerfEval.Visible = True
        MnuSwitchSides.Enabled = True
        MnuTikzCopy.Enabled = True
        PlayerTypeMenuItems.ForEach(Sub(m) m.Enabled = True)
        SetupModeControls.ForEach(Sub(c) c.Visible = False)
        MnuSetupMode.Text = "Set Up Position"
        AdjustStatusStrip1Visibility()
        StatusStrip1.ResumeLayout()
        Me.Focus() 'kulonben esetleg az updown-okon marada  focus, es akkor nem kapja meg a KeyPress-t a form
    End Sub

    Public Sub CancelSetup()
        If SetupMode Then
            EndSetup()
            UpdateUI(Game.s)
        End If
    End Sub

    Private Sub ApplySetup()
        Debug.Assert(SetupMode)
        Debug.Assert(SetupModeWhiteToBePlaced.Value = Rules.MaxKSZ - SetupGameState.SetStoneCount(0))
        Debug.Assert(SetupModeBlackToBePlaced.Value = Rules.MaxKSZ - SetupGameState.SetStoneCount(1))
        Debug.Assert(SetupModeKLE.Checked = SetupGameState.KLE)

        SetupNoUpdateUI = True
        SetupModeWhiteToBePlaced_ValueChanged(Nothing, Nothing)
        SetupModeBlackToBePlaced_ValueChanged(Nothing, Nothing)
        SetupNoUpdateUI = False

        Dim valid = SetupGameState.SetOverAndCheckValidSetup
        If valid <> "" Then
            UpdateUI(SetupGameState)
            MsgBox("Invalid GameState: " & valid, MsgBoxStyle.Exclamation)
            Return
        End If
        EndSetup()
        Game.ApplySetup(SetupGameState)
    End Sub

    Private Sub SetupModeWhiteToBePlaced_ValueChanged(s As Object, e As EventArgs)
        SetupGameState.SetStoneCount(0) = Rules.MaxKSZ - SetupModeWhiteToBePlaced.Value
        SetupGameState.phase = If(SetupGameState.SetStoneCount(0) = Rules.MaxKSZ And SetupGameState.SetStoneCount(1) = Rules.MaxKSZ, 2, 1)
        UpdateUI(SetupGameState)
    End Sub

    Private Sub SetupModeBlackToBePlaced_ValueChanged(s As Object, e As EventArgs)
        SetupGameState.SetStoneCount(1) = Rules.MaxKSZ - SetupModeBlackToBePlaced.Value
        SetupGameState.phase = If(SetupGameState.SetStoneCount(0) = Rules.MaxKSZ And SetupGameState.SetStoneCount(1) = Rules.MaxKSZ, 2, 1)
        UpdateUI(SetupGameState)
    End Sub

    Private Sub SetupModeSet00_Click(s As Object, e As EventArgs)
        Debug.Assert(SetupMode)
        SetupNoUpdateUI = True
        SetupModeWhiteToBePlaced.Value = 0
        SetupModeBlackToBePlaced.Value = 0
        SetupNoUpdateUI = False
        UpdateUI(SetupGameState)
    End Sub

    Private Sub SetupModeSwitchSTM()
        SetupGameState.SideToMove = 1 - SetupGameState.SideToMove
        UpdateUI(SetupGameState)
    End Sub

    Private Sub SetupModeSwitchSTM_Click(s As Object, e As EventArgs)
        Debug.Assert(SetupMode)
        SetupModeSwitchSTM()
    End Sub

    Private Sub LblKov_Click(sender As Object, e As EventArgs) Handles LblKov.Click
        If SetupMode Then
            SetupModeSwitchSTM()
        End If
    End Sub

    Private Sub EndSetupMode_Click(sender As Object, e As EventArgs)
        Debug.Assert(SetupMode)
        ApplySetup()
    End Sub

    Private Sub SetupModeKLE_CheckedChanged(sender As Object, e As EventArgs)
        Debug.Assert(SetupMode)

        'Ez redundanssa valt, mivel kilepeskor ugyis ellenorizzuk a SetOverAndCheckValidSetup-ban.
        'Jobb ott ellenorizni, mivelhogy lehet ilyen helyzetet ugy is eloallitani, hogy a checkbox-ra kattintaskor meg van korongja az ellenfelnek.
        'If SetupModeKLE.Checked AndAlso SetupGameState.T.Count(Function(m) m = 1 - SetupGameState.SideToMove) = 0 Then
        '    'Ugyebar az lenne a gond, hogy nem lehetne kijutni a KLE-bol, mivel nincs valid lepes.
        '    '(Ugyebar a valosagban nem lehet ilyen helyzet (meg Laskernel sem), es emiatt regebben foltettuk, hogy malombecsukast minding koronglevetel kovet.)
        '    MsgBox("Stone taking cannot be activated when the opponent has no stones.", MsgBoxStyle.Exclamation)
        '    SetupModeKLE.Checked = False
        'End If

        SetupGameState.KLE = SetupModeKLE.Checked
        UpdateUI(SetupGameState)
    End Sub

    Public Sub AdjustStatusStrip1Visibility()
        If SetupMode Then
            StatusStrip1.Visible = True
        Else
            StatusStrip1.Visible = Settings.ChkShowLastIrrev.Checked
        End If
    End Sub


    Private Sub MnuExit_Click(sender As Object, e As EventArgs) Handles MnuExit.Click
        Close()
    End Sub

    Public Sub LblPerfEvalSetText(s As String, Optional red As Boolean = False)
        LblPerfEval.Text = s
        If red Then
            LblPerfEval.ForeColor = Color.Red
        Else
            LblPerfEval.ForeColor = Color.Black
        End If
    End Sub

    Public Sub DrawWhiteStoneIcon(g As Graphics, size As Integer)
        g.FillEllipse(Brushes.White, New Rectangle(1, 1, size - 2, size - 2))
        g.DrawEllipse(Pens.Black, New Rectangle(1, 1, size - 2, size - 2))
    End Sub

    Public Sub DrawBlackStoneIcon(g As Graphics, size As Integer)
        g.FillEllipse(Brushes.Black, New Rectangle(1, 1, size - 2, size - 2))
    End Sub
End Class


Public Class Game
    Public frm As FrmMain 'the main form
    Private _Ply(1) As Player 'players in the game
    Public history As New LinkedList(Of GameState) 'GameStates in this (and previous) games
    Private current As LinkedListNode(Of GameState) 'the node of the current GameState in history
    'Private MoveList As New List(Of Move)
    'Private MoveListCurIndex As Integer = 0
    Public ReadOnly Property s As GameState 'wrapper of current.value
        Get
            Return current.Value
        End Get
    End Property

    Public Sub New(ByVal p1 As Player, ByVal p2 As Player, ByVal _frm As FrmMain)
        frm = _frm
        history.AddLast(New GameState)
        current = history.Last
        Ply(0) = p1
        Ply(1) = p2
        frm._Board.UpdateGameState(s)
        UpdateLabels()
    End Sub

    Public Function Plys() As Player()
        Return _Ply
    End Function
    Public Property Ply(ByVal i As Integer) As Player 'get or set players in the game
        Get
            Return _Ply(i)
        End Get
        Set(ByVal p As Player)
            If p Is Nothing Then
                _Ply(i) = Nothing
                Return
            End If

            p.Quit() 'we exit p to see if it was in a game (e.g. NewGame in the previous one)
            If _Ply(i) IsNot Nothing Then _Ply(i).Quit() 'the player replaced by p is kicked out
            _Ply(i) = p
            If i = 0 Then 'set menus
                frm.MnuPly1Human.Checked = p.GetType() = GetType(HumanPlayer)
                frm.MnuPly1Computer.Checked = p.GetType() = GetType(ComputerPlayer)
                frm.MnuPly1Perfect.Checked = p.GetType() = GetType(PerfectPlayer)
                frm.MnuPly1Combined.Checked = p.GetType() = GetType(CombinedPlayer)
            Else
                frm.MnuPly2Human.Checked = p.GetType() = GetType(HumanPlayer)
                frm.MnuPly2Computer.Checked = p.GetType() = GetType(ComputerPlayer)
                frm.MnuPly2Perfect.Checked = p.GetType() = GetType(PerfectPlayer)
                frm.MnuPly2Combined.Checked = p.GetType() = GetType(CombinedPlayer)
            End If
            p.Enter(Me)
            NotifyPlayer(i)
        End Set
    End Property

    Private Sub UpdateUIAndNotifyPlayers()
        frm.UpdateUI(s)

        If s.over Then
            Ply(0).Over(s)
            Ply(1).Over(s)
        Else
            NotifyPlayers()
        End If
    End Sub

    <System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptionsAttribute>
    Public Sub MakeMove(ByVal M As Move) 'called by player objects when they want to move
        Try
            'Debug.Print(Microsoft.VisualBasic.Timer & " MakeMove, sidetomove: " & s.SideToMove) '

            Debug.Assert(Not Main.SetupMode)

            Ply(1 - s.SideToMove).FollowMove(M)

            history.AddAfter(current, New GameState(s))
            current = current.Next

            s.MakeMove(M)

            'MoveList.RemoveRange(MoveListCurIndex, MoveList.Count - MoveListCurIndex)
            'MoveList.Add(M)
            'MoveListCurIndex += 1

            UpdateUIAndNotifyPlayers()
        Catch ex As Exception
            If TypeOf ex Is KeyNotFoundException Then Throw
            MsgBox("Exception in MakeMove" & vbCrLf & ex.ToString)
        End Try
    End Sub

    Public Sub ApplySetup(toSet As GameState)
        history.AddAfter(current, toSet)
        current = current.Next

        UpdateUIAndNotifyPlayers()
    End Sub

    Delegate Sub DToMove(ByVal s As GameState)
    Private Sub NotifyPlayer(ByVal i As Integer)
        If Not s.over Then
            If s.SideToMove = i Then
                frm.BeginInvoke(New DToMove(AddressOf Ply(i).ToMove), s) 'Here we need to use BeginInvoke because we would like the caller to be able to finish their work before the player is informed that they need to move.
            Else
                frm.BeginInvoke(New DToMove(AddressOf Ply(i).OppToMove), s)
            End If
        End If
    End Sub
    Private Sub NotifyPlayers()
        'Debug.Print(Microsoft.VisualBasic.Timer & " NotifyPlayers, sidetomove: " & s.SideToMove) '
        For i = 0 To 1
            NotifyPlayer(i)
        Next
    End Sub
    'Public Sub QuitPlayers()
    '    For i = 0 To 1
    '        Ply(i).Quit()
    '    Next
    'End Sub
    Public Sub CancelThinking()
        For i = 0 To 1
            Ply(i).CancelThinking()
        Next
    End Sub
    Public Sub UpdateLabels()
        frm.StatusGraphics.Clear(StatusStrip.DefaultBackColor)
        If If(Main.SetupMode, Main.SetupGameState.SideToMove = 0, s.SideToMove = 0) Then
            frm.DrawWhiteStoneIcon(frm.StatusGraphics, frm.StatusStrip.Height)
        Else
            frm.DrawBlackStoneIcon(frm.StatusGraphics, frm.StatusStrip.Height)
        End If
        frm.LblKov.Invalidate()

        If Main.SetupMode Then
            frm.LblKov.Font = New Font(frm.LblKov.Font, FontStyle.Bold)
            frm.LblKov.Text = "Free setup; left/right click places white/black stone."
            frm.LblSetnum.Text = ""
            frm.LblLastIrrev.Text = ""
            frm.SetupModeSet00.Enabled = Not (frm.SetupModeWhiteToBePlaced.Value = 0 And frm.SetupModeBlackToBePlaced.Value = 0)
        Else
            frm.LblKov.Font = New Font(frm.LblKov.Font, FontStyle.Regular)
            If s.over Then
                frm.StatusGraphics.Clear(StatusStrip.DefaultBackColor)
                If s.winner = 0 Then
                    frm.LblKov.Text = "White won."
                Else
                    If s.winner = 1 Then
                        frm.LblKov.Text = "Black won."
                    Else
                        frm.LblKov.Text = "The game ended in a draw."
                    End If
                End If
            Else
                If Ply(0).GetType = Ply(1).GetType Then
                    If Not s.KLE Then frm.LblKov.Text = If(s.SideToMove = 0, "1st", "2nd") & " player to move."
                ElseIf TypeOf Ply(s.SideToMove) Is HumanPlayer Then
                    frm.LblKov.Text = "Human to move."
                ElseIf TypeOf Ply(s.SideToMove) Is CombinedPlayer Then 'sorrend!
                    frm.LblKov.Text = "Computer to move."
                ElseIf TypeOf Ply(s.SideToMove) Is ComputerPlayer Then
                    frm.LblKov.Text = "Computer to move."
                ElseIf TypeOf Ply(s.SideToMove) Is PerfectPlayer Then
                    frm.LblKov.Text = "Computer to move."
                ElseIf TypeOf Ply(s.SideToMove) Is UDPPlayer Then
                    frm.LblKov.Text = "UDP to move."
                Else
                    frm.LblKov.Text = "Fld to move."
                End If
            End If

            If TypeOf Ply(s.SideToMove) Is HumanPlayer Then
                If s.KLE Then frm.LblKov.Text = "Take a stone."
                If s.phase = 1 Then
                    'Dim pr = MaxKSZ - s.StackedPuckCount(s.SideToMove)
                    'If pr > 0 Then
                    '    frm.LblUpload.Text = pr & " stones to place."
                    'Else
                    '    frm.LblUpload.Text = "No more stones to place."
                    'End If
                    frm.LblSetnum.Text = MaxKSZ - s.SetStoneCount(0) & ", " & MaxKSZ - s.SetStoneCount(1) & " stones to place."
                Else
                    frm.LblSetnum.Text = ""
                End If
            Else
                frm.LblSetnum.Text = ""
            End If

            frm.LblLastIrrev.Text = "Last irreversible move: " & s.LastIrrev & ".  Draw in " & Rules.LastIrrevLimit - s.LastIrrev & " moves."
        End If
    End Sub
    Public Function PlayertypeChangingCmdAllowed() As Boolean
        'Return TypeOf Ply(s.SideToMove) Is HumanPlayer
        Return True
    End Function
    Public Function Undo() As Boolean
        If Not PlayertypeChangingCmdAllowed() Then Return False

        If Main.SetupMode Then
            Main.CancelSetup()
            Return True
        End If

        If TypeOf Ply(current.Value.SideToMove) Is ComputerPlayer Then Return False
        frm._Board.ClearMezoSelection()

        If current.Previous Is Nothing Then Return False
        Dim tmp = current
        Do
            current = current.Previous
        Loop While (current.Previous IsNot Nothing AndAlso Not TypeOf Ply(current.Value.SideToMove) Is HumanPlayer) 'we withdraw until no machine follows
        If TypeOf Ply(current.Value.SideToMove) Is ComputerPlayer Then 'if it failed, we restore the original state
            current = tmp
            Return False
        Else
            'MoveListCurIndex -= 1

            frm.UpdateUI(s)
            NotifyPlayers() '
            Return True
        End If
    End Function
    Public Function Redo() As Boolean
        'If MoveListCurIndex = MoveList.Count Then Return False 'hack arra, hogy ne lehessen a regebbi agba atkerulni, hogy ne romolhasson el a MoveList konzisztenciaja

        If Not PlayertypeChangingCmdAllowed() Then Return False
        If Main.SetupMode Then Return False
        If TypeOf Ply(current.Value.SideToMove) Is ComputerPlayer Then Return False
        frm._Board.ClearMezoSelection()

        If current.Next Is Nothing Then Return False
        Dim tmp = current
        Do
            current = current.Next
        Loop While (current.Next IsNot Nothing AndAlso Not TypeOf Ply(current.Value.SideToMove) Is HumanPlayer) 'we go forward until no machine follows
        If TypeOf Ply(current.Value.SideToMove) Is ComputerPlayer Then 'if it failed, we restore the original state
            current = tmp
            Return False
        Else
            'MoveListCurIndex += 1

            frm.UpdateUI(s)
            NotifyPlayers() '
            Return True
        End If
    End Function

    Public Function ClpString(s As GameState) As String
        Return s.ToString() & "," & If(TypeOf Ply(0) Is CombinedPlayer, 4, If(TypeOf Ply(0) Is ComputerPlayer, 2, If(TypeOf Ply(0) Is PerfectPlayer, 3, 0))) & "," & If(TypeOf Ply(1) Is CombinedPlayer, 4, If(TypeOf Ply(1) Is ComputerPlayer, 2, If(TypeOf Ply(1) Is PerfectPlayer, 3, 0))) & ",malom2" 'sorrend az ifekben!
    End Function
    Public Sub copy()
        'Clipboard.SetText(s.ToString() & "," & If(TypeOf Ply(0) Is ComputerPlayer, 2, 0) & "," & If(TypeOf Ply(1) Is ComputerPlayer, 2, 0) & ",malom2")
        If Not Main.SetupMode Then
            Clipboard.SetText(ClpString(s))
        Else
            Clipboard.SetText(ClpString(Main.SetupGameState))
        End If
    End Sub
    Public Sub paste()
        If Not PlayertypeChangingCmdAllowed() Then Return
        CancelThinking()
        Dim clpml As String = Clipboard.GetText()
        Dim clplines = clpml.Split(New String() {vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
        Dim success_count = 0
        Dim first = True
        For Each clp In clplines
            If Int64.TryParse(clp, vbNull) Then clp = Wrappers.Helpers.toclp(Int64.Parse(clp))
            Dim ss() As String = clp.Split(",")
            Try
                Try
                    Try
                        Dim newGameState = New GameState(clp)
                        Dim invalidMsg = newGameState.SetOverAndCheckValidSetup()
                        If invalidMsg <> "" Then
                            MsgBox("Tried to paste an invalid position:" & vbCrLf & vbCrLf & invalidMsg, MsgBoxStyle.Exclamation)
                            Continue For
                        End If
                        history.AddAfter(current, newGameState)
                        current = current.Next
                        success_count += 1
                        If success_count = clplines.Length Then
                            Ply(0) = If(ss(35) = 4, New CombinedPlayer, If(ss(35) = 2, New ComputerPlayer, If(ss(35) = 3, New PerfectPlayer, New HumanPlayer)))
                            Ply(1) = If(ss(36) = 4, New CombinedPlayer, If(ss(36) = 2, New ComputerPlayer, If(ss(36) = 3, New PerfectPlayer, New HumanPlayer)))
                        End If
                    Catch ex As FormatException
                        If clplines.Count = 1 Then
                            frm.LblSetnum.Text = frm.LblSetnum.Text & " Not game state on clipboard."
                            Return
                        End If
                    Catch ex As InvalidGameStateException
                        If clplines.Count = 1 Then
                            frm.LblSetnum.Text = frm.LblSetnum.Text & ex.mymsg
                            Return
                        End If
                    Catch ex As IndexOutOfRangeException
                        Ply(0) = If(ss(25) = 2, New ComputerPlayer, If(ss(25) = 3, New PerfectPlayer, New HumanPlayer))
                        Ply(1) = If(ss(26) = 2, New ComputerPlayer, If(ss(26) = 3, New PerfectPlayer, New HumanPlayer))
                        success_count += 1
                    End Try
                Catch ex As InvalidCastException When ss(35) = "malom"
                    Ply(0) = If(ss(25) = 2, New ComputerPlayer, If(ss(25) = 3, New PerfectPlayer, New HumanPlayer))
                    Ply(1) = If(ss(26) = 2, New ComputerPlayer, If(ss(26) = 3, New PerfectPlayer, New HumanPlayer))
                    success_count += 1
                End Try
            Catch ex As InvalidCastException
                If clplines.Count = 1 Then
                    frm.LblSetnum.Text = frm.LblSetnum.Text & " Not game state on clipboard."
                    Return
                End If
            End Try
        Next
        frm.UpdateUI(s)
        If clplines.Count > 1 Then frm.LblSetnum.Text = frm.LblSetnum.Text & " " & success_count & " game states pasted."
    End Sub
    Public Sub SwitchPlayers()
        If TypeOf Ply(0) Is ComputerPlayer And TypeOf Ply(1) Is ComputerPlayer Then Return

        CancelThinking()

        Dim p0 = Ply(0)
        Dim p1 = Ply(1)
        Ply(0) = Nothing
        Ply(1) = Nothing
        Ply(0) = p1
        Ply(1) = p0

        'Dim tmp = Ply(0)
        'Ply(0) = Ply(1)
        'Ply(1) = tmp

        UpdateLabels()
    End Sub

    Public Sub CopyMoveList()
        Throw New NotImplementedException

        'this is buggy with undo

        'Dim s = ""
        'For i = 0 To MoveListCurIndex - 1
        '    s &= MoveList(i).ToString
        '    If i < MoveListCurIndex - 1 AndAlso Not TypeOf MoveList(i + 1) Is LeveszKorong Then s &= ", "
        'Next
        'Clipboard.SetText(s)
    End Sub
End Class

Public Class GameState
    Public T(23) As Integer 'the board (-1: empty, 0: white piece, 1: black piece)
    Public phase As Integer = 1
    Public SetStoneCount(1) As Integer 'how many stones the players have set
    Public StoneCount(1) As Integer
    Public KLE As Boolean 'is there a puck removal coming?
    Public SideToMove As Integer
    Public MoveCount As Integer
    Public over As Boolean
    Public winner As Integer '(-1, if a draw)
    Public block As Boolean
    Public LastIrrev As Integer

    Public Function FutureStoneCount(p As Integer) As Integer
        Return StoneCount(p) + MaxKSZ - SetStoneCount(p)
    End Function

    Public Sub New() 'start of game
        For i = 0 To 23
            T(i) = -1
        Next
    End Sub
    Public Sub New(ByVal s As GameState)
        T = s.T.ToArray 'deep copy 
        phase = s.phase
        SetStoneCount = s.SetStoneCount.ToArray
        StoneCount = s.StoneCount.ToArray
        KLE = s.KLE
        SideToMove = s.SideToMove
        MoveCount = s.MoveCount
        over = s.over
        winner = s.winner
        block = s.block
        LastIrrev = s.LastIrrev
    End Sub

    'Sets the state for Setup Mode: the placed stones are unchanged, but we switch to phase 2.
    Public Sub InitSetup()
        'T is unchanged
        'phase is unchanged
        'SetStoneCount is unchanged
        'StoneCount is unchanged, since T is unchanged
        'KLE is unchanged
        'SideToMove is unchanged
        MoveCount = 10 'majdnem mindegy, csak ne legyen tul kicsi, ld. mashol comment
        over = False
        'Winner can be undefined, as over = False
        block = False
        LastIrrev = 0
    End Sub

    Public Sub MakeMove(ByVal M As Object)
        If Not TypeOf (M) Is Move Then Throw New ArgumentException()

        CheckInvariants()
        CheckValidMove(M)

        MoveCount += 1
        If TypeOf M Is SetKorong Then
            T(M.hov) = SideToMove
            SetStoneCount(SideToMove) += 1
            StoneCount(SideToMove) += 1
            LastIrrev = 0
        ElseIf TypeOf M Is MoveKorong Then
            T(M.hon) = -1
            T(M.hov) = SideToMove
            LastIrrev += 1
            If LastIrrev >= LastIrrevLimit Then
                over = True
                winner = -1 'draw
            End If
        ElseIf TypeOf M Is LeveszKorong Then
            T(M.hon) = -1
            StoneCount(1 - SideToMove) -= 1
            KLE = False
            'If szakasz = 2 And KorongCount(1 - SideToMove) = 2 Then
            If StoneCount(1 - SideToMove) + MaxKSZ - SetStoneCount(1 - SideToMove) < 3 Then 'TODO: refactor to call to FutureStoneCount
                over = True
                winner = SideToMove
            End If
            LastIrrev = 0
        End If
        If (TypeOf M Is SetKorong Or TypeOf M Is MoveKorong) AndAlso Malome(M.hov, Me) > -1 And StoneCount(1 - SideToMove) > 0 Then 'if he made a monkey, your move is your opponent's puck
            KLE = True
        Else
            SideToMove = 1 - SideToMove
            If SetStoneCount(0) = MaxKSZ And SetStoneCount(1) = MaxKSZ And phase = 1 Then phase = 2 'switching to disc movement
            If Not YouCanMove(Me) Then
                over = True
                block = True
                winner = 1 - SideToMove
                If Wrappers.Constants.FBD AndAlso StoneCount(0) = 12 AndAlso StoneCount(1) = 12 Then
                    winner = -1
                End If
            End If
        End If

        CheckInvariants()
    End Sub

    Private Sub CheckValidMove(M As Move)
        Debug.Assert(Not over Or winner = -1) 'Nehez megcsinalni, hogy az 'over and winner = -1' eset sose forduljon elo. Pl. a PerfectPlayer.MakeMoveInState-nek a WithTaking esete gazos, mert lehet, hogy az elotte levo MakeMove mar dontetlenne tette
        If TypeOf M Is SetKorong Then
            Debug.Assert(phase = 1)
            Dim setKorong = CType(M, SetKorong)
            Debug.Assert(T(setKorong.hov) = -1)
        End If
        If TypeOf M Is MoveKorong Then
            Dim slide = CType(M, MoveKorong)
            Debug.Assert(T(slide.hon) = SideToMove)
            Debug.Assert(T(slide.hov) = -1)
        End If
        If TypeOf M Is LeveszKorong Then
            Debug.Assert(KLE)
            Dim take = CType(M, LeveszKorong)
            Debug.Assert(T(take.hon) = 1 - SideToMove)
        End If
    End Sub

    Private Sub CheckInvariants()
        Debug.Assert(SetStoneCount(0) >= 0)
        Debug.Assert(SetStoneCount(0) <= Rules.MaxKSZ)
        Debug.Assert(SetStoneCount(1) >= 0)
        Debug.Assert(SetStoneCount(1) <= Rules.MaxKSZ)
        Debug.Assert(phase = 1 Or phase = 2 And SetStoneCount(0) = MaxKSZ And SetStoneCount(1) = MaxKSZ)
    End Sub


    Public Sub SetupClick(b As Windows.Forms.MouseButtons, m As Integer)
        Debug.Assert(Not over And Not block)
        Main.SetupNoUpdateUI = True
        If T(m) = -1 Then
            If b = Windows.Forms.MouseButtons.Left Then
                T(m) = 0
                StoneCount(0) += 1
                If Main.SetupModeAutoAdjust.Checked And Main.SetupModeWhiteToBePlaced.Value > 0 Then Main.SetupModeWhiteToBePlaced.Value -= 1
            ElseIf b = Windows.Forms.MouseButtons.Right Then
                T(m) = 1
                StoneCount(1) += 1
                If Main.SetupModeAutoAdjust.Checked And Main.SetupModeBlackToBePlaced.Value > 0 Then Main.SetupModeBlackToBePlaced.Value -= 1
            End If
        Else
            If T(m) = 0 Then
                T(m) = -1
                StoneCount(0) -= 1
                If Main.SetupModeAutoAdjust.Checked And Main.SetupModeWhiteToBePlaced.Value < Rules.MaxKSZ Then Main.SetupModeWhiteToBePlaced.Value += 1
            Else
                Debug.Assert(T(m) = 1)
                T(m) = -1
                StoneCount(1) -= 1
                If Main.SetupModeAutoAdjust.Checked And Main.SetupModeBlackToBePlaced.Value < Rules.MaxKSZ Then Main.SetupModeBlackToBePlaced.Value += 1
            End If
        End If
        Main.SetupNoUpdateUI = False
        Debug.Assert(T.Count(Function(x) x = 0) = StoneCount(0) And T.Count(Function(x) x = 1) = StoneCount(1))
    End Sub

    'Called when applying a free setup. It sets over and checks whether the position is valid. Returns "" if valid, reason str otherwise.
    'Also called when pasting a position.
    Public Function SetOverAndCheckValidSetup() As String
        Debug.Assert(Not over And Not block)

        'Validity checks:
        'Note: this should be before setting over, because we will deny applying the setup if the state is not valid, and we want to maintain the 'Not over and Not block' invariants.

        Dim toBePlaced0 = Rules.MaxKSZ - SetStoneCount(0)
        If StoneCount(0) + toBePlaced0 > Rules.MaxKSZ Then
            Return "Too many white stones (on the board + to be placed). Please remove some white stones from the board and/or decrease the number of white stones to be placed."
        End If
        Dim toBePlaced1 = Rules.MaxKSZ - SetStoneCount(1)
        If StoneCount(1) + toBePlaced1 > Rules.MaxKSZ Then
            Return "Too many black stones (on the board + to be placed). Please remove some black stones from the board and/or decrease the number of black stones to be placed."
        End If

        Debug.Assert(Not (phase = 1 And toBePlaced0 = 0 And toBePlaced1 = 0))
        Debug.Assert(Not (phase = 2 And (toBePlaced0 > 0 Or toBePlaced1 > 0)))
        If Wrappers.Constants.Variant <> Wrappers.Constants.Variants.lask And Not Wrappers.Constants.Extended Then
            If phase = 1 Then
                '(Amugy ezek a feltetelek kizarnak nehany olyan allast is, amihez van adatbazisunk: a szintukrozes miatt pl. van adatbazis olyan allashoz, ahol fekete kovetkezik, es egyenlo a toBePlaced)
                If toBePlaced0 <> toBePlaced1 - If(SideToMove = 0 Xor KLE, 0, 1) Then
                    Return "If Black is to move in the placement phase, then the number of black stones to be placed should be one more than the number of white stones to placed. If White is to move in the placement phase, then the number of white and black stones to be placed should be equal. (Except in a stone taking position, where these conditions are reversed.)" & vbCrLf & vbCrLf & "Note: The Lasker variant (and the extended solutions) doesn't have these constraints." & vbCrLf & vbCrLf & "Note: You can switch the side to move by the ""Switch STM"" button in position setup mode."
                End If
            Else
                Debug.Assert(phase = 2)
                Debug.Assert(toBePlaced0 = 0 And toBePlaced1 = 0)
            End If
        End If

        If KLE And StoneCount(1 - SideToMove) = 0 Then
            Return "A position where the opponent doesn't have any stones cannot be a stone taking position."
        End If

        '-----------------------------------------------------------
        'Set over if needed:

        Dim whiteLose, blackLose
        If StoneCount(0) + MaxKSZ - SetStoneCount(0) < 3 Then
            whiteLose = True
        End If
        If StoneCount(1) + MaxKSZ - SetStoneCount(1) < 3 Then
            blackLose = True
        End If
        If whiteLose Or blackLose Then
            over = True
            If whiteLose And blackLose Then
                winner = -1 'draw
            Else
                If whiteLose Then
                    winner = 1
                Else
                    Debug.Assert(blackLose)
                    winner = 0
                End If
            End If
        End If
        If Not KLE AndAlso Not YouCanMove(Me) Then 'YouCanMove doesn't handle the KLE case. However, we should always have a move in KLE, see the validity check above.
            over = True
            block = True
            winner = 1 - SideToMove
            If Wrappers.Constants.FBD AndAlso StoneCount(0) = 12 AndAlso StoneCount(1) = 12 Then
                winner = -1
            End If
        End If

        'Even though LastIrrev is always 0 while in free setup mode, it can be non-0 when pasting
        If LastIrrev >= Rules.LastIrrevLimit Then
            over = True
            winner = -1
        End If

        Return ""
    End Function


    Public Sub New(ByVal s As String) 'to paste from clipboard
        Dim ss() As String = s.Split(",")
        Try
            If ss(33) = "malom" OrElse ss(34) = "malom" OrElse ss(35) = "malom" OrElse ss(37) = "malom2" Then 'you need to be able to interpret older formats as well
                For i = 0 To 23
                    T(i) = ss(i)
                Next
                SideToMove = ss(24)
                phase = ss(27)
                SetStoneCount(0) = ss(28)
                SetStoneCount(1) = ss(29)
                StoneCount(0) = ss(30)
                StoneCount(1) = ss(31)
                KLE = ss(32)
                If ss(33) <> "malom" Then MoveCount = ss(33) Else MoveCount = 10 'It's 10 just so it wouldn't be 0, because then it wouldn't think about the next two steps, because it would think that the game is just beginning.
                If ss(33) <> "malom" AndAlso ss(34) <> "malom" Then LastIrrev = ss(34) Else LastIrrev = 0
                If StoneCount(0) <> T.Count(Function(x) x = 0) Or StoneCount(1) <> T.Count(Function(x) x = 1) Then Throw New InvalidGameStateException(" Number of stones is incorrect.")
            Else
                Throw New FormatException
            End If
        Catch ex As InvalidGameStateException
            Throw ex
        Catch ex As Exception
            Throw New FormatException
        End Try
    End Sub

    Public Overrides Function ToString() As String 'for clipboard
        Dim s As New System.IO.StringWriter
        For i = 0 To 23
            s.Write(T(i) & ",")
        Next
        s.Write(SideToMove & "," & 0 & "," & 0 & "," & phase & "," & SetStoneCount(0) & "," & SetStoneCount(1) & "," & StoneCount(0) & "," & StoneCount(1) & "," & KLE & "," & MoveCount & "," & LastIrrev)
        Return s.ToString()
    End Function
End Class

Class InvalidGameStateException
    Inherits Exception
    Public mymsg As String
    Public Sub New(ByVal msg As String)
        Me.mymsg = msg
    End Sub
End Class


' https://docs.microsoft.com/en-us/dotnet/framework/winforms/controls/how-to-wrap-a-windows-forms-control-with-toolstripcontrolhost
Public Class ToolStripNumericUpDown
    Inherits ToolStripControlHost

    Public Sub New()
        MyBase.New(New NumericUpDown With {.Minimum = 0, .Maximum = Rules.MaxKSZ})
    End Sub

    Public ReadOnly Property NumericUpDownControl() As NumericUpDown
        Get
            Return CType(Control, NumericUpDown)
        End Get
    End Property

    Public Property Value() As Integer
        Get
            Return CInt(NumericUpDownControl.Value)
        End Get
        Set(value As Integer)
            NumericUpDownControl.Value = value
        End Set
    End Property

    Protected Overrides Sub OnSubscribeControlEvents(ByVal c As Control)
        MyBase.OnSubscribeControlEvents(c)
        Dim numericUpDownControl As NumericUpDown = CType(c, NumericUpDown)
        AddHandler numericUpDownControl.ValueChanged, AddressOf HandleValueChanged
    End Sub

    Protected Overrides Sub OnUnsubscribeControlEvents(ByVal c As Control)
        MyBase.OnUnsubscribeControlEvents(c)
        Dim numericUpDownControl As NumericUpDown = CType(c, NumericUpDown)
        RemoveHandler numericUpDownControl.ValueChanged, AddressOf HandleValueChanged
    End Sub

    Public Event ValueChanged As EventHandler

    Private Sub HandleValueChanged(ByVal sender As Object, ByVal e As EventArgs)
        RaiseEvent ValueChanged(Me, e)
    End Sub
End Class

Public Class ToolStripCheckBox
    Inherits ToolStripControlHost

    Public Sub New(text As String, toolTipText As String)
        MyBase.New(New CheckBox() With {.Checked = True, .Text = text})
        Dim tooltip As New ToolTip
        tooltip.SetToolTip(Control, toolTipText)
    End Sub

    Public ReadOnly Property CheckBoxControl() As CheckBox
        Get
            Return CType(Control, CheckBox)
        End Get
    End Property

    Public Property Checked() As Boolean
        Get
            Return CheckBoxControl.Checked
        End Get
        Set(value As Boolean)
            CheckBoxControl.Checked = value
        End Set
    End Property

    Protected Overrides Sub OnSubscribeControlEvents(ByVal c As Control)
        MyBase.OnSubscribeControlEvents(c)
        Dim checkBoxControl As CheckBox = CType(c, CheckBox)
        AddHandler checkBoxControl.CheckedChanged, AddressOf HandleCheckedChanged
    End Sub

    Protected Overrides Sub OnUnsubscribeControlEvents(ByVal c As Control)
        MyBase.OnUnsubscribeControlEvents(c)
        Dim checkBoxControl As CheckBox = CType(c, CheckBox)
        RemoveHandler checkBoxControl.CheckedChanged, AddressOf HandleCheckedChanged
    End Sub

    Public Event CheckedChanged As EventHandler

    Private Sub HandleCheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
        RaiseEvent CheckedChanged(Me, e)
    End Sub
End Class

Public Class ToolStripButton
    Inherits ToolStripControlHost

    Public Sub New(text As String, toolTipText As String)
        MyBase.New(New Button With {.Text = text})
        Dim tooltip As New ToolTip
        tooltip.SetToolTip(Control, toolTipText)
    End Sub

    Public ReadOnly Property ButtonControl() As Button
        Get
            Return CType(Control, Button)
        End Get
    End Property
End Class