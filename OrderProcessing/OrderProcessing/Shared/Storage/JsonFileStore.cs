
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using OrderProcessing.Shared.Exceptions;

namespace OrderProcessing.Shared.Storage;

public class JsonFileStore
{
    public List<T> Read<T>(string fileName)
    {
        string path = GetPath(fileName);

if (!File.Exists(path))
            return new List<T>();

        string json = File.ReadAllText(path);

        try
        {
            return JsonSerializer.Deserialize<List<T>>(json, JsonOptions)
                   ?? new List<T>();
        }
        catch (JsonException ex)
        {
            throw new DataFileCorruptedException(fileName, ex);
        }


}

    public void Write<T>(string fileName, List<T> data)
    {
        string path = GetPath(fileName);
        SemaphoreSlim fileLock = GetFileLock(path);

        fileLock.Wait();

        try
        {
            string json = JsonSerializer.Serialize(data, JsonOptions);

            string tempPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";

            try
            {
                File.WriteAllText(tempPath, json, Encoding.UTF8);
                File.Move(tempPath, path, true);
            }
            finally
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
        }
        finally
        {
            fileLock.Release();
        }
    }

    private readonly string _dataDirectory;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private static readonly ConcurrentDictionary<string, SemaphoreSlim> FileLocks = new();

    public JsonFileStore(string dataDirectory)
    {
        _dataDirectory = dataDirectory;
        Directory.CreateDirectory(_dataDirectory);
    }

    public async Task<List<T>> ReadAsync<T>(
        string fileName,
        CancellationToken ct)
    {
        string path = GetPath(fileName);
        SemaphoreSlim fileLock = GetFileLock(path);

        await fileLock.WaitAsync(ct);

        try
        {
            return await ReadFileAsync<T>(path, fileName, ct);
        }
        finally
        {
            fileLock.Release();
        }
    }

    public async Task WriteAsync<T>(
        string fileName,
        List<T> data,
        CancellationToken ct)
    {
        string path = GetPath(fileName);
        SemaphoreSlim fileLock = GetFileLock(path);

        await fileLock.WaitAsync(ct);

        try
        {
            await WriteFileAsync(path, data, ct);
        }
        finally
        {
            fileLock.Release();
        }
    }

    public async Task UpdateAsync<T>(
        string fileName,
        Action<List<T>> updateAction,
        CancellationToken ct)
    {
        string path = GetPath(fileName);
        SemaphoreSlim fileLock = GetFileLock(path);

        await fileLock.WaitAsync(ct);

        try
        {
            List<T> data = await ReadFileAsync<T>(path, fileName, ct);

            updateAction(data);

            await WriteFileAsync(path, data, ct);
        }
        finally
        {
            fileLock.Release();
        }
    }

    private string GetPath(string fileName)
    {
        return Path.GetFullPath(
            Path.Combine(_dataDirectory, fileName));
    }

    private static SemaphoreSlim GetFileLock(string path)
    {
        return FileLocks.GetOrAdd(
            path,
            _ => new SemaphoreSlim(1, 1));
    }

    private async Task<List<T>> ReadFileAsync<T>(
        string path,
        string fileName,
        CancellationToken ct)
    {
        if (!File.Exists(path))
            return new List<T>();

        string json = await File.ReadAllTextAsync(path, ct);

        try
        {
            return JsonSerializer.Deserialize<List<T>>(json, JsonOptions)
                   ?? new List<T>();
        }
        catch (JsonException ex)
        {
            throw new DataFileCorruptedException(fileName, ex);
        }
    }

    private async Task WriteFileAsync<T>(
        string path,
        List<T> data,
        CancellationToken ct)
    {
        string tempPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";

        try
        {
            string json = JsonSerializer.Serialize(data, JsonOptions);

            await File.WriteAllTextAsync(
                tempPath,
                json,
                Encoding.UTF8,
                ct);

            File.Move(tempPath, path, true);
        }
        finally
        {
            try
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
            catch
            {
                
            }
        }
    }
}

