namespace InstantineAPI.Data
{
    public class EmailObject
    {
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public string DisplayName { get; set; }
        public byte[] QRCode { get; set; }
    }
}
