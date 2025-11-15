using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DotNetEnv;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using ZEN.Domain.Interfaces;

namespace ZEN.Infrastructure.Integrations.CloudStorage
{
    public class SavePhotoToCloud : ISavePhotoToCloud
    {
        private readonly Cloudinary _cloudinary;
        private readonly string _cloudName;
        private readonly string _apiKey;
        private readonly string _apiSecret;

        public SavePhotoToCloud()
        {
            // Try to load .env file for local development (will fail silently if not found)
            try
            {
                Env.Load();
                Env.TraversePath().Load();
            }
            catch
            {
                // Ignore if .env file not found (e.g., in production)
            }

            // Read from environment variables (works for both local .env and production)
            _cloudName = Environment.GetEnvironmentVariable("CLOUDNAME") ?? Env.GetString("CLOUDNAME", "");
            _apiKey = Environment.GetEnvironmentVariable("APIKEY") ?? Env.GetString("APIKEY", "");
            _apiSecret = Environment.GetEnvironmentVariable("APISECRET") ?? Env.GetString("APISECRET", "");

            // Trim whitespace
            _cloudName = _cloudName?.Trim() ?? "";
            _apiKey = _apiKey?.Trim() ?? "";
            _apiSecret = _apiSecret?.Trim() ?? "";

            // Log detailed info
            Console.WriteLine($"[Cloudinary] Loading credentials...");
            Console.WriteLine($"[Cloudinary] CLOUDNAME from env: {Environment.GetEnvironmentVariable("CLOUDNAME") != null}");
            Console.WriteLine($"[Cloudinary] APIKEY from env: {Environment.GetEnvironmentVariable("APIKEY") != null}");
            Console.WriteLine($"[Cloudinary] APISECRET from env: {Environment.GetEnvironmentVariable("APISECRET") != null}");
            Console.WriteLine($"[Cloudinary] CLOUDNAME length: {_cloudName.Length}");
            Console.WriteLine($"[Cloudinary] APIKEY length: {_apiKey.Length}");
            Console.WriteLine($"[Cloudinary] APISECRET length: {_apiSecret.Length}");

            if (string.IsNullOrWhiteSpace(_cloudName) || string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_apiSecret))
            {
                Console.WriteLine("[Cloudinary] ERROR: Cloudinary credentials not found or empty. Image upload will fail.");
                Console.WriteLine($"[Cloudinary] CLOUDNAME: {(!string.IsNullOrEmpty(_cloudName) ? $"SET (length: {_cloudName.Length})" : "NOT SET")}");
                Console.WriteLine($"[Cloudinary] APIKEY: {(!string.IsNullOrEmpty(_apiKey) ? $"SET (length: {_apiKey.Length})" : "NOT SET")}");
                Console.WriteLine($"[Cloudinary] APISECRET: {(!string.IsNullOrEmpty(_apiSecret) ? $"SET (length: {_apiSecret.Length})" : "NOT SET")}");
                throw new InvalidOperationException("Cloudinary credentials are not configured. Please set CLOUDNAME, APIKEY, and APISECRET environment variables.");
            }
            else
            {
                Console.WriteLine("[Cloudinary] Credentials loaded successfully");
            }

            var acc = new Account(_cloudName, _apiKey, _apiSecret);
            _cloudinary = new Cloudinary(acc);
            Console.WriteLine("[Cloudinary] Cloudinary client initialized");
        }

        public async Task<bool> DeletePhotoAsync(string? img_url)
        {
            if (string.IsNullOrWhiteSpace(img_url))
                return false;

            var uri = new Uri(img_url);
            var segments = uri.Segments;

            // Tìm vị trí upload/
            var uploadIndex = Array.FindIndex(segments, s => s.Trim('/').Equals("upload", StringComparison.OrdinalIgnoreCase));
            if (uploadIndex == -1)
                throw new Exception("Invalid Cloudinary URL format");

            // Lấy phần sau upload/ và sau version vxxxx
            var publicIdSegments = segments.Skip(uploadIndex + 2) // bỏ cả version vxxxx/
                                            .Select(s => Uri.UnescapeDataString(s.Trim('/'))) // giải mã các kí tự encode như %20 thành khoảng trắng
                                            .ToArray();

            var fullFileName = string.Join("/", publicIdSegments); // vì có thể có folder
            var extensionIndex = fullFileName.LastIndexOf('.');
            if (extensionIndex >= 0)
            {
                fullFileName = fullFileName.Substring(0, extensionIndex); // bỏ phần mở rộng
            }

            var deleteParams = new DeletionParams(fullFileName);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok";
        }

        public async Task<string> UploadPhotoAsync(Stream fileStream, string fileName)
        {
            using (var memoryStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var compressedStream = await CompressImageAsync(memoryStream);

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(fileName, compressedStream),
                    PublicId = fileName,
                    Overwrite = true
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    var errorMsg = $"Failed to upload image. Status: {uploadResult.StatusCode}, Error: {uploadResult.Error?.Message ?? "Unknown error"}";
                    Console.WriteLine($"[Cloudinary] {errorMsg}");
                    throw new Exception(errorMsg);
                }

                Console.WriteLine($"[Cloudinary] Image uploaded successfully: {uploadResult.SecureUrl.AbsoluteUri}");
                return uploadResult.SecureUrl.AbsoluteUri;
            }
        }

        private async Task<Stream> CompressImageAsync(Stream inputStream)
        {
            inputStream.Position = 0;

            using var image = await Image.LoadAsync(inputStream);
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new SixLabors.ImageSharp.Size(1024, 1024)
            }));

            var outputStream = new MemoryStream();
            await image.SaveAsJpegAsync(outputStream, new JpegEncoder
            {
                Quality = 75
            });

            outputStream.Position = 0;
            return outputStream;
        }
    }
}
