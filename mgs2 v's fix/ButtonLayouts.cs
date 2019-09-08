using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mgs2_v_s_fix
{

    public enum ButtonActions
    {
        X,
        A,
        B,
        Y,
        L2,
        EX3,
        AR,
        L1,
        R1,
        SEL,
        STA,
        EX2,
        EX1,
        R2,
    }

    public enum AnalogActions
    {
        X,
        Y,
        Rx,
        Ry,
        Z,
        Rz,
    }

    public enum AvailableGamepads
    {
        XBOX,
        DUALSHOCK4,
    }

    public enum AvailableLayouts
    {
        V,
        PS2
    }

    public class ButtonLayouts
    {
        public List<(ButtonActions, string)> ButtonBindings { get; }
        public List<(string, AnalogActions, string)> AnalogBindings { get; }

        public ButtonLayouts(AvailableGamepads choosenGamepad, AvailableLayouts choosenLayout, bool InvertTriggersWithDorsals)
        {
            // Build the choosen layout...

            if (choosenGamepad == AvailableGamepads.DUALSHOCK4)
            {
                List<(ButtonActions, string)> dorsalButton;

                if (choosenLayout == AvailableLayouts.V)
                {
                    // V's layout

                    ButtonBindings = new List<(ButtonActions, string)>
                    {
                        (ButtonActions.X, "00"),
                        (ButtonActions.A, "01"),
                        (ButtonActions.B, "02"),
                        (ButtonActions.Y, "03"),
                        (ButtonActions.SEL, "08"),
                        (ButtonActions.STA, "09"),
                        (ButtonActions.EX2, "0A"),
                        (ButtonActions.EX1, "0B"),

                    };                    

                    if (!InvertTriggersWithDorsals)
                    {
                        // Use the standard MGS dorsal button layout

                        dorsalButton = new List<(ButtonActions, string)>
                        {
                            (ButtonActions.L2, "04"),
                            (ButtonActions.EX3, "05"),
                            (ButtonActions.AR, "05"),
                            (ButtonActions.L1, "06"),
                            (ButtonActions.R1, "07"),
                        };

                    }
                    else
                    {
                        // Invert 4 with 6 (and vice versa) - L buttons
                        // Invert 5 with 7 (and vice versa) - R buttons

                        dorsalButton = new List<(ButtonActions, string)>
                        {
                            (ButtonActions.L2, "06"),
                            (ButtonActions.EX3, "07"),
                            (ButtonActions.AR, "07"),
                            (ButtonActions.L1, "04"),
                            (ButtonActions.R1, "05"),
                        };

                    }          

                }
                else
                {
                    // PS2 layout

                    ButtonBindings = new List<(ButtonActions, string)>
                    {
                        (ButtonActions.X, "00"),
                        (ButtonActions.A, "01"),
                        (ButtonActions.B, "02"),
                        (ButtonActions.Y, "03"),                       
                        (ButtonActions.SEL, "08"),
                        (ButtonActions.STA, "09"),
                        (ButtonActions.EX2, "0A"),
                        (ButtonActions.EX3, "0A"),
                        (ButtonActions.AR, "0B"),

                    };

                    if (!InvertTriggersWithDorsals)
                    {
                        // Use the standard MGS dorsal button layout

                        dorsalButton = new List<(ButtonActions, string)>
                        {
                            (ButtonActions.L2, "04"),
                            (ButtonActions.R2, "05"),
                            (ButtonActions.L1, "06"),
                            (ButtonActions.R1, "07"),
                        };

                    }
                    else
                    {
                        // Invert 4 with 6 (and vice versa) - L buttons
                        // Invert 5 with 7 (and vice versa) - R buttons

                        dorsalButton = new List<(ButtonActions, string)>
                        {
                            (ButtonActions.L2, "06"),
                            (ButtonActions.R2, "07"),
                            (ButtonActions.L1, "04"),
                            (ButtonActions.R1, "05"),
                        };

                    }

                }

                ButtonBindings.AddRange(dorsalButton);

                // Common for both layouts

                AnalogBindings = new List<(string, AnalogActions, string)>
                {
                    ("00", AnalogActions.Z, "N"),
                    ("01", AnalogActions.Rz, "N"),
                    ("02", AnalogActions.X, "N"),
                    ("03", AnalogActions.Y, "N"),

                };

            }
            else // XBOX
            {

                List<(ButtonActions, string)> dorsalButton;

                if (choosenLayout == AvailableLayouts.V)
                {
                    // V's layout

                    ButtonBindings = new List<(ButtonActions, string)>
                    {
                        (ButtonActions.A, "00"),
                        (ButtonActions.B, "01"),
                        (ButtonActions.X, "02"),
                        (ButtonActions.Y, "03"),                      
                        (ButtonActions.AR, "05"),
                        (ButtonActions.SEL, "06"),
                        (ButtonActions.STA, "07"),
                        (ButtonActions.EX2, "08"),
                        (ButtonActions.EX1, "09"),
                        

                    };

                    if (!InvertTriggersWithDorsals)
                    {
                        // Use the standard MGS dorsal button layout

                        dorsalButton = new List<(ButtonActions, string)>
                        {
                            (ButtonActions.L2, "04"),
                            (ButtonActions.EX3, "05"),
                            (ButtonActions.L1, "0A"),
                            (ButtonActions.R1, "0B"),
                        };

                    }
                    else
                    {
                        // Invert 4 with A (and vice versa) - L buttons
                        // Invert 5 with B (and vice versa) - R buttons

                        dorsalButton = new List<(ButtonActions, string)>
                        {
                            (ButtonActions.L2, "0A"),
                            (ButtonActions.EX3, "0B"),
                            (ButtonActions.L1, "04"),
                            (ButtonActions.R1, "05"),
                        };

                    }

                }
                else
                {
                    // PS2 layout

                    ButtonBindings = new List<(ButtonActions, string)>
                    {
                        (ButtonActions.A, "00"),
                        (ButtonActions.B, "01"),
                        (ButtonActions.X, "02"),
                        (ButtonActions.Y, "03"),                     
                        (ButtonActions.SEL, "06"),
                        (ButtonActions.STA, "07"),
                        (ButtonActions.EX2, "08"),
                        (ButtonActions.EX3, "08"),
                        (ButtonActions.AR, "09"), 
                    };

                    if (!InvertTriggersWithDorsals)
                    {
                        // Use the standard MGS dorsal button layout

                        dorsalButton = new List<(ButtonActions, string)>
                        {
                            (ButtonActions.L2, "04"),
                            (ButtonActions.R2, "05"),
                            (ButtonActions.L1, "0A"),
                            (ButtonActions.R1, "0B"),
                        };

                    }
                    else
                    {
                        // Invert 4 with A (and vice versa) - L buttons
                        // Invert 5 with B (and vice versa) - R buttons

                        dorsalButton = new List<(ButtonActions, string)>
                        {
                            (ButtonActions.L2, "0A"),
                            (ButtonActions.R2, "0B"),
                            (ButtonActions.L1, "04"),
                            (ButtonActions.R1, "05"),
                        };

                    }

                }

                ButtonBindings.AddRange(dorsalButton);

                // Common for both layouts

                AnalogBindings = new List<(string, AnalogActions, string)>
                {
                    ("00", AnalogActions.Rx, "N"),
                    ("01", AnalogActions.Ry, "N"),
                    ("02", AnalogActions.X, "N"),
                    ("03", AnalogActions.Y, "N"),

                };


            }

        }


    }

}
