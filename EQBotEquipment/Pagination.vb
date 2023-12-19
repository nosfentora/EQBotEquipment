Public Class Pagination

    Private Const PageSize = 10
    Private ReadOnly MyData As List(Of Object)
    Private _currentPage As Integer = 1

    Public Sub New(myData As List(Of Object))
        Me.MyData = myData
    End Sub

    Public Function NumberOfItems() As Integer
        Return Me.MyData.Count
    End Function

    Public Function PageItemsCount() As Integer
        Return PageItems.Count
    End Function

    Public ReadOnly Property CurrentPage As Integer
        Get
            Return _currentPage
        End Get
    End Property

    Public ReadOnly Property AllItems As List(Of Object)
        Get
            Return MyData
        End Get
    End Property
    Public ReadOnly Property PageItems As List(Of Object)
        Get
            Dim startIndex = (CurrentPage - 1) * PageSize
            Return MyData.Skip(startIndex).Take(PageSize).ToList()
        End Get
    End Property

    Public ReadOnly Property HasMore As Boolean
        Get
            Return (CurrentPage * PageSize) < Me.MyData.Count
        End Get
    End Property

    Public ReadOnly Property HasFewer As Boolean
        Get
            Return CurrentPage > 1
        End Get
    End Property

    Public Sub MoveToNextPage()
        If HasMore Then
            _currentPage += 1
        End If
    End Sub

    Public Sub MoveToPreviousPage()
        If HasFewer Then
            _currentPage -= 1
        End If
    End Sub
End Class
