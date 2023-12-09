
namespace engenious.UserDefined.Shaders
{
    /// <summary>Implementation for the map effect.</summary>
    public partial class map : engenious.Graphics.Effect
    {
        /// <summary>Initializes a new instance of the <see cref="map"/> class.</summary>
        /// <param name="graphicsDevice">The graphics device for the effect.</param>
        public  map(engenious.Graphics.GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            
        }
        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();
            var techniques = Techniques;
            Ambient = techniques["Ambient"] as AmbientImpl;
        }
        /// <summary>Gets the <see cref="Ambient"/> technique.</summary>
        public AmbientImpl Ambient{ get; private set; }
        /// <summary>Implementation for the Ambient effect technique</summary>
        public partial class AmbientImpl : engenious.Graphics.EffectTechnique
        {
            /// <summary>Initializes a new instance of the <see cref="AmbientImpl" /> class.</summary>
            /// <param name="name">The name of the effect technique.</param>
            public  AmbientImpl(string name)
                : base(name)
            {
                
            }
            /// <inheritdoc />
            protected override void Initialize()
            {
                base.Initialize();
                var passes = Passes;
                MainPass = passes["MainPass"] as MainPassImpl;
            }
            /// <summary>Gets the MainPass pass.</summary>
            public MainPassImpl MainPass{ get; private set; }
            /// <summary>Sets or gets the Textures parameter.</summary>
            public engenious.Graphics.Texture2DArray Textures
            {
                get => MainPass.Textures;
                set => MainPass.Textures = value;
            }
            /// <summary>Sets or gets the WorldViewProj parameter.</summary>
            public engenious.Matrix WorldViewProj
            {
                get => MainPass.WorldViewProj;
                set => MainPass.WorldViewProj = value;
            }
            /// <summary>Implementation of the <see cref="MainPass"/>effect pass.</summary>
            public partial class MainPassImpl : engenious.Graphics.EffectPass
            {
                private engenious.Graphics.EffectPassParameter _TexturesPassParameter;
                private engenious.Graphics.Texture2DArray _Textures;
                private engenious.Graphics.EffectPassParameter _WorldViewProjPassParameter;
                private engenious.Matrix _WorldViewProj;
                /// <summary>Initializes a new instance of the <see cref="MainPassImpl"/> class.</summary>
                public  MainPassImpl(engenious.Graphics.GraphicsDevice graphicsDevice, string name)
                    : base(graphicsDevice, name)
                {
                }
                /// <inheritdoc />
                protected override void CacheParameters()
                {
                    base.CacheParameters();
                    var parameters = Parameters;
                    _TexturesPassParameter = parameters["Textures"];
                    _WorldViewProjPassParameter = parameters["WorldViewProj"];
                }
                /// <summary>Gets or sets the Textures parameter.</summary>
                public engenious.Graphics.Texture2DArray Textures
                {
                    get => _Textures;
                    set
                    {
                        if (_Textures == value || (value != null && value.Equals(_Textures)))
                            return;
                        _Textures = value;
                        _TexturesPassParameter.SetValue(value);
                    }

                }
                /// <summary>Gets or sets the WorldViewProj parameter.</summary>
                public engenious.Matrix WorldViewProj
                {
                    get => _WorldViewProj;
                    set
                    {
                        if (_WorldViewProj == value)
                            return;
                        _WorldViewProj = value;
                        _WorldViewProjPassParameter.SetValue(value);
                    }

                }
            }
        }
    }
}
