Public Class Utility

    Public Shared Sub WriteWrappedLine(message As String, Optional addNewline As Boolean = False)
        Dim modifiedMessage As String = $"* {message} *"
        Dim asteriskLine As New String("*", modifiedMessage.Length)
        Console.WriteLine(asteriskLine)
        Console.WriteLine(modifiedMessage)
        Console.WriteLine(asteriskLine)
        If addNewline Then
            Console.WriteLine()
        End If
    End Sub
End Class
