using Microsoft.Extensions.Options;

using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.Exceptions;

using ProyectTemplate.Utils.Options;

namespace ProyectTemplate.Services.MinioService
{
    public class MinioBlobServices:IBlobServices
    {
        private readonly string _bucketName;
        private readonly IMinioClient _minioClient;
        private readonly string _endpoint;


        public MinioBlobServices(IOptions<MinioOptions> options)
        {
            _bucketName = options.Value.Bucket;
            _endpoint = options.Value.Endpoint;
            _minioClient = new MinioClient()
                .WithEndpoint(options.Value.Endpoint)
                .WithCredentials(options.Value.AccessKey, options.Value.SecretKey)
                .WithSSL(true)
                .Build();
        }
        private async Task CreateBucketIfNotExistsAsync(CancellationToken ct)
        {
            if (!await BucketExistsAsync(ct))
            {
                var makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(_bucketName);
                await _minioClient.MakeBucketAsync(makeBucketArgs, ct);
            }
        }

        private async Task<bool> BucketExistsAsync(CancellationToken ct)
        {
            try
            {
                var buckets = await _minioClient.ListBucketsAsync(ct);
                // Access the Buckets property of ListAllMyBucketsResult to use LINQ's Any method
                return buckets.Buckets.Any(b => b.Name == _bucketName);
            }
            catch (BucketNotFoundException)
            {
                return false;
            }
        }
        public async Task<string> PresignedGetUrl(string objPath, CancellationToken ct)
        {
            var args = new PresignedGetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objPath)
                .WithExpiry(60 * 5);
            return await _minioClient.PresignedGetObjectAsync(args);
        }
        public async Task<string> UploadBlob(IFormFile file, string? previousUrl, CancellationToken ct)
        {
            // Validar el archivo
            if (file == null || file.Length == 0)
                throw new ArgumentException("Archivo inválido");

            // Validar extensiones permitidas
            var ext = Path.GetExtension(file.FileName).ToLower();
            var allowedExtensions = new[] { ".jpg", ".png", ".gif" };
            if (!allowedExtensions.Contains(ext))
                throw new InvalidOperationException("Extensión de archivo no permitida");

            await CreateBucketIfNotExistsAsync(ct);

            // Generar un ID único para el archivo
            var fileId = Guid.NewGuid().ToString();
            var objectPath = $"images/{fileId}{ext}";

            // Eliminar el archivo anterior si existe
            if (!string.IsNullOrEmpty(previousUrl))
            {
                await DeleteBlob(previousUrl, ct);
            }

            // Subir el nuevo archivo
            await using var fileStream = file.OpenReadStream();
            var uploadArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectPath)
                .WithStreamData(fileStream)
                .WithObjectSize(file.Length)  // Usar file.Length en lugar de fileStream.Length
                .WithContentType(file.ContentType);

            var res = await _minioClient.PutObjectAsync(uploadArgs, ct);
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(res));
            return objectPath;
        }

        public Task<string> UploadBlobByUrl(string fileUrl, string? previousUrl, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteBlob(string url, CancellationToken ct)
        {
            var args = new RemoveObjectArgs()
               .WithBucket(_bucketName)
               .WithObject(url);

            await _minioClient.RemoveObjectAsync(args, ct);
        }

        public async Task<bool> ValidateBlobExistance(string url, CancellationToken ct)
        {
            try
            {
                var statObjectArgs = new StatObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(url);

                await _minioClient.StatObjectAsync(statObjectArgs, ct);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ValidateConnection(CancellationToken ct = default)
        {
            try
            {
                Console.WriteLine("🔄 Validando conexión a MinIO...");

                // Listar todos los buckets para validar la conexión
                var buckets = await _minioClient.ListBucketsAsync(ct);

                Console.WriteLine($"✅ Conexión a MinIO exitosa");
                Console.WriteLine($"📦 Buckets disponibles: {buckets.Buckets.Count}");

                foreach (var bucket in buckets.Buckets)
                {
                    Console.WriteLine($"   - {bucket.Name} (Creado: {bucket.CreationDate})");
                }

                // Verificar si el bucket configurado existe
                var targetBucketExists = buckets.Buckets.Any(b => b.Name == _bucketName);
                if (targetBucketExists)
                {
                    Console.WriteLine($"✅ Bucket configurado '{_bucketName}' encontrado");
                }
                else
                {
                    Console.WriteLine($"⚠️ Bucket configurado '{_bucketName}' NO encontrado");
                    Console.WriteLine($"   Buckets disponibles: {string.Join(", ", buckets.Buckets.Select(b => b.Name))}");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error conectando a MinIO: {ex.Message}");
                Console.WriteLine($"   Endpoint: {_minioClient.Config.Endpoint}");
                Console.WriteLine($"   Bucket configurado: {_bucketName}");
                return false;
            }
        }

    }
}
