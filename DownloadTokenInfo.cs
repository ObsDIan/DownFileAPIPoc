namespace DownFileAPIPoc
{
    public class DownloadTokenInfo
    {
        public string Token { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;

        public DateTime ExpireAt { get; set; }

        public bool Used { get; set; }
    }
}
