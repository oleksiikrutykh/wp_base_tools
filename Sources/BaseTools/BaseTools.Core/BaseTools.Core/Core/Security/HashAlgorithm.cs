namespace BaseTools.Core.Security
{
    using BaseTools.Core.Utility;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Extensions for array of bytes. Used for code compatibility.
    /// </summary>
    internal static class ByteArrayExtension
    {
        public static byte[] Clone(this byte[] array)
        {
            Guard.CheckIsNotNull(array, "array");
            var copiedArray = new byte[array.Length];
            array.CopyTo(copiedArray, 0);
            return copiedArray;
        }
    }

    /// <summary>
    /// Base class for hash algorithmes.
    /// This class was ported from Silverlight4 mscorlib library.
    /// </summary>
    internal abstract class HashAlgorithm : IDisposable
    {
        protected int HashSizeValue { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "This is ported implementation logic")]
        protected internal byte[] HashValue { get; set; }

        private bool m_bDisposed;

        protected int State { get; set; }



        // Methods

        protected HashAlgorithm()
        {

        }



        public void Clear()
        {

            this.Dispose();

        }



        public byte[] ComputeHash(byte[] buffer)
        {

            if (this.m_bDisposed)
            {

                throw new ObjectDisposedException(null);

            }

            if (buffer == null)
            {

                throw new ArgumentNullException("buffer");

            }

            this.HashCore(buffer, 0, buffer.Length);

            this.HashValue = this.HashFinal();

            byte[] buffer2 = (byte[])this.HashValue.Clone();

            this.Initialize();

            return buffer2;

        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This is ported logic")]
        public byte[] ComputeHash(System.IO.Stream inputStream)
        {

            int num;

            if (this.m_bDisposed)
            {

                throw new ObjectDisposedException(null);

            }

            byte[] buffer = new byte[0x1000];

            do
            {

                num = inputStream.Read(buffer, 0, 0x1000);

                if (num > 0)
                {

                    this.HashCore(buffer, 0, num);

                }

            }

            while (num > 0);

            this.HashValue = this.HashFinal();



            byte[] buffer2 = (byte[])this.HashValue.Clone();

            this.Initialize();

            return buffer2;

        }



        public byte[] ComputeHash(byte[] buffer, int offset, int count)
        {

            if (buffer == null)
            {

                throw new ArgumentNullException("buffer");

            }

            if (offset < 0)
            {

                throw new ArgumentOutOfRangeException("offset", "ArgumentOutOfRange_NeedNonNegNum");

            }

            if ((count < 0) || (count > buffer.Length))
            {

                throw new ArgumentException("Argument_InvalidValue");

            }

            if ((buffer.Length - count) < offset)
            {

                throw new ArgumentException("Argument_InvalidOffLen");

            }

            if (this.m_bDisposed)
            {

                throw new ObjectDisposedException(null);

            }

            this.HashCore(buffer, offset, count);

            this.HashValue = this.HashFinal();

            byte[] buffer2 = (byte[])this.HashValue.Clone();

            this.Initialize();

            return buffer2;

        }



        public static HashAlgorithm Create()
        {

            return Create("System.Security.Cryptography.HashAlgorithm");

        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "hashName", Justification = "This member not fully ported.")]
        public static HashAlgorithm Create(string hashName)
        {

            throw new NotImplementedException();

        }



        public void Dispose()
        {

            this.Dispose(true);

            GC.SuppressFinalize(this);

        }

        protected virtual void Dispose(bool disposing)
        {

            if (disposing)
            {

                if (this.HashValue != null)
                {

                    Array.Clear(this.HashValue, 0, this.HashValue.Length);

                }

                this.HashValue = null;

                this.m_bDisposed = true;

            }

        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "cb", Justification = "This is ported logic")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "ib", Justification = "This is ported logic")]
        protected abstract void HashCore(byte[] array, int ibStart, int cbSize);

        protected abstract byte[] HashFinal();

        public abstract void Initialize();



        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {

            if (inputBuffer == null)
            {

                throw new ArgumentNullException("inputBuffer");

            }

            if (inputOffset < 0)
            {

                throw new ArgumentOutOfRangeException("inputOffset", "ArgumentOutOfRange_NeedNonNegNum");

            }

            if ((inputCount < 0) || (inputCount > inputBuffer.Length))
            {

                throw new ArgumentException("Argument_InvalidValue");

            }

            if ((inputBuffer.Length - inputCount) < inputOffset)
            {

                throw new ArgumentException("Argument_InvalidOffLen");

            }

            if (this.m_bDisposed)
            {

                throw new ObjectDisposedException(null);

            }

            this.State = 1;

            this.HashCore(inputBuffer, inputOffset, inputCount);

            if ((outputBuffer != null) && ((inputBuffer != outputBuffer) || (inputOffset != outputOffset)))
            {
                throw new NotImplementedException();

                //Buffer.BlockCopy(inputBuffer, inputOffset, outputBuffer, outputOffset, inputCount);

            }

            return inputCount;

        }



        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (inputBuffer == null)
            {

                throw new ArgumentNullException("inputBuffer");

            }

            if (inputOffset < 0)
            {

                throw new ArgumentOutOfRangeException("inputOffset", "ArgumentOutOfRange_NeedNonNegNum");

            }

            if ((inputCount < 0) || (inputCount > inputBuffer.Length))
            {

                throw new ArgumentException("Argument_InvalidValue");

            }

            if ((inputBuffer.Length - inputCount) < inputOffset)
            {

                throw new ArgumentException("Argument_InvalidOffLen");

            }

            if (this.m_bDisposed)
            {

                throw new ObjectDisposedException(null);

            }

            this.HashCore(inputBuffer, inputOffset, inputCount);

            this.HashValue = this.HashFinal();

            byte[] dst = new byte[inputCount];

            if (inputCount != 0)
            {
                throw new NotImplementedException();
                //Buffer.InternalBlockCopy(inputBuffer, inputOffset, dst, 0, inputCount);

            }

            this.State = 0;

            return dst;

        }



        // Properties

        public virtual bool CanReuseTransform
        {

            get
            {

                return true;

            }

        }



        public virtual bool CanTransformMultipleBlocks
        {

            get
            {

                return true;

            }

        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "This is ported logic")]
        public virtual byte[] Hash
        {

            get
            {

                if (this.m_bDisposed)
                {

                    throw new ObjectDisposedException(null);

                }

                if (this.State != 0)
                {

                    throw new InvalidOperationException("Cryptographic unexpected operation exception - cryptography hash not yet finalized");

                }

                var copiedArray = new byte[this.HashValue.Length];
                this.HashValue.CopyTo(copiedArray, 0);
                return copiedArray;
            }
        }



        public virtual int HashSize
        {

            get
            {

                return this.HashSizeValue;

            }

        }



        public virtual int InputBlockSize
        {

            get
            {

                return 1;

            }

        }



        public virtual int OutputBlockSize
        {

            get
            {

                return 1;

            }

        }



    }
}
