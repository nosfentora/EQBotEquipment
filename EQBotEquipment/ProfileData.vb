Imports System.Xml

Public Class ProfileData
    Public ReadOnly Property Profile As XmlNode
    Public ReadOnly Property Inventory As List(Of BotInventoryData)

    Public Sub New(_profile As XmlNode, _inventory As List(Of BotInventoryData))
        Profile = _profile
        Inventory = _inventory
    End Sub

End Class
