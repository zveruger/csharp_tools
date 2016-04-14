using System.Text;

namespace Tools.CSharp.Extensions
{
    public static class StringBuilderExtensions
    {
        //---------------------------------------------------------------------
        public static StringBuilder Append(this StringBuilder stringBuilder, params string[] values)
        {
            foreach (var value in values)
            { stringBuilder.Append(value); }

            return stringBuilder;
        }
        //---------------------------------------------------------------------
    }
}
