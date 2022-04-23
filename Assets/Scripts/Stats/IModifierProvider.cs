using System.Collections.Generic;

namespace Rpg.Stats
{
    public interface IModifierProvider
    {
        IEnumerable<float> GetAdditiveModifier(Stat stat);

        IEnumerable<float> GetPercentageModifier(Stat stat);
    }
}