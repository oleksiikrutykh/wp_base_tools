
namespace BaseTools.Core.Security
{
#if SILVERLIGHT
    using System.Security.Cryptography;
#endif

#if WINRT
    using Windows.Storage.Streams;
    using Windows.Security.Cryptography;
    using Windows.Security.Cryptography.Core;
#endif
    using BaseTools.Core.Serialization;
    using System;
    using System.Collections.Generic;
    using System.IO;

    using System.Text;
    using BaseTools.Core.Info;
    using BaseTools.Core.Ioc;
    using BaseTools.Core.Storage;
    using BaseTools.Core.IO;
    using System.Runtime.InteropServices.WindowsRuntime;

    /// <summary>
    /// Provides a managed implementation of the Advanced Encryption Standard (AES) symmetric algorithm.
    /// </summary>
    public static class AesCryptoProvider
    {

#if SILVERLIGHT
        public static Stream CreateEncryptStream(Stream inputStream, byte[] key, byte[] iv)
        {
            var aesProvider = new AesManaged();
            var cryptoTransform = aesProvider.CreateEncryptor(key, iv);
            var cryptoStream = new CryptoStream(inputStream, cryptoTransform, CryptoStreamMode.Write);
            return cryptoStream;
        }

        public static Stream CreateDecryptStream(Stream encryptedStream, byte[] key, byte[] iv)
        {
            var aesProvider = new AesManaged();
            var cryptoTransform = aesProvider.CreateDecryptor(key, iv);
            var cryptoStream = new CryptoStream(encryptedStream, cryptoTransform, CryptoStreamMode.Read);
            return cryptoStream;
        }

#endif

#if WINRT
        public static Stream CreateEncryptStream(Stream inputStream, byte[] key, byte[] iv)
        {
            IBuffer pwBuffer = CryptographicBuffer.CreateFromByteArray(key);
            IBuffer saltBuffer = CryptographicBuffer.CreateFromByteArray(iv);

            // Derive key material for password size 32 bytes for AES256 algorithm
            KeyDerivationAlgorithmProvider keyDerivationProvider = KeyDerivationAlgorithmProvider.OpenAlgorithm("PBKDF2_SHA1");
            KeyDerivationParameters pbkdf2Parms = KeyDerivationParameters.BuildForPbkdf2(saltBuffer, 1000);

            // create a key based on original key and derivation parmaters
            CryptographicKey keyOriginal = keyDerivationProvider.CreateKey(pwBuffer);
            IBuffer keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Parms, 32);
            CryptographicKey derivedPwKey = keyDerivationProvider.CreateKey(pwBuffer);

            // derive buffer to be used for encryption salt from derived password key 
            IBuffer saltMaterial = CryptographicEngine.DeriveKeyMaterial(derivedPwKey, pbkdf2Parms, 16);

            // display the keys - because KeyDerivationProvider always gets cleared after each use, they are very similar unforunately
            string keyMaterialString = CryptographicBuffer.EncodeToBase64String(keyMaterial);
            string saltMaterialString = CryptographicBuffer.EncodeToBase64String(saltMaterial);

            SymmetricKeyAlgorithmProvider symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");
            // create symmetric key from derived password material
            CryptographicKey symmKey = symProvider.CreateSymmetricKey(keyMaterial);

            var memoryStream = new MemoryStream();
            var stream = new NotificationStream(memoryStream);
            stream.Disposing += (s, e) =>
            {
                var content = memoryStream.ToArray();
                var input = CryptographicBuffer.CreateFromByteArray(content);
                IBuffer resultBuffer = CryptographicEngine.Encrypt(symmKey, input, saltMaterial);
                var encryptedBytes = new byte[resultBuffer.Length];
                resultBuffer.CopyTo(encryptedBytes);
                inputStream.Write(encryptedBytes, 0, encryptedBytes.Length);
            };

            return stream;
        }

        public static Stream CreateDecryptStream(Stream encryptedStream, byte[] key, byte[] iv)
        {
            IBuffer pwBuffer = CryptographicBuffer.CreateFromByteArray(key);
            IBuffer saltBuffer = CryptographicBuffer.CreateFromByteArray(iv);
            byte[] streamData = new byte[encryptedStream.Length - encryptedStream.Position];
            encryptedStream.Read(streamData, 0, streamData.Length);
            IBuffer cipherBuffer = CryptographicBuffer.CreateFromByteArray(streamData);

            // Derive key material for password size 32 bytes for AES256 algorithm
            KeyDerivationAlgorithmProvider keyDerivationProvider = KeyDerivationAlgorithmProvider.OpenAlgorithm("PBKDF2_SHA1");
            KeyDerivationParameters pbkdf2Parms = KeyDerivationParameters.BuildForPbkdf2(saltBuffer, 1000);

            // create a key based on original key and derivation parmaters
            CryptographicKey keyOriginal = keyDerivationProvider.CreateKey(pwBuffer);
            IBuffer keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Parms, 32);
            CryptographicKey derivedPwKey = keyDerivationProvider.CreateKey(pwBuffer);

            // derive buffer to be used for encryption salt from derived password key 
            IBuffer saltMaterial = CryptographicEngine.DeriveKeyMaterial(derivedPwKey, pbkdf2Parms, 16);

            // display the keys - because KeyDerivationProvider always gets cleared after each use, they are very similar unforunately
            string keyMaterialString = CryptographicBuffer.EncodeToBase64String(keyMaterial);
            string saltMaterialString = CryptographicBuffer.EncodeToBase64String(saltMaterial);

            SymmetricKeyAlgorithmProvider symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");
            // create symmetric key from derived password material
            CryptographicKey symmKey = symProvider.CreateSymmetricKey(keyMaterial);

            // encrypt data buffer using symmetric key and derived salt material
            IBuffer resultBuffer = CryptographicEngine.Decrypt(symmKey, cipherBuffer, saltMaterial);
            return resultBuffer.AsStream();
        }
#endif

    }

    
}
