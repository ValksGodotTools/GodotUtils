using System.Reflection;

namespace GodotUtils;

public static class NetcodeUtils
{
	public static Dictionary<TKey, TValue> LoadInstances<TKey, TValue>(string prefix)
	{
		return Assembly.GetExecutingAssembly()
			.GetTypes()
			.Where(x => typeof(TValue).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
			.Select(Activator.CreateInstance).Cast<TValue>()
			.ToDictionary(
				x =>
				{
					var className = x.GetType().Name;
					var opcodeName = className.Replace(prefix, "");

					try
					{
						return (TKey)Enum.Parse(typeof(TKey), opcodeName);
					}
					catch (ArgumentException)
					{
						var enumName = $"{typeof(TKey).Name }.{opcodeName}";
						var message = $"No enum called '{enumName}' " +
						$"was defined for the '{className}' class. Please remove " +
						$"the '{className}' class or create the '{enumName}' enum.";

						throw new Exception(message);
					}

				}, x => x);
	}
}
