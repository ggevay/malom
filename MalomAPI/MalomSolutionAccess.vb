Imports System.IO

Public Class MalomSolutionAccess

    Private Shared pp As PerfectPlayer
    Private Shared lastError As Exception

    Public Shared Function GetBestMove(whiteBitboard As Integer, blackBitboard As Integer, whiteStonesToPlace As Integer, blackStonesToPlace As Integer, playerToMove As Integer, onlyStoneTaking As Boolean) As Integer
        InitializeIfNeeded()

        Dim s = New GameState

        Const W = 0
        Const B = 1

        If (whiteBitboard And blackBitboard) <> 0 Then
            Throw New ArgumentException("whiteBitboard and blackBitboard shouldn't have any overlap")
        End If
        For i = 0 To 23
            If (whiteBitboard And (1 << i)) <> 0 Then
                s.T(i) = W
                s.StoneCount(W) += 1
            End If
            If (blackBitboard And (1 << i)) <> 0 Then
                s.T(i) = B
                s.StoneCount(B) += 1
            End If
        Next

        s.phase = If(whiteStonesToPlace = 0 And blackStonesToPlace = 0, 2, 1)
        MustBeBetween("whiteStonesToPlace", whiteStonesToPlace, 0, Rules.MaxKSZ)
        MustBeBetween("blackStonesToPlace", blackStonesToPlace, 0, Rules.MaxKSZ)
        s.SetStoneCount(W) = Rules.MaxKSZ - whiteStonesToPlace
        s.SetStoneCount(B) = Rules.MaxKSZ - blackStonesToPlace
        s.KLE = onlyStoneTaking
        MustBeBetween("playerToMove", playerToMove, 0, 1)
        s.SideToMove = playerToMove
        s.MoveCount = 10

        If s.FutureStoneCount(W) > Rules.MaxKSZ Then
            Throw New ArgumentException("Number of stones in whiteBitboard + whiteStonesToPlace > " & Rules.MaxKSZ)
        End If
        If s.FutureStoneCount(B) > Rules.MaxKSZ Then
            Throw New ArgumentException("Number of stones in blackBitboard + blackStonesToPlace > " & Rules.MaxKSZ)
        End If

        Dim errorMsg = s.SetOverAndCheckValidSetup()
        If errorMsg <> "" Then
            Throw New ArgumentException(errorMsg)
        End If
        If s.over Then
            Throw New ArgumentException("Game is already over.")
        End If

        s.LastIrrev = 0

        Try
            Return pp.ChooseRandom(pp.GoodMoves(s)).ToBitBoard()
        Catch ex As KeyNotFoundException
            Throw New Exception("We don't have a database entry for this position. This can happen either if the database is corrupted (missing files), or sometimes when the position is not reachable from the starting position.")
        End Try

    End Function

    Public Shared Function GetBestMoveNoException(whiteBitboard As Integer, blackBitboard As Integer, whiteStonesToPlace As Integer, blackStonesToPlace As Integer, playerToMove As Integer, onlyStoneTaking As Boolean) As Integer
        Try
            lastError = Nothing
            Return GetBestMove(whiteBitboard, blackBitboard, whiteStonesToPlace, blackStonesToPlace, playerToMove, onlyStoneTaking)
        Catch ex As Exception
            lastError = ex
            Return 0
        End Try
    End Function

    Public Shared Function GetLastError() As String
        If lastError Is Nothing Then
            Return "No error"
        End If
        Return lastError.ToString
    End Function



    Private Shared Sub InitializeIfNeeded()
        If pp IsNot Nothing Then
            Return
        End If
        InitRules()
        SetVariantStripped()
        If Not Sectors.HasDatabase Then
            Throw New FileNotFoundException("Database files not found in the current working directory (" & Directory.GetCurrentDirectory & ")")
        End If
        pp = New PerfectPlayer
    End Sub

    Private Shared Sub MustBeBetween(paramName As String, value As Integer, min As Integer, max As Integer)
        If value < min Or value > max Then
            Throw New ArgumentOutOfRangeException(paramName, value, "must be between " & min & " and " & max)
        End If
    End Sub

    Private Shared Sub SetVariantStripped()
        ' copy-paste from Rules.vb, but references to Main stripped

        Select Case Wrappers.Constants.Variant
            Case Wrappers.Constants.Variants.std
                MillPos = StdLaskerMillPos
                InvMillPos = StdLaskerInvMillPos
                BoardGraph = StdLaskerBoardGraph
                ALBoardGraph = StdLaskerALBoardGraph
                MaxKSZ = 9
                VariantName = "std"
            Case Wrappers.Constants.Variants.lask
                MillPos = StdLaskerMillPos
                InvMillPos = StdLaskerInvMillPos
                BoardGraph = StdLaskerBoardGraph
                ALBoardGraph = StdLaskerALBoardGraph
                MaxKSZ = 10
                VariantName = "lask"
            Case Wrappers.Constants.Variants.mora
                MillPos = MoraMillPos
                InvMillPos = MoraInvMillPos
                BoardGraph = MoraBoardGraph
                ALBoardGraph = MoraALBoardGraph
                MaxKSZ = 12
                VariantName = "mora"
        End Select

        If Wrappers.Constants.Extended Then
            MaxKSZ = 12
        End If
    End Sub

End Class
