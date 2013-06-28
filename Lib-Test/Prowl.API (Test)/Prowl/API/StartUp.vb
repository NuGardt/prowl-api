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
Imports System.Threading

Namespace Prowl.API
  Module StartUp
    Const ApiKey = "<< Your API Key >>"
    Const ProviderKey = "<< Your provider key >>"

    Const SyncTest As Boolean = True
    Const AsyncTest As Boolean = True

    Const AddNotificationTest As Boolean = True
    Const VerifyTest As Boolean = True
    Const RetrieveTokenTest As Boolean = True
    Const RetrieveTokenApiKey As Boolean = True

    Private AsyncCallsBusy As Int64

    Sub Main()
      Dim Result As ProwlResult = Nothing
      Dim AsyncResult As IAsyncResult = Nothing
      Dim Ex As Exception = Nothing

      'Add - Sync
      If SyncTest AndAlso AddNotificationTest Then
        Ex = ProwlService.AddNotification(ApiKey := ApiKey, Application := "Test APU: " + DateTime.Now.ToString(), Event := "Prowl API Test", Description := "Some description", Result := Result, ProviderKey := Nothing, Priority := eProwlPriority.Emergency, URL := "http://www.nugardt.com")
        If (Ex IsNot Nothing) Then Call Trace.WriteLine(Ex.ToString())
      End If

      'Add - ASync
      If AsyncTest AndAlso AddNotificationTest Then
        Call Interlocked.Increment(AsyncCallsBusy)
        AsyncResult = ProwlService.AddNotificationBegin(Key := Nothing, ApiKey := ApiKey, Application := "Test APU: " + DateTime.Now.ToString(), Event := "Prowl API Test", Description := "Some description", Callback := AddNotificationCallback, ProviderKey := Nothing, Priority := eProwlPriority.Emergency, URL := "http://www.nugardt.com")
        If (Ex IsNot Nothing) Then Call Trace.WriteLine(Ex.ToString())
      End If

      'Verify - Sync
      If SyncTest And VerifyTest Then
        Ex = ProwlService.Verify(ApiKey := ApiKey, Result := Result)
        If (Ex IsNot Nothing) Then Call Trace.WriteLine(Ex.ToString())
      End If

      'Verify - Async
      If AsyncTest AndAlso VerifyTest Then
        Call Interlocked.Increment(AsyncCallsBusy)
        AsyncResult = ProwlService.VerifyBegin(Key := Nothing, ApiKey := ApiKey, Callback := VerifyCallback)
        If (Ex IsNot Nothing) Then Call Trace.WriteLine(Ex.ToString())
      End If

      'Retrieve/Token - Sync
      If SyncTest AndAlso RetrieveTokenTest Then
        Ex = ProwlService.RetrieveToken(ProviderKey := ProviderKey, Result := Result)
        If (Ex IsNot Nothing) Then Call Trace.WriteLine(Ex.ToString())
      End If

      'Retrieve/Token - ASync
      If AsyncTest AndAlso RetrieveTokenTest Then
        Call Interlocked.Increment(AsyncCallsBusy)
        AsyncResult = ProwlService.RetrieveTokenBegin(Key := Nothing, ProviderKey := ProviderKey, Callback := RetrieveTokenCallback)
        If (Ex IsNot Nothing) Then Call Trace.WriteLine(Ex.ToString())
      End If

      'Retrieve/ApiKey - Sync
      If SyncTest AndAlso RetrieveTokenApiKey Then
        Ex = ProwlService.RetrieveApiKey(ProviderKey := ProviderKey, Token := Result.Retrieve.Token, Result := Result)
        If (Ex IsNot Nothing) Then Call Trace.WriteLine(Ex.ToString())
      End If

      Trace.WriteLine("Waiting on callbacks to complete...")
      Dim CallsBusy As Int64 = - 1

      Do Until CallsBusy = 0
        CallsBusy = Interlocked.Read(AsyncCallsBusy)

        Call Thread.Sleep(50)
      Loop
    End Sub

#Region "Callbacks"
    Private ReadOnly AddNotificationCallback As AsyncCallback = AddressOf iAddNotificationCallback

    Private Sub iAddNotificationCallback(ByVal Result As IAsyncResult)
      Dim Response As ProwlResult = Nothing
      Dim Ex As Exception

      Ex = ProwlService.AddNotificationEnd(Result, Nothing, Response)

      If (Ex IsNot Nothing) Then Call Trace.WriteLine(Ex.ToString())

      Call Interlocked.Decrement(AsyncCallsBusy)
    End Sub

    Private ReadOnly VerifyCallback As AsyncCallback = AddressOf iVerifyCallback

    Private Sub iVerifyCallback(ByVal Result As IAsyncResult)
      Dim Response As ProwlResult = Nothing
      Dim Ex As Exception

      Ex = ProwlService.VerifyEnd(Result, Nothing, Response)

      If (Ex IsNot Nothing) Then Call Trace.WriteLine(Ex.ToString())

      Call Interlocked.Decrement(AsyncCallsBusy)
    End Sub

    Private ReadOnly RetrieveTokenCallback As AsyncCallback = AddressOf iRetrieveTokenCallback

    Private Sub iRetrieveTokenCallback(ByVal Result As IAsyncResult)
      Dim Response As ProwlResult = Nothing
      Dim Ex As Exception

      Ex = ProwlService.RetrieveTokenEnd(Result, Nothing, Response)

      If (Ex IsNot Nothing) Then Call Trace.WriteLine(Ex.ToString())

      Call Interlocked.Decrement(AsyncCallsBusy)
    End Sub

#End Region
  End Module
End Namespace