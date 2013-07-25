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
  Partial Class ProwlResult
    ''' <summary>
    ''' Token/API key retrieve details
    ''' </summary>
    ''' <remarks></remarks>
    <XmlRoot(elementName := "success")>
    Public Class ProwlResultRetrieve
      ''' <summary>
      ''' Token
      ''' </summary>
      ''' <remarks></remarks>
      <XmlAttribute(attributeName := "token")>
      Public Token As String

      ''' <summary>
      ''' URL to authorize Token
      ''' </summary>
      ''' <remarks></remarks>
      <XmlAttribute(attributeName := "url")>
      Public Url As String

      ''' <summary>
      ''' API key.
      ''' </summary>
      ''' <remarks></remarks>
      <XmlAttribute(attributeName := "apikey")>
      Public ApiKey As String

      ''' <summary>
      ''' Constructor
      ''' </summary>
      ''' <remarks></remarks>
      Public Sub New()
        Me.Token = Nothing
        Me.Url = Nothing
        Me.ApiKey = Nothing
      End Sub
    End Class
  End Class
End Namespace