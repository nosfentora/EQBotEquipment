Public Class Accounts

    Private ReadOnly Database As Database
    Public Sub New(_database As Database)
        Me.Database = _database
    End Sub

    Public Property AccountData As List(Of AccountData)
    Private _currentPage As Integer = 1

    Private Const PageSize = 10

    Public Function NumberOfAccounts() As Integer
        Return AccountData.Count
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

    Public ReadOnly Property PageItems As List(Of AccountData)
        Get
            Dim startIndex = (CurrentPage - 1) * PageSize
            Return AccountData.Skip(startIndex).Take(PageSize).ToList()
        End Get
    End Property

    Public ReadOnly Property HasMore As Boolean
        Get
            Return (currentPage * PageSize) < AccountData.Count
        End Get
    End Property

    Public ReadOnly Property HasFewer As Boolean
        Get
            Return currentPage > 1
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
    Public Function PromptForAccount() As AccountData
        AccountData = Database.LoadAccountData
        'Dim accountsData As New Accounts With {
        '    .AccountData = Database.LoadAccountData
        '}

        If NumberOfAccounts() = 0 Then
            Console.WriteLine("No accounts found, goodbye!")
            Environment.Exit(0)
        End If

        Dim selectedAcccountId As Integer

        WriteAccountMenu()

        Do
            Console.Write("Enter the ID of the account you want to work with: ")
            Dim input As String = Console.ReadLine()

            If Integer.TryParse(input, selectedAcccountId) Then
                Dim selectedAccount = AccountData.FirstOrDefault(Function(account) account.AccountId = selectedAcccountId)
                If selectedAccount IsNot Nothing Then
                    Return selectedAccount
                Else
                    Console.WriteLine("Invalid Account ID. Please try again.")
                End If
            ElseIf input.Equals("X", StringComparison.CurrentCultureIgnoreCase) Then
                Environment.Exit(0)
            ElseIf input.Equals("N", StringComparison.CurrentCultureIgnoreCase) And HasMore Then
                MoveToNextPage()
                Console.Clear()
                WriteAccountMenu()
            ElseIf input.Equals("P", StringComparison.CurrentCultureIgnoreCase) And HasFewer Then
                MoveToPreviousPage()
                Console.Clear()
                WriteAccountMenu()
            Else
                Console.WriteLine("Invalid input, please enter a number and try again.")
            End If
        Loop
    End Function

    Private Sub WriteAccountMenu()
        Utility.WriteWrappedLine("Accounts found:")
        For Each account In PageItems
            Console.WriteLine($"({account.AccountId}) - {account.Name})")
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
