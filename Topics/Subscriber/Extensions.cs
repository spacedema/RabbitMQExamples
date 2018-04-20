using System;
using System.ComponentModel;

namespace Subscriber
{
    public static class Extensions
    {
        public static string GetDescription<T>(this T enumValue) where T : struct
        {
            var type = enumValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("Not Enum type", nameof(enumValue));
            }

            var memberInfo = type.GetMember(enumValue.ToString());
            if (memberInfo.Length <= 0)
                return enumValue.ToString();

            var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attrs.Length > 0 ? ((DescriptionAttribute)attrs[0]).Description : enumValue.ToString();
        }
    }
}
