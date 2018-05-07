using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using RollingOutTools.Storage;

namespace RollingOutTools.CmdLine
{
    /// <summary>
    /// Базовый класс для генерации cli.
    /// Можно писать методы с входными параметрами (массив строк).
    /// Вместо Console обязательно используйте CmdLineExtension.
    /// </summary>
    public class CommandLineBase
    {
        protected readonly ICmdSwitcher CurrentCmdSwitcher;
        bool _isAutorunEnabled = false;
        bool _isInRun = true;

        /// <summary>
        /// Показывает открыта ли сейчас эта консоль.
        /// </summary>
        public bool IsInRun {
            get { return _isInRun; }
            private set
            {
                if(!_isInRun)
                    OnQuit?.Invoke();
                _isInRun = value;

            }
        } 

        public Dictionary<string, MethodInfo> CmdNameAndMethod { get; }
        public event Action OnQuit;

        /// <summary>
        /// Используйте это свойство в своих командах, чтоб предложить следующую команду.
        /// </summary>
        public string LastCmdName
        {
            get
            {
                return Storage_HardDrive.Get<string>(this.GetType().Name+".last_cmd").Result;
            }
            set
            {
                Storage_HardDrive.Set(this.GetType().Name + ".last_cmd", value);
            }
        }

        public CommandLineBase(ICmdSwitcher cmdSwitcher)
        {
            CurrentCmdSwitcher = cmdSwitcher;
            CmdNameAndMethod = CreateReflectionDict();
        }


        public virtual void OnStart()
        {
            CmdLineExtension.WriteLineColored(
                $"You have been opened command line '{this.GetType().Name}'. Write 'help' to open commands list.",
                ConsoleColor.Magenta
                );
        }

        [CmdInfo(CmdName = "help")]
        public void HelpCmd()
        {
            StringBuilder res = new StringBuilder("\tВы можете передавать параметры в метод, разделяя из через '/'.\n" +
                "\tИспользуйте параметр /auto для автоматического выполнения команд.\n\n");
            foreach (MethodInfo item in this.GetType().GetMethods())
            {
                CmdInfoAttribute attr = item.GetCustomAttribute(typeof(CmdInfoAttribute)) as CmdInfoAttribute;
                if (attr != null && attr.CmdName != "help")
                {
                    string newStr = "\t" + attr.CmdName + " - "+ item.Name + "(";
                    bool isFirst = true;

                    foreach (var parameter in item.GetParameters())
                    {
                        if (!isFirst)
                        {
                            newStr += ", ";
                        }
                        isFirst = false;
                        newStr += parameter.ParameterType.Name + "  " + parameter.Name;
                    }
                    newStr += ");";

                    if (attr.Description != null)
                    {
                        newStr += $"  /*{attr.Description}*/";
                    }
                    res.AppendLine(newStr);
                }

                
            }
            CmdLineExtension.Write(res.ToString());
        }

        public virtual void OnEveryLoop()
        {
            CmdLineExtension.WriteColored(
                $"cmd ( { LastCmdName ?? ""} ) : ",
                ConsoleColor.DarkGreen
                );

            string cmdName = CmdLineExtension.ReadLine();
            ExecuteCmd(cmdName);


            CmdLineExtension.WriteLine();

        }

