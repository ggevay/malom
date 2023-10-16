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

Imports System.Windows.Forms

Module Rules
    Public MillPos(,), StdLaskerMillPos(15, 2), MoraMillPos(19, 2) As Byte 'gives the sequence number of each field participating in StdLaskerMillPosition
    Public InvMillPos()(), StdLaskerInvMillPos(23)(), MoraInvMillPos(23)() As Integer 'ith element gives those indexes into MalomPoz where the ith field occurs
    Public BoardGraph(,), StdLaskerBoardGraph(24, 24), MoraBoardGraph(24, 24) As Boolean 'adjacency matrix
    Public ALBoardGraph(,), StdLaskerALBoardGraph(24, 4), MoraALBoardGraph(24, 4) As Byte 'adjacency list, 0th element is the number of neighbors
    Public VariantName As String

    Public Const LastIrrevLimit = 50 'on flyordie 50 'how many moves can pass without puck removal/folraker before the game ends in a draw

    Public Sub InitRules()
        StdLaskerMillPos(0, 0) = 1
        StdLaskerMillPos(0, 1) = 2
        StdLaskerMillPos(0, 2) = 3
        StdLaskerMillPos(1, 0) = 3
        StdLaskerMillPos(1, 1) = 4
        StdLaskerMillPos(1, 2) = 5
        StdLaskerMillPos(2, 0) = 5
        StdLaskerMillPos(2, 1) = 6
        StdLaskerMillPos(2, 2) = 7
        StdLaskerMillPos(3, 0) = 7
        StdLaskerMillPos(3, 1) = 0
        StdLaskerMillPos(3, 2) = 1
        For i = 4 To 11
            StdLaskerMillPos(i, 0) = StdLaskerMillPos(i - 4, 0) + 8
            StdLaskerMillPos(i, 1) = StdLaskerMillPos(i - 4, 1) + 8
            StdLaskerMillPos(i, 2) = StdLaskerMillPos(i - 4, 2) + 8
        Next
        StdLaskerMillPos(12, 0) = 0
        StdLaskerMillPos(13, 0) = 2
        StdLaskerMillPos(14, 0) = 4
        StdLaskerMillPos(15, 0) = 6
        For i = 12 To 15
            StdLaskerMillPos(i, 1) = StdLaskerMillPos(i, 0) + 8
            StdLaskerMillPos(i, 2) = StdLaskerMillPos(i, 0) + 16
        Next
        For i = 0 To 15
            StdLaskerInvMillPos(i) = Array.CreateInstance(GetType(Integer), 2)
        Next
        Dim kell As Boolean
        For i = 0 To 23 'mezőkön
            Dim l As New List(Of Integer)
            For j = 0 To 15 'StdLaskerMillPositions
                kell = False
                For k = 0 To 2 'StdLaskerMillPosition fields
                    If StdLaskerMillPos(j, k) = i Then kell = True
                Next
                If kell Then
                    l.Add(j)
                End If
            Next
            StdLaskerInvMillPos(i) = l.ToArray()
        Next

        For i = 0 To 23
            For j = 0 To 23
                StdLaskerBoardGraph(i, j) = False
            Next
        Next
        For i = 0 To 6
            StdLaskerBoardGraph(i, i + 1) = True
        Next
        StdLaskerBoardGraph(7, 0) = True
        For i = 8 To 14
            StdLaskerBoardGraph(i, i + 1) = True
        Next
        StdLaskerBoardGraph(15, 8) = True
        For i = 16 To 22
            StdLaskerBoardGraph(i, i + 1) = True
        Next
        StdLaskerBoardGraph(23, 16) = True
        For j = 0 To 6 Step 2
            For i = 0 To 8 Step 8
                StdLaskerBoardGraph(j + i, j + i + 8) = True
            Next
        Next
        For i = 0 To 23
            For j = 0 To 23
                If StdLaskerBoardGraph(i, j) = True Then StdLaskerBoardGraph(j, i) = True
            Next
        Next
        For i = 0 To 23
            StdLaskerALBoardGraph(i, 0) = 0
        Next
        For i = 0 To 23
            For j = 0 To 23
                If StdLaskerBoardGraph(i, j) = True Then
                    StdLaskerALBoardGraph(i, StdLaskerALBoardGraph(i, 0) + 1) = j
                    StdLaskerALBoardGraph(i, 0) += 1
                End If
            Next
        Next



        For i = 0 To 15
            For j = 0 To 2
                MoraMillPos(i, j) = StdLaskerMillPos(i, j)
            Next
        Next
        MoraMillPos(16, 0) = 1
        MoraMillPos(16, 1) = 9
        MoraMillPos(16, 2) = 17
        MoraMillPos(17, 0) = 3
        MoraMillPos(17, 1) = 11
        MoraMillPos(17, 2) = 19
        MoraMillPos(18, 0) = 5
        MoraMillPos(18, 1) = 13
        MoraMillPos(18, 2) = 21
        MoraMillPos(19, 0) = 7
        MoraMillPos(19, 1) = 15
        MoraMillPos(19, 2) = 23

        For i = 0 To 23 'mezőkön
            Dim l As New List(Of Integer)
            For j = 0 To 19 'MoraMalomPozíciókon
                kell = False
                For k = 0 To 2 'a malompozíció mezőin
                    If MoraMillPos(j, k) = i Then kell = True
                Next
                If kell Then
                    l.Add(j)
                End If
            Next
            MoraInvMillPos(i) = l.ToArray
        Next


        For i = 0 To 23
            For j = 0 To 23
                MoraBoardGraph(i, j) = StdLaskerBoardGraph(i, j)
            Next
        Next
        For i = 0 To 15
            MoraBoardGraph(i, i + 8) = True
        Next
        For i = 0 To 23
            For j = 0 To 23
                If MoraBoardGraph(i, j) = True Then MoraBoardGraph(j, i) = True
            Next
        Next
        For i = 0 To 23
            MoraALBoardGraph(i, 0) = 0
        Next
        For i = 0 To 23
            For j = 0 To 23
                If MoraBoardGraph(i, j) = True Then
                    MoraALBoardGraph(i, MoraALBoardGraph(i, 0) + 1) = j
                    MoraALBoardGraph(i, 0) += 1
                End If
            Next
        Next
    End Sub
    Public Function Malome(ByVal m As Integer, ByVal s As GameState) As Integer '-1 ha nincs malom az adott mezőn, különben a StdLaskerMalomPoz beli sorszáma
        Malome = -1
        For i = 0 To InvMillPos(m).Length - 1
            If s.T(MillPos(InvMillPos(m)(i), 0)) = s.T(m) And s.T(MillPos(InvMillPos(m)(i), 1)) = s.T(m) And s.T(MillPos(InvMillPos(m)(i), 2)) = s.T(m) Then
                Malome = InvMillPos(m)(i)
            End If
        Next
    End Function
    Public Function YouCanMove(ByVal s As GameState) As Boolean 'tells whether the next player can move '(doesn't handle the KLE case)
        Debug.Assert(Not s.KLE)
        If s.SetStoneCount(s.SideToMove) = MaxKSZ And s.StoneCount(s.SideToMove) > 3 Then
            For i = 0 To 23
                If s.T(i) = s.SideToMove Then
                    For j = 1 To ALBoardGraph(i, 0)
                        If s.T(ALBoardGraph(i, j)) = -1 Then Return True
                    Next
                End If
            Next
        Else
            Return True
        End If
        Return False
    End Function
    Public Function MindenEllensegesKorongMalomban(ByVal s As GameState) As Boolean
        For i = 0 To 23
            If s.T(i) = 1 - s.SideToMove And Malome(i, s) = -1 Then Return False
        Next
        Return True
    End Function

    Public MaxKSZ As Integer

    Public Main As FrmMain

    Public Function AlphaBetaAvailable() As Boolean
        Return Wrappers.Constants.Variant = Wrappers.Constants.Variants.std And Not Wrappers.Constants.Extended
    End Function

    Public Sub SetVariant()
        ' part of this is copy-pasted in MalomAPI

        Select Case Wrappers.Constants.Variant
            Case Wrappers.Constants.Variants.std
                MillPos = StdLaskerMillPos
                InvMillPos = StdLaskerInvMillPos
                BoardGraph = StdLaskerBoardGraph
                ALBoardGraph = StdLaskerALBoardGraph
                MaxKSZ = 9
                VariantName = "std"
                Main.Text = "Malom (Nine Men's Morris)"
            Case Wrappers.Constants.Variants.lask
                MillPos = StdLaskerMillPos
                InvMillPos = StdLaskerInvMillPos
                BoardGraph = StdLaskerBoardGraph
                ALBoardGraph = StdLaskerALBoardGraph
                MaxKSZ = 10
                VariantName = "lask"
                Main.Text = "Malom (Lasker)"
            Case Wrappers.Constants.Variants.mora
                MillPos = MoraMillPos
                InvMillPos = MoraInvMillPos
                BoardGraph = MoraBoardGraph
                ALBoardGraph = MoraALBoardGraph
                MaxKSZ = 12
                VariantName = "mora"
                Main.Text = "Malom (Morabaraba)"
        End Select

        If Wrappers.Constants.Extended Then
            MaxKSZ = 12
            Main.Text &= " (Extended)"
        End If

        If MaxKSZ = 12 Then
            If Wrappers.Constants.FBD Then
                Main.Text &= " (FBD)"
            Else
                Main.Text &= " (no FBD)"
            End If
        End If

        Main.MnuPly1Computer.Enabled = AlphaBetaAvailable()
        Main.MnuPly2Computer.Enabled = AlphaBetaAvailable()
        Main.MnuPly1Combined.Enabled = AlphaBetaAvailable()
        Main.MnuPly2Combined.Enabled = AlphaBetaAvailable()

        Main.PlayerTypeMenuItems.AddRange(New ToolStripMenuItem() {Main.MnuPl1, Main.MnuPl2, Main.MnuPly1Human, Main.MnuPly2Human, Main.MnuPly1Perfect, Main.MnuPly2Perfect})
        If AlphaBetaAvailable() Then
            Main.PlayerTypeMenuItems.AddRange(New ToolStripMenuItem() {Main.MnuPly1Combined, Main.MnuPly2Combined, Main.MnuPly1Computer, Main.MnuPly2Computer})
        End If

        If Sectors.HasDatabase Then
            If Wrappers.Constants.dd Then
                Main.Text &= " (Ultra-strong solution)"
            Else
                Main.Text &= " (Strong solution)"
            End If
        Else
            Main.Text &= " (NO DATABASE FILES)"
        End If

        If Main.Loaded Then Main.NewGame()
    End Sub

End Module
