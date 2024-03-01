﻿using System;

namespace Integpg.JniorWebSocket
{
    public class UnauthorizedEventArgs : EventArgs
    {
        // The nonce that is used to build the auth digest
        public string Nonce { get; set; }



        public UnauthorizedEventArgs(string nonce)
        {
            Nonce = nonce;
        }
    }
}
