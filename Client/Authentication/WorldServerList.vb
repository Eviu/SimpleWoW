Imports System.Collections
Imports System.Collections.Generic
Imports System.IO

Namespace Authentication
    Public Class WorldServerList
        Implements IEnumerable(Of WorldServerInfo)
        Public Property Count() As Integer
            Get
                Return m_Count
            End Get
            Private Set(value As Integer)
                m_Count = Value
            End Set
        End Property
        Private m_Count As Integer
        Private serverList As WorldServerInfo()

        Public Sub New(reader As BinaryReader)
            reader.ReadUInt32()

            Count = reader.ReadUInt16()
            serverList = New WorldServerInfo(Count - 1) {}

            For i As Integer = 0 To Count - 1
                serverList(i) = New WorldServerInfo(reader)
            Next
        End Sub

        Default Public ReadOnly Property Item(index As Integer) As WorldServerInfo
            Get
                Return serverList(index)
            End Get
        End Property

#Region "IEnumerable<WorldServerInfo> Members"

        Public Function GetEnumerator() As IEnumerator(Of WorldServerInfo) Implements IEnumerable(Of WorldServerInfo).GetEnumerator
            For Each server As WorldServerInfo In serverList
                Return New List(Of WorldServerInfo) From {server}
            Next
            Return Nothing
        End Function

#End Region

#Region "IEnumerable Members"

        Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            For Each realm As WorldServerInfo In serverList
                Return New List(Of WorldServerInfo) From {realm}
            Next
            Return Nothing
        End Function
#End Region
    End Class
End Namespace
