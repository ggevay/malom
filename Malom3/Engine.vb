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


Imports System.Threading

Class Engine

    'Dim xorMezők(1, 23) As Int64
    Dim xorMalome, xorAP As Int64
    Dim xorStage As Int64
    Dim St2Moves(63) As Move
    Dim FlyMoves(575) As Move
    Dim CP As Integer 'a számítógép játékos sorszáma
    'Const inf As Integer = Integer.MaxValue - 1
    Const inf As Integer = Integer.MaxValue - 1000000000 '
    Const SureWin As Integer = inf - 1000
    Dim TTSize As Integer
    Dim TT() As TTElem
    Dim InvMPozSld(23, 23) As Byte 'megadja, hogy egy st2-es lépés után MPK-ban hol kell ellenőrzést csinálni
    Dim SMalomPoz(23, 23) As Boolean 'true, ha a két mező azonos malompozícióban van

    Public EndTh As Boolean 'true-ra kell állítani, ha félbe akarjuk szakítani a gondolkodást
    Public OppTime As Boolean 'jelzi, hogy a jelenlegi gondolkodás az ellenfél idejében megy-e
    Const NMVal As Byte = 2 'azoknak a mezőknek az értéke, amikből négy másik mezőre lehet lépni
    Dim BCalcLLNum As Boolean 'figyelje-e a lépéslehetőségek számát az értékelőfüggvény
    Dim NoCheck As Boolean 'az ugrálós lépéseknél a bonyolult ellenőrzés kiiktatására van, amikor nem lehet megfelelőt lépni
    Public MezoÉrtékek(23) As Integer

    Dim ThResult As ThinkResult
    Dim HasResult As Boolean

    Public UseAdv As Boolean
    Public Advisor As PerfectPlayer

    Public ThinkThread As System.Threading.Thread

    Const Ponder = False


    Public Structure Move
        Public honnan As SByte
        Public hová As SByte
        Public flm As SByte '-1 invalid, 0 fölrakás, 1 levétel, 2 mozgás, 3 ugrálás
        Public malome As Boolean
        Public Shared Operator =(ByVal a As Move, ByVal b As Move) As Boolean
            Return a.honnan = b.honnan And a.hová = b.hová
        End Operator
        Public Shared Operator <>(ByVal a As Move, ByVal b As Move) As Boolean
            Return Not (a = b)
        End Operator
    End Structure

    Private Structure TTElem
        Dim key As Int64
        Dim value As Int32
        Dim type As SByte
        Dim Bestmove As Int16
        Dim d As Byte
        Dim lc As Int16 'lépéscount
    End Structure

    Private Structure MVPair
        Dim move As Move
        Dim val As Integer
    End Structure

    Public Structure ThinkResult
        Public BestMove As Move
        Public d As Byte
        Public st As Date
        Public ev As Integer
        Public NN As Int64
    End Structure


    Dim DepthKiír, SetMainText, SetLblPerfEvalText As Action(Of String)

    Sub ignore(x As String)
    End Sub

    Dim LimitDepthForWRGM As Boolean

    Sub New(DepthKiír As Action(Of String), SetMainText As Action(Of String), SetLblPerfEvalText As Action(Of String), LimitDepthForWRGM As Boolean, Optional ByVal UseAdv As Boolean = False)
        Me.UseAdv = UseAdv
        Me.DepthKiír = If(IsNothing(DepthKiír), AddressOf ignore, DepthKiír)
        Me.SetMainText = If(IsNothing(SetMainText), AddressOf ignore, SetMainText)
        Me.SetLblPerfEvalText = If(IsNothing(SetLblPerfEvalText), AddressOf ignore, SetLblPerfEvalText)
        Me.LimitDepthForWRGM = LimitDepthForWRGM
    End Sub


    Public Sub InitEngine()
        Debug.Assert(Rules.AlphaBetaAvailable())
        Dim i, j, kov As Integer
        For i = 0 To 23
            For j = 1 To ALBoardGraph(i, 0)
                St2Moves(kov).honnan = i
                St2Moves(kov).hová = ALBoardGraph(i, j)
                St2Moves(kov).flm = 2
                kov += 1
            Next
        Next
        kov = 0
        For i = 0 To 23
            For j = 0 To 23
                If i <> j Then
                    FlyMoves(kov).honnan = i
                    FlyMoves(kov).hová = j
                    FlyMoves(kov).flm = 3
                    kov += 1
                End If
            Next
        Next
        For i = 0 To 23
            For j = 0 To 23
                If MillPos(InvMillPos(j)(0), 0) <> i And MillPos(InvMillPos(j)(0), 1) <> i And MillPos(InvMillPos(j)(0), 2) <> i Then
                    InvMPozSld(i, j) = InvMillPos(j)(0)
                End If
                If MillPos(InvMillPos(j)(1), 0) <> i And MillPos(InvMillPos(j)(1), 1) <> i And MillPos(InvMillPos(j)(1), 2) <> i Then
                    InvMPozSld(i, j) = InvMillPos(j)(1)
                End If
            Next
        Next
        For i = 0 To 15
            SMalomPoz(MillPos(i, 0), MillPos(i, 1)) = True
            SMalomPoz(MillPos(i, 0), MillPos(i, 2)) = True
            SMalomPoz(MillPos(i, 1), MillPos(i, 2)) = True
            SMalomPoz(MillPos(i, 1), MillPos(i, 0)) = True
            SMalomPoz(MillPos(i, 2), MillPos(i, 0)) = True
            SMalomPoz(MillPos(i, 2), MillPos(i, 1)) = True
        Next
        For i = 0 To 23
            MezoÉrtékek(i) = ALBoardGraph(i, 0)
        Next
        'Main.Text = "Av: " & My.Computer.Info.AvailablePhysicalMemory & " Total: " & My.Computer.Info.TotalPhysicalMemory
        'Const MaxTTSize = 70000000
        Const MaxTTSize = 5000000
        Const MinTTSize = 1000000
        If My.Computer.Info.AvailablePhysicalMemory > MaxTTSize * 24 Then
            TTSize = MaxTTSize
        Else
            If My.Computer.Info.AvailablePhysicalMemory > MinTTSize * 24 Then
                TTSize = My.Computer.Info.AvailablePhysicalMemory \ 24
            Else
                TTSize = MinTTSize
            End If
            'MsgBox("Not enough physical memory for MaxTTSize.")
        End If
        'Main.Text &= " TTSize: " & TTSize
        'TTSize = Math.Pow(10, 6)
        'TTSize = 1
        GC.Collect()
