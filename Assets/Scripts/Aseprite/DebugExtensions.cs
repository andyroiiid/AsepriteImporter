using System.Text;

namespace Aseprite
{
    public static class DebugExtensions
    {
        public static string DebugFieldsToString<T>(this T obj)
        {
            var sb = new StringBuilder();
            foreach (var field in obj.GetType().GetFields())
                sb.AppendFormat("{0} = {1}\n", field.Name, field.GetValue(obj));
            return sb.ToString();
        }
    }
}