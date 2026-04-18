using System.Collections.Generic;
using UnityEngine.Profiling;

namespace EC
{
    public static class GameProfiler
    {
        private static Dictionary<string, CustomSampler> s_Samplers = new Dictionary<string, CustomSampler>();

        public static void Begin(string profilerTag)
        {
            bool hasValue = s_Samplers.TryGetValue(profilerTag, out var sampler);
            if(!hasValue)
            {
                CustomSampler tempSampler = CustomSampler.Create(profilerTag);
                s_Samplers.Add(profilerTag, tempSampler);
                sampler = tempSampler;
            }
            sampler.Begin();
        }

        public static void End(string profilerTag)
        {
            s_Samplers[profilerTag].End();
        }
    }
}
