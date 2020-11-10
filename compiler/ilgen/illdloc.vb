﻿Public Class illdloc
    Public _ilmethod As ilformat._ilmethodcollection
    Public Sub New(_ilmethod As ilformat._ilmethodcollection)
        Me._ilmethod = _ilmethod
    End Sub

    Private Function check_integer_type(datatype As String) As String
        For index = 0 To compdt.cilintegertypes.Length - 1
            If compdt.cilintegertypes(index) = datatype Then
                Return datatype
            End If
        Next
        Return Nothing
    End Function

    Public Function load_in_stack(paramtypes As ArrayList, cargcodestruc As xmlunpkd.linecodestruc()) As ilformat._ilmethodcollection
        For index = 0 To cargcodestruc.Length - 1
            Select Case paramtypes(index).ToString
                Case "string"
                    ldstr(cargcodestruc(index))
                Case check_integer_type(paramtypes(index).ToString)
                    ldint(cargcodestruc(index), paramtypes(index).ToString)
                Case "float32"
                    ldflt(cargcodestruc(index), paramtypes(index).ToString)
                Case "float64"
                    ldflt(cargcodestruc(index), paramtypes(index).ToString)
                Case "char"
                    ldchr(cargcodestruc(index), paramtypes(index).ToString)
                Case "bool"
                    ldbool(cargcodestruc(index), paramtypes(index).ToString)
                Case Else
                    'Other Types
            End Select
        Next

        Return _ilmethod
    End Function

    Private Sub ldstr(cargcodestruc As xmlunpkd.linecodestruc)
        Select Case cargcodestruc.tokenid
            Case tokenhared.token.TYPE_DU_STR
                cil.load_string(_ilmethod.codes, cargcodestruc.value)
            Case tokenhared.token.TYPE_CO_STR
                cil.load_string(_ilmethod.codes, cargcodestruc.value)
            Case tokenhared.token.IDENTIFIER
                If assignmentcommondatatype.check_locals_init(_ilmethod.name, cargcodestruc.value, _ilmethod.locallinit, "string") Then
                    cil.load_local_variable(_ilmethod.codes, cargcodestruc.value)
                End If
            Case tokenhared.token.NULL
                cil.push_null_reference(_ilmethod.codes)
            Case Else
                'Set Error 
                dserr.args.Add(cargcodestruc.value)
                dserr.args.Add("string")
                dserr.new_error(conserr.errortype.ASSIGNCONVERT, cargcodestruc.line, ilbodybulider.path, authfunc.get_line_error(ilbodybulider.path, servinterface.get_target_info(cargcodestruc), cargcodestruc.value))
        End Select
    End Sub

    Private Sub ldint(cargcodestruc As xmlunpkd.linecodestruc, datatype As String)
        Dim convtoi8 As Boolean = servinterface.is_i8(datatype)
        servinterface.clinecodestruc = cargcodestruc
        Select Case cargcodestruc.tokenid
            Case tokenhared.token.TYPE_INT
                servinterface.ldc_i_checker(_ilmethod.codes, cargcodestruc.value, convtoi8, datatype)
            Case tokenhared.token.TYPE_FLOAT
                servinterface.ldc_i_checker(_ilmethod.codes, cargcodestruc.value, convtoi8, datatype)
            Case tokenhared.token.IDENTIFIER
                If assignmentcommondatatype.check_locals_init(_ilmethod.name, cargcodestruc.value, _ilmethod.locallinit, datatype) Then
                    cil.load_local_variable(_ilmethod.codes, cargcodestruc.value)
                End If
            'let value : str = NULL
            Case tokenhared.token.NULL
                cil.push_null_reference(_ilmethod.codes)
            Case tokenhared.token.EXPRESSION
                Dim expr As New expressiondt(_ilmethod, "i32")
                Try
                    _ilmethod = expr.parse_expression_data(cargcodestruc.value, convtoi8)
                Catch ex As Exception
                    dserr.args.Add(ex.Message)
                    dserr.new_error(conserr.errortype.EXPRESSIONERROR, cargcodestruc.line, ilbodybulider.path, authfunc.get_line_error(ilbodybulider.path, servinterface.get_target_info(cargcodestruc), cargcodestruc.value))
                End Try
            Case Else
                'Set Error 
                dserr.args.Add(cargcodestruc.value)
                dserr.args.Add(datatype)
                dserr.new_error(conserr.errortype.ASSIGNCONVERT, cargcodestruc.line, ilbodybulider.path, authfunc.get_line_error(ilbodybulider.path, servinterface.get_target_info(cargcodestruc), cargcodestruc.value))
        End Select
    End Sub

    Private Sub ldflt(cargcodestruc As xmlunpkd.linecodestruc, datatype As String)
        Dim convtor8 As Boolean = False
        If datatype = "float64" Then convtor8 = True
        Select Case cargcodestruc.tokenid
            Case tokenhared.token.TYPE_INT
                servinterface.ldc_r_checker(_ilmethod.codes, cargcodestruc.value, convtor8)
            Case tokenhared.token.TYPE_FLOAT
                servinterface.ldc_r_checker(_ilmethod.codes, cargcodestruc.value, convtor8)
            Case tokenhared.token.IDENTIFIER
                If assignmentcommondatatype.check_locals_init(_ilmethod.name, cargcodestruc.value, _ilmethod.locallinit, datatype) Then
                    cil.load_local_variable(_ilmethod.codes, cargcodestruc.value)
                End If
            'let value : str = NULL
            Case tokenhared.token.NULL
                cil.push_null_reference(_ilmethod.codes)
            Case tokenhared.token.EXPRESSION
                Dim expr As New expressiondt(_ilmethod, "f32")
                Try
                    _ilmethod = expr.parse_expression_data(cargcodestruc.value, convtor8)
                Catch ex As Exception
                    dserr.args.Add(ex.Message)
                    dserr.new_error(conserr.errortype.EXPRESSIONERROR, cargcodestruc.line, ilbodybulider.path, authfunc.get_line_error(ilbodybulider.path, servinterface.get_target_info(cargcodestruc), cargcodestruc.value))
                End Try
            Case Else
                'Set Error 
                dserr.args.Add(cargcodestruc.value)
                dserr.args.Add(datatype)
                dserr.new_error(conserr.errortype.ASSIGNCONVERT, cargcodestruc.line, ilbodybulider.path, authfunc.get_line_error(ilbodybulider.path, servinterface.get_target_info(cargcodestruc), cargcodestruc.value))
        End Select
    End Sub

    Private Sub ldchr(cargcodestruc As xmlunpkd.linecodestruc, datatype As String)
        Dim convtoi8 As Boolean = servinterface.is_i8(datatype)
        servinterface.clinecodestruc = cargcodestruc
        Select Case cargcodestruc.tokenid
            Case tokenhared.token.TYPE_DU_STR
                If cargcodestruc.value.Length >= 3 Then
                    cil.push_int32_onto_stack(_ilmethod.codes, AscW(cargcodestruc.value(1)))
                Else
                    'example : value := ""
                    cil.push_null_reference(_ilmethod.codes)
                End If
            Case tokenhared.token.TYPE_CO_STR
                If cargcodestruc.value.Length >= 3 Then
                    cil.push_int32_onto_stack(_ilmethod.codes, AscW(cargcodestruc.value(1)))
                Else
                    'example : value := ''
                    cil.push_null_reference(_ilmethod.codes)
                End If
            Case tokenhared.token.IDENTIFIER
                If assignmentcommondatatype.check_locals_init(_ilmethod.name, cargcodestruc.value, _ilmethod.locallinit, datatype) Then
                    cil.load_local_variable(_ilmethod.codes, cargcodestruc.value)
                End If
                'let value : str = NULL
            Case tokenhared.token.NULL
                cil.push_null_reference(_ilmethod.codes)
            Case Else
                'Set Error 
                dserr.args.Add(cargcodestruc.value)
                dserr.args.Add(datatype)
                dserr.new_error(conserr.errortype.ASSIGNCONVERT, cargcodestruc.line, ilbodybulider.path, authfunc.get_line_error(ilbodybulider.path, servinterface.get_target_info(cargcodestruc), cargcodestruc.value))
        End Select
    End Sub

    Private Sub ldbool(cargcodestruc As xmlunpkd.linecodestruc, datatype As String)
        Dim convtoi8 As Boolean = servinterface.is_i8(datatype)
        servinterface.clinecodestruc = cargcodestruc
        Select Case cargcodestruc.tokenid
            Case tokenhared.token.TRUE
                cil.push_int32_onto_stack(_ilmethod.codes, 1)
            Case tokenhared.token.FALSE
                cil.push_int32_onto_stack(_ilmethod.codes, 0)
            Case tokenhared.token.IDENTIFIER
                If assignmentcommondatatype.check_locals_init(_ilmethod.name, cargcodestruc.value, _ilmethod.locallinit, datatype) Then
                    cil.load_local_variable(_ilmethod.codes, cargcodestruc.value)
                End If
            Case tokenhared.token.TYPE_INT
                servinterface.ldc_i_checker(_ilmethod.codes, cargcodestruc.value, convtoi8, datatype)
            'let value : str = NULL
            Case tokenhared.token.NULL
                cil.push_null_reference(_ilmethod.codes)
            Case Else
                'Set Error 
                dserr.args.Add(cargcodestruc.value)
                dserr.args.Add(datatype)
                dserr.new_error(conserr.errortype.ASSIGNCONVERT, cargcodestruc.line, ilbodybulider.path, authfunc.get_line_error(ilbodybulider.path, servinterface.get_target_info(cargcodestruc), cargcodestruc.value))
        End Select
    End Sub
End Class
