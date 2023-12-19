Public Class BotData
    Private ReadOnly XmlData As XMLData

    Public Sub New(_xmlData As XMLData)
        Me.XmlData = _xmlData
    End Sub

    Public Property BotId As Integer
    Public Property BotName As String
    Public Property BotClass As Integer
    Public ReadOnly Property BotClassName As String
        Get
            Return XmlData.GetByValue("Classes", "Class", BotClass)
        End Get
    End Property
    Public Property BotRace As Integer
    Public ReadOnly Property BotRaceName As String
        Get
            Return XmlData.GetByValue("Races", "Race", BotRace)
        End Get
    End Property


End Class
