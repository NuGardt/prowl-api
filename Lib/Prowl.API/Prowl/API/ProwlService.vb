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
Imports System.Web
Imports System.IO

Namespace Prowl.API
  ''' <summary>
  ''' Class contains synchronous and asynchronous API call function.
  ''' </summary>
  ''' <remarks></remarks>
    Public NotInheritable Class ProwlService
    ''' <summary>
    ''' Base calling URL with SSL.
    ''' </summary>
    ''' <remarks></remarks>
    Private Const ProwlBaseUrlWithSSL As String = "https://api.prowlapp.com/publicapi/"

    ''' <summary>
    ''' Base calling URL without SSL.
    ''' </summary>
    ''' <remarks></remarks>
    Private Const ProwlBaseUrl As String = "http://api.prowlapp.com/publicapi/"

    ''' <summary>
    ''' Serializer for API response
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared ReadOnly ProwlResultSerializer As XmlSerializer = New XmlSerializer(GetType(ProwlResult))

    ''' <summary>
    ''' Don't allow instance creation.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
      '-
    End Sub

#Region "Function AddNotification"

    ''' <summary>
    ''' Add a notification for a particular user.
    ''' 
    ''' You must provide either event or description or both.
    ''' </summary>
    ''' <param name="ApiKey">API keys separated by commas. Each API key is a 40-byte hexadecimal string.
    ''' 
    ''' When using multiple API keys, you will only get a failure response if all API keys are not valid.</param>
    ''' <param name="Application">The name of your application or the application generating the event.</param>
    ''' <param name="Event">The name of the event or subject of the notification.</param>
    ''' <param name="Description">A description of the event, generally terse. [10000]</param>
    ''' <param name="Result">Contains result details if applicable.</param>
    ''' <param name="ProviderKey">Your provider API key. Only necessary if you have been whitelisted. [40]</param>
    ''' <param name="Priority">Default value of 0 if not provided. An integer value ranging [-2, 2] representing:
    ''' 
    '''     -2 = Very Low
    '''     -1 = Moderate
    '''      0 = Normal
    '''      1 = High
    '''      2 = Emergency
    ''' 
    ''' Emergency priority messages may bypass quiet hours according to the user's settings. </param>
    ''' <param name="URL">Requires Prowl 1.2 The URL which should be attached to the notification.
    '''
    ''' This will trigger a redirect when launched, and is viewable in the notification list.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function AddNotification(ByVal ApiKey As String,
                                           ByVal Application As String,
                                           ByVal [Event] As String,
                                           ByVal Description As String,
                                           <Out()> ByRef Result As ProwlResult,
                                           Optional ByVal ProviderKey As String = Nothing,
                                           Optional ByVal Priority As eProwlPriority = eProwlPriority.Normal,
                                           Optional ByVal URL As String = Nothing,
                                           Optional ByVal NoSSL As Boolean = False) As Exception
      Result = Nothing
      Dim Ex As Exception = Nothing

      If (Not String.IsNullOrEmpty(ProviderKey)) AndAlso (ProviderKey.Length > 40) Then
        Ex = New ArgumentException("ProviderKey is too long (40 max)")
      ElseIf (Not String.IsNullOrEmpty(URL)) AndAlso (URL.Length > 512) Then
        Ex = New ArgumentException("URL is too long (512 max)")
      ElseIf (Not String.IsNullOrEmpty(Application)) AndAlso (Application.Length > 512) Then
        Ex = New ArgumentException("Application is too long (256 max)")
      ElseIf (Not String.IsNullOrEmpty([Event])) AndAlso ([Event].Length > 1024) Then
        Ex = New ArgumentException("Event is too long (1024 max)")
      ElseIf (Not String.IsNullOrEmpty(Description)) AndAlso (Description.Length > 10000) Then
        Ex = New ArgumentException("Description is too long (10000 max)")
      ElseIf (String.IsNullOrEmpty([Event])) AndAlso (String.IsNullOrEmpty(Description)) Then
        Ex = New ArgumentNullException("Event or Description must be filled.")
      Else
        Dim SB As New StringBuilder

        'URL
        If NoSSL Then
          Call SB.Append(ProwlBaseUrl)
        Else
          Call SB.Append(ProwlBaseUrlWithSSL)
        End If
        'Call
        Call SB.Append("add")
        'Parameters
        Call SB.AppendFormat("?apikey={0}", HttpUtility.UrlEncode(ApiKey))
        If (Not String.IsNullOrEmpty(ProviderKey)) Then Call SB.AppendFormat("&providerkey={0}", HttpUtility.UrlEncode(ProviderKey))
        Dim tPriority As Int16 = Priority
        Call SB.AppendFormat("&priority={0}", tPriority.ToString())
        If (Not String.IsNullOrEmpty(URL)) Then Call SB.AppendFormat("&url={0}", HttpUtility.UrlEncode(URL))
        Call SB.AppendFormat("&application={0}", HttpUtility.UrlEncode(Application))
        Call SB.AppendFormat("&event={0}", HttpUtility.UrlEncode([Event]))
        Call SB.AppendFormat("&description={0}", HttpUtility.UrlEncode(Description))

        Result = QueryAndParse(SB.ToString(), Ex)
      End If

      Return Ex
    End Function

    ''' <summary>
    ''' Add a notification for a particular user.
    ''' 
    ''' You must provide either event or description or both.
    ''' </summary>
    ''' <param name="Key">Your own Key for tracking asynchronous calls.</param>
    ''' <param name="ApiKey">API keys separated by commas. Each API key is a 40-byte hexadecimal string.
    ''' 
    ''' When using multiple API keys, you will only get a failure response if all API keys are not valid.</param>
    ''' <param name="Application">The name of your application or the application generating the event.</param>
    ''' <param name="Event">The name of the event or subject of the notification.</param>
    ''' <param name="Description">A description of the event, generally terse. [10000]</param>
    ''' <param name="Callback">Method to call on completion or failure.</param>
    ''' <param name="ProviderKey">Your provider API key. Only necessary if you have been whitelisted. [40]</param>
    ''' <param name="Priority">Default value of 0 if not provided. An integer value ranging [-2, 2] representing:
    ''' 
    '''     -2 = Very Low
    '''     -1 = Moderate
    '''      0 = Normal
    '''      1 = High
    '''      2 = Emergency
    ''' 
    ''' Emergency priority messages may bypass quiet hours according to the user's settings. </param>
    ''' <param name="URL">Requires Prowl 1.2 The URL which should be attached to the notification.
    '''
    ''' This will trigger a redirect when launched, and is viewable in the notification list.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function AddNotificationBegin(ByVal Key As Object,
                                                ByVal ApiKey As String,
                                                ByVal Application As String,
                                                ByVal [Event] As String,
                                                ByVal Description As String,
                                                ByVal Callback As AsyncCallback,
                                                Optional ByVal ProviderKey As String = Nothing,
                                                Optional ByVal Priority As eProwlPriority = eProwlPriority.Normal,
                                                Optional ByVal URL As String = Nothing,
                                                Optional ByVal NoSSL As Boolean = False) As IAsyncResult
      Dim SB As New StringBuilder

      'URL
      If NoSSL Then
        Call SB.Append(ProwlBaseUrl)
      Else
        Call SB.Append(ProwlBaseUrlWithSSL)
      End If
      'Call
      Call SB.Append("add")
      'Parameters
      Call SB.AppendFormat("?apikey={0}", HttpUtility.UrlEncode(ApiKey))
      If (Not String.IsNullOrEmpty(ProviderKey)) Then Call SB.AppendFormat("&providerkey={0}", HttpUtility.UrlEncode(ProviderKey))
      Dim tPriority As Int16 = Priority
      Call SB.AppendFormat("&priority={0}", tPriority.ToString())
      If (Not String.IsNullOrEmpty(URL)) Then Call SB.AppendFormat("&url={0}", HttpUtility.UrlEncode(URL))
      Call SB.AppendFormat("&application={0}", HttpUtility.UrlEncode(Application))
      Call SB.AppendFormat("&event={0}", HttpUtility.UrlEncode([Event]))
      Call SB.AppendFormat("&description={0}", HttpUtility.UrlEncode(Description))

      Return QueryAndParseBegin(Key, SB.ToString(), Callback)
    End Function

    ''' <summary>
    ''' Gets the response after the callback.
    ''' </summary>
    ''' <param name="Result">Asynchronous result</param>
    ''' <param name="Key">Contains your key for tracking asynchronous calls.</param>
    ''' <param name="Response">Contains response if applicable.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function AddNotificationEnd(ByVal Result As IAsyncResult,
                                              <Out()> ByRef Key As Object,
                                              <Out()> ByRef Response As ProwlResult) As Exception
      Return QueryAndParseEnd(Result, Key, Response)
    End Function

#End Region

#Region "Function Verify"

    ''' <summary>
    ''' Verify an API key is valid.
    '''
    ''' For the sake of adding a notification do not call verify first; it costs you an API call. You should only use verify to confirm an API key is valid in situations like a user entering an API key into your program. If it's not valid while posting the notification, you will get the appropriate error.
    ''' </summary>
    ''' <param name="ApiKey">The user's API key. A 40-byte hexadecimal string.</param>
    ''' <param name="Result">Contains the result if applicable.</param>
    ''' <param name="ProviderKey">Your provider API key. Only necessary if you have been whitelisted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Verify(ByVal ApiKey As String,
                                  <Out()> ByRef Result As ProwlResult,
                                  Optional ByVal ProviderKey As String = Nothing,
                                  Optional ByVal NoSSL As Boolean = False) As Exception
      Result = Nothing
      Dim Ex As Exception = Nothing

      If (Not String.IsNullOrEmpty(ProviderKey)) AndAlso (ProviderKey.Length > 40) Then
        Ex = New ArgumentException("ProviderKey is too long (40 max)")
      Else
        Dim SB As New StringBuilder

        'URL
        If NoSSL Then
          Call SB.Append(ProwlBaseUrl)
        Else
          Call SB.Append(ProwlBaseUrlWithSSL)
        End If
        'Call
        Call SB.Append("verify")
        'Parameters
        Call SB.AppendFormat("?apikey={0}", HttpUtility.UrlEncode(ApiKey))
        If (Not String.IsNullOrEmpty(ProviderKey)) Then Call SB.AppendFormat("&providerkey={0}", HttpUtility.UrlEncode(ProviderKey))

        Result = QueryAndParse(SB.ToString(), Ex)
      End If

      Return Ex
    End Function

    ''' <summary>
    ''' Verify an API key is valid.
    ''' 
    ''' For the sake of adding a notification do not call verify first; it costs you an API call. You should only use verify to confirm an API key is valid in situations like a user entering an API key into your program. If it's not valid while posting the notification, you will get the appropriate error.
    ''' </summary>
    ''' <param name="Key">Your own Key for tracking asynchronous calls.</param>
    ''' <param name="ApiKey">he user's API key. A 40-byte hexadecimal string.</param>
    ''' <param name="Callback">Method to call on completion or failure.</param>
    ''' <param name="ProviderKey">Your provider API key. Only necessary if you have been whitelisted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function VerifyBegin(ByVal Key As Object,
                                       ByVal ApiKey As String,
                                       ByVal Callback As AsyncCallback,
                                       Optional ByVal ProviderKey As String = Nothing,
                                       Optional ByVal NoSSL As Boolean = False) As IAsyncResult
      Dim SB As New StringBuilder

      'URL
      If NoSSL Then
        Call SB.Append(ProwlBaseUrl)
      Else
        Call SB.Append(ProwlBaseUrlWithSSL)
      End If
      'Call
      Call SB.Append("verify")
      'Parameters
      Call SB.AppendFormat("?apikey={0}", HttpUtility.UrlEncode(ApiKey))
      If (Not String.IsNullOrEmpty(ProviderKey)) Then Call SB.AppendFormat("&providerkey={0}", HttpUtility.UrlEncode(ProviderKey))

      Return QueryAndParseBegin(Key, SB.ToString(), Callback)
    End Function

    ''' <summary>
    ''' Gets the response after the callback.
    ''' </summary>
    ''' <param name="Result">Asynchronous result</param>
    ''' <param name="Key">Contains your key for tracking asynchronous calls.</param>
    ''' <param name="Response">Contains response if applicable.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function VerifyEnd(ByVal Result As IAsyncResult,
                                     <Out()> ByRef Key As Object,
                                     <Out()> ByRef Response As ProwlResult) As Exception
      Return QueryAndParseEnd(Result, Key, Response)
    End Function

