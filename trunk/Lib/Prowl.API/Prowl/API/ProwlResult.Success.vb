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
    <XmlRoot(elementName := "success")>
    Public Class ProwlResultSuccess
      <XmlAttribute(attributeName := "code")>
      Public Code As Int16

      <XmlAttribute(attributeName := "remaining")>
      Public Remaining As Int32

      <XmlAttribute(attributeName := "resetdate")>
      Public ResetDate As Int64

      Public Sub New()
        Me.Code = Nothing
        Me.Remaining = Nothing
        Me.ResetDate = Nothing
      End Sub

      <XmlIgnore()>
      Private ReadOnly Property ResetDateTime As DateTime
        Get
          Return New DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(Me.ResetDate)
        End Get
      End Property
    End Class
  End Class
End Namespace