ujra:
        Try
            TT = Array.CreateInstance(GetType(TTElem), TTSize)
        Catch ex As OutOfMemoryException
            'MsgBox("Could not allocate TT with TTSize " & TTSize.ToString & ". Trying again with smaller size.")
            TTSize = TTSize * 4 / 5
            GoTo ujra
        End Try

        ResetTT()
        ' ''Dim kh As Int64
        ' ''kh = 1
        ' ''For i = 0 To 23
        ' ''    xorMezők(0, i) = kh
        ' ''    kh *= 2
        ' ''Next
        ' ''For i = 0 To 23
        ' ''    xorMezők(1, i) = kh
        ' ''    kh *= 2
        ' ''Next
        xorMalome = xorMezok(1, 23) * 2
        xorAP = xorMalome * 2
        xorStage = xorAP * 2
        'InitUjEval()

        If UseAdv Then Advisor = New PerfectPlayer
    End Sub

    Private Function InitAndCalc(idolimit As Double) As Engine.ThinkResult
        PreThinkInit()
        Return TopLevelNegaMax(idolimit)
    End Function

    Public Sub StopOpptime()
        EndTh = True
        If OppTime Then ThinkThread.Join()
        OppTime = False
        EndTh = False
    End Sub

    Public cancel As Boolean

    Public Sub CancelThinking()
        cancel = True
        EndTh = True
        If ThinkThread IsNot Nothing AndAlso ThinkThread.ThreadState = ThreadState.Running Then ThinkThread.Join()
        EndTh = False
        cancel = False
    End Sub


    Public Function Think(s0_idolimit As Tuple(Of GameState, Double)) As ThinkResult
        Dim s0 = s0_idolimit.Item1, idolimit = s0_idolimit.Item2

        StopOpptime()

        s = s0
        Dim result = InitAndCalc(idolimit)

        Return result
    End Function

    Public Function OppTimeThink(s0_idolimit As Tuple(Of GameState, Double)) As ThinkResult 'todo: atirni voidra
        If Not Ponder Then Return New ThinkResult

        Dim s0 = s0_idolimit.Item1, idolimit = s0_idolimit.Item2

        s = s0
        Dim result = InitAndCalc(idolimit)

        Return result
    End Function


    Public Function xorMezok(ByVal ap As Integer, ByVal i As Integer) As Int64
        Return CType(1, Int64) << (i + ap * 24)
    End Function

    Private Sub ResetTT()
        Dim r As TTElem
        For i = 0 To TTSize - 1
            TT(i) = r
        Next
    End Sub

    Public s As GameState
    Dim T() As Integer 'Integer 'SByte 'tábla
    Dim AP As Integer 'eredetileg Byte volt 'Integer-ről Int64-re váltás kb 6%-os gyorsulást jelentett egyszer, de most mindegynek tunik (masik gep, mas framework verzio, de lehetett akar meresi hiba is)
    Dim KLe As Boolean 'Koronglevétel
    Dim FKC() As Integer 'Byte 'Fölrakottkorongcount
    Dim KC() As Integer 'Korongcount
    Dim Mob() As Integer 'Mobility
    Dim NV() As Integer 'a játékosok korongjai alatt lévő mezőértékek
    Dim MPK(15, 1) As Byte 'malompozíciókon lévő korongok száma a két játékosnak külön
    Dim Stage As Integer 'Byte 'szakasz
    Dim EvalMD As Integer
    Dim Hash As Integer
    Public Key As Int64 'ha ez uint64, akkor modolasnal decimal.remainder-be megy bele, ami majdnem ketszeres lassulast jelent a teljes programra nezve
    Dim MdMd As Integer 'ugráláskor díjjazza, ha cp támad
    Public NN As Int64 'vizsgált csúcsok száma
    Public Klr(KlrSize) As Integer
    Const KlrSize = 32

    'Dim _MPK(15, 1) As Byte
    'Dim MPCov(1) As Byte
    'Private Property MPK(i As Integer, j As Integer)
    '    Get
    '        Return _MPK(i, j)
    '    End Get
    '    Set(x)
    '        If x = 0 AndAlso _MPK(i, j) <> 0 Then MPCov(j) -= 1
    '        If x <> 0 AndAlso _MPK(i, j) = 0 Then MPCov(j) += 1
    '        _MPK(i, j) = x
    '    End Set
    'End Property

    Public Sub PreThinkInit()
        T = Array.CreateInstance(GetType(Integer), 24)
        For i = 0 To 23
            T(i) = s.T(i)
        Next
        AP = s.SideToMove

        'CP = AP 'ez azért nem lenne jó, mert lehet, hogy az ellenfél idejében gondolkodunk
        'If TypeOf G.Ply(0) Is ComputerPlayer Then CP = 0 Else CP = 1
        'If G.Ply(0) Is Me Then CP = 0 Else If G.Ply(1) Is Me Then CP = 1 Else Throw New InvalidOperationException("PreThinkInit nem sikerült, mert nem találom magamat a játékosok között.")
        If Not OppTime Then CP = AP Else CP = AP Xor 1

        KLe = s.KLE
        'FKC = Main.FölrakottKorongCount 'nem elég shallow copy-t csinálni
        FKC = Array.CreateInstance(GetType(Integer), 2)
        FKC(0) = s.SetStoneCount(0)
        FKC(1) = s.SetStoneCount(1)
        KC = Array.CreateInstance(GetType(Integer), 2)
        KC(0) = s.StoneCount(0)
        KC(1) = s.StoneCount(1)

        'Stage = Main.Szakasz 'ez így hibás lenne, mert a NextPlayer() nem fut le, amikor az utolsó korong fölrakása egy malomcsinálás
        If s.SetStoneCount(0) = 9 AndAlso s.SetStoneCount(1) = 9 Then Stage = 2 Else Stage = 1
        Mob = Array.CreateInstance(GetType(Integer), 2)
        NV = Array.CreateInstance(GetType(Integer), 2)
        NV(0) = KC(0) * 4
        NV(1) = KC(1) * 4
        BCalcLLNum = True 'Settings.ChkCalcLLNum.Checked
        '
        For i = 0 To 23
            If T(i) <> -1 Then
                For j = 1 To ALBoardGraph(i, 0)
                    If T(ALBoardGraph(i, j)) = -1 Then Mob(T(i)) += 1
                Next
            End If
        Next
        For i = 8 To 14 Step 2
            If T(i) > -1 Then Mob(T(i)) += NMVal
        Next
        Mob(0) += 1
        Mob(1) += 1
        'Main.Text = Mob(0) & " " & Mob(1)
        '
        Dim korr As Integer = 15
        Mob(0) += korr
        Mob(1) += korr
        For i = 0 To 15
            MPK(i, 0) = 0
            MPK(i, 1) = 0
        Next
        'MPCov(0) = 1
        'MPCov(1) = 1
        For i = 0 To 23
            If T(i) <> -1 Then
                MPK(InvMillPos(i)(0), T(i)) += 1
                MPK(InvMillPos(i)(1), T(i)) += 1
            End If
        Next
        Key = 0
        For i = 0 To 23
            If T(i) > -1 Then Key = Key Xor xorMezok(T(i), i)
        Next
        If AP = 1 Then Key = Key Xor xorAP
        If KLe Then Key = Key Xor xorMalome
        If Stage = 2 Then Key = Key Xor xorStage
        Hash = Key Mod TTSize
        MdMd = 0
        NN = 0
        EvalMD = 0
    End Sub


    Class AbortThinkingException
        Inherits Exception
    End Class

    Dim NodeCountLimit As Integer
    Public Function TopLevelNegaMax(idolimit As Double) As ThinkResult
        Dim MoveList() As MVPair
        Dim m, mh, emh As Move
        Dim max, r, i, j, k, nummoves As Integer
        Dim rnd As New Random
        Dim sz As Byte = 2
        Dim buf As MVPair
        max = -inf

        If Not UseAdv Then
            'vigyazat, ez copy-paste-elve van a lenti catch-be
            MoveList = GetMoveList()
            nummoves = MoveList.GetUpperBound(0)
            If nummoves = -1 Then
                NoCheck = True
                MoveList = GetMoveList()
                nummoves = MoveList.GetUpperBound(0)
                NoCheck = False
            End If
        Else
            Try
                Dim MoveList0 As New List(Of MVPair)
                Dim OkMoves = Advisor.GoodMoves(s)

                For Each pm In OkMoves
                    Dim mv As Move
                    If pm.OnlyTaking Then
                        mv.hová = -1
                        mv.honnan = pm.TakeHon
                        mv.flm = 1
                    Else
                        If pm.MoveType = PerfectPlayer.MoveType.SetMove Then
                            mv.hová = pm.hov
                            mv.honnan = -1
                            mv.flm = 0
                        Else 'SlideMove
                            mv.honnan = pm.hon
                            mv.hová = pm.hov
                            mv.flm = If(s.StoneCount(s.SideToMove) = 3 And s.SetStoneCount(s.SideToMove) = 9, 3, 2)
                        End If
                        mv.malome = pm.WithTaking
                    End If
                    MoveList0.Add(New MVPair With {.move = mv})
                Next
                MoveList = MoveList0.ToArray()
                nummoves = MoveList.GetUpperBound(0)
            Catch ex As KeyNotFoundException
                MoveList = GetMoveList()
                nummoves = MoveList.GetUpperBound(0)
                If nummoves = -1 Then
                    NoCheck = True
                    MoveList = GetMoveList()
                    nummoves = MoveList.GetUpperBound(0)
                    NoCheck = False
                End If
            End Try
        End If

        Dim maxsz As Integer = 100 'Settings.CalcDepths(s)

        Dim st As System.DateTime = System.DateTime.Now
        Dim ud As UndoData

        ThResult = New ThinkResult
        HasResult = False

        DepthKiír(sz)
        'SetMainText("")

        'Const BaseNodeCountLimit As Integer = 100000
        'NodeCountLimit = BaseNodeCountLimit * (1 + rnd.NextDouble() * 3) '''3-ra atirtam 1-rol
        'NodeCountLimit = BaseNodeCountLimit * (1 + rnd.NextDouble() * 1)
        Try
            'WrongProbCuts = 0 : OkProbCuts = 0
            For i = 0 To nummoves
                For j = 0 To KlrSize
                    Klr(j) = -1
                Next
                m = MoveList(i).move
                DoMove(m, ud)
                If m.malome Then
                    'r = NegaMax(sz, max, inf, 0)
                    r = NegaMax(sz, max - 1, inf, 0) 'StoreKLR:=True
                Else
                    'r = -NegaMax(sz - 1, -inf, -max, 1)
                    r = -NegaMax(sz - 1, -inf, -(max - 1), 1) 'StoreKLR:=True
                End If
                'IO.File.AppendAllText("C:\values.txt", m.hová & " " & r & vbCrLf)
                MoveList(i).val = r
                UndoMove(m, ud)
                If If(rnd.NextDouble > 2 / 3, r > max, r >= max) Then
                    max = r
                    mh = m
                End If
                If i = nummoves Then
                    ThResult = New ThinkResult With {.BestMove = mh, .d = sz, .ev = max, .NN = NN, .st = st}
                    HasResult = True
                    'SetMainText(If(sz = 4 Or sz = 8, Main.Text & " (" & max & ")", Main.Text & " " & max))
                    'If (System.DateTime.Now - st).TotalMilliseconds > 1000 * Main.Időlimit AndAlso Not OppTime Then Exit For
                    If Not LimitDepthForWRGM Then
                        If (System.DateTime.Now - st).TotalMilliseconds > 1000 * idolimit AndAlso Not OppTime AndAlso Not (s.MoveCount = 7 And sz = 9) Then Exit For 'van egy gyakori játszmavariáció, aminél a 7. lépésnél nagyon rosszat lép, ha 9-es mélységben gondolja végig
                    Else
                        If If(s.MoveCount < 6, sz > 7, If(s.MoveCount < 10, sz > 8, sz > 10)) Then Exit For 'A WRGM jelenleg csak ezzel mukodik
                        'If sz > If(s.LépésCount < 6, 6, If(s.LépésCount < 10, 9, 11)) - 5 Then Exit For 'rekurziv WRGM
                    End If

                    For j = 0 To nummoves - 1
                        For k = 0 To nummoves - 1
                            If MoveList(k).val < MoveList(k + 1).val Then
                                buf = MoveList(k)
                                MoveList(k) = MoveList(k + 1)
                                MoveList(k + 1) = buf
                            End If
                        Next
                    Next
                    i = -1
                    If sz < KlrSize Then sz += 1 Else Exit For
                    'If sz < KlrSize - 1 Then sz += 2
                    max = -inf
                    emh = mh
                    If s.MoveCount <= 3 Or nummoves = 0 Then Exit For 'marmint a nummoves valojaban upper bound, szoval a 0 az itt 1 lepest jelent
                    If ThResult.ev > SureWin Then Exit For
                    If ThResult.ev < -SureWin Then Exit For
                    If sz > maxsz Then Exit For
                    'If sz = 8 + 1 Or OppTime Then Exit For '
                    If sz > 6 Or maxsz <= 6 Then DepthKiír(sz)
                End If
            Next
        Catch ex As AbortThinkingException
            Debug.Assert(HasResult)
            ThResult.NN = NN
            Return ThResult
        End Try
        'If Not OppTime Then InvokeUseThResult()
        Return ThResult
    End Function

    Private Function NegaMax(ByVal d As Byte, ByVal alfa As Integer, ByVal beta As Integer, ByVal dd As Byte) As Integer 'ByVal storeKLR As Boolean 'a Probcut-nál kell
        'If NN >= NodeCountLimit Then 'don't forget to set the soft and hard time limits to infinity
        '    Throw New AbortThinkingException
        'End If
        NN += 1
        If Stage = 2 AndAlso KC(AP) < 3 Then Return -inf + 300 - d
        If d = 0 Then Return Eval()
        Dim Bestmove As Short = -1
        Dim value As Integer
        'If Lookup(d, alfa, beta, Bestmove, value) Then
        If d > 1 AndAlso Lookup(d, alfa, beta, Bestmove, value) Then 'Úgy tűnik, hogy a d > 1 feltétel általában kicsit vagy semennyit sem gyorsít, ritkán nagyot gyorsít, és csak ritkán lassít,
            Return value
        End If
        Dim LocHash = Hash
        Dim i, r, mh As Integer
        Dim M As Move
        Dim max As Integer = -inf + 300 - d
        Dim UD As UndoData
        Dim UDEtc As UndoDataETC
        Dim LLc As Integer = fLLc() 'lépéslehetőségek száma
        '
        Dim etc As Boolean
        If d > 2 Then
            For i = 0 To LLc
                If i > -1 AndAlso Stage = 2 Then
                    If KC(AP) > 3 AndAlso T(St2Moves(i).honnan) <> AP AndAlso Not KLe Then
                        i += ALBoardGraph(St2Moves(i).honnan, 0) - 1 : Continue For
                    ElseIf KC(AP) = 3 AndAlso T(FlyMoves(i).honnan) <> AP AndAlso Not KLe Then
                        i += 22 : Continue For
                    End If
                End If
                M = GetMove(i)
                If M.flm <> -1 Then
                    UDEtc = DoMoveETC(M)
                    Hash = Key Mod TTSize
                    If TT(Hash).key = Key Then
                        If Not M.malome Then
                            If d - 1 <= TT(Hash).d Then
                                If TT(Hash).type <= 0 Then
                                    If -TT(Hash).value >= beta Then '
                                        etc = True
                                        max = -TT(Hash).value
                                        mh = i
                                    End If
                                End If
                            End If
                        Else
                            If d <= TT(Hash).d Then
                                If TT(Hash).type >= 0 Then
                                    If TT(Hash).value >= beta Then '
                                        etc = True
                                        max = TT(Hash).value
                                        mh = i
                                    End If
                                End If
                            End If
                        End If
                    End If
                    'UndoMoveETC(M, UDEtc)
                    Key = UDEtc.key 'beinline-olva a fentebbi hivas 'talan azert nem inline-olja be, mert struct-ot kap parameterkent? (bar azt irjak 2007-ben, hogy ezt kijavitjak) (http://blogs.msdn.com/b/clrcodegeneration/archive/2007/11/02/how-are-value-types-implemented-in-the-32-bit-clr-what-has-been-done-to-improve-their-performance.aspx)
                    If etc Then Exit For
                End If
            Next
        End If
        '
        '
        'Dim WHRIPC As Boolean = False  'probcut miatt tért volna vissza
        'If d = 8 Then
        '    Dim far As Integer = 0.1 * 1000000 '0.1 * 1000000
        '    Dim e = NegaMax(4, alfa - far, beta + far, dd + 1, False)
        '    If e <= alfa - far Or e >= beta + far Then Return e 'WHRIPC = True
        'End If
        '
        Dim kleklr As Integer
        If Not etc Then
            'For i = If(Bestmove = -1, 0, -1) To LLc
            'kleklr = Klr(dd) 'elmentjük Klr(dd)-t
            If KLe Then kleklr = Klr(dd) : Klr(dd) = -1 'ha koronglevétel van, akkor ne foglalkozzunk Klr-rel
            If Klr(dd) > LLc Then Klr(dd) = -1
            If Bestmove > -1 Then M = GetMove(Bestmove)
            For i = If(Bestmove = -1, -1, -2) To LLc
                If i = -1 AndAlso Klr(dd) = -1 Then i += 1
                If i > -1 AndAlso i = Klr(dd) Then i += 1 : If i > LLc Then Exit For
                If i > -1 AndAlso i = Bestmove Then i += 1 : If i > LLc Then Exit For
                '
                If i > -1 AndAlso Stage = 2 Then 'ha st2-ben vagyunk, és nincs az induló mezőn megfelelő korong, akkor ugorjunk a következő induló mezőre.
                    If KC(AP) > 3 AndAlso T(St2Moves(i).honnan) <> AP AndAlso Not KLe Then
                        i += ALBoardGraph(St2Moves(i).honnan, 0) - 1 : Continue For
                    ElseIf KC(AP) = 3 AndAlso T(FlyMoves(i).honnan) <> AP AndAlso Not KLe Then
                        i += 22 : Continue For
                    End If
                End If
                '
                If i > -1 Then M = GetMove(i) Else If i = -1 Then M = GetMove(Klr(dd))
                'If i = Bestmove Then i += 1 : If i > Lim Then Exit For
                'If i = -1 Then M = GetMove(Bestmove) Else M = GetMove(i)
                If M.flm <> -1 Then
                    DoMove(M, UD)
                    If M.malome Then
                        r = NegaMax(d, If(max > alfa, max, alfa), beta, dd) ',storeKLR
                    Else
                        r = -NegaMax(d - 1, -beta, -If(max > alfa, max, alfa), dd + 1) ',storeKLR
                    End If
                    UndoMove(M, UD)
                    If r > max Then max = r : mh = i
                    If max >= beta Then Exit For '
                End If
            Next
        End If
        'If storeKLR Then 'ez a probcut miatt kell
        Klr(dd) = If(mh > -1, mh, If(mh = -1, Klr(dd), Bestmove))
        'End If
        Hash = LocHash
        'If d + s.LépésCount > TT(Hash).d + TT(Hash).lc Then
        If d > 1 AndAlso d + s.MoveCount > TT(Hash).d + TT(Hash).lc Then
            'TT(Hash).Bestmove = If(mh = -1, Bestmove, mh)
            TT(Hash).Bestmove = If(mh = -2, Bestmove, If(mh = -1, Klr(dd), mh))
            TT(Hash).d = d
            TT(Hash).key = Key
            TT(Hash).value = max
            TT(Hash).lc = s.MoveCount
            If max <= alfa Then TT(Hash).type = -1 Else If max >= beta Then TT(Hash).type = 1 Else TT(Hash).type = 0 '
        End If
        If EndTh AndAlso HasResult Then
            EndTh = False
            ThResult.NN = NN
            'If Not OppTime Then InvokeUseThResult()
            Throw New AbortThinkingException
        End If
        If KLe Then Klr(dd) = kleklr
        'If WHRIPC Then If max > alfa And max < beta Then WrongProbCuts += 1 Else OkProbCuts += 1
        Return max
    End Function
    Private Function Lookup(ByVal d As Byte, ByRef alfa As Integer, ByRef beta As Integer, ByRef Bestmove As Short, ByRef Value As Integer) As Boolean 'Vajon miért push-olja itt az rbx-et?
        Hash = Key Mod TTSize
        If TT(Hash).key <> Key Then Return False
        If d <= TT(Hash).d Then
            If TT(Hash).type = 0 Then
                Value = TT(Hash).value
                Return True
            Else
                If TT(Hash).type = -1 Then
                    If TT(Hash).value < beta Then beta = TT(Hash).value
                    If beta <= alfa Then Value = beta : Return True '
                Else 'type = 1
                    If TT(Hash).value > alfa Then alfa = TT(Hash).value
                    If alfa >= beta Then Value = alfa : Return True '
                End If
                Bestmove = TT(Hash).Bestmove
                Return False
            End If
        Else
            Bestmove = TT(Hash).Bestmove
            Return False
        End If
    End Function
    Private Function GetMoveList() As MVPair()
        Dim MoveList() As MVPair = Array.CreateInstance(GetType(MVPair), 64)
        Dim i, NumMoves As Integer
        Dim m As Move
        For i = 0 To fLLc()
            m = GetMove(i)
            If m.flm > -1 Then
                MoveList(NumMoves).move = GetMove(i)
                NumMoves += 1
            End If
        Next
        Dim RetMoveList() As MVPair = Array.CreateInstance(GetType(MVPair), NumMoves)
        Array.Copy(MoveList, RetMoveList, NumMoves)
        Return RetMoveList
    End Function
    Private Function fLLc() As Integer 'lépéslehetőségek száma
        If KLe Then Return 23
        If Stage = 1 Then Return 23
        If Stage = 2 Then If KC(AP) > 3 Then Return 63 Else Return 575
        Return -1
    End Function
    Private Function GetMove(ByVal i As Integer) As Move
        Dim M As Move
        M.flm = -1
        M.honnan = -1
        M.hová = -1
        MdMd = 0
        If Not KLe Then
            If Stage = 1 Then 'Stage = 1
                If T(i) = -1 Then
                    M.hová = i
                    M.flm = 0
                    'M.malome = MalomeHov(M.hová)
                    If MPK(InvMillPos(M.hová)(0), AP) = 2 Then M.malome = True
                    If MPK(InvMillPos(M.hová)(1), AP) = 2 Then M.malome = True
                End If
            Else 'Stage = 2
                If KC(AP) > 3 Then 'mozgás
                    If T(St2Moves(i).honnan) = AP AndAlso T(St2Moves(i).hová) = -1 Then
                        M = St2Moves(i)
                        'M.malome = MalomeHov(M.hová)
                        If MPK(InvMPozSld(M.honnan, M.hová), AP) = 2 Then M.malome = True
                    End If
                Else 'ugrálás
                    'If T(FlyMoves(i).honnan) = AP AndAlso T(FlyMoves(i).hová) = -1 AndAlso _
                    '    (NoCheck OrElse (KC(1 - AP) > 3 OrElse If(SMalomPoz(FlyMoves(i).honnan, FlyMoves(i).hová), _
                    '    (MPK(InvMPozSld(FlyMoves(i).honnan, FlyMoves(i).hová), AP) >= 1 AndAlso MPK(InvMPozSld(FlyMoves(i).honnan, FlyMoves(i).hová), 1 - AP) = 0) OrElse MPK(InvMPozSld(FlyMoves(i).honnan, FlyMoves(i).hová), 1 - AP) = 2, _
                    '    (MPK(InvMalomPoz(FlyMoves(i).hová, 0), AP) >= 1 AndAlso MPK(InvMalomPoz(FlyMoves(i).hová, 0), 1 - AP) = 0) OrElse MPK(InvMalomPoz(FlyMoves(i).hová, 0), 1 - AP) = 2 OrElse _
                    '    (MPK(InvMalomPoz(FlyMoves(i).hová, 1), AP) >= 1 AndAlso MPK(InvMalomPoz(FlyMoves(i).hová, 1), 1 - AP) = 0) OrElse MPK(InvMalomPoz(FlyMoves(i).hová, 1), 1 - AP) = 2))) Then
                    '    M = FlyMoves(i)
                    '    'M.malome = MalomeHov(M.hová) 'így hibás lenne
                    '    'If MPK(InvMalomPoz(M.hová, 0), AP) = 2 Then M.malome = True
                    '    'If MPK(InvMalomPoz(M.hová, 1), AP) = 2 Then M.malome = True
                    '    T(M.hová) = AP
                    '    T(M.honnan) = -1
                    '    M.malome = Malome(M.hová)
                    '    T(M.honnan) = AP
                    '    T(M.hová) = -1
                    '    'If AP = CP Then If MPK(InvMalomPoz(M.hová, 0), 1 - AP) = 2 OrElse MPK(InvMalomPoz(M.hová, 1), 1 - AP) = 2 Then MdMd = -10000 Else MdMd = 10000
                    '    Dim mhov As Byte = M.hová
                    '    If AP = CP Then If MPK(InvMalomPoz(mhov, 0), 1 - AP) = 2 OrElse MPK(InvMalomPoz(mhov, 1), 1 - AP) = 2 Then MdMd = -10000 Else MdMd = 10000
                    'End If
                    GetFlyMove(i, M) 'ezt, és a GetKLEMove-ot azért érdemes kiemelni külön függvénybe, mert különben egy csomó register-t push-ol a függvény elején fölöslegesen, ahelyett, hogy csak ezekben a blokkokban push-olná (akkor is, ha az egyik már ki van emelve)
                End If
            End If
        Else 'Koronglevétel
            'If T(i) = 1 - AP AndAlso ((MPK(InvMalomPoz(i, 0), 1 - AP) <> 3 AndAlso MPK(InvMalomPoz(i, 1), 1 - AP) <> 3) OrElse KC(1 - AP) = 3) Then
            '    M.honnan = i
            '    M.flm = 1
            'End If
            GetKLEMove(i, M)
        End If
        Return M
    End Function
    Private Sub GetFlyMove(ByVal i As Integer, ByRef M As Move)
        If T(FlyMoves(i).honnan) = AP AndAlso T(FlyMoves(i).hová) = -1 AndAlso _
                        (NoCheck OrElse (KC(1 - AP) > 3 OrElse If(SMalomPoz(FlyMoves(i).honnan, FlyMoves(i).hová), _
                        (MPK(InvMPozSld(FlyMoves(i).honnan, FlyMoves(i).hová), AP) >= 1 AndAlso MPK(InvMPozSld(FlyMoves(i).honnan, FlyMoves(i).hová), 1 - AP) = 0) OrElse MPK(InvMPozSld(FlyMoves(i).honnan, FlyMoves(i).hová), 1 - AP) = 2, _
                        (MPK(InvMillPos(FlyMoves(i).hová)(0), AP) >= 1 AndAlso MPK(InvMillPos(FlyMoves(i).hová)(0), 1 - AP) = 0) OrElse MPK(InvMillPos(FlyMoves(i).hová)(0), 1 - AP) = 2 OrElse _
                        (MPK(InvMillPos(FlyMoves(i).hová)(1), AP) >= 1 AndAlso MPK(InvMillPos(FlyMoves(i).hová)(1), 1 - AP) = 0) OrElse MPK(InvMillPos(FlyMoves(i).hová)(1), 1 - AP) = 2))) Then
            M = FlyMoves(i)
            'M.malome = MalomeHov(M.hová) 'így hibás lenne
            'If MPK(InvMalomPoz(M.hová, 0), AP) = 2 Then M.malome = True
            'If MPK(InvMalomPoz(M.hová, 1), AP) = 2 Then M.malome = True
            T(M.hová) = AP
            T(M.honnan) = -1
            M.malome = Malome(M.hová)
            T(M.honnan) = AP
            T(M.hová) = -1
            'If AP = CP Then If MPK(InvMalomPoz(M.hová, 0), 1 - AP) = 2 OrElse MPK(InvMalomPoz(M.hová, 1), 1 - AP) = 2 Then MdMd = -10000 Else MdMd = 10000
            Dim mhov As Byte = M.hová
            If AP = CP Then If MPK(InvMillPos(mhov)(0), 1 - AP) = 2 OrElse MPK(InvMillPos(mhov)(1), 1 - AP) = 2 Then MdMd = -10000 Else MdMd = 10000
        End If
    End Sub
    Private Sub GetKLEMove(ByVal i As Integer, ByRef m As Move)
        If T(i) = 1 - AP AndAlso ((MPK(InvMillPos(i)(0), 1 - AP) <> 3 AndAlso MPK(InvMillPos(i)(1), 1 - AP) <> 3) OrElse KC(1 - AP) = 3) Then
            m.honnan = i
            m.flm = 1
        End If
    End Sub
    Private Function Malome(ByVal mezo As Byte) As Boolean
        Malome = False
        'lehetne cache-elni a T(mezo)-t, meg az InvMillPos(mezo)(i)-t
        For i As Integer = 0 To 1
            If T(MillPos(InvMillPos(mezo)(i), 0)) = T(mezo) AndAlso T(MillPos(InvMillPos(mezo)(i), 1)) = T(mezo) AndAlso T(MillPos(InvMillPos(mezo)(i), 2)) = T(mezo) Then
                Malome = True 'ezt at kene irni returnre
            End If
        Next
    End Function

    Structure UndoData
        Dim Mob0, Mob1 As Integer
        Dim malome As Boolean
        Dim eval As Integer
        Dim key As Int64
    End Structure
    Private Sub DoMove(ByVal M As Move, ByRef UD As UndoData)
        UD.malome = False
        UD.eval = 0
        UD.Mob0 = Mob(0)
        UD.Mob1 = Mob(1)
        If Not M.malome Then UD.eval = NTrEval() + MdMd
        EvalMD += UD.eval
        UD.key = Key
        Select Case M.flm '-1 invalid, 0 fölrakás, 1 levétel, 2 mozgás, 3 ugrálás
            Case 0
                T(M.hová) = AP
                KC(AP) += 1
                FKC(AP) += 1
                If FKC(0) = 9 And FKC(1) = 9 Then
                    Stage = 2
                    Key = Key Xor xorStage
                End If
                CalcMob(M.honnan, M.hová)
                'If Settings.CalcNodeValues Then NV(AP) += MezőÉrtékek(M.hová)
                MPK(InvMillPos(M.hová)(0), AP) += 1
                MPK(InvMillPos(M.hová)(1), AP) += 1
                Key = Key Xor xorMezok(AP, M.hová)
                If M.malome Then Key = Key Xor xorMalome
            Case 1
                T(M.honnan) = -1
                KC(1 - AP) -= 1
                CalcMobKLe(M.honnan)
                MPK(InvMillPos(M.honnan)(0), 1 - AP) -= 1
                MPK(InvMillPos(M.honnan)(1), 1 - AP) -= 1
                'If Settings.CalcNodeValues Then NV(1 - AP) -= MezőÉrtékek(M.honnan)
                Key = Key Xor xorMezok(1 - AP, M.honnan)
                Key = Key Xor xorMalome
            Case 2, 3
                T(M.honnan) = -1
                CalcMob(M.honnan, M.hová)
                T(M.hová) = AP
                MPK(InvMillPos(M.hová)(0), AP) += 1
                MPK(InvMillPos(M.hová)(1), AP) += 1
                MPK(InvMillPos(M.honnan)(0), AP) -= 1
                MPK(InvMillPos(M.honnan)(1), AP) -= 1
                'If Settings.CalcNodeValues Then NV(AP) += MezőÉrtékek(M.hová) - MezőÉrtékek(M.honnan)
                Key = Key Xor xorMezok(AP, M.honnan)
                Key = Key Xor xorMezok(AP, M.hová)
                If M.malome Then Key = Key Xor xorMalome
        End Select
        If M.flm <> 1 Then
            If M.malome Then
                KLe = True
                UD.malome = True
            Else
                AP = 1 - AP
                Key = Key Xor xorAP
            End If
        Else 'levétel
            AP = 1 - AP
            Key = Key Xor xorAP
            KLe = False
        End If
    End Sub
    Structure UndoDataETC
        Dim key As Int64
    End Structure
    Private Function DoMoveETC(ByVal M As Move) As UndoDataETC
        Dim UD As UndoDataETC
        UD.key = Key
        Select Case M.flm '-1 invalid, 0 fölrakás, 1 levétel, 2 mozgás, 3 ugrálás
            Case 0
                FKC(AP) += 1
                If FKC(0) = 9 And FKC(1) = 9 Then
                    Key = Key Xor xorStage
                End If
                FKC(AP) -= 1 'talán gyorsabb itt visszavonni, és úgyse használjuk az UndoMoveETC előtt
                Key = Key Xor xorMezok(AP, M.hová)
                If M.malome Then Key = Key Xor xorMalome
            Case 1
                Key = Key Xor xorMezok(1 - AP, M.honnan)
                Key = Key Xor xorMalome
            Case 2, 3
                Key = Key Xor xorMezok(AP, M.honnan)
                Key = Key Xor xorMezok(AP, M.hová)
                If M.malome Then Key = Key Xor xorMalome
        End Select
        If M.flm <> 1 Then
            If M.malome Then
            Else
                Key = Key Xor xorAP
            End If
        Else 'levétel
            Key = Key Xor xorAP
        End If
        Return UD
    End Function
    Private Sub UndoMove(ByVal M As Move, ByVal UD As UndoData)
        Mob(0) = UD.Mob0
        Mob(1) = UD.Mob1
        EvalMD -= UD.eval
        Key = UD.key
        If M.flm <> 1 Then
            If UD.malome Then
                KLe = False
            Else
                AP = 1 - AP
            End If
        Else 'levétel
            AP = 1 - AP
            KLe = True
        End If
        Select Case M.flm '-1 invalid, 0 fölrakás, 1 levétel, 2 mozgás, 3 ugrálás
            Case 0
                T(M.hová) = -1
                KC(AP) -= 1
                If FKC(0) = 9 And FKC(1) = 9 Then Stage = 1
                FKC(AP) -= 1
                MPK(InvMillPos(M.hová)(0), AP) -= 1
                MPK(InvMillPos(M.hová)(1), AP) -= 1
                'If Settings.CalcNodeValues Then NV(AP) -= MezőÉrtékek(M.hová)
            Case 1
                T(M.honnan) = 1 - AP
                KC(1 - AP) += 1
                MPK(InvMillPos(M.honnan)(0), 1 - AP) += 1
                MPK(InvMillPos(M.honnan)(1), 1 - AP) += 1
                'If Settings.CalcNodeValues Then NV(1 - AP) += MezőÉrtékek(M.honnan)
            Case 2, 3
                T(M.honnan) = AP
                T(M.hová) = -1
                MPK(InvMillPos(M.hová)(0), AP) -= 1
                MPK(InvMillPos(M.hová)(1), AP) -= 1
                MPK(InvMillPos(M.honnan)(0), AP) += 1
                MPK(InvMillPos(M.honnan)(1), AP) += 1
                'If Settings.CalcNodeValues Then NV(AP) -= MezőÉrtékek(M.hová) - MezőÉrtékek(M.honnan)
        End Select
    End Sub
    'Private Sub UndoMoveETC(ByVal M As Move, ByVal UD As UndoDataETC) 'Be van inline-olva, mert a JIT compiler nem teszi meg
    '    Key = UD.key
    'End Sub

    Const NMMask As Integer = 21760 '101 0101 0000 0000

    Private Sub CalcMob(ByVal honnan As SByte, ByVal hová As SByte)
        Dim i As Byte
        If honnan > -1 Then
            For i = 1 To ALBoardGraph(honnan, 0)
                Select Case T(ALBoardGraph(honnan, i))
                    Case -1 : Mob(AP) -= 1
                    Case AP : Mob(AP) += 1
                        'Case 1 - AP : Mob(1 - AP) += 1
                    Case Else : Mob(1 - AP) += 1
                End Select
            Next
            If ((1 << honnan) And NMMask) <> 0 Then Mob(AP) -= NMVal
        End If
        If hová > -1 Then
            For i = 1 To ALBoardGraph(hová, 0)
                Select Case T(ALBoardGraph(hová, i))
                    Case -1 : Mob(AP) += 1
                    Case AP : Mob(AP) -= 1
                        'Case 1 - AP : Mob(1 - AP) -= 1
                    Case Else : Mob(1 - AP) -= 1
                End Select
            Next
            If ((1 << hová) And NMMask) <> 0 Then Mob(AP) += NMVal
        End If
    End Sub
    Private Sub CalcMobKLe(ByVal honnan As SByte)
        Dim i As Byte
        If honnan > -1 Then
            For i = 1 To ALBoardGraph(honnan, 0)
                Select Case T(ALBoardGraph(honnan, i))
                    Case -1 : Mob(1 - AP) -= 1
                    Case AP : Mob(AP) += 1
                    Case 1 - AP : Mob(1 - AP) += 1
                End Select
            Next
            For i = 8 To 14 Step 2
                If honnan = i Then Mob(1 - AP) -= NMVal
            Next
        End If
    End Sub
    Private Function Eval() As Integer
        Dim r As Integer
        'r = (KC(CP) - KC(1 - CP)) * 1000
        'r += -Math.Sign(r) * KC(CP)
        'r += (Mob(CP) - Mob(1 - CP)) * 500

        If KC(0) = 0 Or KC(1) = 0 Then
            KC(0) += 1
            KC(1) += 1
            r = Eval()
            KC(0) -= 1
            KC(1) -= 1
            Return r
        End If

        If BCalcLLNum Then
            If Stage = 1 Then
                r = 500000 * KC(CP) \ KC(1 - CP) + 500000 * Mob(CP) \ Mob(1 - CP) 'ez az alap

                'r = 444445 * KC(CP) \ KC(1 - CP) + 444444 * Mob(CP) \ Mob(1 - CP) + 111111 * MPCov(CP) \ MPCov(1 - CP)

                'r = (500000 + md2) * KC(CP) \ KC(1 - CP) + (500000 - md2) * Mob(CP) \ Mob(1 - CP)

                'r = 666666 * KC(CP) \ KC(1 - CP) + 333333 * Mob(CP) \ Mob(1 - CP)

                'r = 600000 * KC(CP) \ KC(1 - CP) + 400000 * Mob(CP) \ Mob(1 - CP)
                'r = 400000 * KC(CP) \ KC(1 - CP) + 600000 * Mob(CP) \ Mob(1 - CP)
                'If AP = 0 Then
                '    r = 500000 * KC(CP) \ KC(1 - CP) + 500000 * Mob(CP) \ Mob(1 - CP)
                'Else
                '    KC(1) += 1
                '    Mob(1) += 1
                '    r = 500000 * KC(CP) \ KC(1 - CP) + 500000 * Mob(CP) \ Mob(1 - CP)
                '    Mob(1) -= 1
                '    KC(1) -= 1
                'End If
                'r = 1000000 * KC(CP) \ KC(1 - CP)
            Else
                If KC(1 - CP) > 3 And KC(CP) > 3 Then
                    'If TudMalmotBezárni() Then 'ez talán bugos lehet, vagy legalábbis gyengíteni látszik
                    '    KC(1 - AP) -= 1
                    '    r = 666666 * KC(CP) \ KC(1 - CP) + 333333 * Mob(CP) \ Mob(1 - CP)
                    '    KC(1 - AP) += 1
                    'Else

                    r = 666666 * KC(CP) \ KC(1 - CP) + 333333 * Mob(CP) \ Mob(1 - CP) 'ennyi itt az alap

                    'r = 555556 * KC(CP) \ KC(1 - CP) + 333333 * Mob(CP) \ Mob(1 - CP) + 111111 * MPCov(CP) \ MPCov(1 - CP)

                    'If KC(1 - CP) >= 6 And KC(CP) >= 6 Then
                    '    Static rnd As New Random()
                    '    Static bb As Boolean
                    '    If Not bb Then
                    '        bb = True
                    '        If rnd.Next(10000) = 0 Then System.IO.File.AppendAllText("teszt.txt", -NegaMax(8, -inf + 1, inf - 1, 10) & " " & 1000000 * KC(CP) \ KC(1 - CP) & " " & 1000000 * Mob(CP) \ Mob(1 - CP) & vbCrLf)
                    '        bb = False
                    '    End If
                    'End If

                    'Dim oMob = 1000000 * Mob(CP) \ Mob(1 - CP)
                    'Dim normMob = (oMob - 1000000) / 1.4 + 1000000
                    'r = (1000000 * KC(CP) \ KC(1 - CP) + normMob) \ 2


                    'újevaluate-es rész
                    'If KC(1 - CP) >= 6 And KC(CP) >= 6 Then
                    '    'r = (666666 * KC(CP) \ KC(1 - CP) + 333333 * Mob(CP) \ Mob(1 - CP) + UjEvaluate()) \ 2
                    '    'r = UjEvaluate()
                    '    'r = (666666 * KC(CP) \ KC(1 - CP) + 333333 * Mob(CP) \ Mob(1 - CP) + 2 * UjEvaluate()) \ 3
                    '    r = (6 * (666666 * KC(CP) \ KC(1 - CP) + 333333 * Mob(CP) \ Mob(1 - CP)) + 4 * UjEvaluate()) \ 10 'ez a legjobb ezek közül
                    '    'r = (500000 + md1) * KC(CP) \ KC(1 - CP) + (500000 - md1) * Mob(CP) \ Mob(1 - CP)
                    'Else
                    '    r = 666666 * KC(CP) \ KC(1 - CP) + 333333 * Mob(CP) \ Mob(1 - CP)
                    'End If

                    'End If
                    'r = 500000 * KC(CP) \ KC(1 - CP) + 500000 * Mob(CP) \ Mob(1 - CP)
                Else
                    r = 1000000 * KC(CP) \ KC(1 - CP)
                End If
            End If
        Else
            'If Settings.CalcNodeValues Then
            'r = 1000000 * NV(CP) \ NV(1 - CP)
            'Else
            r = 1000000 * KC(CP) \ KC(1 - CP)
            'End If
        End If
        r += EvalMD
        If AP = CP Then Return r Else Return -r
    End Function
    Private Function NTrEval() As Integer
        Dim r As Integer
        If Stage = 1 Then
            'r = 1000000 * KC(CP) \ KC(1 - CP) + 1000000 * Mob(CP) \ Mob(1 - CP)
            If KC(1 - CP) > 0 Then r = 100 * KC(CP) \ KC(1 - CP) Else r = 0
        Else
            r = 100 * KC(CP) \ KC(1 - CP)
        End If
        Return r
    End Function

    'Private Function UjEvaluate() As Integer
    '    Dim index As Integer
    '    index = Key And 255 '1111 1111
    '    index = index Xor ((Key And 4278190080) >> 12) '1111 1111 0000 0000 0000 0000 0000 0000

    '    'fehér 4-esek
    '    index = index Xor (Key And 256) '1 0000 0000
    '    index = index Xor ((Key And 1024) >> 1) '100 0000 0000
    '    index = index Xor ((Key And 4096) >> 2) '1 0000 0000 0000
    '    index = index Xor ((Key And 16384) >> 3) '100 0000 0000 0000

    '    'fekete 4-esek
    '    index = index Xor ((Key And 4294967296) >> (33 - 21)) '1 0000 0000 0000 0000 0000 0000 0000 0000
    '    index = index Xor ((Key And 17179869184) >> (35 - 22)) '100 0000 0000 0000 0000 0000 0000 0000 0000
    '    index = index Xor ((Key And 68719476736) >> (37 - 23)) '1 0000 0000 0000 0000 0000 0000 0000 0000 0000
    '    index = index Xor ((Key And 274877906944) >> (39 - 24)) '100 0000 0000 0000 0000 0000 0000 0000 0000 0000

    '    Dim eval1 = Main.UjEval(index)

    '    'kivesszük a külső négyzetet
    '    index = index Xor (Key And 255) '1111 1111
    '    index = index Xor ((Key And 4278190080) >> 12) '1111 1111 0000 0000 0000 0000 0000 0000

    '    'betesszük belsőt
    '    index = index Xor ((Key And 16711680) >> 16) '1111 1111 0000 0000 0000 0000
    '    index = index Xor ((Key And 280375465082880) >> (48 - 20)) '1111 1111 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000

    '    Dim eval2 = Main.UjEval(index)
    '    Dim ret As Integer = (eval1 + eval2) \ 2
    '    If CP = 1 Then ret = 1000000 / CDbl(ret) * 1000000

    '    'Static bb As Boolean
    '    'If Not bb Then
    '    '    bb = True
    '    '    System.IO.File.AppendAllText("teszt.txt", -NegaMax(6, -inf + 1, inf - 1, 7) & " " & ret & " " & 666666 * KC(CP) \ KC(1 - CP) + 333333 * Mob(CP) \ Mob(1 - CP) & vbCrLf)
    '    '    bb = False
    '    'End If

    '    Return ret
    'End Function

    'Dim md1, md2, lastmd1, lastmd2 As Integer
    'Dim rnd As New System.Random()
    'Public Sub NewParams()
    '    lastmd1 = md1
    '    lastmd2 = md2
    '    Const delta = 400000
    '    md1 += rnd.Next(2 * delta) - delta
    '    md2 += rnd.Next(2 * delta) - delta
    '    If md1 < -500000 + 1 Then md1 = -500000 + 1
    '    If md1 > 500000 - 1 Then md1 = 500000 - 1
    '    If md2 < -500000 + 1 Then md2 = -500000 + 1
    '    If md2 > 500000 - 1 Then md2 = 500000 - 1
    '    SetMainText(md1 & " " & md2)
    'End Sub
    'Public Sub RevertParams()
    '    md1 = lastmd1
    '    md2 = lastmd2
    '    SetMainText(md1 & " " & md2)
    'End Sub
    'Public Sub LogParams(ByVal result As Double)
    '    System.IO.File.AppendAllText("params.txt", result & " " & md1 & " " & md2 & vbCrLf)
    'End Sub
    'Public Sub RandomParams()
    '    md1 = rnd.Next(800000) + 100000
    '    md2 = rnd.Next(800000) + 100000
    '    SetMainText(md1 & " " & md2)
    'End Sub
End Class