#End Region

#Region "Function Retrieve/Token"

    ''' <summary>
    ''' Get a registration token for use in retrieve/apikey and the associated URL for the user to approve the request.
    ''' 
    ''' This is the first step in fetching an API key for a user. The token retrieved expires after 24 hours.
    ''' </summary>
    ''' <param name="ProviderKey">Your provider API key. Required</param>
    ''' <param name="Result">Contains the result if applicable.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function RetrieveToken(ByVal ProviderKey As String,
                                         <Out()> ByRef Result As ProwlResult,
                                         Optional ByVal NoSSL As Boolean = False) As Exception
      Result = Nothing
      Dim Ex As Exception = Nothing

      If (Not String.IsNullOrEmpty(ProviderKey)) AndAlso (ProviderKey.Length > 40) Then
        Ex = New ArgumentException("ProviderKey is too long (40 max)")
      Else
        Dim SB As New StringBuilder

        'URL
        If NoSSL Then
          Call SB.Append(ProwlBaseUrl)
        Else
          Call SB.Append(ProwlBaseUrlWithSSL)
        End If
        'Call
        Call SB.Append("retrieve/token")
        'Parameters
        Call SB.AppendFormat("?providerkey={0}", HttpUtility.UrlEncode(ProviderKey))

        Result = QueryAndParse(SB.ToString(), Ex)
      End If

      Return Ex
    End Function

    ''' <summary>
    ''' Get a registration token for use in retrieve/apikey and the associated URL for the user to approve the request.
    ''' 
    ''' This is the first step in fetching an API key for a user. The token retrieved expires after 24 hours.
    ''' </summary>
    ''' <param name="Key">Your own Key for tracking asynchronous calls.</param>
    ''' <param name="ProviderKey">Your provider API key. Required</param>
    ''' <param name="Callback">Method to call on completion or failure.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function RetrieveTokenBegin(ByVal Key As Object,
                                              ByVal ProviderKey As String,
                                              ByVal Callback As AsyncCallback,
                                              Optional ByVal NoSSL As Boolean = False) As IAsyncResult
      Dim SB As New StringBuilder

      'URL
      If NoSSL Then
        Call SB.Append(ProwlBaseUrl)
      Else
        Call SB.Append(ProwlBaseUrlWithSSL)
      End If
      'Call
      Call SB.Append("retrieve/token")
      'Parameters
      Call SB.AppendFormat("?providerkey={0}", HttpUtility.UrlEncode(ProviderKey))

      Return QueryAndParseBegin(Key, SB.ToString(), Callback)
    End Function

    ''' <summary>
    ''' Gets the response after the callback.
    ''' </summary>
    ''' <param name="Result">Asynchronous result</param>
    ''' <param name="Key">Contains your key for tracking asynchronous calls.</param>
    ''' <param name="Response">Contains response if applicable.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function RetrieveTokenEnd(ByVal Result As IAsyncResult,
                                            <Out()> ByRef Key As Object,
                                            <Out()> ByRef Response As ProwlResult) As Exception
      Return QueryAndParseEnd(Result, Key, Response)
    End Function

#End Region

#Region "Function Retrieve/ApiKey"

    ''' <summary>
    ''' Get an API key from a registration token retrieved in retrieve/token. The user must have approved your request first, or you will get an error response.
    '''
    ''' This is the second/final step in fetching an API key for a user.
    ''' </summary>
    ''' <param name="ProviderKey">Your provider API key. Required.</param>
    ''' <param name="Token">The token returned from retrieve/token. Required.</param>
    ''' <param name="Result">Contains the result if applicable.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function RetrieveApiKey(ByVal ProviderKey As String,
                                          ByVal Token As String,
                                          <Out()> ByRef Result As ProwlResult,
                                          Optional ByVal NoSSL As Boolean = False) As Exception
      Result = Nothing
      Dim Ex As Exception = Nothing

      If (Not String.IsNullOrEmpty(ProviderKey)) AndAlso (ProviderKey.Length > 40) Then
        Ex = New ArgumentException("ProviderKey is too long (40 max)")
      ElseIf (Not String.IsNullOrEmpty(Token)) AndAlso (Token.Length > 40) Then
        Ex = New ArgumentException("Token is too long (40 max)")
      Else
        Dim SB As New StringBuilder

        'URL
        If NoSSL Then
          Call SB.Append(ProwlBaseUrl)
        Else
          Call SB.Append(ProwlBaseUrlWithSSL)
        End If
        'Call
        Call SB.Append("retrieve/apikey")
        'Parameters
        Call SB.AppendFormat("?providerkey={0}", HttpUtility.UrlEncode(ProviderKey))
        Call SB.AppendFormat("&token={0}", HttpUtility.UrlEncode(Token))

        Result = QueryAndParse(SB.ToString(), Ex)
      End If

      Return Ex
    End Function

    ''' <summary>
    ''' Get an API key from a registration token retrieved in retrieve/token. The user must have approved your request first, or you will get an error response.
    '''
    ''' This is the second/final step in fetching an API key for a user.
    ''' </summary>
    ''' <param name="Key">Your own Key for tracking asynchronous calls.</param>
    ''' <param name="ProviderKey">Your provider API key. Required.</param>
    ''' <param name="Token">The token returned from retrieve/token. Required.</param>
    ''' <param name="Callback">Method to call on completion or failure.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function RetrieveApiKeyBegin(ByVal Key As Object,
                                               ByVal ProviderKey As String,
                                               ByVal Token As String,
                                               ByVal Callback As AsyncCallback,
                                               Optional ByVal NoSSL As Boolean = False) As IAsyncResult
      Dim SB As New StringBuilder

      'URL
      If NoSSL Then
        Call SB.Append(ProwlBaseUrl)
      Else
        Call SB.Append(ProwlBaseUrlWithSSL)
      End If
      'Call
      Call SB.Append("retrieve/apikey")
      'Parameters
      Call SB.AppendFormat("?providerkey={0}", HttpUtility.UrlEncode(ProviderKey))
      Call SB.AppendFormat("&token={0}", HttpUtility.UrlEncode(Token))

      Return QueryAndParseBegin(Key, SB.ToString(), Callback)
    End Function

    ''' <summary>
    ''' Gets the response after the callback.
    ''' </summary>
    ''' <param name="Result">Asynchronous result</param>
    ''' <param name="Key">Contains your key for tracking asynchronous calls.</param>
    ''' <param name="Response">Contains response if applicable.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function RetrieveApiKeyEnd(ByVal Result As IAsyncResult,
                                             <Out()> ByRef Key As Object,
                                             <Out()> ByRef Response As ProwlResult) As Exception
      Return QueryAndParseEnd(Result, Key, Response)
    End Function

#End Region

    Private Shared Function QueryAndParse(ByVal URL As String,
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

          If (WebEx.Response IsNot Nothing) Then
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
          End If

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