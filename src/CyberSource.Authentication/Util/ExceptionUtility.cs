// Decompiled with JetBrains decompiler
// Type: AuthenticationSdk.util.ExceptionUtility
// Assembly: AuthenticationSdk, Version=0.0.0.3, Culture=neutral, PublicKeyToken=null
// MVID: 20997894-17CE-414B-B502-B8B103C3242C
// Assembly location: D:\Sources\Decompile\AuthenticationSdk.dll

namespace CyberSource.Authentication.Util
{
    public class ExceptionUtility
    {
        private static bool _exceptionIsCaughtAlready;

        public static void Exception(string exceptionMessage, string stackTrace)
        {
            Logger currentClassLogger = LogManager.GetCurrentClassLogger();
            if (ExceptionUtility._exceptionIsCaughtAlready)
                return;
            if (!string.IsNullOrEmpty(exceptionMessage))
            {
                currentClassLogger.Error(exceptionMessage);
                if (!string.IsNullOrEmpty(stackTrace))
                    currentClassLogger.Trace(stackTrace);
            }

            ExceptionUtility._exceptionIsCaughtAlready = true;
        }
    }
}
