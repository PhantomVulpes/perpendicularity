namespace Vulpes.Perpendicularity.Api.Services;

/// <summary>
/// A FileStream wrapper that deletes the underlying file when the stream is disposed.
/// Useful for temporary files that need to be cleaned up after being sent in an HTTP response.
/// </summary>
public class TempFileStream : FileStream
{
    private readonly string filePath;

    public TempFileStream(string path, FileMode mode, FileAccess access, FileShare share)
        : base(path, mode, access, share)
    {
        filePath = path;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
                // Silently fail if we can't delete the temp file
                // It will be cleaned up by the OS eventually
            }
        }
    }
}
