using System.Collections.Concurrent;

namespace DownFileAPIPoc
{
    public class DownloadTokenService
    {
        private readonly ConcurrentDictionary<string, DownloadTokenInfo> _tokens
            = new();

        public DownloadTokenInfo Create(
            string filePath,
            string fileName)
        {
            var token = Guid.NewGuid().ToString("N");

            var info = new DownloadTokenInfo
            {
                Token = token,
                FilePath = filePath,
                FileName = fileName,
                ExpireAt = DateTime.UtcNow.AddMinutes(10),
                Used = false
            };

            _tokens[token] = info;

            return info;
        }

        public DownloadTokenInfo? Get(string token)
        {
            if (!_tokens.TryGetValue(token, out var info))
                return null;

            return info;
        }

        public void MarkUsed(string token)
        {
            if (_tokens.TryGetValue(token, out var info))
            {
                info.Used = true;
            }
        }
    }
}

