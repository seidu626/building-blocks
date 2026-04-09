using System.ComponentModel;
using System.Reflection;

namespace BuildingBlocks.Ldap.Extensions;

internal class Enum<T> where T : struct, IConvertible
{
    public static string[] Descriptions
    {
        get
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");
            
            var stringList = new List<string>();
            foreach (var value in System.Enum.GetValues(typeof(T)))
            {
                var valueString = value.ToString();
                if (string.IsNullOrEmpty(valueString))
                    continue;
                    
                var field = value.GetType().GetField(valueString);
                if (field != null)
                {
                    var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (attributes.Length > 0 && attributes[0] is DescriptionAttribute descriptionAttr)
                    {
                        if (!stringList.Contains(descriptionAttr.Description))
                            stringList.Add(descriptionAttr.Description);
                    }
                }
            }
            return stringList.ToArray();
        }
    }
}