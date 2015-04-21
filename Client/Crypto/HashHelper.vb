Imports CryptoNS = System.Security.Cryptography
Imports HashAlgo = System.Security.Cryptography.HashAlgorithm
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.IO
Namespace Crypto

    Enum HashAlgorithm
        SHA1
    End Enum

    Module HashHelper
        Public Delegate Function HashFunction(data As Byte()()) As Byte()
        Public HashFunctions As Dictionary(Of HashAlgorithm, HashFunction)
        Public HashAlgorithms As Dictionary(Of HashAlgorithm, HashAlgo)

        Sub New()
            HashFunctions = New Dictionary(Of HashAlgorithm, HashFunction)()

            HashFunctions(HashAlgorithm.SHA1) = AddressOf SHA1
        End Sub

        Private Function Combine(buffers As Byte()()) As Byte()
            Dim length As Integer = 0
            For Each buffer__1 As Byte() In buffers
                length += buffer__1.Length
            Next

            Dim result As Byte() = New Byte(length - 1) {}

            Dim position As Integer = 0

            For Each buffer__1 As Byte() In buffers
                Buffer.BlockCopy(buffer__1, 0, result, position, buffer__1.Length)
                position += buffer__1.Length
            Next

            Return result
        End Function

        <System.Runtime.CompilerServices.Extension> _
        Public Function Hash(algorithm As HashAlgorithm, ParamArray data As Byte()()) As Byte()
            Return HashFunctions(algorithm)(data)
        End Function

        Private Function SHA1(ParamArray data As Byte()()) As Byte()
            Using alg As System.Security.Cryptography.SHA1 = CryptoNS.SHA1.Create()
                Return alg.ComputeHash(Combine(data))
            End Using
        End Function
    End Module
End Namespace