namespace Pepperoni
{
    public class Mod : Loggable, IMod
    {
        protected readonly string _name;
        protected readonly string _version;

        /// <inheritdoc />
        public string GetName() => _name;

        /// <inheritdoc />
        public virtual string GetVersion() => _version;

        /// <inheritdoc />
        public virtual void Initialize() { }

        /// <inheritdoc />
        public int LoadPriority() => 1;

        public Mod() : this(null, null) { }

        public Mod(string name) : this(name, null) { }

        public Mod(string name, string version)
        {
            _name = string.IsNullOrEmpty(name) ? GetType().Name : name;
            _version = string.IsNullOrEmpty(version) ? "N/A" : version;
        }
    }
}
