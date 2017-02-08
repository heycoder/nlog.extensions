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

            var frameList = st.GetFrames().ToList();
            frameList.RemoveAll(f => f.GetFileName() == null);
            var sf = frameList.Skip(frameList.Count - 1).Take(1).First();
            var sti = new StackTraceInfo
            {
                ClassName = new FileInfo(sf.GetFileName()).Name,
                MethodName = sf.GetMethod().Name,
                FileLineNumber = sf.GetFileLineNumber()
            };
            return sti;
        }
    }

    internal class StackTraceInfo
    {
        public string ClassName { get; set; }

        public string MethodName { get; set; }

        public int FileLineNumber { get; set; }
    }
}
