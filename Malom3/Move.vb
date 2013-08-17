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


Public MustInherit Class Move 'lépés
    Public MustOverride Function GetMezok() As Integer() 'a lépésben szereplõ mezõket adja vissza

    Protected MezoToString = {"a4", "a7", "d7", "g7", "g4", "g1", "d1", "a1", _
                              "b4", "b6", "d6", "f6", "f4", "f2", "d2", "b2", _
                              "c4", "c5", "d5", "e5", "e4", "e3", "d3", "c3"}
End Class
Public Class SetKorong
    Inherits Move
    Public hov As Integer
    Public Sub New(ByVal m As Integer)
        hov = m
    End Sub
    Public Overrides Function GetMezok() As Integer()
        Return New Integer() {hov}
    End Function
    Public Overrides Function ToString() As String
        Return MezoToString(hov)
    End Function
End Class
Public Class MoveKorong
    Inherits Move
    Public hon, hov As Integer 'from, to
    Public Sub New(ByVal m1 As Integer, ByVal m2 As Integer)
        hon = m1
        hov = m2
    End Sub
    Public Overrides Function GetMezok() As Integer()
        Return New Integer() {hon, hov}
    End Function
    Public Overrides Function ToString() As String
        Return MezoToString(hon) & "-" & MezoToString(hov)
    End Function
End Class
Public Class LeveszKorong
    Inherits Move
    Public hon As Integer
    Public Sub New(ByVal m As Integer)
        hon = m
    End Sub
    Public Overrides Function GetMezok() As Integer()
        Return New Integer() {hon}
    End Function
    Public Overrides Function ToString() As String
        Return "x" & MezoToString(hon)
    End Function
End Class