Public Class Bots
    Private ReadOnly Database As Database
    Private ReadOnly SelectedCharacter As CharacterData
    Private Pagination As Pagination

    Public Sub New(_database As Database, _character As CharacterData)
        Database = _database
        SelectedCharacter = _character
    End Sub

    Public Function PromptForBot() As BotData
        Console.Clear()
        Pagination = New Pagination(Database.LoadBotData(SelectedCharacter.Id).Cast(Of Object)().ToList())

        If Pagination.NumberOfItems = 0 Then
            Console.WriteLine($"No bots found for {SelectedCharacter.Name}")
            Return Nothing
        End If

        Dim selectedBotId As Integer

        WriteMenu()

        Do
            Console.Write("Enter Bot ID: ")
            Dim input As String = Console.ReadLine()

            If Integer.TryParse(input, selectedBotId) Then
                Dim selectedBot = Pagination.PageItems.First(Function(bot) bot.BotId = selectedBotId)

                If selectedBot IsNot Nothing Then
                    Return selectedBot
                Else
                    Console.WriteLine("Invalid Bot ID. Please enter a valid Bot ID.")
                End If
            ElseIf input.Equals("X", StringComparison.CurrentCultureIgnoreCase) Then
                Environment.Exit(0)
            ElseIf input.Equals("N", StringComparison.CurrentCultureIgnoreCase) And Pagination.HasMore Then
                Pagination.MoveToNextPage()
                Console.Clear()
                WriteMenu()
            ElseIf input.Equals("P", StringComparison.CurrentCultureIgnoreCase) And Pagination.HasFewer Then
                Pagination.MoveToPreviousPage()
                Console.Clear()
                WriteMenu()
            Else
                Console.WriteLine("Invalid Bot ID. Please try again..")
            End If
        Loop While True
        Return Nothing
    End Function

    Private Sub WriteMenu()
        Utility.WriteWrappedLine($"Bots found for {SelectedCharacter.Name}:")
        For Each botData As BotData In Pagination.PageItems
            Console.WriteLine($"({botData.BotId}) - Name: {botData.BotName} [{botData.BotRaceName} {botData.BotClassName} ({botData.BotRace}|{botData.BotClass})]")
        Next
        If Pagination.HasMore Then
            Console.WriteLine($"{vbCrLf}(N) - Next")
        End If
        If Pagination.HasFewer Then
            Console.WriteLine($"{vbCrLf}(P) - Previous")
        End If
        Console.WriteLine($"{vbCrLf}(X) - Exit")
    End Sub
End Class
