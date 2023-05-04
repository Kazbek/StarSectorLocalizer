using Microsoft.JSInterop;

namespace StarSectorWebTools.Services
{
    public class FileSaverService
    {
        private readonly IJSRuntime _jsRuntime;
        public FileSaverService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task SaveAsFileAsync(string fileName, Stream fileStream)
        {
            using var streamRef = new DotNetStreamReference(stream: fileStream);
            await _jsRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        }
    }
}
