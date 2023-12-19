Public Class Accounts

    Private ReadOnly Database As Database
    Private Pagination As Pagination

    Public Sub New(_database As Database)
        Database = _database
    End Sub


    Public Function PromptForAccount() As AccountData
        Pagination = New Pagination(Database.LoadAccountData.Cast(Of Object)().ToList())

        If Pagination.NumberOfItems = 0 Then
            Console.WriteLine("No accounts found, goodbye!")
            Environment.Exit(0)
        End If

        Dim selectedAcccountId As Integer

        WriteMenu()

        Do
            Console.Write("Enter the ID of the account you want to work with: ")
            Dim input As String = Console.ReadLine()

            If Integer.TryParse(input, selectedAcccountId) Then
                Dim selectedAccount = Pagination.PageItems.FirstOrDefault(Function(account) account.AccountId = selectedAcccountId)
                If selectedAccount IsNot Nothing Then
                    Return selectedAccount
                Else
                    Console.WriteLine("Invalid Account ID. Please try again.")
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
                Console.WriteLine("Invalid input, please enter a number and try again.")
            End If
        Loop
    End Function

    Private Sub WriteMenu()
        Utility.WriteWrappedLine("Accounts found:")
        For Each account In Pagination.PageItems
            Console.WriteLine($"({account.AccountId}) - {account.Name})")
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
