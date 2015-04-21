
Imports Client.Crypto
Imports HMACSHA1 = System.Security.Cryptography.HMACSHA1

Namespace World.Network
    Enum AuthStatus
        Uninitialized
        Pending
        Ready
    End Enum

    NotInheritable Class AuthenticationCrypto
        Private Sub New()
        End Sub
        Private Shared ReadOnly encryptionKey As Byte() = New Byte() {&HC2, &HB3, &H72, &H3C, &HC6, &HAE, _
            &HD9, &HB5, &H34, &H3C, &H53, &HEE, _
            &H2F, &H43, &H67, &HCE}
        Private Shared ReadOnly decryptionKey As Byte() = New Byte() {&HCC, &H98, &HAE, &H4, &HE8, &H97, _
            &HEA, &HCA, &H12, &HDD, &HC0, &H93, _
            &H42, &H91, &H53, &H57}

        Shared ReadOnly outputHMAC As New HMACSHA1(encryptionKey)
        Shared ReadOnly inputHMAC As New HMACSHA1(decryptionKey)

        Shared encryptionStream As ARC4
        Shared decryptionStream As ARC4

        Public Shared Property Status() As AuthStatus
            Get
                Return m_Status
            End Get
            Private Set(value As AuthStatus)
                m_Status = Value
            End Set
        End Property
        Private Shared m_Status As AuthStatus

        Shared Sub New()
            Status = AuthStatus.Uninitialized
        End Sub

        <Obsolete("NYI", True)> _
        Public Shared Sub Pending()
            Status = AuthStatus.Pending
        End Sub

        Public Shared Sub Initialize(sessionKey As Byte())
            ' create RC4-drop[1024] stream
            encryptionStream = New ARC4(outputHMAC.ComputeHash(sessionKey))
            encryptionStream.Process(New Byte(1023) {}, 0, 1024)

            ' create RC4-drop[1024] stream
            decryptionStream = New ARC4(inputHMAC.ComputeHash(sessionKey))
            decryptionStream.Process(New Byte(1023) {}, 0, 1024)

            Status = AuthStatus.Ready
        End Sub

        Public Shared Sub Decrypt(data As Byte(), start As Integer, count As Integer)
            If Status = AuthStatus.Ready Then
                decryptionStream.Process(data, start, count)
            End If
        End Sub

        Public Shared Sub Encrypt(data As Byte(), start As Integer, count As Integer)
            If Status = AuthStatus.Ready Then
                encryptionStream.Process(data, start, count)
            End If
        End Sub
    End Class
End Namespace