        /// <summary>
        /// Запускает соответствующий метод по названию команды. 
        /// Перехватывает ошибки выполнения.
        /// </summary>
        public void ExecuteCmd(string cmdName)
        {
            try
            {
                cmdName = cmdName.Trim();
                if (string.IsNullOrWhiteSpace(cmdName))
                {
                    cmdName = LastCmdName;
                }
                string[] cmdArray = cmdName.Split('/');

                //имя команды
                string cmdShortName= cmdArray[0].Trim();

               
                if (CmdNameAndMethod.ContainsKey(cmdShortName))
                {
                    //параметры{
                    List<string> cmdParams = new List<string>();
                    for (int i = 1; i < cmdArray.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(cmdArray[i]))
                        {
                            continue;
                        }
                        cmdParams.Add(cmdArray[i].Trim());
                    }
                    //}параметры
                    var currentMethodInfo = CmdNameAndMethod[cmdShortName];
                   
                    //Включаем автозапуск, если такой параметр был передан.
                    //ВНИМАНИЕ! Автозапуск не потокобезопасен.
                    _isAutorunEnabled = cmdParams.Contains("auto") && GetCurrentMemberAttribute(currentMethodInfo.Name).CanAutorun;
                    if (_isAutorunEnabled)
                    {
                        cmdParams.Remove("auto");
                    }

                    var firstParamInfo = currentMethodInfo.GetParameters().Length > 0 ? currentMethodInfo.GetParameters()[0] : null;
                    bool? isTakeParams=firstParamInfo?.ParameterType.IsAssignableFrom(typeof(List<string>));

                    if (isTakeParams == true)
                        currentMethodInfo.Invoke(this, new object[] { cmdParams });
                    else
                        currentMethodInfo.Invoke(this, new object[] {});

                    LastCmdName = cmdName;
                }

                else
                {
                    CmdLineExtension.WriteLine("Command not found.");
                }
            }
            catch (Exception ex)
            {
                CmdLineExtension.WriteLineColored("Executing command throwed exception: " + ex.ToString(), ConsoleColor.DarkRed);
                //ApiException apiEx = ExceptionsHelper.FindInnerExceptionInAggregateException<ApiException>(ex);
                //if (apiEx!=null)
                //{
                //    var apiErr = apiEx.GetApiError();
                //    HandleApiError(apiErr);
                //}
                CmdLineExtension.WriteColored("\nWant to ignore it? Press y/n (y): ", ConsoleColor.DarkRed);
                var consoleText = CmdLineExtension.ReadLine();
                if (consoleText.Trim()=="")
                {
                    //throw;
                }
                if (consoleText.Trim().StartsWith("n"))
                {
                    throw;
                }
            }
            finally
            {
                _isAutorunEnabled = false;
            }
        }

        /// <summary>
        /// Костиль для отслеживания нужных мне исключений. Ведь я не могу их нормально получить из таска.
        /// </summary>
        /// <param name="apiErr"></param>
        //void HandleApiError(ApiError apiErr)
        //{            
        //    var apiErrStr = JsonSerializeHelper.Inst.ToJson(apiErr, true);
        //    CmdLineExtension.WriteLineColored("\nApi error: " + apiErrStr, ConsoleColor.DarkRed);
        //}
                
        /// <summary>
        /// НЕ ПЕРЕДАВАТЬ НИКАКИХ ПАРАМЕТРОВ. 
        /// </summary>
        CmdInfoAttribute GetCurrentMemberAttribute(string memberName)
        {
            CmdInfoAttribute attr = GetType().GetMethod(memberName)
                .GetCustomAttribute(typeof(CmdInfoAttribute)) as CmdInfoAttribute;
            if (attr == null)
                throw new Exception("Can call this method only from methods with CmdInfoAttribute.");
            return attr;
        }

        [CmdInfo(CmdName = "q", Description = "Используйте эту команду для выхода из консоли.")]
        public void QuitCmd()
        {
            IsInRun = false;
        }

        /// <summary>
        /// Создает словарь с названиями команд cli и соответствующими им методы.
        /// </summary>
        Dictionary<string, MethodInfo> CreateReflectionDict()
        {
            Dictionary<string, MethodInfo> cmdNameAndMethod = new Dictionary<string, MethodInfo>();
            foreach (MethodInfo item in this.GetType().GetMethods())
            {
                CmdInfoAttribute attr = item.GetCustomAttribute(typeof(CmdInfoAttribute)) as CmdInfoAttribute;
                if (attr != null)
                {
                    cmdNameAndMethod.Add(attr.CmdName, item);
                }
            }
            return cmdNameAndMethod;
        }

        /// <summary>
        /// Надстройка для получения ресурсов через CmdLineExtension с кешированием и по имени метода из консоли, а не только по имени ресурса.
        /// </summary>
        protected T ReadResource<T>(string resourceName, ReadResourseOptions options = null,
           [CallerMemberName]string memberName = null)
        {
            return (T)ReadResource(typeof(T), resourceName, options, memberName);
        }

        /// <summary>
        /// Надстройка для получения ресурсов через CmdLineExtension с кешированием и по имени метода из консоли, а не только по имени ресурса.
        /// </summary>
        protected object ReadResource(Type resourceType,string resourceName, ReadResourseOptions options = null,
          [CallerMemberName]string memberName = null)
        {
            //Включаем автосчитывание если нужно
            options = options ?? new ReadResourseOptions();
            options.UseAutoread = _isAutorunEnabled;

            return CmdLineExtension.ReadResource(resourceType,memberName + "." + resourceName, options);
        }
    }
}
