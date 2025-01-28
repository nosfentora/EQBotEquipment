' Database class to store connection information
Imports System.Data
Imports System.Xml
Imports MySql.Data.MySqlClient

Public Class Database
    Private ReadOnly Host As String
    Private ReadOnly DBName As String
    Private ReadOnly User As String
    Private ReadOnly Password As String
    Private ReadOnly ConnectionString As String

    Private ReadOnly Connection As MySqlConnection
    Private ReadOnly XmlData As XMLData

    Public Sub New(_xmlData As XMLData)
        XmlData = _xmlData

        Dim databaseElement As XmlElement = XmlData.GetElement("Database")
        If databaseElement IsNot Nothing Then
            Host = databaseElement.GetAttribute("host")
            DBName = databaseElement.GetAttribute("db_name")
            User = databaseElement.GetAttribute("user")
            Password = databaseElement.GetAttribute("password")
            ConnectionString = $"Server={Host};Database={DBName};User ID={User};Password={Password};"

            Connection = New MySqlConnection(ConnectionString)
            Try
                Connection.Open()
                Utility.WriteWrappedLine($"Connected to MySQL database {DBName} on {Host} successfully as {User}", True)
            Catch ex As Exception
                Console.WriteLine("Error: " & ex.Message)
                Environment.Exit(1)
            End Try
        Else
            Console.WriteLine("Database element not found.")
            Environment.Exit(1)
        End If
    End Sub

    Public Function LoadAccountData() As List(Of AccountData)
        Dim accountDataList As New List(Of AccountData)
        Dim selectQuery As String = "SELECT id, name FROM account ORDER BY name ASC"

        Using cmd As New MySqlCommand(selectQuery, Connection)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    accountDataList.Add(New AccountData With {.AccountId = reader.GetInt32("id"), .Name = reader.GetString("name")})
                End While
            End Using
        End Using

        Return accountDataList
    End Function

    Public Function LoadCharacterData(account_id As Integer) As List(Of CharacterData)
        Dim characterDataList As New List(Of CharacterData)

        Dim selectQuery As String = $"SELECT id, account_id, name, race, class FROM character_data WHERE account_id = {account_id} ORDER BY name ASC"

        Using cmd As New MySqlCommand(selectQuery, Connection)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    Dim character As New CharacterData(XmlData)
                    With character
                        .Id = reader.GetInt32("id")
                        .AccountId = reader.GetInt32("account_id")
                        .Name = reader.GetString("name")
                        .RaceId = reader.GetInt32("race")
                        .ClassId = reader.GetInt32("class")
                    End With

                    characterDataList.Add(character)
                End While
            End Using
        End Using

        Return characterDataList
    End Function

    Public Function LoadBotData(selectedCharacterId As Integer) As List(Of BotData)
        Dim botDataList As New List(Of BotData)

        Dim query As String = $"SELECT bot_id, name, race, class FROM bot_data WHERE owner_id = {selectedCharacterId} ORDER BY name ASC"

        Using command As New MySqlCommand(query, Connection)
            Using reader As MySqlDataReader = command.ExecuteReader()
                While reader.Read()
                    Dim bot As New BotData(XmlData)
                    With bot
                        .BotId = reader.GetInt32("bot_id")
                        .BotName = reader.GetString("name")
                        .BotRace = reader.GetString("race")
                        .BotClass = reader.GetString("class")
                    End With
                    botDataList.Add(bot)
                End While
            End Using
        End Using

        Return botDataList
    End Function

    Public Sub UpdateBotInventory(botId As Integer, validEntries As List(Of BotInventoryData))

        If validEntries IsNot Nothing And validEntries.Count > 0 Then
            Using transaction As MySqlTransaction = Connection.BeginTransaction()
                Dim success As Boolean = False
                Try
                    Using cmd As MySqlCommand = Connection.CreateCommand()
                        cmd.CommandText = $"DELETE FROM bot_inventories WHERE bot_id = {botId}"
                        cmd.ExecuteNonQuery()

                        For Each entry In validEntries
                            cmd.CommandText = $"INSERT INTO bot_inventories (bot_id, slot_id, item_id) VALUES ({entry.BotId}, {entry.SlotId}, {entry.ItemId})"
                            cmd.ExecuteNonQuery()
                        Next

                        transaction.Commit()
                        success = True
                    End Using

                Catch ex As Exception
                    transaction.Rollback()
                    Console.WriteLine($"Error: {ex.Message}")
                End Try
                If success Then
                    Console.WriteLine($"{vbCrLf}Updated bot inventory with {validEntries.Count} items.{vbCrLf}")
                End If
            End Using
        Else
            Console.WriteLine("There are no inventory entries to update.")
        End If
    End Sub

    Public Function GetItemData(itemId As Integer)
        Dim itemName As String = Nothing

        Dim selectQuery As String = $"SELECT id, name FROM items WHERE id = {itemId}"

        Using cmd As New MySqlCommand(selectQuery, Connection)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    itemName = reader.GetString("name")
                End If
            End Using
        End Using

        Return itemName
    End Function

    Public Sub Cleanup()
        If Connection.State = ConnectionState.Open Then
            Connection.Close()
        End If
    End Sub
End Class