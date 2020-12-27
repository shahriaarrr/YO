﻿Imports System.Text.RegularExpressions

Public Class attr
    Friend Shared lastexpression As String = String.Empty
    Friend Shared lastlinecinf As lexer.targetinf
    Private attribute As yocaattribute.yoattribute
    Private path As String
    Public Sub New(path As String)
        Me.path = path
        attribute = New yocaattribute.yoattribute
        setinattr.init(attribute)
    End Sub

    Public Sub parse_attribute(expression As String, linecinf As lexer.targetinf)
        lastexpression = expression
        lastlinecinf = linecinf
        If Regex.IsMatch(expression, "\#\[\w+::\w+\(.+\)\]") Then
            Dim resultattr As yocaattribute.resultattribute = get_properties(expression)
            set_attribute(resultattr)
        Else
            'Set error
            dserr.new_error(conserr.errortype.ATTRIBUTESTRUCTERROR, linecinf.line, path, authfunc.get_line_error(path, linecinf, lastexpression), "#[cfg::CIL(true)]")
        End If
    End Sub

    Private Function get_properties(expression As String) As yocaattribute.resultattribute
        Dim resultattr As New yocaattribute.resultattribute
        expression = expression.Remove(0, 2)
        Dim value As String = expression.Remove(expression.IndexOf("::"))
        resultattr.typeattribute = value
        expression = expression.Remove(0, expression.IndexOf("::") + 2)

        value = expression.Remove(expression.IndexOf("("))
        resultattr.fieldattribute = value
        expression = expression.Remove(0, expression.IndexOf("(") + 1)

        value = expression.Remove(expression.IndexOf(")"))
        resultattr.valueattribute = value

        Return resultattr
    End Function

    Public Sub set_attribute(resultattr As yocaattribute.resultattribute)
        Select Case resultattr.typeattribute.ToLower
            Case "cfg"
                set_cfg_attribute(resultattr)
            Case "app"
                set_app_attribute(resultattr)
            Case Else
                'Set Error
                dserr.args.Add(resultattr.typeattribute)
                dserr.new_error(conserr.errortype.ATTRIBUTECLASSERROR, lastlinecinf.line, path, authfunc.get_line_error(path, lastlinecinf, resultattr.typeattribute))
        End Select
    End Sub

    Private Sub set_app_attribute(resultattr As yocaattribute.resultattribute)
        Select Case resultattr.fieldattribute.ToLower
            Case "namespace"
                attribute._app._namespace = setinattr.get_str_val(resultattr, path)
            Case "classname"
                attribute._app._classname = setinattr.get_str_val(resultattr, path)
            Case "wait"
                attribute._app._wait = setinattr.get_bool_val(resultattr, path)
            Case "issealed"
                attribute._app._issealed = setinattr.get_bool_val(resultattr, path)
            Case Else
                'Set Error
                dserr.args.Add(resultattr.fieldattribute)
                dserr.new_error(conserr.errortype.ATTRIBUTEPROPERTYERROR, lastlinecinf.line, path, authfunc.get_line_error(path, lastlinecinf, resultattr.fieldattribute))
        End Select
    End Sub

    Private Sub set_cfg_attribute(resultattr As yocaattribute.resultattribute)
        Select Case resultattr.fieldattribute.ToLower
            Case "cil"
                attribute._cfg._cilinject = setinattr.get_bool_val(resultattr, path)
            Case "optimize_expression"
                attribute._cfg._optimize_expression = setinattr.get_bool_val(resultattr, path)
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
