Imports System.Xml

Public Class Profiles

    Private ReadOnly XMLData As XMLData
    Private ReadOnly Database As Database
    Private ReadOnly SelectedCharacter As CharacterData

    Private Pagination As Pagination

    Public Sub New(_xmlData As XMLData, _database As Database, _selectedCharacter As CharacterData)
        XMLData = _xmlData
        Database = _database
        SelectedCharacter = _selectedCharacter
    End Sub

    Public Function PromptForProfile() As ProfileData
        Pagination = New Pagination(XMLData.GetProfilesByClass(SelectedCharacter.ClassId).Cast(Of Object)().ToList())

        If Pagination.NumberOfItems = 0 Then
            Console.WriteLine("No inventory profiles found...")
            Environment.Exit(0)
        End If

        Do
            WriteMenu()

            Dim selectedIndex As Integer
            Do
                Console.Write("Enter the number corresponding to the profile: ")
                Dim input As String = Console.ReadLine()

                If Integer.TryParse(input, selectedIndex) AndAlso selectedIndex >= 1 AndAlso selectedIndex <= Pagination.PageItemsCount Then
                    ' User entered a valid index
                    Exit Do
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
                    Console.WriteLine("Invalid input. Please enter a valid number.")
                End If
            Loop While True

            Dim selectedProfile As XmlNode = Pagination.PageItems(selectedIndex - 1)
            Dim profileItems As List(Of InventoryData) = XMLData.ExtractItems(Database, SelectedCharacter.Id, selectedProfile)

            Console.Write("Select this profile? (Y/N): ")
            If Console.ReadLine().Equals("Y", StringComparison.CurrentCultureIgnoreCase) Then
                Return New ProfileData(selectedProfile, profileItems)
            End If
        Loop While True

        Return Nothing
    End Function

    Private Sub WriteMenu()
        Console.Clear()
        Utility.WriteWrappedLine($"Select an equipment profile for the {SelectedCharacter.RaceName} / {SelectedCharacter.ClassName} Bot ''{SelectedCharacter.Name}'':")
        For i = 0 To Pagination.PageItemsCount - 1
            Console.WriteLine($"{i + 1}. {Pagination.PageItems(i).Attributes("Name").Value}")
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
