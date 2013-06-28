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
Imports System.Runtime.InteropServices
Imports System.Net
Imports System.Threading
Imports System.Text
Imports System.IO

Namespace Prowl.API
  Public Class ProwlService
    Private Const ProwlBaseUrl As String = "https://api.prowlapp.com/publicapi/"

    Private Shared ReadOnly ProwlResultSerializer As XmlSerializer = New XmlSerializer(GetType(ProwlResult))

    Private Sub New()
      '-
    End Sub

#Region "Function AddNotification"

    Public Shared Function AddNotification(ByVal ApiKey As String,
                                           ByVal ApplicationName As String,
                                           ByVal EventName As String,
                                           ByVal Description As String,
                                           <Out()> ByRef Result As ProwlResult,
                                           Optional ByVal ProviderKey As String = Nothing,
                                           Optional ByVal Priority As eProwlPriority = eProwlPriority.Normal,
                                           Optional ByVal URL As String = Nothing) As Exception
      Result = Nothing
      Dim Ex As Exception = Nothing

      If (Not String.IsNullOrEmpty(ProviderKey)) AndAlso (ProviderKey.Length > 40) Then
        Ex = New ArgumentException("ProviderKey is too long (40 max)")
      ElseIf (Not String.IsNullOrEmpty(URL)) AndAlso (URL.Length > 512) Then
        Ex = New ArgumentException("URL is too long (512 max)")
      ElseIf (Not String.IsNullOrEmpty(ApplicationName)) AndAlso (ApplicationName.Length > 512) Then
        Ex = New ArgumentException("ApplicationName is too long (256 max)")
      ElseIf (Not String.IsNullOrEmpty(EventName)) AndAlso (EventName.Length > 1024) Then
        Ex = New ArgumentException("EventName is too long (1024 max)")
      ElseIf (Not String.IsNullOrEmpty(Description)) AndAlso (Description.Length > 10000) Then
        Ex = New ArgumentException("Description is too long (10000 max)")
      ElseIf (String.IsNullOrEmpty(EventName)) AndAlso (String.IsNullOrEmpty(Description)) Then
        Ex = New ArgumentNullException("EventName or Description must be filled.")
      Else
        Dim SB As New StringBuilder

        'URL
        Call SB.Append(ProwlBaseUrl)
        'Call
        Call SB.Append("add")
        'Parameters
        Call SB.AppendFormat("?apikey={0}", ApiKey)
        If (Not String.IsNullOrEmpty(ProviderKey)) Then Call SB.AppendFormat("&providerkey={0}", ProviderKey)
        Dim tPriority As Int16 = Priority
        Call SB.AppendFormat("&priority={0}", tPriority.ToString())
        Call SB.AppendFormat("&url={0}", URL)
        Call SB.AppendFormat("&application={0}", ApplicationName)
        Call SB.AppendFormat("&event={0}", EventName)
        Call SB.AppendFormat("&url={0}", Description)

        Result = QueryAndParse(SB.ToString(), Ex)
      End If

      Return Ex
    End Function

    Public Shared Function AddNotificationBegin(ByVal Key As Object,
                                                ByVal ApiKey As String,
                                                ByVal ApplicationName As String,
                                                ByVal EventName As String,
                                                ByVal Description As String,
                                                ByVal Callback As AsyncCallback,
                                                Optional ByVal ProviderKey As String = Nothing,
                                                Optional ByVal Priority As eProwlPriority = eProwlPriority.Normal,
                                                Optional ByVal URL As String = Nothing) As IAsyncResult
      Dim SB As New StringBuilder

      'URL
      Call SB.Append(ProwlBaseUrl)
      'Call
      Call SB.Append("add")
      'Parameters
      Call SB.AppendFormat("?apikey={0}", ApiKey)
      If (Not String.IsNullOrEmpty(ProviderKey)) Then Call SB.AppendFormat("&providerkey={0}", ProviderKey)
      Dim tPriority As Int16 = Priority
      Call SB.AppendFormat("&priority={0}", tPriority.ToString())
      Call SB.AppendFormat("&url={0}", URL)
      Call SB.AppendFormat("&application={0}", ApplicationName)
      Call SB.AppendFormat("&event={0}", EventName)
      Call SB.AppendFormat("&url={0}", Description)

      Return QueryAndParseBegin(Key, SB.ToString(), Callback)
    End Function

    Public Shared Function AddNotificationEnd(ByVal Result As IAsyncResult,
                                              <Out()> ByRef Key As Object,
                                              <Out()> ByRef Response As ProwlResult) As Exception
      Return QueryAndParseEnd(Result, Key, Response)
    End Function

