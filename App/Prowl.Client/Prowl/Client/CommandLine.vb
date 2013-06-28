' NuGardt Prowl Client
' Copyright (C) 2011-2013 NuGardt Software
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
Namespace Prowl.Client
  ''' <summary>
  '''   This class simplifies command line analysis.
  ''' </summary>
  ''' <remarks></remarks>
    Public NotInheritable Class CommandLine
    Public Shared ReadOnly SwitchChar() As Char = New Char() {"/"c}
    Public Shared ReadOnly SwitchCharUnix() As String = New String() {"--"}
    Public ReadOnly DefinitionChar As Char = "="c

    ''' <summary>
    '''   Return a list of command line switches and their Values, or sets this.
    ''' </summary>
    ''' <remarks></remarks>
    Private ReadOnly DictSwitches As IDictionary(Of String, String)

    ''' <summary>
    '''   Return the command line.
    ''' </summary>
    ''' <remarks></remarks>
    Private ReadOnly m_CommandLine As String

    'Command Line Samples:
    '---------------------
    '/switch
    '--switch
    '/switch=Value1,Value2
    '--switch=Value1,Value2

    ''' <summary>
    '''   Construct.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(Optional ByVal UnixStyle As Boolean = False)
      Me.m_CommandLine = Command$()
      Me.DictSwitches = New Dictionary(Of String, String)(StringComparer.CurrentCultureIgnoreCase)
      Dim SwitchSplitEnumerator As IEnumerator

      If UnixStyle Then
        SwitchSplitEnumerator = Me.m_CommandLine.Split(SwitchCharUnix, StringSplitOptions.RemoveEmptyEntries).GetEnumerator()
      Else
        SwitchSplitEnumerator = Me.m_CommandLine.Split(SwitchChar, StringSplitOptions.RemoveEmptyEntries).GetEnumerator()
      End If

      With SwitchSplitEnumerator
        Call .Reset()

        Do While .MoveNext
          Dim Item As String = .Current.ToString
          Dim Key As String
          Dim Value As String
          Dim EqualIndex As Integer = Item.IndexOf(DefinitionChar)

          If EqualIndex = - 1 Then
            Key = Item.Trim
            Value = Nothing
          Else
            Key = Item.Substring(0, EqualIndex).Trim
            Value = Item.Substring(EqualIndex + 1).Trim
          End If

          If (Not Me.DictSwitches.ContainsKey(Key)) Then
            If (EqualIndex <> - 1) Then
              '/switch=Value1,Value2
              '--switch=Value1,Value2
              Call Me.DictSwitches.Add(Key, Value)
            Else
              '/switch
              '--switch
              Call Me.DictSwitches.Add(Key, Nothing)
            End If
          End If
        Loop
      End With
    End Sub

    ''' <summary>
    '''   Returns <c>True</c> when a switch is set, otherwiese <c>False</c>.
    ''' </summary>
    ''' <param name="Switch">The name of the switch.</param>
    ''' <returns>
    '''   Returns <c>True</c> when a switch is set, otherwiese <c>False</c>.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function IsSet(ByVal Switch As String) As Boolean
      Return Me.DictSwitches.ContainsKey(Switch)
    End Function

    ''' <summary>
    '''   Returns the Values belonging to a switch, if the switch has any.
    ''' </summary>
    ''' <param name="Switch">The name of the switch.</param>
    ''' <param name="Values">
    '''   Contains the Values of the switch, if it has any, otherwise <c>Nothing</c>.
    ''' </param>
    ''' <returns>
    '''   Returns <c>True</c> when a Values for a switch are available, otherwiese <c>False</c>.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetValues(ByVal Switch As String,
                              ByRef Values As String) As Boolean
      Values = Nothing
      Dim Erg As Boolean

      Erg = Me.DictSwitches.TryGetValue(Switch, Values)

      Return Erg AndAlso (Values IsNot Nothing)
    End Function

    Public Overrides Function ToString() As String
      Return Me.m_CommandLine
    End Function
  End Class
End Namespace