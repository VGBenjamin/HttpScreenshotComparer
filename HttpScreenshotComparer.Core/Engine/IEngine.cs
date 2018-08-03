using HttpScreenshotComparer.Core.Configuration;

namespace HttpScreenshotComparer.Core.Engine
{
    public interface IEngine
    {
        void Run(ExecutionOptions options);
    }
}