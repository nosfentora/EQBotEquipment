Public Class CharacterData
    Private ReadOnly XmlData As XMLData

    Public Sub New(_xmlData As XMLData)
        XmlData = _xmlData
    End Sub

    Public Property Id As Integer
    Public Property AccountId As Integer
    Public Property Name As String
    Public Property ClassId As Integer
    Public ReadOnly Property ClassName As String
        Get
            Return XMLData.GetByValue("Classes", "Class", ClassId)
        End Get
    End Property
    Public Property RaceId As Integer
    Public ReadOnly Property RaceName As String
        Get
            Return XmlData.GetByValue("Races", "Race", RaceId)
        End Get
    End Property
    Public Property IsBot As Boolean
    Public ReadOnly Property Type As String
        Get
            Return If(IsBot, "Bot", "Character")
        End Get
    End Property
    Public Property MageloId As String
End Class