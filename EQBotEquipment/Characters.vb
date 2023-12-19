Public Class Characters
    Private ReadOnly Database As Database
    Private ReadOnly Account As AccountData
    Public Sub New(_database As Database, _account As AccountData)
        Me.Database = _database
        Me.Account = _account
    End Sub

    Public Property CharacterData As List(Of CharacterData)

    Private _currentPage As Integer = 1

    Private Const PageSize = 2

    Public Function NumberOfAccounts() As Integer
        Return CharacterData.Count
    End Function

    Public ReadOnly Property CurrentPage As Integer
        Get
            Return _currentPage
        End Get
    End Property

    Public ReadOnly Property PageSizeValue As Integer
        Get
            Return PageSize
        End Get
    End Property

    Public ReadOnly Property PageItems As List(Of CharacterData)
        Get
            Dim startIndex = (CurrentPage - 1) * PageSize
            Return CharacterData.Skip(startIndex).Take(PageSize).ToList()
        End Get
    End Property

    Public ReadOnly Property HasMore As Boolean
        Get
            Return (CurrentPage * PageSize) < CharacterData.Count
        End Get
    End Property

    Public ReadOnly Property HasFewer As Boolean
        Get
            Return CurrentPage > 1
        End Get
    End Property

    Public Sub MoveToNextPage()
        If HasMore Then
            _currentPage += 1
        End If
    End Sub

    Public Sub MoveToPreviousPage()
        If HasFewer Then
            _currentPage -= 1
        End If
    End Sub

    Public Function PromptForCharacter() As CharacterData
        Console.Clear()
        CharacterData = Database.LoadCharacterData(Account.AccountId)

        If CharacterData.Count = 0 Then
            Console.WriteLine("Not characters found")
            Return Nothing
        End If

        Dim selectedCharacterId As Integer

        WriteCharacterMenu()

        Do
            Console.Write("Enter the ID of the character you want to work with: ")
            Dim input As String = Console.ReadLine()

            If Integer.TryParse(input, selectedCharacterId) Then
                Dim selectedCharacter = CharacterData.FirstOrDefault(Function(character) character.Id = selectedCharacterId)
                If selectedCharacter IsNot Nothing Then
                    Return selectedCharacter
                Else
                    Console.WriteLine("Invalid Character ID. Please try again.")
                End If

            ElseIf input.Equals("X", StringComparison.CurrentCultureIgnoreCase) Then
                Environment.Exit(0)
            ElseIf input.Equals("N", StringComparison.CurrentCultureIgnoreCase) And HasMore Then
                MoveToNextPage()
                Console.Clear()
                WriteCharacterMenu()
            ElseIf input.Equals("P", StringComparison.CurrentCultureIgnoreCase) And HasFewer Then
                MoveToPreviousPage()
                Console.Clear()
                WriteCharacterMenu()
            Else
                Console.WriteLine("Invalid input, pleaser enter a number and try again")
            End If

        Loop
    End Function
    Private Sub WriteCharacterMenu()
        Utility.WriteWrappedLine($"Characters found for {Account.Name}:")
        For Each character In PageItems
            Console.WriteLine($"({character.Id}) - {character.Name}")
        Next
        If HasMore Then
            Console.WriteLine($"{vbCrLf}(N) - Next")
        End If
        If HasFewer Then
            Console.WriteLine($"{vbCrLf}(P) - Previous")
        End If
        Console.WriteLine($"{vbCrLf}(X) - Exit")
    End Sub
End Class
