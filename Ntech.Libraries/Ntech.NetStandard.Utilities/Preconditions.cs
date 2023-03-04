using Common.Logging;
using System;
using System.Globalization;
using System.IO;

namespace Ntech.NetStandard.Utilities
{
    public static class Preconditions
    {
        #region Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Preconditions));

        /// <summary>
        /// The default error message for null argument.
        /// </summary>
        private const string NullErrorMessage = "must not be NULL.";

        /// <summary>
        /// The default error message for blank argument.
        /// </summary>
        private const string BlankErrorMessage = "must not be BLANK.";

        /// <summary>
        /// The value parameter name.
        /// </summary>
        private const string ValueParamName = "value";

        /// <summary>
        /// The name parameter name.
        /// </summary>
        private const string NameParamName = "name";

        /// <summary>
        /// The message parameter name.
        /// </summary>
        private const string MessageParamName = "message";

        #endregion

        /// <summary>
        /// Ensures that <paramref name="value" /> is not null.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="value">The value to check, must not be NULL.</param>
        /// <param name="name">The name of the parameter the value is taken from, must not be BLANK.</param>
        public static void CheckNotNull<T>(T value, string name)
        {
            CheckNotNull(value, name, NullErrorMessage);
        }

        /// <summary>
        /// Ensures that <paramref name="value" /> is not null.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="value">The value to check, must not be NULL.</param>
        public static void CheckNotNull<T>(T value)
        {
            CheckNotNull(value, nameof(value), NullErrorMessage);
        }


        /// <summary>
        /// Ensures that <paramref name="value" /> is not null.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="value">The value to check, must not be NULL.</param>
        /// <param name="name">The name of the parameter the value is taken from, must not be BLANK.</param>
        /// <param name="message">The message to provide to the exception if <paramref name="value" />
        /// is null, must not be BLANK.</param>
        /// <exception cref="Exception"></exception>
        public static void CheckNotNull<T>(T value, string name, string message)
        {
            if (value != null)
            {
                return;
            }

            CheckNotBlank(name, NameParamName, BlankErrorMessage);
            CheckNotBlank(message, MessageParamName, BlankErrorMessage);

            var errorMessage = string.Format(CultureInfo.CurrentCulture, "{0} {1}", name, message);
            Logger.Error(errorMessage);
            throw new Exception(errorMessage);
        }

        /// <summary>
        /// Ensures that <paramref name="value" /> is not null.
        /// </summary>
        /// <param name="value">The value to check, must not be NULL.</param>
        /// <param name="name">The name of the parameter the value is taken from, must not be BLANK.</param>
        /// is null, must not be BLANK.
        /// <exception cref="Exception">
        /// name must not be BLANK
        /// or
        /// message must not be BLANK
        /// or
        /// </exception>
        public static void CheckNotBlank(string value, string name)
        {
            CheckNotBlank(value, name, BlankErrorMessage);
        }

        /// <summary>
        /// Ensures that <paramref name="value" /> is not null.
        /// </summary>
        /// <param name="value">The value to check, must not be NULL.</param>
        /// <param name="name">The name of the parameter the value is taken from, must not be BLANK.</param>
        /// <param name="message">The message to provide to the exception if <paramref name="value" />
        /// is null, must not be BLANK.</param>
        /// <exception cref="Exception">
        /// name must not be BLANK
        /// or
        /// message must not be BLANK
        /// or
        /// </exception>
        public static void CheckNotBlank(string value, string name, string message)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                var errorMessage = string.Format(CultureInfo.CurrentCulture, "{0} {1}", NameParamName, BlankErrorMessage);
                Logger.Error(errorMessage);
                throw new Exception(errorMessage);
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                var errorMessage = string.Format(CultureInfo.CurrentCulture, "{0} {1}", MessageParamName, BlankErrorMessage);
                Logger.Error(errorMessage);
                throw new Exception(errorMessage);
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                var errorMessage = string.Format(CultureInfo.CurrentCulture, "{0} {1}", name, message);
                Logger.Error(errorMessage);
                throw new Exception(errorMessage);
            }
        }

        /// <summary>
        /// Checks the file exist.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="name">The name of the parameter the value is taken from.</param>
        /// <exception cref="Exception"></exception>
        public static void CheckFileExist(string value, string name)
        {
            CheckFileExist(value, name, "does not exist.");
        }

        /// <summary>
        /// Checks the file exist.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="name">The name of the parameter the value is taken from.</param>
        /// <param name="message"/>The message to provide to the exception if <paramref name="value" />
        /// <exception cref="Exception"></exception>
        public static void CheckFileExist(string value, string name, string message)
        {
            CheckNotNull(value, name);
            CheckNotBlank(name, NameParamName);
            CheckNotBlank(message, MessageParamName);
            CheckNotBlank(value, name);
            if (!File.Exists(value))
            {
                Logger.Error($"{value} does not exist");
                throw new Exception($"{value} does not exist");
            }
        }


        /// <summary>
        /// Checks the file path correct.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="name">The name of the parameter the value is taken from.</param>
        /// <param name="message">The message to provide to the exception if <paramref name="value" />
        /// <exception cref="Exception"></exception>
        public static void CheckFilePathCorrect(string value, string name)
        {
            CheckFilePathCorrect(value, name, "file path not correct");
        }

        /// <summary>
        /// Checks the file path correct.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="name">The name of the parameter the value is taken from.</param>
        /// <param name="message">The message to provide to the exception if <paramref name="value" />
        /// <exception cref="Exception"></exception>
        public static void CheckFilePathCorrect(string value, string name, string message)
        {
            CheckNotNull(value, name);
            CheckNotBlank(name, NameParamName);
            CheckNotBlank(message, MessageParamName);
            CheckNotBlank(value, name);
            if (!Directory.Exists(value))
            {
                Logger.Error($"{name} does not exist");
                throw new Exception($"{name} does not exist");
            }
        }

        /// <summary>
        /// Check not null or empty
        /// </summary>
        /// <param name="value"></param>
        public static void CheckNotNullOrEmpty(string value)
        {
            var paramName = nameof(value);
            CheckNotNull(value, paramName);
            CheckNotBlank(value, paramName);
        }

        /// <summary>
        /// Check enum is correct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public static void CheckEnumCorrect<T>(T value)
        {
            var exists = Enum.IsDefined(typeof(T), value);
            if (!exists)
            {
                Logger.Error($"{value} does not exist");
                throw new Exception($"{value} does not exist");
            }
        }
        
    }
}
