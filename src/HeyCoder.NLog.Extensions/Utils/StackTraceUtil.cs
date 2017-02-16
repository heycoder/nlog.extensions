using System.Diagnostics;
using System.IO;
using System.Linq;

namespace HeyCoder.NLog.Extensions.Utils
{
    internal class StackTraceUtil
    {
        public static StackTraceInfo GetStackTrace()
        {
            StackTrace st = new StackTrace(1, true);
            var frameList = st.GetFrames()?.ToList();
            if (frameList == null) return new StackTraceInfo();
            if (frameList.Count(f => f.GetFileName() == null) < frameList.Count)
            {
                frameList.RemoveAll(f => f.GetFileName() == null);
            }
            frameList.Reverse();
            var sf = frameList.First();
            var fileNameFull = sf.GetFileName();
            var fileName = fileNameFull == null ? "unknown" : new FileInfo(fileNameFull).Name;
            var sti = new StackTraceInfo
            {
                ClassName = fileName,
                MethodName = sf.GetMethod().Name,
                FileLineNumber = sf.GetFileLineNumber()
            };
            return sti;
        }
    }

    internal class StackTraceInfo
    {
        public string ClassName { get; set; } = "null";

        public string MethodName { get; set; } = "null";

        public int FileLineNumber { get; set; }
    }
}
