namespace ServiceLocator
{
    public static class SingletonLocator
    {
        private static readonly Dictionary<Type, object> services = new();

        public static void Register<T>(T service) where T : class
        {
            services[typeof(T)] = service;
        }

        public static T Resolve<T>() where T : class
        {
            return (T)services[typeof(T)];
        }

        public static void Reset()
        {
            services.Clear();
        }
    }
}
