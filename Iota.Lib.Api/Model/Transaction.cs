using System;
using System.Numerics;
using Iota.Lib.Utils;
using static Iota.Lib.Utils.Constants;

namespace Iota.Lib.Model
{
    /// <summary>
    /// This class represents an iota transaction
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Transaction"/> class.
        /// </summary>
        public Transaction()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transaction"/> class by providing raw transactions data.
        /// </summary>
        /// <param name="trytes">The raw transaction data.</param>
        /// <exception cref="System.ArgumentException">Is thrown when the provided trytes do not actually represent a transaction.</exception>
        public Transaction(string trytes)
        {
            Kerl kerl = new Kerl();
            if (string.IsNullOrEmpty(trytes))
            {
                throw new ArgumentException("trytes must non-null");
            }

            for (int i = 2279; i < 2295; i++)
            {
                if (trytes[i] != '9')
                {
                    throw new ArgumentException("position " + i + "must not be '9'");
                }
            }

            int[] transactionTrits = Converter.ConvertTrytesToTrits(trytes);
            int[] hash = new int[243];

            kerl.Reset();
            kerl.Absorb(transactionTrits);

            Hash = Converter.ConvertTritsToTrytes(kerl.Squeeze());
            SignatureMessageFragment = trytes.Substring(0, SIGNATURE_MESSAGE_LENGTH);
            Address = trytes.Substring(2187, ADDRESSLENGTH_WITHOUT_CHECKSUM);
            Value = Converter.ConvertTritsToBigInt(ArrayUtils.CreateSubArray(transactionTrits, 6804, 33));
            ObsoleteTag = trytes.Substring(2295, TAG_LENGTH);
            Timestamp = Converter.ConvertTritsToLong(ArrayUtils.CreateSubArray(transactionTrits, 6966, 33));
            CurrentIndex = Converter.ConvertTritsToInteger(ArrayUtils.CreateSubArray(transactionTrits, 6993, 27));
            LastIndex = Converter.ConvertTritsToInteger(ArrayUtils.CreateSubArray(transactionTrits, 7020, 27));
            Bundle = trytes.Substring(2349, BUNDLE_HASH_LENGTH);
            TrunkTransaction = trytes.Substring(2430, TRANSACTION_HASH_LENGTH);
            BranchTransaction = trytes.Substring(2511, TRANSACTION_HASH_LENGTH);
            Tag = trytes.Substring(2592, TAG_LENGTH);
            AttachmentTimestamp = Converter.ConvertTritsToLong(ArrayUtils.CreateSubArray(transactionTrits, 7857, 27));
            AttachmentTimestampLowerBound = Converter.ConvertTritsToLong(ArrayUtils.CreateSubArray(transactionTrits, 7884, 27));
            AttachmentTimestampUpperBound = Converter.ConvertTritsToLong(ArrayUtils.CreateSubArray(transactionTrits, 7911, 27));
            Nonce = trytes.Substring(2646, NONCE_LENGTH);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transaction"/> class
        /// </summary>
        /// <param name="address">The address</param>
        /// <param name="value">The value</param>
        /// <param name="message">The message</param>
        /// <param name="tag">The tag</param>
        public Transaction(string address, BigInteger value, string message = null, string tag = null)
        {
            Address = address;
            Value = value;
            SignatureMessageFragment = message;
            Tag = tag;
            Timestamp = IotaApiUtils.CreateTimeStampNow();
        }

        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        /// <value>A unique hash which is 81-trytes long.</value>
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the signatureMessageFragment.
        /// </summary>
        /// <value>A 2187-trytes long string. In case there is a spent input, the signature of the private key is stored here. If no signature is required,
        /// it is empty (all 9's) and can be used for storing a message.
        /// </value>
        public string SignatureMessageFragment { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>A 81-trytes long string. In case this is an 'output', then this is the address of the recipient.
        /// In case this is an 'input', then this is the address from where the tokens shall be sended from.
        /// </value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value transfered in this transaction.</value>
        public BigInteger Value { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>Timestamp of the transaction. Timestamps are not enforced in iota.</value>
        public long Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the current index.
        /// </summary>
        /// <value>The index this transaction has in its bundle.</value>
        public int CurrentIndex { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The last index of the bundle where this transaction is placed in.</value>
        public int LastIndex { get; set; }

        /// <summary>
        /// Gets or sets the bundle hash.
        /// </summary>
        /// <value>
        /// 81 trytes long string which represents the bundle hash, which is used for grouping transactions of a bundle together.
        /// With the bundle hash you can identify transactions which were in the same bundle.
        /// </value>
        public string Bundle { get; set; }

        /// <summary>
        /// Gets or sets the 'TrunkTransaction'.
        /// </summary>
        /// <value>A 81-trytes string which represents an address</value>
        public string TrunkTransaction { get; set; }

        /// <summary>
        /// Gets or sets the 'BranchTransaction'.
        /// </summary>
        /// <value>A 81-trytes string which represents an address</value>
        public string BranchTransaction { get; set; }

        /// <summary>
        /// Gets or sets the nounce.
        /// </summary>
        /// <value>
        /// A 81-trytes hash. The nounce is required for the transaction to be accepted by the network.
        /// It is generated by doing the proof of work localy or by an IRI via the 'AttachToTangle'-method.
        /// </value>
        public string Nonce { get; set; }

        public string ObsoleteTag { get; set; }
        public string Tag { get; set; }
        public long AttachmentTimestamp { get; set; }
        public long AttachmentTimestampLowerBound { get; set; }
        public long AttachmentTimestampUpperBound { get; set; }

        /// <summary>
        /// Converts the transaction to the corresponding trytes representation
        /// </summary>
        /// <returns></returns>
        public string ToTransactionTrytes()
        {
            int[] valueTrits = Converter.ConvertBigIntToTrits(Value);
            valueTrits = ArrayUtils.PadArrayWithZeros(valueTrits, 81);
            int[] timestampTrits = Converter.ConvertLongToTrits(Timestamp);
            timestampTrits = ArrayUtils.PadArrayWithZeros(timestampTrits, 27);
            int[] currentIndexTrits = Converter.ConvertIntegerToTrits(CurrentIndex);
            currentIndexTrits = ArrayUtils.PadArrayWithZeros(currentIndexTrits, 27);
            int[] lastIndexTrits = Converter.ConvertIntegerToTrits(LastIndex);
            lastIndexTrits = ArrayUtils.PadArrayWithZeros(lastIndexTrits, 27);

            return SignatureMessageFragment
                   + Address
                   + Converter.ConvertTritsToTrytes(valueTrits)
                   + Converter.ConvertTritsToTrytes(timestampTrits)
                   + Converter.ConvertTritsToTrytes(currentIndexTrits)
                   + Converter.ConvertTritsToTrytes(lastIndexTrits)
                   + Bundle
                   + TrunkTransaction
                   + BranchTransaction
                   + Nonce;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Value)}: {Value}, {nameof(Hash)}: {Hash}, {nameof(Address)}: {Address}, {nameof(Timestamp)}: {Timestamp}, {nameof(Bundle)}: {Bundle}, {nameof(TrunkTransaction)}: {TrunkTransaction}, {nameof(BranchTransaction)}: {BranchTransaction}, {nameof(SignatureMessageFragment)}: {SignatureMessageFragment}, {nameof(LastIndex)}: {LastIndex}, {nameof(CurrentIndex)}: {CurrentIndex}, {nameof(Nonce)}: {Nonce}";
        }
    }
}