#End Region

#Region "Function Verify"

    Public Shared Function Verify(ByVal ApiKey As String,
                                  <Out()> ByRef Result As ProwlResult,
                                  Optional ByVal ProviderKey As String = Nothing) As Exception
      Result = Nothing
      Dim Ex As Exception = Nothing

      If (Not String.IsNullOrEmpty(ProviderKey)) AndAlso (ProviderKey.Length > 40) Then
        Ex = New ArgumentException("ProviderKey is too long (40 max)")
      Else
        Dim SB As New StringBuilder

        'URL
        Call SB.Append(ProwlBaseUrl)
        'Call
        Call SB.Append("verify")
        'Parameters
        Call SB.AppendFormat("?apikey={0}", ApiKey)
        If (Not String.IsNullOrEmpty(ProviderKey)) Then Call SB.AppendFormat("&providerkey={0}", ProviderKey)

        Result = QueryAndParse(SB.ToString(), Ex)
      End If

      Return Ex
    End Function

    Public Shared Function VerifyBegin(ByVal Key As Object,
                                       ByVal ApiKey As String,
                                       ByVal Callback As AsyncCallback,
                                       Optional ByVal ProviderKey As String = Nothing) As IAsyncResult
      Dim SB As New StringBuilder

      'URL
      Call SB.Append(ProwlBaseUrl)
      'Call
      Call SB.Append("verify")
      'Parameters
      Call SB.AppendFormat("?apikey={0}", ApiKey)
      If (Not String.IsNullOrEmpty(ProviderKey)) Then Call SB.AppendFormat("&providerkey={0}", ProviderKey)

      Return QueryAndParseBegin(Key, SB.ToString(), Callback)
    End Function

    Public Shared Function VerifyEnd(ByVal Result As IAsyncResult,
                                     <Out()> ByRef Key As Object,
                                     <Out()> ByRef Response As ProwlResult) As Exception
      Return QueryAndParseEnd(Result, Key, Response)
    End Function

#End Region

#Region "Function Retrieve/Token"

    Public Shared Function RetrieveToken(ByVal ProviderKey As String,
                                         <Out()> ByRef Result As ProwlResult) As Exception
      Result = Nothing
      Dim Ex As Exception = Nothing

      If (Not String.IsNullOrEmpty(ProviderKey)) AndAlso (ProviderKey.Length > 40) Then
        Ex = New ArgumentException("ProviderKey is too long (40 max)")
      Else
        Dim SB As New StringBuilder

        'URL
        Call SB.Append(ProwlBaseUrl)
        'Call
        Call SB.Append("retrieve/token")
        'Parameters
        Call SB.AppendFormat("?providerkey={0}", ProviderKey)

        Result = QueryAndParse(SB.ToString(), Ex)
      End If

      Return Ex
    End Function

    Public Shared Function RetrieveTokenBegin(ByVal Key As Object,
                                              ByVal ProviderKey As Object,
                                              ByVal Callback As AsyncCallback) As IAsyncResult
      Dim SB As New StringBuilder

      'URL
      Call SB.Append(ProwlBaseUrl)
      'Call
      Call SB.Append("retrieve/token")
      'Parameters
      Call SB.AppendFormat("?providerkey={0}", ProviderKey)

      Return QueryAndParseBegin(Key, SB.ToString(), Callback)
    End Function

    Public Shared Function RetrieveTokenEnd(ByVal Result As IAsyncResult,
                                            <Out()> ByRef Key As Object,
                                            <Out()> ByRef Response As ProwlResult) As Exception
      Return QueryAndParseEnd(Result, Key, Response)
    End Function

