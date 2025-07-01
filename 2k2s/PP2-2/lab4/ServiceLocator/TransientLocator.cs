    namespace ServiceLocator
    {
        public static class TransientLocator
        {
            private static readonly Dictionary<Type, Func<object>> services = new();

            public static void Register<T>(Func<T> resolver) where T : class
            {
                services[typeof(T)] = () => resolver();
            }

            public static T Resolve<T>() where T : class
            {
                return (T)services[typeof(T)]();
            }

            public static void Reset()
            {
                services.Clear();
            }
        }
    }
