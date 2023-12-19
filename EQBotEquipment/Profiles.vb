Imports System.Xml

Public Class Profiles

    Private ReadOnly XMLData As XMLData
    Private ReadOnly Database As Database
    Private ReadOnly SelectedBot As BotData

    Private Pagination As Pagination

    Public Sub New(_xmlData As XMLData, _database As Database, _selectedBot As BotData)
        Me.XMLData = _xmlData
        Me.Database = _database
        Me.SelectedBot = _selectedBot
    End Sub

    Public Function PromptForProfile() As ProfileData

        Dim ProfileData As List(Of XmlNode) = XMLData.GetProfilesByClass(SelectedBot.BotClass)
        Me.Pagination = New Pagination(ProfileData.Cast(Of Object)().ToList())

        If Me.Pagination.NumberOfItems = 0 Then
            Console.WriteLine("No inventory profiles found...")
            Environment.Exit(0)
        End If

        Do
            WriteMenu()

            Dim selectedIndex As Integer
            Do
                Console.Write("Enter the number corresponding to the profile: ")
                Dim input As String = Console.ReadLine()

                If Integer.TryParse(input, selectedIndex) AndAlso selectedIndex >= 1 AndAlso selectedIndex <= Me.Pagination.PageItemsCount Then
                    ' User entered a valid index
                    Exit Do
                ElseIf input.Equals("X", StringComparison.CurrentCultureIgnoreCase) Then
                    Environment.Exit(0)
                ElseIf input.Equals("N", StringComparison.CurrentCultureIgnoreCase) And Me.Pagination.HasMore Then
                    Me.Pagination.MoveToNextPage()
                    Console.Clear()
                    WriteMenu()
                ElseIf input.Equals("P", StringComparison.CurrentCultureIgnoreCase) And Me.Pagination.HasFewer Then
                    Me.Pagination.MoveToPreviousPage()
                    Console.Clear()
                    WriteMenu()
                Else
                    Console.WriteLine("Invalid input. Please enter a valid number.")
                End If
            Loop While True

            Dim selectedProfile As XmlNode = ProfileData(selectedIndex - 1)
            Dim profileItems As List(Of BotInventoryData) = XMLData.ExtractItems(Database, SelectedBot.BotId, selectedProfile)

            Console.Write("Select this profile? (Y/N): ")
            If Console.ReadLine().Equals("Y", StringComparison.CurrentCultureIgnoreCase) Then
                Return New ProfileData(selectedProfile, profileItems)
            End If
        Loop While True

        Return Nothing
    End Function

    Private Sub WriteMenu()
        Console.Clear()
        Utility.WriteWrappedLine($"Select an equipment profile for the {selectedBot.BotRaceName} / {selectedBot.BotClassName} Bot ''{selectedBot.BotName}'':")
        For i = 0 To Me.Pagination.PageItemsCount - 1
            Console.WriteLine($"{i + 1}. {Me.Pagination.PageItems(i).Attributes("Name").Value}")
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
