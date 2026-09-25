using UnityEngine;
using FMODUnity;

namespace Game.Audio.Environment
{
    public class windAudioManager : PersistentAudioInstance
    {
        public float windLevel;

        // Constructor: Inherits from PersistentAudioInstance and adds variables for wind level
        public windAudioManager(EventReference eventReference, Transform attach, float windLevelInput = 0) 
            : base(eventReference, attach)
        {
            windLevel = windLevelInput;
        }

        //Prevents parent class from updating position automatically
        public override void Update()
        {

        }
    }
}

