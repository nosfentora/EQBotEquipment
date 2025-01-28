Public Class Characters
    Private ReadOnly Database As Database
    Private ReadOnly Account As AccountData
    Private Pagination As Pagination

    Public Sub New(_database As Database, _account As AccountData)
        Database = _database
        Account = _account
    End Sub

    Public Function PromptForCharacter() As CharacterData
        Console.Clear()
        Pagination = New Pagination(Database.LoadCharacterData(Account.AccountId).Cast(Of Object)().ToList())

        If Pagination.NumberOfItems = 0 Then
            Console.WriteLine("Not characters found")
            Return Nothing
        End If

        Dim selectedCharacterId As Integer

        WriteMenu()

        Do
            Console.Write("Enter the ID of the character you want to work with: ")
            Dim input As String = Console.ReadLine()

            If Integer.TryParse(input, selectedCharacterId) Then
                Dim selectedCharacter = Pagination.PageItems.First(Function(character) character.Id = selectedCharacterId)
                If selectedCharacter IsNot Nothing Then
                    Return selectedCharacter
                Else
                    Console.WriteLine("Invalid Character ID. Please try again.")
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
                Console.WriteLine("Invalid input, pleaser enter a number and try again")
            End If
        Loop
    End Function

    Private Sub WriteMenu()
        Utility.WriteWrappedLine($"Characters found for {Account.Name}:")
        For Each character In Pagination.PageItems
            Console.WriteLine($"({character.Id}) - {character.Name} ({character.RaceName} | {character.ClassName})")
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
