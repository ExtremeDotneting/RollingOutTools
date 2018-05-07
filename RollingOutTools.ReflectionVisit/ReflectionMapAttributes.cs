using System;
using System.Collections.Generic;
using System.Text;

namespace RollingOutTools.ReflectionVisit
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class ClassInfo_ReflectionMapAttribute :Attribute
    {
        public string Prefix { get; set; }
    }

    /// <summary>
    /// Этим атрибутом вы должны отметить методы, которые хотите отобразить в ReflectionMap.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class Method_ReflectionMapAttribute : Base_ReflectionMapAttribute
    {        
    }

    /// <summary>
    /// Этим атрибутом вы должны отметить поля, которые хотите преобразовать в методы в ReflectionMap.
    /// Еще раз, вместо поля вы получите методы get_[имя свойства] и  set_[имя свойства].
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SimpleProp_ReflectionMapAttribute : Base_ReflectionMapAttribute
    {
        public bool CanGet { get; set; } = true;
        public bool CanSet { get; set; } = true;

        public SimpleProp_ReflectionMapAttribute()
        {
            ///throw new Exception("SimpleProp was deprecated becouse propertyes can`t be normally wrapped to PureApiResponse<>.");
            //Can be used in some cases, but not where you use PureApiResponse
        }
    }

    /// <summary>
    /// Этим атрибутом отвечайте поля, для которых вы также хотите построить ReflectionMap. Их методы станут доступны
    /// через такую конструкцию [имя свойства].[имя метода]
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CategoryProp_ReflectionMapAttribute : Base_ReflectionMapAttribute
    {
        
    }

    public abstract class Base_ReflectionMapAttribute : Attribute
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }
}
