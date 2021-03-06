﻿using System;
using System.Numerics;
using Iota.Lib.Exception;
using Iota.Lib.Utils;
using static Iota.Lib.Utils.Constants;
using static Iota.Lib.Utils.ArrayUtils;

namespace Iota.Lib.Model
{
    /// <summary>
    /// This class represents an iota transaction
    /// </summary>
    public class Transaction : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Transaction"/> class.
        /// </summary>
        public Transaction()
        {
            SignatureMessageFragment = AdjustTryteString(string.Empty, SIGNATURE_MESSAGE_LENGTH);
            Tag = AdjustTryteString(string.Empty, Constants.TAG_LENGTH);
            ObsoleteTag = AdjustTryteString(string.Empty, Constants.TAG_LENGTH);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transaction"/> class by providing raw transactions data.
        /// </summary>
        /// <param name="trytes">The raw transaction data.</param>
        /// <exception cref="System.ArgumentException">Is thrown when the provided trytes do not actually represent a transaction.</exception>
        public Transaction(string trytes)
        {
            if (string.IsNullOrEmpty(trytes))
            {
                throw new InvalidTryteException();
            }

            if(trytes.Length != RAW_TRANSACTION_LENGTH)
            {
                trytes = AdjustTryteString(trytes, RAW_TRANSACTION_LENGTH);
            }

            Curl curl = new Curl(81);

            int[] transactionTrits = Converter.ConvertTrytesToTrits(trytes);

            curl.Reset();
            int[] hashInTrits = new int[TRANSACTION_HASH_LENGTH * 3];
            curl.Absorb(transactionTrits);
            curl.Squeeze(ref hashInTrits, 0 ,hashInTrits.Length);

            Hash = Converter.ConvertTritsToTrytes(hashInTrits);
            SignatureMessageFragment = trytes.Substring(0, SIGNATURE_MESSAGE_LENGTH);
            Address = trytes.Substring(2187, ADDRESSLENGTH_WITHOUT_CHECKSUM);
            Value = Converter.ConvertTritsToBigInt(ArrayUtils.CreateSubArray(transactionTrits, 6804, 33));
            ObsoleteTag = trytes.Substring(2295, TAG_LENGTH);
            Timestamp = Converter.ConvertTritsToBigInt(ArrayUtils.CreateSubArray(transactionTrits, 6966, 27));
            CurrentIndex = Converter.ConvertTritsToInteger(ArrayUtils.EraseNullValuesFromEnd(ArrayUtils.CreateSubArray(transactionTrits, 6993, 27)));
            LastIndex = Converter.ConvertTritsToInteger(ArrayUtils.EraseNullValuesFromEnd(ArrayUtils.CreateSubArray(transactionTrits, 7020, 27)));
            Bundle = trytes.Substring(2349, BUNDLE_HASH_LENGTH);
            TrunkTransaction = trytes.Substring(2430, TRANSACTION_HASH_LENGTH);
            BranchTransaction = trytes.Substring(2511, TRANSACTION_HASH_LENGTH);
            Tag = trytes.Substring(2592, TAG_LENGTH);
            AttachmentTimestamp = Converter.ConvertTritsToBigInt(ArrayUtils.CreateSubArray(transactionTrits, 7857, 27));
            AttachmentTimestampLowerBound = Converter.ConvertTritsToBigInt(ArrayUtils.CreateSubArray(transactionTrits, 7884, 27));
            AttachmentTimestampUpperBound = Converter.ConvertTritsToBigInt(ArrayUtils.CreateSubArray(transactionTrits, 7911, 27));
            Nonce = trytes.Substring(2646, NONCE_LENGTH);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transaction"/> class
        /// </summary>
        /// <param name="address">The address</param>
        /// <param name="value">The value</param>
        /// <param name="message">The message</param>
        /// <param name="tag">The tag</param>
        /// <param name="keyIndex">The keyindex of the address with which the transaction is beeing created with</param>
        /// <param name="securityLevel">The securityLevel of the address with which the transaction is beeing created with</param>
        public Transaction(string address, BigInteger value, string message = null, string tag = null, int keyIndex = -1, int securityLevel = 0)
        {
            if(!InputValidator.IsValidAddress(address))
            {
                throw new InvalidAddressException($"{address} is not a valid address");
            }
            if(address.Length == ADDRESSLENGTH_WITH_CHECKSUM)
            {
                Address = Checksum.RemoveChecksum(address); 
            }
            else
            {
                Address = address;
            }
            if(value < 0)
            {
                if(keyIndex > -1 && securityLevel > 0)
                {
                    Value = value;
                    KeyIndex = keyIndex;
                    SecurityLevel = securityLevel;
                }
                else
                {
                    throw new InvalidTransactionException();
                } 
            }
            Value = value;
            KeyIndex = keyIndex;
            SecurityLevel = securityLevel;
            if (string.IsNullOrEmpty(message))
            {
                SignatureMessageFragment = AdjustTryteString(string.Empty, SIGNATURE_MESSAGE_LENGTH);
            }
            else
            {
                SignatureMessageFragment = AdjustTryteString(message, SIGNATURE_MESSAGE_LENGTH);
            }
            if (string.IsNullOrEmpty(tag))
            {
                Tag = AdjustTryteString(string.Empty, TAG_LENGTH);
            }
            else
            {
                Tag = AdjustTryteString(tag, TAG_LENGTH);
            }

            ObsoleteTag = AdjustTryteString(string.Empty, TAG_LENGTH);
            Timestamp = IotaApiUtils.CreateTimeStampNow();
            Bundle = AdjustTryteString(string.Empty, BUNDLE_HASH_LENGTH);
            BranchTransaction = AdjustTryteString(string.Empty, TRANSACTION_HASH_LENGTH);
            TrunkTransaction = AdjustTryteString(string.Empty, TRANSACTION_HASH_LENGTH);
            Nonce = AdjustTryteString(string.Empty, NONCE_LENGTH);
        }

        /// <summary>
        /// A 81-tryte long string that represents the transaction hash
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// A 2187-tryte long string. In case there is a spent input, the signature of the private key is stored here. If no signature is required,
        /// it is empty (all 9's) and can be used for storing a message
        /// </summary>
        public string SignatureMessageFragment { get; set; }

        /// <summary>
        /// A 81-tryte long string. In case this is an 'output', then this is the address of the recipient.
        /// In case this is an 'input', then this is the address from where the tokens shall be sended from
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The value transfered in this transaction.
        /// </summary>
        public BigInteger Value { get; set; }

        /// <summary>
        /// Timestamp of the transaction
        /// </summary>
        public BigInteger Timestamp { get; set; }

        /// <summary>
        /// The key index of the address with which the transaction is beeing created with. Just used internal for proper signing
        /// </summary>
        public int KeyIndex { get; set; }

        /// <summary>
        /// The security level of the address with which the transaction is beeing created with. Just used internal for proper signing
        /// </summary>
        public int SecurityLevel { get; set; }

        /// <summary>
        /// States if the transaction is persistent
        /// </summary>
        public bool Persistance { get; set; }

        /// <summary>
        /// The index this transaction has in its bundle
        /// </summary>
        public int CurrentIndex { get; set; }

        /// <summary>
        /// The last index of the bundle where this transaction is placed in
        /// </summary>
        public int LastIndex { get; set; }

        /// <summary>
        /// a 81-tryte long string which represents the bundle hash
        /// </summary>
        public string Bundle { get; set; }

        /// <summary>
        /// A 81-tryte string which represents a transaction hash
        /// </summary>
        public string TrunkTransaction { get; set; }

        /// <summary>
        /// A 81-tryte string which represents a transaction hash
        /// </summary>
        public string BranchTransaction { get; set; }

        /// <summary>
        /// A 27-tryte long string. The nounce is required for the transaction to be accepted by the network.
        /// </summary>
        public string Nonce { get; set; }
        
        /// <summary>
        /// (Will be removed soon!) The obsulete tag is a nine tryte long string that can be used for various reasons 
        /// </summary>
        public string ObsoleteTag { get; set; }

        /// <summary>
        /// A nine tryte long string that can be used for various reasons
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Placeholder for later Iota implementations
        /// </summary>
        public BigInteger AttachmentTimestamp { get; set; }

        /// <summary>
        /// The timestamp before the proof-of-work
        /// </summary>
        public BigInteger AttachmentTimestampLowerBound { get; set; }

        /// <summary>
        /// The timestamp after the proof-of-work
        /// </summary>
        public BigInteger AttachmentTimestampUpperBound { get; set; }

        /// <summary>
        /// Converts the transaction to the corresponding trytes representation
        /// </summary>
        /// <returns>The raw transaction as 2673-tryte long string</returns>
        public string ToTransactionTrytes()
        {
            int[] valueTrits = Converter.ConvertBigIntToTrits(Value);
            valueTrits = ArrayUtils.PadArrayWithZeros(valueTrits, 81);
            int[] timestampTrits = Converter.ConvertBigIntToTrits(Timestamp);
            timestampTrits = ArrayUtils.PadArrayWithZeros(timestampTrits, 27);
            int[] currentIndexTrits = Converter.ConvertIntegerToTrits(CurrentIndex);
            currentIndexTrits = ArrayUtils.PadArrayWithZeros(currentIndexTrits, 27);
            int[] lastIndexTrits = Converter.ConvertIntegerToTrits(LastIndex);
            lastIndexTrits = ArrayUtils.PadArrayWithZeros(lastIndexTrits, 27);
            int[] attachmentTimestampTrits = Converter.ConvertBigIntToTrits(AttachmentTimestamp);
            attachmentTimestampTrits = ArrayUtils.PadArrayWithZeros(attachmentTimestampTrits, 27);
            int[] attachmentTimestampLowerBoundTrits = Converter.ConvertBigIntToTrits(AttachmentTimestampLowerBound);
            attachmentTimestampLowerBoundTrits = ArrayUtils.PadArrayWithZeros(attachmentTimestampLowerBoundTrits, 27);
            int[] attachmentTimestampUpperBoundTrits = Converter.ConvertBigIntToTrits(AttachmentTimestampUpperBound);
            attachmentTimestampUpperBoundTrits = ArrayUtils.PadArrayWithZeros(attachmentTimestampUpperBoundTrits, 27);

            string trytes = AdjustTryteString(SignatureMessageFragment, SIGNATURE_MESSAGE_LENGTH);
            trytes += AdjustTryteString(Address, ADDRESSLENGTH_WITHOUT_CHECKSUM);
            trytes += Converter.ConvertTritsToTrytes(valueTrits);
            trytes += AdjustTryteString(ObsoleteTag, TAG_LENGTH);
            trytes += Converter.ConvertTritsToTrytes(timestampTrits).Substring(0, 9);
            trytes += Converter.ConvertTritsToTrytes(currentIndexTrits).Substring(0, 9);
            trytes += Converter.ConvertTritsToTrytes(lastIndexTrits).Substring(0, 9);
            trytes += AdjustTryteString(Bundle, BUNDLE_HASH_LENGTH);
            trytes += AdjustTryteString(TrunkTransaction, TRANSACTION_HASH_LENGTH);
            trytes += AdjustTryteString(BranchTransaction, TRANSACTION_HASH_LENGTH);
            trytes += AdjustTryteString(Tag, TAG_LENGTH);
            trytes += Converter.ConvertTritsToTrytes(attachmentTimestampTrits).Substring(0, 9);
            trytes += Converter.ConvertTritsToTrytes(attachmentTimestampLowerBoundTrits).Substring(0, 9);
            trytes += Converter.ConvertTritsToTrytes(attachmentTimestampUpperBoundTrits).Substring(0, 9);
            trytes += AdjustTryteString(Nonce, NONCE_LENGTH);
            return trytes;
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

        /// <summary>
        /// Clones the transaction
        /// </summary>
        /// <returns>The clone</returns>
        public Object Clone()
        {
            Transaction clone = new Transaction
            {
                Address = this.Address,
                ObsoleteTag = this.ObsoleteTag,
                Timestamp = this.Timestamp,
                Tag = this.Tag,
                BranchTransaction = this.BranchTransaction,
                Bundle = this.Bundle,
                TrunkTransaction = this.TrunkTransaction,
                Nonce = this.Nonce
            };
            return clone;
        }
    }
}