using Newtonsoft.Json.Linq;

namespace Altera.Properties
{
    internal class ReadClassName
    {
        public static string ReadClassOriginName(int ClassID)
        {
            foreach (var mstClasstmp in GlobalPathsAndDatas.mstClassArray)
                if (((JObject)mstClasstmp)["id"].ToString() == ClassID.ToString())
                {
                    return ((JObject)mstClasstmp)["name"].ToString();
                }
            return "";
        }
    }
}
