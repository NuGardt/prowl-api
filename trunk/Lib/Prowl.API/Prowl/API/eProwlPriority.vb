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
Namespace Prowl.API
  ''' <summary>
  ''' Notification priorities
  ''' </summary>
  ''' <remarks></remarks>
    Public Enum eProwlPriority As Int16
    ''' <summary>
    ''' Very Low
    ''' </summary>
    ''' <remarks></remarks>
      VeryLow = - 2

    ''' <summary>
    ''' Moderate
    ''' </summary>
    ''' <remarks></remarks>
      Moderate = - 1

    ''' <summary>
    ''' Normal
    ''' </summary>
    ''' <remarks></remarks>
      Normal = 0

    ''' <summary>
    ''' High
    ''' </summary>
    ''' <remarks></remarks>
      High = 1

    ''' <summary>
    ''' Emergency
    ''' </summary>
    ''' <remarks>Emergency priority messages may bypass quiet hours according to the user's settings.</remarks>
      Emergency = 2
  End Enum
End Namespace