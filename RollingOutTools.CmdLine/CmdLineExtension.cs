using RollingOutTools.Json;
using RollingOutTools.Storage;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace RollingOutTools.CmdLine
{
    /// <summary>
    /// Класс-адаптер для класса Console. Добавляет некоторые крутые фичи и еще я его использую для абстракции от консоли, на случай если захочу сделать веб версию консоли.
    /// </summary>
    public static class CmdLineExtension
    {
        /// <summary>
        /// Для сложных методов, типа считывания сложных типов через json editor вы можете отключить исключения. Тогда, к примеру, если пользователь допустит 
        /// ошибку при редактировании json файла - ему выведут сообщение, но исключение не завершит работу программы.
        /// </summary>
        public static bool ThrowConsoleParseExeptions { get; } = false;
        const string jsonEditorFilePath = "json_editor_buf.json";

        public static void WriteLineColored(object objToWrite, ConsoleColor cc, bool prettyJson = false)
        {
            var current = Console.ForegroundColor;
            Console.ForegroundColor = cc;
            WriteLine(objToWrite, prettyJson);
            Console.ForegroundColor = current;
        }

        public static void WriteColored(object objToWrite, ConsoleColor cc, bool prettyJson = false)
        {
            var current = Console.ForegroundColor;
            Console.ForegroundColor = cc;
            Write(objToWrite, prettyJson);
            Console.ForegroundColor = current;
        }

        /// <summary>
        /// Отличается от стандартного метода Console тем, что если передаваемый объект не IConvertible, то он будет сериализирован в json строку.
        /// </summary>
        public static void WriteLine(object objToWrite, bool prettyJson=false)
        {
            Console.WriteLine(
                JsonSerializeHelper.Inst.ToConvertibleOrJson(objToWrite, new JsonSerializeOptions() { WithNormalFormating = prettyJson })
                );
        }

        /// <summary>
        /// Отличается от стандартного метода Console тем, что если передаваемый объект не IConvertible, то он будет сериализирован в json строку.
        /// </summary>
        public static void Write(object objToWrite, bool prettyJson = false)
        {
            Console.Write(
               JsonSerializeHelper.Inst.ToConvertibleOrJson(objToWrite, new JsonSerializeOptions() { WithNormalFormating = prettyJson })
                );
        }

        public static void WriteLine()
        {
            Console.WriteLine();
        }

        public static string ReadLine()
        {
            return Console.ReadLine();
        }

        /// <summary>
        /// Запрашивайте данные от пользователя ТОЛЬКО ЧЕРЕЗ ЭТОТ МЕТОД.
        /// Если запрашиваемый тип реализует IConvertible, то он будет запрошен через консоль. Иначе - пользователю дадут отредактировать Json файл.
        /// <para></para>
        /// Еще этот метод может кешировать значение по имени ресурса, используя его можно с легкостью реализовать астозаполнение и даже автоматическое тестирование.
        /// </summary>
        public static T ReadResource<T>(string resourceName, ReadResourseOptions options = null)
        {
            return (T)readResource(typeof(T), 10, resourceName, options, default(T));
        }

        /// <summary>
        /// Запрашивайте данные от пользователя ТОЛЬКО ЧЕРЕЗ ЭТОТ МЕТОД.
        /// Если запрашиваемый тип реализует IConvertible, то он будет запрошен через консоль. Иначе - пользователю дадут отредактировать Json файл.
        /// <para></para>
        /// Еще этот метод может кешировать значение по имени ресурса, используя его можно с легкостью реализовать астозаполнение и даже автоматическое тестирование.
        /// </summary>
        public static object ReadResource(Type typeOfResource, string resourceName, ReadResourseOptions options = null)
        {            
            return readResource(typeOfResource, 10, resourceName, options, null);
        }

        static object readResource(Type objectType, int tryesCount, string resourceName, ReadResourseOptions options, object defaultValue)
        {
            if (options == null)
                options = new ReadResourseOptions();
            string hint = options.Hint;
            var longResName = resourceName + "";
            try
            {
                CmdLineExtension.WriteLineColored($"Resource '{longResName}' with type {objectType} requested from console.", ConsoleColor.Yellow);
                if (!string.IsNullOrWhiteSpace(hint))
                    CmdLineExtension.WriteLineColored($"Hint: {hint}", ConsoleColor.Yellow);

                string cachedValueString = Storage_HardDrive.Get<string>(longResName).Result;

                if (typeof(IConvertible).IsAssignableFrom(objectType))
                {
                    return IfResourceIsIConvertible(objectType,longResName, cachedValueString, options);
                }
                else
                {
                    return IfResourceIsDifficult(objectType,longResName, cachedValueString, defaultValue, options);
                }

            }
            catch (Exception ex)
            {
                if (ThrowConsoleParseExeptions || tryesCount < 0)
                {
                    throw;
                }
                else
                {
                    CmdLineExtension.WriteLineColored("Exeption in console resource receiving method: ", ConsoleColor.DarkRed);
                    CmdLineExtension.WriteLine(("\t" + ex.Message).Replace("\n", "\n\t"));

                    //try again
                    return readResource(objectType, tryesCount - 1, resourceName, options, defaultValue);
                }
            }
        }

        static object IfResourceIsDifficult(Type objectType, string longResName, string cachedValueString, object defaultValue, ReadResourseOptions options)
        {
            //Else, will be converted by json.
            //
            CmdLineExtension.WriteColored(
                $"Difficult type. Will be opened in json editor. ",
                ConsoleColor.Yellow
                );            

            object cachedValue;
            try
            {
                if (defaultValue == null)
                {
                    cachedValue = JsonSerializeHelper.Inst.FromJson(objectType,cachedValueString);
                }
                else
                {
                    cachedValue = defaultValue;
                }
            }
            catch
            {
                ConstructorInfo ctor = objectType.GetConstructor(new Type[] { });
                cachedValue = ctor.Invoke(new object[] { });
            }

            //Если автоматическое считывание, то возвращаем закешированное значение.
            if (options.UseAutoread)
            {
                return cachedValue;
            }

            CmdLineExtension.ReadLine();
            File.WriteAllText(
                jsonEditorFilePath, 
                JsonSerializeHelper.Inst.ToJson(
                    objectType, 
                    cachedValue, 
                    new JsonSerializeOptions() { WithNormalFormating = true }
                    )
                );
            bool isAccept;
            do
            {
                Process editorProcess = Process.Start(Environment.CurrentDirectory + "\\" + jsonEditorFilePath);
                editorProcess.WaitForExit();
                CmdLineExtension.WriteColored("Accept changes? Press y/n (y): ", ConsoleColor.Yellow);
                isAccept = CmdLineExtension.ReadLine().Trim().StartsWith("n");
            } while (isAccept);
            string editedJson = File.ReadAllText(jsonEditorFilePath);

            object res = JsonSerializeHelper.Inst.FromJson(objectType,editedJson);

            //Convert again to normal parse json.
            if(options.SaveToCache)
                Storage_HardDrive.Set(longResName, JsonSerializeHelper.Inst.ToJson(objectType, res));
            return res;
            //
            //Else, will be converted by json.
        }

        static object IfResourceIsIConvertible(Type objectType, string longResName,string cachedValueString,ReadResourseOptions options)
        {
            //If IConvertible
            //
            string cachedValue = cachedValueString;
            var cachedValueInHint = cachedValue ?? "";
            if (cachedValueInHint.Length > 80)
            {
                cachedValueInHint = cachedValueInHint.Substring(0, 80) + "... ";
            }

            CmdLineExtension.WriteColored(
                $"Input ({cachedValueInHint}): ",
                ConsoleColor.Yellow
                );

            //Если автоматическое считывание, то возвращаем закешированное значение.
            string val = "";
            if (!options.UseAutoread)
            {
                val= CmdLineExtension.ReadLine();
            }

            
            if (val=="" && cachedValue != null)
            {
                val = cachedValue;
            }
            else
            {
                if (options.SaveToCache)
                    Storage_HardDrive.Set(longResName, val);
            }
            return Convert.ChangeType(val, objectType);
            //
            //If IConvertible
        }

        static object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }
    }
}
