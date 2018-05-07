using RollingOutTools.Json;
using System;
using System.IO;
using System.Linq;

namespace RollingOutTools.Common
{
    public static class CommonHelpers
    {
        /// <summary>
        /// Вернет истину, если пароль соответствует всем правилам.
        /// </summary>
        public static bool CheckPass(string pass, out string msg)
        {
            if (TextExtensions.IsLatinOrNumber(pass))
            {
                msg = "Normal";
                return true;
            }
            else
            {
                msg = "Password must contain only a-z,A-Z,0-9 chars.";
                return false;
            }

        }

        /// <summary>
        /// Вернет истину, если ник соответствует всем правилам.
        /// </summary>
        public static bool CheckNick(string nick, out string msg)
        {
            if (TextExtensions.IsLatinOrNumber(nick))
            {
                msg = "Normal";
                return true;
            }
            else
            {
                msg = "Login must contain only a-z,A-Z,0-9 chars.";
                return false;
            }

        }

        /// <summary>
        /// Глубокое копирование с использованием json серилизации.
        /// Использовать ОЧЕНЬ осторожно, а лучше вообще никогда.
        /// </summary>
        public static T DeepCopy<T>(object obj)
        {
            return JsonSerializeHelper.Inst.FromJson<T>(
                JsonSerializeHelper.Inst.ToJson(obj)
            );
        }

        public static bool TryReadAllTextFromStream(Stream stream, out string res)
        {
            try
            {
                res=ReadAllTextFromStream(stream);
                return true;
            }
            catch
            {
                res = null;
                return false;
            }
        }

        public static string ReadAllTextFromStream(Stream stream)
        {
            using (StreamReader streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();

            }
        }
    }
}
