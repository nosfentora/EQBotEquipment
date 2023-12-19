Public Class Accounts

    Private ReadOnly Database As Database
    Public Sub New(_database As Database)
        Me.Database = _database
    End Sub

    Public Property Pagination As Pagination

    Public Function PromptForAccount() As AccountData
        Dim AccountData As List(Of AccountData) = Database.LoadAccountData
        Me.Pagination = New Pagination(AccountData.Cast(Of Object)().ToList())

        If Me.Pagination.NumberOfItems = 0 Then
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
            ElseIf input.Equals("N", StringComparison.CurrentCultureIgnoreCase) And Me.Pagination.HasMore Then
                Me.Pagination.MoveToNextPage()
                Console.Clear()
                WriteAccountMenu()
            ElseIf input.Equals("P", StringComparison.CurrentCultureIgnoreCase) And Me.Pagination.HasFewer Then
                Me.Pagination.MoveToPreviousPage()
                Console.Clear()
                WriteAccountMenu()
            Else
                Console.WriteLine("Invalid input, please enter a number and try again.")
            End If
        Loop
    End Function

    Private Sub WriteAccountMenu()
        Utility.WriteWrappedLine("Accounts found:")
        For Each account In Me.Pagination.PageItems
            Console.WriteLine($"({account.AccountId}) - {account.Name})")
        Next
        If Me.Pagination.HasMore Then
            Console.WriteLine($"{vbCrLf}(N) - Next")
        End If
        If Me.Pagination.HasFewer Then
            Console.WriteLine($"{vbCrLf}(P) - Previous")
        End If
        Console.WriteLine($"{vbCrLf}(X) - Exit")
    End Sub
End Class
