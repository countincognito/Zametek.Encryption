using System;
using System.Runtime.Serialization;

namespace Zametek.Utility.Encryption
{
    [DataContract]
    public class EncryptionContext
    {
        private static readonly object s_Lock = new object();

        static EncryptionContext()
        {
            FullName = typeof(EncryptionContext).FullName;
        }

        public EncryptionContext(Guid symmetricKeyId)
        {
            SymmetricKeyId = symmetricKeyId;
        }

        public static string FullName
        {
            get;
        }

        public static EncryptionContext Current
        {
            get
            {
                lock (s_Lock)
                {
                    return AmbientContext.GetData<EncryptionContext>();
                }
            }
            private set
            {
                lock (s_Lock)
                {
                    AmbientContext.SetData(value);
                }
            }
        }

        [DataMember]
        public Guid SymmetricKeyId
        {
            get;
        }

        public static void NewCurrentIfEmpty(Guid symmetricKeyId)
        {
            lock (s_Lock)
            {
                EncryptionContext tc = Current;
                if (tc == null)
                {
                    NewCurrent(symmetricKeyId);
                }
            }
        }

        public static byte[] Serialize(EncryptionContext encryptionContext)
        {
            if (encryptionContext == null)
            {
                throw new ArgumentNullException(nameof(encryptionContext));
            }

            return encryptionContext.ObjectToByteArray();
        }

        public static EncryptionContext DeSerialize(byte[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            return array.ByteArrayToObject<EncryptionContext>();
        }

        /// <summary>
        /// Clears the current EncryptionContext. Use with caution.
        /// </summary>
        public static void ClearCurrent()
        {
            lock (s_Lock)
            {
                AmbientContext.Clear<EncryptionContext>();
            }
        }

        /// <summary>
        /// Sets the instance to the current EncryptionContext. Use with caution.
        /// </summary>
        public void SetAsCurrent()
        {
            lock (s_Lock)
            {
                Current = this;
            }
        }

        /// <summary>
        /// Replaces the current EncryptionContext. Use with caution.
        /// </summary>
        /// <param name="symmetricKeyId">SymmetricKeyId to be included in the new current EncryptionContext.</param>
        public static void NewCurrent(Guid symmetricKeyId)
        {
            lock (s_Lock)
            {
                Current = new EncryptionContext(symmetricKeyId);
            }
        }
    }
}
