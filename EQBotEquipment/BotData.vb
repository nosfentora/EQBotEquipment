Public Class BotData

    Public Sub New(_xmlData As XMLData)
        Me.XmlData = _xmlData
    End Sub

    Public Property BotId As Integer
    Public Property BotName As String
    Public Property BotClass As Integer
    Public Property BotClassName As String
        Get
            Return XmlData.GetByValue("Classes", "Class", BotClass)
        End Get
        Set(value As String)

        End Set
    End Property
    Public Property BotRace As Integer
    Public Property BotRaceName As String
        Get
            Return XmlData.GetByValue("Races", "Race", BotRace)
        End Get
        Set(value As String)

        End Set
    End Property

    Private ReadOnly Property XmlData As XMLData
End Class
