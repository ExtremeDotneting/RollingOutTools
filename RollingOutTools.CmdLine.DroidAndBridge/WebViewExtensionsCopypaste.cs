using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Webkit;

namespace RollingOutTools.CmdLine.DroidAndBridge
{
    /// <summary>
    /// Копипаста кода с SiteToApp.Droid, т.к. не должно быть зависимости от той сборки.
    /// </summary>
    static class WebViewExtensionsCopypaste
    {
        /// <summary>
        /// Расширение WebView для выполнения скрипта  и получения результата.
        /// </summary>
        public static Task<object> ExJsWithResult(this WebView wv, string script)
        {
            var callback = new JsValueCallback();

            Application.SynchronizationContext.Post((obj) =>
            {
                wv.EvaluateJavascript(script, callback);
            }, null);
            //TODO Такой код лочит главный поток. Не знаю возможноно ли вообще это исправить, но желательно.
            return callback.WaitValue();

        }

        class JsValueCallback : Java.Lang.Object, IValueCallback
        {
            AutoResetEvent are = new AutoResetEvent(false);
            Java.Lang.Object val;

            public void OnReceiveValue(Java.Lang.Object value)
            {
                val = value;
                are.Set();
            }

            public async Task<object> WaitValue()
            {
                return await Task<object>.Run(() =>
                {
                    are.WaitOne();
                    return (object)val;
                }).ConfigureAwait(false);
            }
        }
    }
}