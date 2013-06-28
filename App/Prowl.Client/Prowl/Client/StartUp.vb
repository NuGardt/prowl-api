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
Imports System.IO
Imports NuGardt.Prowl.API
Imports System.Reflection

Namespace Prowl.Client
  Module StartUp
    Sub Main()
      Dim cl As New CommandLine()
      Dim Ex As Exception
      Dim Result As ProwlResult = Nothing

      Dim tApiKey As String = Nothing
      Dim tApplication As String = Nothing
      Dim tEvent As String = Nothing
      Dim tDescription As String = Nothing
      Dim tProviderKey As String = Nothing
      Dim tPriority As String = Nothing
      Dim ttPriority As Int16
      Dim tUrl As String = Nothing

      If cl.IsSet("help") Then
        Call Console.WriteLine("NuGardt Prowl Client v" + My.Application.Info.Version.ToString())
        Call Console.WriteLine(My.Application.Info.Copyright)
        Call Console.WriteLine("")
        Call Console.WriteLine("Add a notification for a particular user. You must provide either event or description or both.")
        Call Console.WriteLine("")
        Call Console.WriteLine("Usage: " + Path.GetFileName(Assembly.GetEntryAssembly().Location) + " /apikey=123.. /event=Event /description=""Something""")
        Call Console.WriteLine("")
        Call Console.WriteLine("  /apikey=... [Required]")
        Call Console.WriteLine("    API keys separated by commas. Each API key is a 40-byte hexadecimal string. When using multiple API keys, you will only get a failure response if all API keys are not valid.")
        Call Console.WriteLine("  /application=... [Optional/256]")
        Call Console.WriteLine("    The name of your application or the application generating the event.")
        Call Console.WriteLine("  /event=...[Required/1024]")
        Call Console.WriteLine("    The name of the event or subject of the notification.")
        Call Console.WriteLine("  /description=...[Required/10000]")
        Call Console.WriteLine("    A description of the event, generally terse.")
        Call Console.WriteLine("  /providerkey=...[Optional/40]")
        Call Console.WriteLine("    Your provider API key. Only necessary if you have been whitelisted.")
        Call Console.WriteLine("  /priority=... [-2=Very Low, -1=Low, 0=Normal, 1=High, 2=Emergency]")
        Call Console.WriteLine("    efault value of 0 if not provided. Emergency priority messages may bypass quiet hours according to the user's settings. ")
        Call Console.WriteLine("  /url=... [Optional/512]")
        Call Console.WriteLine("    This will trigger a redirect when launched, and is viewable in the notification list.")

        Call Environment.Exit(0)
      Else
        Call cl.GetValues("apikey", tApiKey)
        Call cl.GetValues("application", tApplication)
        Call cl.GetValues("event", tEvent)
        Call cl.GetValues("description", tDescription)
        Call cl.GetValues("providerkey", tProviderKey)
        Call cl.GetValues("priority", tPriority)
        If (Not Int16.TryParse(tPriority, ttPriority)) Then ttPriority = eProwlPriority.Normal
        Call cl.GetValues("url", tUrl)

        Ex = ProwlService.AddNotification(ApiKey := tApiKey, Application := tApplication, [Event] := tEvent, Description := tDescription, Result := Result, ProviderKey := tProviderKey, Priority := CType(ttPriority, eProwlPriority), URL := tUrl)

        If (Ex IsNot Nothing) Then
          If (Result IsNot Nothing) Then
            If Result.HasException Then
              Call Console.WriteLine(Result.ToException.ToString())
              Call Environment.Exit(Result.Error.Code)
            End If
          Else
            Call Console.WriteLine(Ex.ToString())
            Call Environment.Exit(- 1)
          End If
        Else
          Call Environment.Exit(0)
        End If
      End If
    End Sub
  End Module
End Namespace