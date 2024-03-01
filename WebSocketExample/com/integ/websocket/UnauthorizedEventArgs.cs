namespace com.integpg.websocket
{
    public class UnauthorizedEventArgs
    {
        // The nonce that is used to build the auth digest
        public string Nonce { get; set; }



        public UnauthorizedEventArgs(string nonce)
        {
            Nonce = nonce;
        }
    }
}
