using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace GodotUtils;

/*
 * This is meant to replace all GD.Print(...) with Logger.Log(...) to make
 * logging multi-thread friendly. Remember to put Logger.Update() in
 * _PhysicsProcess(double delta) otherwise you will be wondering why Logger.Log(...)
 * is printing nothing to the console.
 */
public static class Logger
{
	private static ConcurrentQueue<LogInfo> Messages { get; } = new();

	/// <summary>
	/// Log a message
	/// </summary>
	public static void Log(object message, ConsoleColor color = ConsoleColor.Gray) =>
		Messages.Enqueue(new LogInfo(LoggerOpcode.Message, new LogMessage($"{message}"), color));

	/// <summary>
	/// Log a warning
	/// </summary>
	public static void LogWarning(object message, ConsoleColor color = ConsoleColor.Yellow) =>
		Log($"[Warning] {message}", color);

	/// <summary>
	/// Log a todo
	/// </summary>
	public static void LogTodo(object message, ConsoleColor color = ConsoleColor.White) =>
		Log($"[Todo] {message}", color);

	/// <summary>
	/// Logs an exception with trace information. Optionally allows logging a human readable hint
	/// </summary>
	public static void LogErr
	(
		Exception e,
		string hint = default,
		ConsoleColor color = ConsoleColor.Red,
		[CallerFilePath] string filePath = default,
		[CallerLineNumber] int lineNumber = 0
	) => LogDetailed(LoggerOpcode.Exception, $"[Error] {(string.IsNullOrWhiteSpace(hint) ? "" : $"'{hint}' ")}{e.Message}{e.StackTrace}", color, true, filePath, lineNumber);

	/// <summary>
	/// Logs a debug message that optionally contains trace information
	/// </summary>
	public static void LogDebug
	(
		object message,
		ConsoleColor color = ConsoleColor.Magenta,
		bool trace = true,
		[CallerFilePath] string filePath = default,
		[CallerLineNumber] int lineNumber = 0
	) => LogDetailed(LoggerOpcode.Debug, $"[Debug] {message}", color, trace, filePath, lineNumber);

	/// <summary>
	/// Log the time it takes to do a section of code
	/// </summary>
	public static void LogMs(Action code)
	{
		var watch = new Stopwatch();
		watch.Start();
		code();
		watch.Stop();
		GD.Print($"Took {watch.ElapsedMilliseconds} ms");
	}

	/// <summary>
	/// Checks to see if there are any messages left in the queue
	/// </summary>
	public static bool StillWorking() => !Messages.IsEmpty;

	/// <summary>
	/// Dequeues a Requested Message and Logs it
	/// </summary>
	public static void Update()
	{
		if (!Messages.TryDequeue(out LogInfo result))
			return;

		switch (result.Opcode)
		{
			case LoggerOpcode.Message:
				UIConsole.AddMessage(result.Data.Message);
				Print(result.Data.Message, result.Color);
				Console.ResetColor();
				break;

			case LoggerOpcode.Exception:
				UIConsole.AddMessage(result.Data.Message);
				PrintErr(result.Data.Message, result.Color);

				if (result.Data is LogMessageTrace exceptionData && exceptionData.ShowTrace)
					PrintErr(exceptionData.TracePath, ConsoleColor.DarkGray);

				Console.ResetColor();
				break;

			case LoggerOpcode.Debug:
				UIConsole.AddMessage(result.Data.Message);
				Print(result.Data.Message, result.Color);

				if (result.Data is LogMessageTrace debugData && debugData.ShowTrace)
					Print(debugData.TracePath, ConsoleColor.DarkGray);

				Console.ResetColor();
				break;
		}
	}

	/// <summary>
	/// Logs a message that may contain trace information
	/// </summary>
	private static void LogDetailed(LoggerOpcode opcode, string message, ConsoleColor color, bool trace, string filePath, int lineNumber)
	{
		string tracePath;

		if (filePath.Contains("Scripts"))
		{
			// Ex: Scripts/Main.cs:23
			tracePath = $"  at {filePath.Substring(filePath.IndexOf("Scripts"))}:{lineNumber}";
			tracePath = tracePath.Replace('\\', '/');
		}
		else
		{
			// Main.cs:23
			var elements = filePath.Split('\\');
			tracePath = $"  at {elements[elements.Length - 1]}:{lineNumber}";
		}

		Messages.Enqueue(
			new LogInfo(opcode, 
				new LogMessageTrace(
					message, 
					trace,
					tracePath
				), 
			color
		));
	}

	private static void Print(object v, ConsoleColor color)
	{
		Console.ForegroundColor = color;

		if (GOS.IsExportedRelease())
			GD.Print(v);
		else
			GD.PrintRich($"[color={color}]{v}");
	}

	private static void PrintErr(object v, ConsoleColor color)
	{
		Console.ForegroundColor = color;
		GD.PrintErr(v);
		//GD.PushError("" + v);
	}
}

public class LogInfo
{
	public LoggerOpcode Opcode { get; set; }
	public LogMessage Data { get; set; }
	public ConsoleColor Color { get; set; }

	public LogInfo(LoggerOpcode opcode, LogMessage data, ConsoleColor color = ConsoleColor.Gray)
	{
		Opcode = opcode;
		Data = data;
		Color = color;
	}
}

public class LogMessage
{
	public string Message { get; set; }
	public LogMessage(string message) => this.Message = message;

}
public class LogMessageTrace : LogMessage
{
	// Show the Trace Information for the Message
	public bool ShowTrace { get; set; }
	public string TracePath { get; set; }

	public LogMessageTrace(string message, bool trace = true, string tracePath = default)
	: base(message)
	{
		ShowTrace = trace;
		TracePath = tracePath;
	}
}

public enum LoggerOpcode
{
	Message,
	Exception,
	Debug
}
