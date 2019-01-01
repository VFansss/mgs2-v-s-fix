using System.Collections.Generic;


namespace mgs2_v_s_fix
{
    public class ConfSheet
    {
        public Dictionary<string, string> Resolution;
        public Dictionary<string, string> Controls;
        public Dictionary<string, string> Graphics;
        public Dictionary<string, string> Sound;
        public Dictionary<string, string> Others;


        public ConfSheet()
        {
            Resolution = new Dictionary<string, string>
            {
                {"Height", null},
                {"Width", null},
                {"WideScreenFIX", null},
                {"GraphicAdapterName", null},
                {"WindowMode", null},
                {"LaptopMode", null},
                {"FullscreenCutscene", null},
                {"OptimizedFOV", null}
            };

            Controls = new Dictionary<string, string>
            {
                //{"XboxGamepad",null},
                {"EnableController", null},
                {"PreferredLayout", null},
            };

            Graphics = new Dictionary<string, string>
            {
                {"RenderingSize", null},
                {"ShadowDetail", null},
                {"ModelQuality", null},
                {"RenderingClearness", null},
                {"EffectQuantity", null},
                {"BunchOfCoolEffect", null},
                {"MotionBlur", null},
                {"AA", null},
                {"DepthOfField", null}
            };

            Sound = new Dictionary<string, string>
            {
                {"SoundAdapterName", null},
                {"Quality", null},
                {"SE", null},
                {"SoundQuality", null},
                {"FixAfterPlaying", null}
            };

            Others = new Dictionary<string, string>
            {
                {"CompatibilityWarningDisplayed", null},
            };
        }
    }
}