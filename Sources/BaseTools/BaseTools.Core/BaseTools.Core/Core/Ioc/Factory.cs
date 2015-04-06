namespace BaseTools.Core.Ioc
{
    using BaseTools.Core.Utility;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Allows to receive a real types by their abstractions. 
    /// Represents a simple container in "Inversion of control" pattern. 
    /// </summary>
    public class Factory
    {
        private Dictionary<Type, TypeBindingBase> typeBindings = new Dictionary<Type, TypeBindingBase>();

        private bool isBindExecuted;

        static Factory()
        {
            Common = new Factory();
            Common.Initialize(new LocalInitializer());
            Common.isBindExecuted = false;
            // TODO: think about initializer.
        }

        public static Factory Common { get; private set; }

        public void Initialize(FactoryInitializer initializer)
        {
            this.isBindExecuted = true;
            Guard.CheckIsNotNull(initializer, "initializer");
            initializer.SetupBindnings(this);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type parameter passed to TypeBinding class.")]
        public void Bind<TSource, TReal>() where TReal : TSource, new()
        {
            this.isBindExecuted = true;
            var binding = new TypeBinding<TSource, TReal>();
            var sourceType = typeof(TSource);
            this.typeBindings[sourceType] = binding;
        }

        public TSource GetInstance<TSource>()
        {
            if (!this.isBindExecuted)
            {
                this.TryInitializeDesignMode();
            }

            var sourceType = typeof(TSource);
            var binding = this.GetBinding(sourceType);
            var instance = binding.GetRealInstance();
            return (TSource)instance;
        }

        public bool IsBindingExist(Type checkedType)
        {
            var isExist = this.typeBindings.ContainsKey(checkedType);
            return isExist;
        }

        private TypeBindingBase GetBinding(Type sourceType)
        {
            TypeBindingBase binding = null;
            bool boundExist = this.typeBindings.TryGetValue(sourceType, out binding);
            if (!boundExist)
            {
                var errorMessage = String.Format(CultureInfo.InvariantCulture, "Can't find bindings for type {0}. Please call Bind method for this type.", sourceType.FullName);
                throw new InvalidOperationException(errorMessage);
            }

            return binding;
        }

        private void TryInitializeDesignMode()
        {
            if (!this.isBindExecuted)
            {
                //Try silverlight assembly
                var initializerTypeNames = new string[] 
                {
                    //"BaseTools.Core.CoreFactoryInitializer, BaseTools.Core.WinrtUniversal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                    "BaseTools.Core.CoreFactoryInitializer, BaseTools.Core.WpSilverlgiht, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                };

                foreach (var typeName in initializerTypeNames)
                {
                    var type = Type.GetType(typeName, false);
                    if (type != null)
                    {
                        FactoryInitializer instance = null;
                        var instance1 = Activator.CreateInstance(type);
                        if (instance1 is FactoryInitializer)
                        {
                            instance = instance1 as FactoryInitializer;
                            if (instance != null)
                            {
                                //throw new Exception("success1");
                            }
                        }
                        
                        //instance = (FactoryInitializer)Activator.CreateInstance(type);
                        Factory.Common.Initialize(instance);
                        this.isBindExecuted = true;
                        var isInDesignMode = BaseTools.Core.Info.EnvironmentInfo.Current.IsInDesignMode;
                        if (isInDesignMode)
                        {
                            
                            // TODO: think about reflection.
                            var additionalInitializerNames = new string[]
                            {
                                //"BaseTools.UI.UIFactoryInitializer, BaseTools.UI.WpWinrt, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                                "BaseTools.UI.UIFactoryInitializer, BaseTools.UI.WpSilverlight, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                            };

                            foreach (var item in additionalInitializerNames)
                            {
                                var additionalType = Type.GetType(item, false);
                                if (additionalType != null)
                                {
                                    
                                    var additionalInitializer = (FactoryInitializer)Activator.CreateInstance(additionalType);
                                    Factory.Common.Initialize(additionalInitializer);
                                }
                            }
                        }

                        break;
                    }
                }
            }
        }

    }
}