#End Region

#Region "Function Retrieve/ApiKey"

    Public Shared Function RetrieveApiKey(ByVal ProviderKey As String,
                                          ByVal Token As String,
                                          <Out()> ByRef Result As ProwlResult) As Exception
      Result = Nothing
      Dim Ex As Exception = Nothing

      If (Not String.IsNullOrEmpty(ProviderKey)) AndAlso (ProviderKey.Length > 40) Then
        Ex = New ArgumentException("ProviderKey is too long (40 max)")
      ElseIf (Not String.IsNullOrEmpty(Token)) AndAlso (Token.Length > 40) Then
        Ex = New ArgumentException("Token is too long (40 max)")
      Else
        Dim SB As New StringBuilder

        'URL
        Call SB.Append(ProwlBaseUrl)
        'Call
        Call SB.Append("retrieve/apikey")
        'Parameters
        Call SB.AppendFormat("?providerkey={0}", ProviderKey)
        Call SB.AppendFormat("&token={0}", Token)

        Result = QueryAndParse(SB.ToString(), Ex)
      End If

      Return Ex
    End Function

    Public Shared Function RetrieveApiKeyBegin(ByVal Key As Object,
                                               ByVal ProviderKey As Object,
                                               ByVal Token As String,
                                               ByVal Callback As AsyncCallback) As IAsyncResult
      Dim SB As New StringBuilder

      'URL
      Call SB.Append(ProwlBaseUrl)
      'Call
      Call SB.Append("retrieve/apikey")
      'Parameters
      Call SB.AppendFormat("?providerkey={0}", ProviderKey)
      Call SB.AppendFormat("&token={0}", Token)

      Return QueryAndParseBegin(Key, SB.ToString(), Callback)
    End Function

    Public Shared Function RetrieveApiKeyEnd(ByVal Result As IAsyncResult,
                                             <Out()> ByRef Key As Object,
                                             <Out()> ByRef Response As ProwlResult) As Exception
      Return QueryAndParseEnd(Result, Key, Response)
    End Function

#End Region

    Protected Shared Function QueryAndParse(ByVal URL As String,
                                            <Out()> ByRef Ex As Exception) As ProwlResult
      Ex = Nothing
      Dim Result As ProwlResult = Nothing
      Dim Stream As Stream
      Dim Request As WebRequest
      Dim ResponseStream As Stream
      Dim Response As WebResponse

      Try
        Request = HttpWebRequest.Create(URL)

        Response = Request.GetResponse()

        ResponseStream = Response.GetResponseStream()

        'Copy stream
        Stream = New MemoryStream()
        Call ResponseStream.CopyTo(Stream)
        Stream.Position = 0

        'Close stream
        Call ResponseStream.Close()
        Call ResponseStream.Dispose()

        'Close response
        Call Response.Close()

        'Replace Stream with MemoryStream so we can seek (read it more than once)
        ResponseStream = Stream

        'Deserialize
        Result = DirectCast(ProwlResultSerializer.Deserialize(ResponseStream), ProwlResult)

        'Close stream
        Call ResponseStream.Close()
        Call ResponseStream.Dispose()
      Catch iEx As WebException
        Try
          Dim WebEx As WebException = iEx

          ResponseStream = WebEx.Response.GetResponseStream()

          'Copy stream
          Stream = New MemoryStream()
          Call ResponseStream.CopyTo(Stream)
          Stream.Position = 0

          'Close stream
          Call ResponseStream.Close()
          Call ResponseStream.Dispose()

          'Close response
          Call WebEx.Response.Close()

          Result = DirectCast(ProwlResultSerializer.Deserialize(Stream), ProwlResult)

          Ex = iEx
        Catch iiEx As Exception
          Ex = iiEx
        End Try
      Catch iEx As Exception
        Ex = iEx
      End Try

      Return Result
    End Function

