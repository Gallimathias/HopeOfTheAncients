
namespace engenious.UserDefined.Shaders
{
    public class map : engenious.Graphics.Effect
    {
        public  map(engenious.Graphics.GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            
        }
        protected override void Initialize()
        {
            base.Initialize();
            var techniques = Techniques;
            Ambient = techniques["Ambient"] as AmbientImpl;
        }
        public AmbientImpl Ambient{ get; private set; }
        public class AmbientImpl : engenious.Graphics.EffectTechnique
        {
            public  AmbientImpl(string name)
                : base(name)
            {
                
            }
            protected override void Initialize()
            {
                base.Initialize();
                var passes = Passes;
                MainPass = passes["MainPass"] as MainPassImpl;
            }
            public MainPassImpl MainPass{ get; private set; }
            public engenious.Graphics.Texture2DArray Textures
            {
                set => MainPass.Textures = value;
            }
            public engenious.Matrix WorldViewProj
            {
                set => MainPass.WorldViewProj = value;
            }
            public class MainPassImpl : engenious.Graphics.EffectPass
            {
                private engenious.Graphics.EffectPassParameter _TexturesPassParameter;
                private engenious.Graphics.Texture2DArray _Textures;
                private engenious.Graphics.EffectPassParameter _WorldViewProjPassParameter;
                private engenious.Matrix _WorldViewProj;
                public  MainPassImpl(engenious.Graphics.GraphicsDevice graphicsDevice, string name)
                    : base(graphicsDevice, name)
                {
                }
                protected override void CacheParameters()
                {
                    base.CacheParameters();
                    var parameters = Parameters;
                    _TexturesPassParameter = parameters["Textures"];
                    _WorldViewProjPassParameter = parameters["WorldViewProj"];
                }
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
