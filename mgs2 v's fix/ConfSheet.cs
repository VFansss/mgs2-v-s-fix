using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mgs2_v_s_fix
{
    public class ConfSheet
    {

        public Dictionary<string, string> Resolution;
        public Dictionary<string, string> Controls;
        public Dictionary<string, string> Graphics;
        public Dictionary<string, string> Sound;

        
        public ConfSheet()
        {

            this.Resolution = new Dictionary<string, string>
            { 
        
                {"Height",null},
                {"Width",null},
                {"WideScreenFIX",null},
                {"GraphicAdapterName",null},
                {"WindowMode",null},
                {"LaptopMode",null}

             };

            this.Controls = new Dictionary<string, string>
            {
                {"360Gamepad",null}
            };

            this.Graphics = new Dictionary<string, string>
            {
                {"RenderingSize",null},
                {"ShadowDetail",null},
                {"ModelQuality",null},
                {"RenderingClearness",null},
                {"CrossFade",null},
                {"EffectQuantity",null},
                {"BunchOfCoolEffect",null},
                {"MotionBlur",null}

            };

            this.Sound = new Dictionary<string, string>
            {
                {"SoundAdapterName",null},
                {"Quality",null},
                {"SE",null},
                {"SoundQuality",null}

            };


        }

    }

}
