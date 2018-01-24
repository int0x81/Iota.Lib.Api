﻿using System;
using Iota.Lib.CSharp.Api.Utils;

namespace Iota.Lib.CSharp.Api.Model
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
        /// Initializes a new instance of the <see cref="Transaction"/> class.
        /// </summary>
        /// <param name="trytes">The trytes representing the transaction</param>
        /// <param name="curl">The curl implementation.</param>
        /// <exception cref="System.ArgumentException">
        /// trytes must non-null
        /// or
        /// position " + i + "must not be '9'
        /// </exception>
        public Transaction(string trytes, ISponge curl)
        {
            if (string.IsNullOrEmpty(trytes))
            {
                throw new ArgumentException("trytes must non-null");
            }

            // validity check
            for (int i = 2279; i < 2295; i++)
            {
                if (trytes[i] != '9')
                {
                    throw new ArgumentException("position " + i + "must not be '9'");
                }
            }

            int[] transactionTrits = Converter.ConvertTrytesToTrits(trytes);
            int[] hash = new int[243];

            // generate the correct transaction hash
            hash = curl.Reset()
                   .Absorb(transactionTrits)
                   .Squeeze(Curl.HASH_LENGTH);

            Hash = Converter.ConvertTritsToTrytes(hash);
            SignatureFragment = trytes.Substring(0, 2187);
            Address = trytes.Substring(2187, 2268 - 2187);
            Value = "" + Converter.ConvertTritsToLong(ArrayUtils.SubArray(transactionTrits, 6804, 6837));
            Tag = trytes.Substring(2295, 2322 - 2295);
            Timestamp = "" + Converter.ConvertTritsToLong(ArrayUtils.SubArray(transactionTrits, 6966, 6993));
            CurrentIndex = "" + Converter.ConvertTritsToLong(ArrayUtils.SubArray(transactionTrits, 6993, 7020));
            LastIndex = "" + Converter.ConvertTritsToLong(ArrayUtils.SubArray(transactionTrits, 7020, 7047));
            Bundle = trytes.Substring(2349, 2430 - 2349);
            TrunkTransaction = trytes.Substring(2430, 2511 - 2430);
            BranchTransaction = trytes.Substring(2511, 2592 - 2511);
            Nonce = trytes.Substring(2592, 2673 - 2592);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transaction"/> class.
        /// </summary>
        /// <param name="trytes">The trytes representing the transaction</param>
        public Transaction(string trytes) : this(trytes, new Curl())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transaction"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="value">The value.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="timestamp">The timestamp.</param>
        public Transaction(string address, string value, string tag, string timestamp)
        {
            Address = address;
            Value = value;
            Tag = tag;
            Timestamp = timestamp;
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        /// <value>
        /// The hash.
        /// </value>
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the signature message chunk.
        /// </summary>
        /// <value>
        /// The signature message chunk.
        /// </value>
        public string SignatureMessageChunk { get; set; }

        /// <summary>
        /// Gets or sets the digest.
        /// </summary>
        /// <value>
        /// The digest.
        /// </value>
        public string Digest { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public string Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the bundle.
        /// </summary>
        /// <value>
        /// The bundle.
        /// </value>
        public string Bundle { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the trunk transaction.
        /// </summary>
        /// <value>
        /// The trunk transaction.
        /// </value>
        public string TrunkTransaction { get; set; }

        /// <summary>
        /// Gets or sets the branch transaction.
        /// </summary>
        /// <value>
        /// The branch transaction.
        /// </value>
        public string BranchTransaction { get; set; }

        /// <summary>
        /// Gets or sets the signature fragment.
        /// </summary>
        /// <value>
        /// The signature fragment.
        /// </value>
        public string SignatureFragment { get; set; }

        /// <summary>
        /// Gets or sets the last index.
        /// </summary>
        /// <value>
        /// The last index.
        /// </value>
        public string LastIndex { get; set; }

        /// <summary>
        /// Gets or sets the index of the current.
        /// </summary>
        /// <value>
        /// The index of the current.
        /// </value>
        public string CurrentIndex { get; set; }

        /// <summary>
        /// Gets or sets the nonce.
        /// </summary>
        /// <value>
        /// The nonce.
        /// </value>
        public string Nonce { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Transaction"/> is persistance.
        /// </summary>
        /// <value>
        ///   <c>true</c> if persistance; otherwise, <c>false</c>.
        /// </value>
        public bool Persistance { get; set; }

        /// <summary>
        /// Converts the transaction to the corresponding trytes representation
        /// </summary>
        /// <returns></returns>
        public string ToTransactionTrytes()
        {
            int[] valueTrits = Converter.ConvertTrytesToTrits(Value);
            valueTrits = ArrayUtils.PadArrayWithZeros(valueTrits, 81);
            int[] timestampTrits = Converter.ConvertTrytesToTrits(Timestamp);
            timestampTrits = ArrayUtils.PadArrayWithZeros(timestampTrits, 27);
            int[] currentIndexTrits = Converter.ConvertTrytesToTrits(CurrentIndex);
            currentIndexTrits = ArrayUtils.PadArrayWithZeros(currentIndexTrits, 27);
            int[] lastIndexTrits = Converter.ConvertTrytesToTrits(LastIndex);
            lastIndexTrits = ArrayUtils.PadArrayWithZeros(lastIndexTrits, 27);

            return SignatureFragment
                   + Address
                   + Converter.ConvertTritsToTrytes(valueTrits)
                   + Tag
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
            return $"{nameof(Value)}: {Value}, {nameof(Persistance)}: {Value}, {nameof(Tag)}: {Tag}, {nameof(Hash)}: {Hash}, {nameof(Type)}: {Type}, {nameof(SignatureMessageChunk)}: {SignatureMessageChunk}, {nameof(Digest)}: {Digest}, {nameof(Address)}: {Address}, {nameof(Timestamp)}: {Timestamp}, {nameof(Bundle)}: {Bundle}, {nameof(Index)}: {Index}, {nameof(TrunkTransaction)}: {TrunkTransaction}, {nameof(BranchTransaction)}: {BranchTransaction}, {nameof(SignatureFragment)}: {SignatureFragment}, {nameof(LastIndex)}: {LastIndex}, {nameof(CurrentIndex)}: {CurrentIndex}, {nameof(Nonce)}: {Nonce}";
        }
    }
}