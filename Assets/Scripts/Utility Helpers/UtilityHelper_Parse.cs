namespace Utilities
{
    public static partial class UtilityHelper
    {
		// Parse a float, return default if failed
		public static float Parse_Float(string txt, float _default)
		{
			float f;
			if (!float.TryParse(txt, out f))
				f = _default;
			return f;
		}

		// Parse a int, return default if failed
		public static int Parse_Int(string txt, int _default)
		{
			int i;
			if (!int.TryParse(txt, out i))
				i = _default;
			return i;
		}

        public static int Parse_Int(string txt) => Parse_Int(txt, -1);
    }
}