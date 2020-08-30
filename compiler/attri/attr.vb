﻿Imports System.Text.RegularExpressions

Public Class attr
    Friend Shared lastexperssion As String = String.Empty
    Friend Shared lastlinecinf As lexer.targetinf
    Private attribute As yocaattribute.yoattribute
    Private path As String
    Public Sub New(path As String)
        Me.path = path
        attribute = New yocaattribute.yoattribute
        setinattr.init(attribute)
    End Sub

    Public Function parse_attribute(experssion As String, linecinf As lexer.targetinf) As yocaattribute.yoattribute
        lastexperssion = experssion
        lastlinecinf = linecinf
        If Regex.IsMatch(experssion, "\#\[\w+::\w+\(.+\)\]") Then
            Dim resultattr As yocaattribute.resultattribute = get_properties(experssion)
            set_attribute(resultattr)
        Else
            'Set error
            dserr.new_error(conserr.errortype.ATTRIBUTESTRUCTERROR, linecinf.line, path, authfunc.get_line_error(path, linecinf, lastexperssion), "#[cfg::CIL(true)]")
        End If
    End Function

    Private Function get_properties(experssion As String) As yocaattribute.resultattribute
        Dim resultattr As New yocaattribute.resultattribute
        experssion = experssion.Remove(0, 2)
        Dim value As String = experssion.Remove(experssion.IndexOf("::"))
        resultattr.typeattribute = value
        experssion = experssion.Remove(0, experssion.IndexOf("::") + 2)

        value = experssion.Remove(experssion.IndexOf("("))
        resultattr.fieldattribute = value
        experssion = experssion.Remove(0, experssion.IndexOf("(") + 1)

        value = experssion.Remove(experssion.IndexOf(")"))
        resultattr.valueattribute = value

        Return resultattr
    End Function

    Public Sub set_attribute(resultattr As yocaattribute.resultattribute)
        Select Case resultattr.typeattribute.ToLower
            Case "cfg"
                set_cfg_attribute(resultattr)
            Case Else
                'Set Error
                dserr.args.Add(resultattr.typeattribute)
                dserr.new_error(conserr.errortype.ATTRIBUTECLASSERROR, lastlinecinf.line, path, authfunc.get_line_error(path, lastlinecinf, resultattr.typeattribute))
        End Select
    End Sub

    Private Sub set_cfg_attribute(resultattr As yocaattribute.resultattribute)
        Select Case resultattr.fieldattribute.ToLower
            Case "cil"
                attribute._cfg._cilinject = setinattr.get_bool_val(resultattr, path)
            Case Else
                'Set Error
                dserr.args.Add(resultattr.fieldattribute)
                dserr.new_error(conserr.errortype.ATTRIBUTEPROPERTYERROR, lastlinecinf.line, path, authfunc.get_line_error(path, lastlinecinf, resultattr.fieldattribute))
        End Select
    End Sub

    Public Function get_attribute() As yocaattribute.yoattribute
        Return attribute
    End Function
End Class
