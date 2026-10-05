using System.Text.Json;
namespace ASP_Lesson_14.Extentsions
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };
            string str = JsonSerializer.Serialize(value, options);

            session.SetString(key, str);
        }
        public static T? Get<T>(this ISession session, string key)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };

            string? str = session.GetString(key);
            T? res = default; 
            if (str != null)
               res = JsonSerializer.Deserialize<T>(str, options);
            return res;
        }
    }
}
