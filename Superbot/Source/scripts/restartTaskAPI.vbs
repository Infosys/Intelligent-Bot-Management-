
msgbox "-- Starting Application --"
UserName=Wscript.Arguments.Item(0)
Password=Wscript.Arguments.Item(1)
Path="My Tasks\\"
TaskRelativePath=Path & Wscript.Arguments.Item(2)
Client=Wscript.Arguments.Item(3)
User=Wscript.Arguments.Item(4)
URL=Wscript.Arguments.Item(5)
MsgBox UserName &" -- "& Password&"---"&TaskRelativePath&"---"&Client&"---"&User&"---"&URL


restRequest = "{""UserName"": """ & UserName & """ ,""Password"": """& Password &"""}"
MsgBox(restRequest)

Set objStream = Nothing

contentType ="application/json"

Set oWinHttp = CreateObject("WinHttp.WinHttpRequest.5.1")

oWinHttp.Open "POST", "http://"&URL&"/controlroomapi/v1/authenticate", False

oWinHttp.setRequestHeader "Content-Type", contentType

oWinHttp.Send restRequest

response = oWinHttp.StatusText



Dim AuthToken

AuthToken = oWinHttp.ResponseText
'msgbox "Hello"
MsgBox AuthToken

'AUTHENTICATION API - ENDS

'---------------------------------------------------

'RESPONSE HEADER PARSING - START
Dim sToken

sToken = Right(AuthToken, Len(AuthToken) - 15)

MsgBox("Token1 : "&sToken) 

sToken = Left(sToken, Len(sToken) - 4)

'MsgBox(sToken)

'RESPONSE HEADER PARSING - END

'DEPLOYMENT API - START


restRequest = "{""TaskRelativePath"":"""& TaskRelativePath &""",""BotRunners"":[{""Client"":"""& Client &""",""User"":"""& User &"""}]}"

MsgBox(restRequest)

Set objStream = Nothing

contentType ="application/json"

Set oWinHttp = CreateObject("WinHttp.WinHttpRequest.5.1")

oWinHttp.Open "POST", "http://"&URL&"/controlroomapi/v1/deploy", False



oWinHttp.setRequestHeader "Content-Type", contentType

oWinHttp.setRequestHeader "X-Authorization", sToken

oWinHttp.Send restRequest

response = oWinHttp.StatusText
'msgbox "bbbb"
MsgBox response

Dim DeployResponse

DeployResponse = oWinHttp.ResponseText
'Wscript.Stdout.Writeline (DeployResponse)
'msgbox DeployResponse
'DEPLOYMENT API - ENDS