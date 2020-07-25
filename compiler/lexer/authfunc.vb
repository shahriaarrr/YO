﻿Imports System.IO

Public Class authfunc
    Public Shared Function get_line_code(path As String, line As Integer) As String
        If File.Exists(path) Then
            Dim fsource As String = File.ReadAllText(path)
            Dim splsource() As String = fsource.Split(Chr(10)).ToArray
            If line <= splsource.Length - 1 Then
                Return splsource(line)
            Else
                'Error...
            End If
        Else
            dserr.new_error(conserr.errortype.YOFILENOTFOUND, "path => " & path)
            Return conrex.NULL
        End If
        Return conrex.NULL
    End Function

    Public Shared Function get_line_error(path As String, linecinf As lexer.targetinf, errtoken As String)
        Dim code As String = get_line_code(path, linecinf.line + 1) & vbCrLf
        Dim startindexof As Integer = code.IndexOf(errtoken)
        Dim excreviewer As String = Space(startindexof)
        For index = 0 To errtoken.Length - 1
            excreviewer &= conrex.CURSORERR
        Next
        Return code & vbCr & excreviewer
    End Function
End Class
