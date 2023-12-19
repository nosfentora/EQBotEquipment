Public Class Accounts

    Public Property AccountData As List(Of AccountData)
    Private _currentPage As Integer = 10

    Private Const PageSize = 1

    Public Function NumberOfAccounts() As Integer
        Return AccountData.Count
    End Function

    Public ReadOnly Property CurrentPage As Integer
        Get
            Return _currentPage
        End Get
    End Property

    Public ReadOnly Property PageSizeValue As Integer
        Get
            Return PageSize
        End Get
    End Property

    Public ReadOnly Property PageItems As List(Of AccountData)
        Get
            Dim startIndex = (CurrentPage - 1) * PageSize
            Return AccountData.Skip(startIndex).Take(PageSize).ToList()
        End Get
    End Property

    Public ReadOnly Property HasMore As Boolean
        Get
            Return (currentPage * PageSize) < AccountData.Count
        End Get
    End Property

    Public ReadOnly Property HasFewer As Boolean
        Get
            Return currentPage > 1
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
