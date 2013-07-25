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
    ''' Success details.
    ''' </summary>
    ''' <remarks></remarks>
    <XmlRoot(elementName := "success")>
    Public Class ProwlResultSuccess
      ''' <summary>
      ''' Returns Sucess code.
      ''' </summary>
      ''' <remarks>Normally 200</remarks>
      <XmlAttribute(attributeName := "code")>
      Public Code As Int16

      ''' <summary>
      ''' Returns API call quota remaining until reset date.
      ''' </summary>
      ''' <remarks></remarks>
      <XmlAttribute(attributeName := "remaining")>
      Public Remaining As Int32

      ''' <summary>
      ''' Returns the reset date of API calls quota.
      ''' </summary>
      ''' <remarks>Unix time. Seconds since 01.01.1970 UTC.</remarks>
      <XmlAttribute(attributeName := "resetdate")>
      Public ResetDate As Int64

      ''' <summary>
      ''' Constructor.
      ''' </summary>
      ''' <remarks></remarks>
      Public Sub New()
        Me.Code = Nothing
        Me.Remaining = Nothing
        Me.ResetDate = Nothing
      End Sub

      ''' <summary>
      ''' Returns the reset date of API calls quota.
      ''' </summary>
      ''' <value></value>
      ''' <returns></returns>
      ''' <remarks></remarks>
      <XmlIgnore()>
      Private ReadOnly Property ResetDateTime As DateTime
        Get
          Return New DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(Me.ResetDate)
        End Get
      End Property
    End Class
  End Class
End Namespace