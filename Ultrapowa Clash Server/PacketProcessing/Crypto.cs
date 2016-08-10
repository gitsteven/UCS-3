/*
 * Program : Ultrapowa Clash Server
 * Description : A C# Writted 'Clash of Clans' Server Emulator !
 *
 * Authors:  Jean-Baptiste Martin <Ultrapowa at Ultrapowa.com>,
 *           And the Official Ultrapowa Developement Team
 *
 * Copyright (c) 2016  UltraPowa
 * All Rights Reserved.
 */

using System;
using Sodium;

namespace UCS.PacketProcessing
{
    public static class Key
    {
        #region Public Properties

        public static Crypto Crypto
        {
            // We return a keypair , public key + private key
            get { return new Crypto((byte[]) _standardPublicKey.Clone(), (byte[]) _standardPrivateKey.Clone()); }
        }

        #endregion Public Properties

        #region Private Fields

        // We store the standard private key in a byte array
        static readonly byte[] _standardPrivateKey =
        {
            0x36, 0x16, 0x84, 0xC4, 0x73, 0x54, 0x9F, 0xD9, 0x1E, 0x6D, 0x05, 0x9B,
            0x48, 0xB5, 0x90, 0x40, 0x67, 0x1E, 0x58, 0x43, 0xBC, 0x34, 0xCE, 0xD4,
            0xBC, 0x58, 0x7D, 0x82, 0x41, 0xAB, 0x32, 0xD5
        };

        // We store the standard public key in a byte array
        static readonly byte[] _standardPublicKey =
        {
            0x91, 0xD7, 0x42, 0x45, 0x09, 0x99, 0x1A, 0x8C, 0xB4, 0x12, 0x65, 0x26,
            0x32, 0x1D, 0xDB, 0xB5, 0x08, 0x8E, 0x0F, 0x1A, 0x1B, 0x12, 0x0C, 0x17,
            0x33, 0x4B, 0xAE, 0xC3, 0x2D, 0xFD, 0x34, 0x37
        };

        #endregion Private Fields
    }

    // This class is disposable
    public class Crypto : IDisposable
    {
        #region Public Constructors

        public Crypto(byte[] publicKey, byte[] privateKey)
        {
            if (publicKey == null)
                // If the public key is empty, something wrong
                throw new ArgumentNullException(nameof(publicKey));
            if (publicKey.Length != PublicKeyBox.PublicKeyBytes)
                // If the public key length is not 32 bytes length, something wrong
                throw new ArgumentOutOfRangeException(nameof(publicKey), "publicKey must be 32 bytes in length.");

            if (privateKey == null)
                // If private key is empty, something wrong
                throw new ArgumentNullException(nameof(privateKey));
            if (privateKey.Length != PublicKeyBox.SecretKeyBytes)
                // If private key length is not 32 bytes, something wrong
                throw new ArgumentOutOfRangeException(nameof(privateKey), "publicKey must be 32 bytes in length.");

            // We return a keypair
            _keyPair = new KeyPair(publicKey, privateKey);
        }

        #endregion Public Constructors

        #region Public Methods

        // Function for dispose the class
        public void Dispose()
        {
            if (_disposed)
                // If function already disposed, no need to do it again
                return;

            _keyPair.Dispose();
            // We dispose the keypair ( We suppress it from memory )
            _disposed = true;
            // We set the boolean var to true
            GC.SuppressFinalize(this);
            // Garbage Collector is collecting all useless data dropped
        }

        #endregion Public Methods

        #region Public Fields

        // This is the key length ( constant )
        public const int KeyLength = 32;

        // This is the nonce length ( constant )
        public const int NonceLength = 24;

        #endregion Public Fields

        #region Private Fields

        // Storing keypair
        readonly KeyPair _keyPair;

        // Disposed or no, who know ?
        bool _disposed;

        #endregion Private Fields

        #region Public Properties

        // The private key of server
        public byte[] PrivateKey
        {
            get
            {
                if (_disposed)
                    // If the function is already disposed, we can't access to it
                    throw new ObjectDisposedException(null, "Cannot access CoCKeyPair object because it was disposed.");
                // We return the private key of the generated keypair
                return _keyPair.PrivateKey;
            }
        }

        // The public key of server
        public byte[] PublicKey
        {
            get
            {
                if (_disposed)
                    // If the function is already dispoed, we can't access to the key
                    throw new ObjectDisposedException(null, "Cannot access CoCKeyPair object because it was disposed.");

                // We return the public key from the generated keypair
                return _keyPair.PublicKey;
            }
        }

        #endregion Public Properties
    }
}