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

        Dim reSelectPlayerCharacter As Boolean = False
        Dim selectedAccount As AccountData = Nothing
        Dim playerCharacter As CharacterData

        Do
            If reSelectPlayerCharacter Then
                If selectedAccount Is Nothing Then
                    selectedAccount = (New Accounts(database)).PromptForAccount()
                End If
                playerCharacter = (New Characters(database, selectedAccount)).PromptForCharacter()
            Else
                selectedAccount = (New Accounts(database)).PromptForAccount()
                playerCharacter = (New Characters(database, selectedAccount)).PromptForCharacter()
            End If
            reSelectPlayerCharacter = False

            If playerCharacter IsNot Nothing Then
                Do
                    Dim selectedCharacter As CharacterData = (New Bots(database, playerCharacter)).PromptForBot()
                    If selectedCharacter IsNot Nothing And TypeOf selectedCharacter Is CharacterData Then
                        Dim selectedProfile As ProfileData = (New Profiles(xmlData, database, selectedCharacter)).PromptForProfile()

                        If PromptForConfirmation(selectedAccount, playerCharacter, selectedCharacter, selectedProfile) Then
                            database.UpdateInventory(selectedCharacter, selectedProfile.Inventory)
                        Else
                            Console.WriteLine($"{vbCrLf}No changes were written to the database.{vbCrLf}")
                        End If
                        Console.Write($"Equip another from the same character? (Y/N): ")
                        Dim userResponse As String = Console.ReadLine().ToUpper()
                        If userResponse = "N" Then
                            Exit Do
                        Else
                            reSelectPlayerCharacter = True
                        End If
                    Else
                        reSelectPlayerCharacter = True
                        Exit Do
                    End If
                Loop While True
            End If
            Console.Clear()
        Loop While True

        database.Cleanup()
    End Sub

    Public Function PromptForConfirmation(account As AccountData, playerCharacter As CharacterData, selectedCharacter As CharacterData, profile As ProfileData)
        Console.Clear()
        Utility.WriteWrappedLine($"Confirm {selectedCharacter.Type} Equipment Change", True)
        Console.WriteLine($"Account: {account.Name} | Owning Character: {playerCharacter.Name}")
        Console.WriteLine($"{selectedCharacter.Type}: {selectedCharacter.Name} ({selectedCharacter.RaceName } {selectedCharacter.ClassName})")
        Console.WriteLine($"Using Profile: {profile.Profile.Attributes("Name").Value}{vbCrLf}")
        Utility.WriteWrappedLine("Equipment change to be made", True)

        For Each item As InventoryData In profile.Inventory
            Console.WriteLine($"{item.SlotName} ({item.SlotId}) -> ({item.ItemId}) {item.Name}")
        Next

        Console.Write($"{vbCrLf}Commit changes to the database? (Y/N): ")
        If Console.ReadLine().Equals("Y", StringComparison.CurrentCultureIgnoreCase) Then
            Return True
        End If

        Return False

    End Function
End Module