#Region "Class AsyncStateWithKey"

    Private NotInheritable Class AsyncStateWithKey
      Public ReadOnly Key As Object
      Public ReadOnly Request As WebRequest

      Public Sub New(ByVal Key As Object,
                     ByVal State As WebRequest)
        Me.Key = Key
        Me.Request = State
      End Sub
    End Class

#End Region

#Region "Class StateAsyncResult"

    Private NotInheritable Class StateAsyncResult
      Implements IAsyncResult

      Private ReadOnly m_AsyncState As Object

      Public Sub New(ByVal AsyncState As Object)
        Me.m_AsyncState = AsyncState
      End Sub

      Public ReadOnly Property AsyncState As Object Implements IAsyncResult.AsyncState
        Get
          Return Me.m_AsyncState
        End Get
      End Property

      Public ReadOnly Property AsyncWaitHandle As WaitHandle Implements IAsyncResult.AsyncWaitHandle
        Get
          Return Nothing
        End Get
      End Property

      Public ReadOnly Property CompletedSynchronously As Boolean Implements IAsyncResult.CompletedSynchronously
        Get
          Return True
        End Get
      End Property

      Public ReadOnly Property IsCompleted As Boolean Implements IAsyncResult.IsCompleted
        Get
          Return True
        End Get
      End Property
    End Class

#End Region

    Public Shared Function QueryAndParseBegin(ByVal Key As Object,
                                              ByVal URL As String,
                                              ByVal Callback As AsyncCallback) As IAsyncResult
      Dim Result As IAsyncResult
      Dim Request As WebRequest

      Try

        Request = HttpWebRequest.Create(URL)

        Result = Request.BeginGetResponse(Callback, New AsyncStateWithKey(Key, Request))
      Catch iEx As Exception
        Result = Callback.BeginInvoke(New StateAsyncResult(iEx), Nothing, Nothing)
      End Try

      Return Result
    End Function

    Public Shared Function QueryAndParseEnd(ByVal Result As IAsyncResult,
                                            <Out()> ByRef Key As Object,
                                            <Out()> ByRef Response As ProwlResult) As Exception
      Key = Nothing
      Response = Nothing
      Dim Ex As Exception = Nothing
      Dim State As AsyncStateWithKey
      Dim ResponseStream As Stream = Nothing
      Dim Stream As Stream

      If (Result Is Nothing) Then
        Ex = New ArgumentNullException("Result")
      Else
        State = TryCast(Result.AsyncState, AsyncStateWithKey)
        Key = State.Key

        If (State Is Nothing) Then
          Ex = New Exception("Invalid AsyncState.")
        Else
          Try
            If (State.Request Is Nothing) Then
              Ex = New ArgumentNullException("Request")
            Else
              Dim WebResponse As WebResponse = State.Request.EndGetResponse(Result)
              ResponseStream = WebResponse.GetResponseStream()

              'Copy stream
              Stream = New MemoryStream()
              Call ResponseStream.CopyTo(Stream)
              Stream.Position = 0

              'Close stream
              Call ResponseStream.Close()
              Call ResponseStream.Dispose()

              'Close response
              Call WebResponse.Close()

              'Replace Stream with MemoryStream so we can seek (read it more than once)
              ResponseStream = Stream
            End If

            'Deserialize
            Response = DirectCast(ProwlResultSerializer.Deserialize(ResponseStream), ProwlResult)

            'Close stream
            Call ResponseStream.Close()
            Call ResponseStream.Dispose()
          Catch iEx As Exception
            Ex = iEx
          End Try
        End If
      End If

      Return Ex
    End Function
  End Class
End Namespace