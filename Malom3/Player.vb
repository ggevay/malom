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


Public MustInherit Class Player
    Protected G As Game
    Public Overridable Sub Enter(ByVal _g As Game) 'a paraméterben megadott játszmába való belépésrõl értesíti az objektumot
        G = _g
    End Sub
    Public Overridable Sub Quit() 'a játszmából való kilépésrõl értesíti az objektumot
        If G Is Nothing Then Return 'ez azt jelzi, hogy nem biztos, hogy ez az fv meghivodik leszarmazott osztalybol, ha mar ki vagyunk lepve
        G = Nothing
    End Sub
    Public MustOverride Sub ToMove(ByVal s As GameState) 'arról értesíti az objektumot, hogy õ következik lépni
    Public Overridable Sub FollowMove(ByVal M As Object) 'az ellenfél lépésérõl értesít

    End Sub
    Public Overridable Sub OppToMove(ByVal s As GameState) 'arról értesíti az objektumot, hogy az ellenfél következik lépni (kell pl. a gépi játékos ellenfél idejében gondolkodásához)

    End Sub
    Public Overridable Sub Over(ByVal s As GameState)

    End Sub
    Public Overridable Sub CancelThinking()

    End Sub
    Protected Function Opponent() As Player
        Return If(Object.ReferenceEquals(G.Ply(0), Me), G.Ply(1), G.Ply(0))
    End Function
End Class

Class HumanPlayer
    Inherits Player
    Public Overrides Sub ToMove(ByVal s As GameState)
        G.frm._Board.Enabled = True
    End Sub
    Public Overrides Sub FollowMove(ByVal M As Object)
        G.frm._Board.JelolMezo(M.GetMezok())
    End Sub
    Public Overrides Sub Quit()
        If G Is Nothing Then Return
        G.frm._Board.ClearMezoSelection()
        MyBase.Quit()
    End Sub
    Public Overrides Sub Over(ByVal s As GameState)
        'Return
        If TypeOf Opponent() Is HumanPlayer AndAlso Not Object.ReferenceEquals(Me, G.Ply(1)) Then Exit Sub 'ha az ellenfél ember, és nem mi nyertük meg a játszmát, akkor ne csináljunk semmit (majd õ kiírja, amit kell)
        Dim result As MsgBoxResult
        If s.winner > -1 Then
            result = MsgBox("The " & s.winner + 1 & ". player won." & If(s.block, " (because the other player can't move)", "") & " New game?", MsgBoxStyle.YesNo + MsgBoxStyle.Question, "Game over")
        Else
            result = MsgBox("The game ended in a draw. New game?", MsgBoxStyle.YesNo + MsgBoxStyle.Question, "Game over")
        End If
        If result = MsgBoxResult.Yes Then
            'G.frm.NewGame()
            G.frm.BeginInvoke(New Action(AddressOf G.frm.NewGame))
        End If
    End Sub
End Class