Imports System.Security.Principal
Imports System.Xml

Module Module1

    Private xmlData As XMLData
    Private database As Database
    Private Utility As Utility

    Sub Main()
        Utility = New Utility

        Console.Clear()

        xmlData = New XMLData(Utility)
        xmlData.ImportExistingProfiles()

        database = New Database(xmlData, Utility)

        Do
            Dim selectedAccount As AccountData = (New Accounts(database)).PromptForAccount()
            Dim selectedCharacter As CharacterData = (New Characters(database, selectedAccount)).PromptForCharacter()
            If selectedCharacter IsNot Nothing Then
                Do
                    Dim selectedBot As BotData = (New Bots(database, selectedCharacter)).PromptForBot()
                    If selectedBot IsNot Nothing Then
                        Dim selectedProfile As ProfileData = PromptForProfile(selectedBot)

                        If PromptForConfirmation(selectedAccount, selectedCharacter, selectedBot, selectedProfile) Then
                            database.UpdateBotInventory(selectedBot.BotId, selectedProfile.Inventory)
                        Else
                            Console.WriteLine($"{vbCrLf}No changes were written to the database.{vbCrLf}")
                        End If


                        Console.Write("Equip another bot from the same character? (Y/N): ")
                        Dim userResponse As String = Console.ReadLine().ToUpper()
                        If userResponse = "N" Then
                            Exit Do
                        End If
                    Else
                        Exit Do
                    End If
                Loop While True
            End If
        Loop While True

    End Sub

    Public Function PromptForProfile(selectedBot As BotData) As ProfileData

        Dim profiles As List(Of XmlNode) = xmlData.GetProfilesByClass(selectedBot.BotClass)

        If profiles.Count = 0 Then
            Console.WriteLine("No inventory profiles found...")
            Environment.Exit(0)
        End If

        Do
            Console.Clear()
            Utility.WriteWrappedLine($"Select an equipment profile for the {selectedBot.BotRaceName} / {selectedBot.BotClassName} Bot ''{selectedBot.BotName}'':")
            For i = 0 To profiles.Count - 1
                Console.WriteLine($"{i + 1}. {profiles(i).Attributes("Name").Value}")
            Next
            Console.WriteLine($"{vbCrLf}(X) - Exit")

            Dim selectedIndex As Integer
            Do
                Console.Write("Enter the number corresponding to the profile: ")
                Dim input As String = Console.ReadLine()

                If Integer.TryParse(input, selectedIndex) AndAlso selectedIndex >= 1 AndAlso selectedIndex <= profiles.Count Then
                    ' User entered a valid index
                    Exit Do
                ElseIf input.Equals("X", StringComparison.CurrentCultureIgnoreCase) Then
                    Environment.Exit(0)
                Else
                    Console.WriteLine("Invalid input. Please enter a valid number.")
                End If
            Loop While True
            Dim selectedProfile As XmlNode = profiles(selectedIndex - 1)
            Dim profileItems As List(Of BotInventoryData) = xmlData.ExtractItems(database, selectedBot.BotId, selectedProfile)

            Console.Write("Select this profile? (Y/N): ")
            If Console.ReadLine().Equals("Y", StringComparison.CurrentCultureIgnoreCase) Then
                Return New ProfileData(selectedProfile, profileItems)
            End If
        Loop While True

        Return Nothing
    End Function

    Public Function PromptForConfirmation(account As AccountData, character As CharacterData, bot As BotData, profile As ProfileData)
        Console.Clear()
        Utility.WriteWrappedLine("Confirm Bot Equipment Change", True)
        Console.WriteLine($"Account: {account.Name} | Character: {character.Name}")
        Console.WriteLine($"Bot: {bot.BotName} ({bot.BotRaceName } {bot.BotClassName})")
        Console.WriteLine($"Using Profile: {profile.Profile.Attributes("Name").Value}{vbCrLf}")
        Utility.WriteWrappedLine("Equipment change to be made", True)

        For Each item As BotInventoryData In profile.Inventory
            Console.WriteLine($"{item.SlotName} ({item.SlotId}) -> ({item.ItemId}) {item.ItemName}")
        Next

        Console.Write($"{vbCrLf}Commit changes to the database? (Y/N): ")
        If Console.ReadLine().Equals("Y", StringComparison.CurrentCultureIgnoreCase) Then
            Return True
        End If

        Return False

    End Function
End Module


