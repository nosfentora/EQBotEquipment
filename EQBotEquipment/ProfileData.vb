Imports System.Xml

Public Class ProfileData
    Public ReadOnly Property Profile As XmlNode
    Public ReadOnly Property Inventory As List(Of InventoryData)

    Public Sub New(_profile As XmlNode, _inventory As List(Of InventoryData))
        Profile = _profile
        Inventory = _inventory
    End Sub

End Class
