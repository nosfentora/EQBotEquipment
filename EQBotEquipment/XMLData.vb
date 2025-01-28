Imports System.IO
Imports System.Runtime
Imports System.Xml
Imports Org.BouncyCastle.Crypto

Public Class XMLData

    Private ReadOnly Property XmlData As XmlDocument
    Private Const FILENAME As String = "Data.xml"
    Private ReadOnly Property XmlFilePath As String
    Private ReadOnly Property ExePath As String

    Public Sub New()
        ExePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        XmlFilePath = System.IO.Path.Combine(ExePath, FILENAME)

        XmlData = New XmlDocument()
        XmlData.Load(XmlFilePath)
    End Sub

    Public Function GetElement(elementName As String) As XmlElement
        If XmlData IsNot Nothing Then
            Dim rootElement As XmlElement = XmlData.DocumentElement

            Dim element As XmlElement = rootElement.SelectSingleNode(elementName)
            If element IsNot Nothing Then
                Return element
            Else
                Console.WriteLine($"Element {elementName} not found.")
            End If
        Else
            Console.WriteLine("XML data not loaded.")
            Environment.Exit(1)
        End If

        Return Nothing
    End Function

    Public Function GetByValue(root As String, node As String, value As Integer)
        Dim val As String = "Value"

        If node = "Race" Then
            val = "Special"
        End If

        Dim selectedNode As XmlNode = GetElement($"Data/{root}/{node}[@{val}='{value - 1}']")
        If selectedNode IsNot Nothing Then
            Return selectedNode.Attributes("Name").Value
        Else
            Console.WriteLine($"{root} with value {value - 1} not found.")
        End If

        Return String.Empty

    End Function

    Public Function GetProfilesByClass(classValue As Integer) As List(Of XmlNode)
        Dim profiles As New List(Of XmlNode)
        Dim nodeTypes As Array = {"PreDefined", "Imported"}

        For Each nodeType As String In nodeTypes
            Dim nodes As XmlNode = GetElement($"Bots/{nodeType}")
            If nodes IsNot Nothing Then
                For Each profileNode As XmlNode In nodes
                    Dim profileClass As Integer
                    If Integer.TryParse(profileNode.Attributes("Class").Value, profileClass) AndAlso (profileClass = classValue - 1 Or profileClass = -1) Then
                        profiles.Add(profileNode)
                    End If
                Next
            Else
                Console.WriteLine($"{nodeType} Profile nodes not found.")
            End If
        Next

        Return profiles
    End Function

    Private Function GuessClassIdByFileName(fileName As String) As Integer
        Dim classesNode As XmlNode = GetElement("Data/Classes")
        If classesNode IsNot Nothing Then
            For Each classNode As XmlNode In classesNode.ChildNodes
                Dim name As String = classNode.Attributes("Name").Value
                Dim alt As String = classNode.Attributes("Alt").Value

                If IsMatch(fileName, name, alt) Then
                    Return Integer.Parse(classNode.Attributes("Value").Value)
                End If
            Next
        End If

        Return -1
    End Function

    Private Shared Function IsMatch(fileName As String, className As String, altNames As String) As Boolean
        Dim sanitizedFileName As String = fileName.Replace(" ", "").ToLower()
        Dim sanitizedClassName As String = className.ToLower()
        Dim sanitizedAltNames As String() = altNames.ToLower().Split(","c)

        Return sanitizedFileName.Contains(sanitizedClassName) OrElse sanitizedAltNames.Any(Function(alt) sanitizedFileName.Contains(alt))
    End Function

    Public Function ExtractItems(database As Database, characterId As Integer, selectedProfile As XmlNode) As List(Of InventoryData)

        If selectedProfile IsNot Nothing And characterId > 0 Then
            Dim idsAttribute As String = selectedProfile.SelectSingleNode("Items").Attributes("IDS").Value
            Dim idsArray As String() = idsAttribute.Split(","c)
            Dim validEntries As New List(Of InventoryData)

            For i = 0 To idsArray.Length - 1
                Dim itemId As Integer = Convert.ToInt32(idsArray(i))

                If itemId > 0 Then
                    Dim itemData As ItemData = database.GetItemData(itemId)
                    validEntries.Add(New InventoryData With {.CharacterId = characterId, .SlotId = i, .ItemId = itemId, .SlotName = GetByValue("Slots", "Slot", i + 1), .Name = itemData.Name, .Charges = itemData.Charges, .Color = itemData.Color, .GUID = itemData.GUID})
                End If
            Next

            Return validEntries
        Else
            Console.WriteLine("Selected profile is null.")
            Return Nothing
        End If
    End Function

    Public Sub ImportExistingProfiles()
        Dim folderPath As String = ExePath + "\profiles"

        If Not Directory.Exists(folderPath) Then
            Console.WriteLine("No profile folder found. Skipping import.")
            Return
        End If

        Utility.WriteWrappedLine($"Importing bot equipment profiles to {FILENAME}")

        Dim botFiles As String() = Directory.GetFiles(folderPath, "*.bot")

        Dim dataNode As XmlNode = GetElement("Bots/Imported")
        Dim importCount = 0
        Dim skipCount = 0
        For Each botFile In botFiles
            Dim lines As String() = File.ReadAllLines(botFile)

            Try
                Dim botName As String = Path.GetFileNameWithoutExtension(botFile)
                Dim ids As String = GetCommaSeparatedIds(lines)
                If ids Is String.Empty Then
                    Utility.WriteWrappedLine($"There was an error reading in equipment from {botFile}")
                    Continue For
                End If
                Dim classId As Integer = GuessClassIdByFileName(botName)

                If (IsProfileUnique(GetElement("Bots/Imported"), classId, ids) And IsProfileUnique(GetElement("Bots/PreDefined"), classId, ids)) Then

                    Dim profileNode As XmlNode = XmlData.CreateElement("Profile")
                    profileNode.Attributes.Append(CreateXmlAttribute(XmlData, "Class", classId))
                    profileNode.Attributes.Append(CreateXmlAttribute(XmlData, "Name", botName))

                    Dim itemsNode As XmlNode = XmlData.CreateElement("Items")
                    itemsNode.Attributes.Append(CreateXmlAttribute(XmlData, "IDS", ids))
                    profileNode.AppendChild(itemsNode)

                    dataNode.AppendChild(profileNode)
                    importCount += 1
                Else
                    skipCount += 1
                End If
            Catch ex As Exception
                Console.WriteLine($"Unable to import equipment file {botFile}")
            End Try
        Next

        XmlData.Save(XmlFilePath)
        Console.WriteLine($"Imported {importCount} new bot equipment profiles to {FILENAME}, skipped {skipCount} duplicates.{vbCrLf}")

    End Sub

    Public Function CreateProfileNode(character As CharacterData, ids As String) As XmlNode

        Dim dataNode As XmlNode = GetElement("Bots/Imported")

        Dim profileNode As XmlNode = XmlData.CreateElement("Profile")
        profileNode.Attributes.Append(CreateXmlAttribute(XmlData, "Class", character.ClassId - 1))
        profileNode.Attributes.Append(CreateXmlAttribute(XmlData, "Name", $"{character.Name} - Magelo ID {character.MageloId}"))

        Dim itemsNode As XmlNode = XmlData.CreateElement("Items")
        itemsNode.Attributes.Append(CreateXmlAttribute(XmlData, "IDS", ids))

        profileNode.AppendChild(itemsNode)

        If (IsProfileUnique(GetElement("Bots/Imported"), character.ClassId - 1, ids) And IsProfileUnique(GetElement("Bots/PreDefined"), character.ClassId - 1, ids)) Then
            dataNode.AppendChild(profileNode)
            XmlData.Save(XmlFilePath)
        End If

        Return profileNode
    End Function

    Private Shared Function IsProfileUnique(dataNode As XmlElement, classId As Integer, ids As String) As Boolean
        For Each existingProfile As XmlNode In dataNode.ChildNodes
            Dim existingClass As Integer = Integer.Parse(existingProfile.Attributes("Class").Value)
            Dim existingIds As String = existingProfile.SelectSingleNode("Items").Attributes("IDS").Value

            If existingClass = classId And existingIds = ids Then
                Return False
            End If
        Next

        Return True
    End Function

    Private Shared Function CreateXmlAttribute(xmlDocument As XmlDocument, attributeName As String, attributeValue As String) As XmlAttribute
        Dim attribute As XmlAttribute = xmlDocument.CreateAttribute(attributeName)
        attribute.Value = attributeValue
        Return attribute
    End Function

    Private Shared Function GetCommaSeparatedIds(lines As String()) As String
        Dim ids As New List(Of String)()

        For i As Integer = 0 To lines.Length - 1 Step 3
            ' Assuming every three lines represent a set of data
            If i + 2 < lines.Length Then
                ids.Add(lines(i).Trim())
            End If
        Next
        If (ids.Count = 22) Then
            ids.Add(0) ' Compensate for power source missing from GeorgeS' EQItems bot files
        End If

        If ids.Count <> 23 Then
            Return String.Empty
        End If

        Return String.Join(",", ids)
    End Function

End Class
