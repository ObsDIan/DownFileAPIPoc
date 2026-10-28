using Microsoft.AspNetCore.Mvc;

namespace DownFileAPIPoc.Controllers
{
    [ApiController]
    [Route("api/download")]
    public class DownloadController : ControllerBase
    {
        private readonly DownloadTokenService _tokenService;

        public DownloadController(
            DownloadTokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("request")]
        public IActionResult RequestDownload()
        {
            var filePath =
                @"\\172.28.197.20\ShareDownload\test.pdf";

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("檔案不存在");
            }

            var tokenInfo = _tokenService.Create(
                filePath,
                "test.pdf"
            );

            var url = Url.Action(
                nameof(Download),
                "Download",
                new
                {
                    token = tokenInfo.Token
                },
                Request.Scheme
            );

            return Ok(new
            {
                downloadUrl = url,
                expireAt = tokenInfo.ExpireAt
            });
        }
        [HttpGet("{token}")]
        public IActionResult Download(string token)
        {
            var info = _tokenService.Get(token);

            if (info == null)
            {
                return NotFound("下載連結不存在");
            }

            if (info.Used)
            {
                return BadRequest("下載連結已使用");
            }

            if (DateTime.UtcNow > info.ExpireAt)
            {
                return BadRequest("下載連結已過期");
            }

            if (!System.IO.File.Exists(info.FilePath))
            {
                return NotFound("檔案不存在");
            }

            _tokenService.MarkUsed(token);

            var stream = new FileStream(
                info.FilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 64 * 1024,
                useAsync: true
            );

            return File(
                stream,
                "application/octet-stream",
                info.FileName,
                enableRangeProcessing: true
            );
        }

        [HttpGet("diagnostic")]
        public IActionResult Diagnostic()
        {
            var path =
                @"\\172.28.197.20\ShareDownload\test.pdf";

            return Ok(new
            {
                User = Environment.UserName,
                Domain = Environment.UserDomainName,
                Exists = System.IO.File.Exists(path),
                Machine = Environment.MachineName
            });
        }
    }
}
