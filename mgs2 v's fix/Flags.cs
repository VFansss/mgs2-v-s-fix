using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mgs2_v_s_fix
{
    public enum UPDATE_AVAILABILITY
    {
        ResponseMismatch = 0,
        NetworkError,
        NoUpdates,
        UpdateAvailable,
    };

    public enum ADD2STEAMSTATUS
    {
        SteamIsRunning = 0,
        CantFindNecessaryPaths,
        CantFindVdfFile,
        AddedForOneUser,
        AddedForMoreUsers,
        NothingDone,
        AccessError

    };

    [Flags]
    public enum FATALERRORSFOUND
    {
        NoneDetected = 0,
        WrongVideoAdapter,
        ErrorWhileReadingFile
    }

    public enum SAVEGAMEMOVING
    {
        NoSuccesfulEvaluationPerformed = 0,
        NoSavegame2Move,
        BothFolderExist,
        MovingPossible

    };

    public static class FlagExtension
    {

        public static FATALERRORSFOUND Add(this FATALERRORSFOUND baseEnum, FATALERRORSFOUND addThis)
        {
            return baseEnum | addThis;
        }

        public static FATALERRORSFOUND Remove(this FATALERRORSFOUND baseEnum, FATALERRORSFOUND removeThis)
        {
            return baseEnum &= ~removeThis;
        }


    }
}
