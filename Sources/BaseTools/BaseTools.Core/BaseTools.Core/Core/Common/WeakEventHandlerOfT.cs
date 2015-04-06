namespace BaseTools.Core.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using System.Threading.Tasks;
    using BaseTools.Core.Utility;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification="Name is correct")]
    public sealed class WeakEventHandler<TEventArgs>
    {
        private readonly WeakReference _targetReference;

        private readonly System.Reflection.MethodInfo _method;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validate by Guard")]
        public WeakEventHandler(EventHandler<TEventArgs> callback)
        {
            Guard.CheckIsNotNull(callback, "callback");
            _method = callback.GetMethodInfo();
            _targetReference = new WeakReference(callback.Target, true);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification="Validate by Guard")]
        public WeakEventHandler(object target, string method)
        {
            Guard.CheckIsNotNull(target, "target");
            var typeInfo = target.GetType().GetTypeInfo();
            _method = typeInfo.GetDeclaredMethod(method);
            _targetReference = new WeakReference(target, true);
        }

        public void Handler(object sender, TEventArgs args)
        {
            var target = _targetReference.Target;
            if (target != null)
            {
                var parameters = new object[2] { sender, args };
                _method.Invoke(target, parameters);
            }
        }
    }
}
