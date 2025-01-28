Imports System.Security.Principal
Imports System.Xml

Module Module1

    Private xmlData As XMLData
    Private database As Database

    Sub Main()
        Console.Clear()

        xmlData = New XMLData()
        xmlData.ImportExistingProfiles()

        database = New Database(xmlData)

        Dim reSelectCharacter As Boolean = False
        Dim selectedAccount As AccountData = Nothing
        Dim selectedCharacter As CharacterData = Nothing

        Do
            If reSelectCharacter Then
                If selectedAccount Is Nothing Then
                    selectedAccount = (New Accounts(database)).PromptForAccount()
                End If
                selectedCharacter = (New Characters(database, selectedAccount)).PromptForCharacter()
            Else
                selectedAccount = (New Accounts(database)).PromptForAccount()
                selectedCharacter = (New Characters(database, selectedAccount)).PromptForCharacter()
            End If
            reSelectCharacter = False

            If selectedCharacter IsNot Nothing Then
                Do
                    Dim selectedBot As BotData = (New Bots(database, selectedCharacter)).PromptForBot()
                    If selectedBot IsNot Nothing Then
                        Dim selectedProfile As ProfileData = (New Profiles(xmlData, database, selectedBot)).PromptForProfile()

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
                        reSelectCharacter = True
                        Exit Do
                    End If
                Loop While True
            End If
            Console.Clear()
        Loop While True

        database.Cleanup()
    End Sub

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


