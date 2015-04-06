

namespace BaseTools.Core.Common
{
    using BaseTools.Core.Utility;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification="Name is correct"), 
    System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification="Name is correct")]
    public sealed class WeakEventHandler : EventArgs
    {
        private readonly WeakReference _targetReference;

        private readonly MethodInfo _method;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validate by Guard")]
        public WeakEventHandler(EventHandler callback)
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

        public void Handler(object sender, EventArgs e)
        {
            var target = _targetReference.Target;
            if (target != null)
            {
                var parameters = new object[2] { sender, e };
                _method.Invoke(target, parameters);
            }
        }
    }
}
