' NuGardt Prowl API
' Copyright (C) 2013 NuGardt Software
' http://www.nugardt.com
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
'
Imports System.Xml.Serialization

Namespace Prowl.API
  ''' <summary>
  ''' Prowl API response.
  ''' </summary>
  ''' <remarks></remarks>
  <XmlRoot(elementName := "prowl")>
  Public Class ProwlResult
    ''' <summary>
    ''' Contains error details if applicable.
    ''' </summary>
    ''' <remarks></remarks>
    <XmlElement(elementName := "error")>
    Public [Error] As ProwlResulterror

    ''' <summary>
    ''' Contains success details if applicable.
    ''' </summary>
    ''' <remarks></remarks>
    <XmlElement(elementName := "success")>
    Public Success As ProwlResultSuccess

    ''' <summary>
    ''' Contains Token/API key retrieval details if applicable.
    ''' </summary>
    ''' <remarks></remarks>
    <XmlElement(elementName := "retrieve")>
    Public Retrieve As ProwlResultRetrieve

    ''' <summary>
    ''' Returns <c>True</c> if there was an exception, otherwise <c>False</c>.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlIgnore()>
    Public ReadOnly Property HasException As Boolean
      Get
        Return (Me.[Error] IsNot Nothing)
      End Get
    End Property

    ''' <summary>
    ''' Converts error details into <c>System.Exception</c>.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlIgnore()>
    Public ReadOnly Property ToException As Exception
      Get
        If (Me.[Error] Is Nothing) Then
          Return Nothing
        Else
          Return New Exception(String.Format("{0}: {1}", Me.Error.Code.ToString(), Me.Error.Message))
        End If
      End Get
    End Property
  End Class
End Namespace