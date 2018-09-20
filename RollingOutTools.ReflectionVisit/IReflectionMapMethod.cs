using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RollingOutTools.ReflectionVisit
{
    /// <summary>
    /// Класс для выполнения метода.
    /// </summary>
    public interface IReflectionMapMethod
    {
        string Description { get; }

        string DisplayName { get; }

        string RealName { get; }

        /// <summary>
        /// Типы входных параметров по порядку.
        /// </summary>
        IReadOnlyCollection<Parameter> Parameters { get;}

        Type ReturnType { get; }

        MethodKind Kind { get;  }

        /// <summary>
        /// Return result from called method without any manipulations on result.
        /// </summary>
        /// <param name="globalInstance">Экземпляр типа, для которого был построен reflection map.</param>
        /// <param name="parameters">Параметры передаваемые в метод</param>
        object Execute(object globalInstance, object[] parameters);

        /// <summary>
        /// If method return task, this method will await it.
        /// </summary>
        /// <param name="globalInstance">Экземпляр типа, для которого был построен reflection map.</param>
        /// <param name="parameters">Параметры передаваемые в метод</param>
        Task<object> ExecuteAndAwait(object globalInstance, object[] parameters);
        
    }
}