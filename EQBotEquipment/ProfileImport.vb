Imports System.Globalization
Imports System.Net.Http
Imports System.Security.Policy
Imports System.Text.RegularExpressions
Imports System.Xml

Public Class ProfileImport

    Private Const MAGELO_URL As String = "https://eq.magelo.com/profile"

    Private ReadOnly SelectedCharacter As CharacterData
    Private ReadOnly XMLData As XMLData

    Public Sub New(_selectedCharacter As CharacterData, _xmlData As XMLData)
        SelectedCharacter = _selectedCharacter
        XMLData = _xmlData
    End Sub

    Public Function PromptForProfile() As XmlNode
        Do
            WriteMenu()
            Console.Write($"{vbCrLf}Magelo Profile: ")
            Dim input As String = Console.ReadLine()
            Dim match As Match = Regex.Match(input, "\d+")
            If input.Equals("R", StringComparison.CurrentCultureIgnoreCase) Then
                Return Nothing
            ElseIf input.Equals("X", StringComparison.CurrentCultureIgnoreCase) Then
                Environment.Exit(0)
            ElseIf match.Success Then
                Dim mageloId As String = match.Value
                Dim html As String = GetHtml(mageloId)
                Dim itemIds(22) As String
                For i As Integer = 0 To 22
                    itemIds(i) = "0"
                Next

                Dim pattern As String = "items\[(\d+)\]\s*=\s*new\s*Item\((\d+),"
                Dim matches As MatchCollection = Regex.Matches(html, pattern)

                For Each m As Match In matches
                    Dim index As Integer = Convert.ToInt32(m.Groups(1).Value)
                    Dim itemId As String = m.Groups(2).Value

                    If index >= 0 AndAlso index <= 22 Then
                        itemIds(index) = itemId
                    End If
                Next
                SelectedCharacter.MageloId = mageloId
                Dim profile As XmlNode = XMLData.CreateProfileNode(SelectedCharacter, String.Join(",", itemIds))
                Return profile
            Else
                Console.WriteLine("Invalid input. Please enter a valid Magelo ID.")
            End If
        Loop

    End Function
    Private Shared Function GetHtml(ByVal id As String) As String
        Using client As New HttpClient()
            Return client.GetStringAsync($"{MAGELO_URL}/{id}").Result
        End Using
    End Function

    Private Shared Sub WriteMenu()
        Console.Clear()

        Utility.WriteWrappedLine("Scrape Magelo Profile")
        Console.WriteLine($"{vbCrLf}Enter the Magelo detail of the character you want to import in one of the following forms:")
        Console.WriteLine($"123456789 | javascript:viewProfile(123456789) | https://eq.magelo.com/profile/123456789")
        Console.WriteLine($"{vbCrLf}(R) Return to Profile Select")
        Console.WriteLine($"{vbCrLf}(X) - Exit")
    End Sub
End Class
